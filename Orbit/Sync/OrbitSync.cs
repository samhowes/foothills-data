using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using JsonApi;
using JsonApiSerializer.JsonApi;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.CheckIns;
using Serilog;
using Person = PlanningCenter.Api.People.Person;

namespace Sync
{
    public enum SyncMode
    {
        Create,
        Update,
        Seek
    }

    public enum KeyExistsMode
    {
        // ignore key exists errors and continue uploading
        Skip,

        // assume we have finished uploading new data and stop uploading
        Stop
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public record SyncImplConfig
    {
        public bool ShouldDelete { get; set; }
        public SyncMode Mode { get; set; } = SyncMode.Update;
        public KeyExistsMode KeyExistsMode { get; set; } = KeyExistsMode.Stop;
        public string DefaultWorkspace { get; set; }
        public string ChildWorkspace { get; set; }
    }

    public class SyncDeps
    {
        public SyncImplConfig Config { get; }
        public OrbitApiClient OrbitClient { get; }
        public DataCache Cache { get; }
        public ILogger Log { get; }
        public LogDbContext LogDb { get; }
        public PeopleClient PeopleClient { get; set; }
        public OrbitSync OrbitSync { get; }

        public SyncDeps(SyncImplConfig config, OrbitApiClient orbitClient, DataCache cache, ILogger log,
            LogDbContext logDb, PeopleClient peopleClient, OrbitSync orbitSync)
        {
            Config = config;
            OrbitClient = orbitClient;
            Cache = cache;
            Log = log;
            LogDb = logDb;
            PeopleClient = peopleClient;
            OrbitSync = orbitSync;
        }
    }

    public class OrbitSync
    {
        private readonly SyncImplConfig _config;
        private readonly OrbitApiClient _orbitClient;
        private readonly DataCache _cache;
        private readonly ILogger _log;
        private readonly LogDbContext _logDb;
        private readonly PeopleClient _peopleClient;
        private readonly Dictionary<string, Loader<Member>> _loaders;


        public OrbitSync(SyncImplConfig config, OrbitApiClient orbitClient, DataCache cache, ILogger log,
            LogDbContext logDb, PeopleClient peopleClient)
        {
            _config = config;
            _orbitClient = orbitClient;
            _cache = cache;
            _log = log;
            _logDb = logDb;
            _peopleClient = peopleClient;

            Loader<Member> MakeCursor(string workspace, string name)
            {
                var cursor = new ApiCursor<Member>(_orbitClient!, $"{workspace}/members?sort=created_at", name);
                return new(cursor, (m) => m.CreatedAt, _cache);
            }
            
            _loaders = new Dictionary<string, Loader<Member>>()
            {
                [_config.ChildWorkspace] = MakeCursor(_config.ChildWorkspace, "Children"),
                [_config.DefaultWorkspace] = MakeCursor(_config.DefaultWorkspace, "Adults")
            };
        }

        public DateTime LastDate { get; set; }

        protected Dictionary<string, ActivityLoader> OrbitInfo { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public async Task<DocumentRoot<List<TSource>>> GetInitialDataAsync<TSource>(string? nextUrl)
        {
            if (_config.Mode == SyncMode.Update)
            {
                var type = typeof(TSource).Name;
                var loader = new ActivityLoader(type, _orbitClient);

                OrbitInfo[type] = loader;
                await loader.InitAsync();
            }

            return new DocumentRoot<List<TSource>>();
        }

        public async Task<SyncStatus> UploadActivity<TSource, TActivitySource>(TActivitySource source, UploadActivity activity, string personId)
            where TActivitySource : EntityBase
        {
            if (string.IsNullOrEmpty(activity.ActivityType))
            {
                throw new ArgumentException("No activity type specified", nameof(activity));
            }
            LastDate = activity.OccurredAt;
            var person = await GetPersonAsync(personId);
            if (person == null) return SyncStatus.Ignored;

            try
            {
                var identity = new OtherIdentity(source: Constants.PlanningCenterSource)
                {
                    Uid = personId
                };
                OrbitInfo? info = null;
                if (OrbitInfo.TryGetValue(typeof(TSource).Name, out var loader))
                {
                    info = await loader.GetUntil(activity.OccurredAt);
                }

                if (_config.Mode == SyncMode.Create ||
                    (info != null && info.MaxDate < activity.OccurredAt))
                {
                    await _orbitClient.CreateActivity(person.OrbitWorkspace, activity, identity);
                    return SyncStatus.Success;
                }

                await UpsertActivity(person, source, activity, identity);
                return SyncStatus.Success;
            }
            catch (Exception ex)
            {
                return await HandleApiError<TSource, TActivitySource>(source, activity, ex);
            }
        }

        private async Task<SyncStatus> HandleApiError<TSource, TActivitySource>(TActivitySource source, UploadActivity activity,
            Exception ex) where TActivitySource : EntityBase
        {
            string? errorMessage = null;
            if (ex is ApiException orbitException)
            {
                if (ex.Message.Contains("has already been taken"))
                {
                    return SyncStatus.Exists;
                }

                _log.Error("Orbit api error for PlanningCenterId {PlanningCenterId}: {ApiError}", source.Id,
                    orbitException.Message);
                errorMessage = orbitException.Message;
            }
            else
            {
                _log.Error(ex, "Unexpected error for PlanningCenterId {PlanningCenterId}", source.Id);
                errorMessage = ex.StackTrace;
            }

            var mapping = new Mapping()
            {
                PlanningCenterId = source.Id,
                Type = typeof(TActivitySource).Name,
                Error = errorMessage
            };

            _logDb.Add(mapping);
            await _logDb.SaveChangesAsync();

            return SyncStatus.Failed;
        }

        private void SetWorkspace(Person person)
        {
            person.OrbitWorkspace = person.Child ? _config.ChildWorkspace : _config.DefaultWorkspace;
        }

        private static bool SetChild(Person person)
        {
            person.Child = person.Child ||
                           person.Membership?.Contains("(Child)", StringComparison.OrdinalIgnoreCase) == true;
            return person.Child;
        }

        private async Task UpsertActivity<TActivitySource>(
            Person person, TActivitySource source, UploadActivity activity, OtherIdentity identity)
            where TActivitySource : EntityBase
        {
            if (!_cache.TryGetMapping(source, out var activityId))
            {
                var existing = await GetActivity(source);
                activityId = existing?.Id;
            }
            if (activityId == null)
            {
                await _orbitClient.CreateActivity(person.OrbitWorkspace, activity, identity!);
            }
            else
            {
                // if we put these tags on when we create, then the api will duplicate them with its own tags
                // if we leave these tags off of the PUT, then the api will delete the tags
                activity.Tags.AddRange(new[]
                {
                    OrbitUtil.ActivityTypeTag(activity.Type),
                    $"custom_title:{activity.Title.Kebaberize()}"
                });

                var updated = await _orbitClient.PutAsync<DocumentRoot<UploadActivity>>(
                    $"{person.OrbitWorkspace}/members/{person.Id}/activities/{activityId}", activity);
            }
        }

        public async Task UpdateMemberAsync(string memberId, Person person, List<string>?tags = null)
        {
            LastDate = person.CreatedAt;
            SetChild(person);
            SetWorkspace(person);
            var member = MakeMember(person, tags);
            await _orbitClient.PutAsync<Member>($"{person.OrbitWorkspace}/members/{memberId}", member);
        }
        
        public async Task<Member?> CreateMemberAsync(Person person, List<string>? tags = null)
        {
            LastDate = person.CreatedAt;
            SetChild(person);
            SetWorkspace(person);

            var loader = _loaders[person.OrbitWorkspace];
            await loader.GetUntil(person.CreatedAt);
            
            tags ??= new List<string>();
            if (person.Child)
                tags.Add("child");

            var memberEntity = MakeMember(person, tags);
            var member = new UpsertMember()
            {
                Member = memberEntity,
                Identity = new OtherIdentity(source: Constants.PlanningCenterSource)
                {
                    // Name = person.Name,
                    Uid = person.Id,
                    Url = PlanningCenterUtil.PersonLink(person.Id!),
                }
            };

            try
            {
                var created = await _orbitClient.PostAsync<Member>($"{person.OrbitWorkspace}/members", member);
                _cache.SetEntity(person);
                return created.Data;
            }
            catch (ApiErrorException)
            {
                throw;
            }
            catch (ApiException orbitEx)
            {
                _log.Error("Orbit api error for PlanningCenterId {PlanningCenterId}: {ApiError}", person.Id,
                    orbitEx.Message);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Unexpected error for PlanningCenterId {PlanningCenterId}", person.Id);
            }
            return null;
        }

        private static Member MakeMember(Person person, List<string>? tags)
        {
            var memberEntity = new Member()
            {
                Url = PlanningCenterUtil.PersonLink(person.Id!),
                Email = $"{person.Id}@foothillsuu.org",
                Birthday = person.Birthdate,
                Name = person.Name,
                Slug = person.Id!,
                TagsToAdd = string.Join(",", tags),
            };
            return memberEntity;
        }

        public async Task CleanActivitiesAsync(string channel)
        {
            throw new NotImplementedException();
            channel = OrbitUtil.ChannelTag(channel);

            _log.Information("Cleaning {Channel}", channel);
            for (;;)
            {
                var batch = await _orbitClient.GetAsync<List<CustomActivity>>(
                    $"activities?items=100&direction=DESC&sort=occurred_at&activity_tags={channel}");
                if (!batch.Data.Any()) break;
                _log.Information("{Date:MM/dd/yyyy}", batch.Data.First().OccurredAt);
                foreach (var activity in batch.Data)
                {
                    await _orbitClient.Delete($"members/{activity.Member.Slug}/activities/{activity.Id}");
                }
            }
        }

        public async Task<Person?> GetPersonAsync(string personId)
        {
            return await _cache.GetOrAddEntity(personId, async (_) =>
            {
                try
                {
                    var doc = await _peopleClient.GetAsync<Person>($"people/{personId}");
                    SetChild(doc.Data);
                    SetWorkspace(doc.Data);
                    return doc.Data;
                }
                catch (ApiErrorException ex)
                {
                    if (ex.Type != ErrorTypeEnum.NotFound) throw;
                }

                return null;
            });
        }

        public async Task<ActivityBase?> GetActivity<TSource>(TSource item) where TSource : EntityBase
        {
            if (item is not IHavePerson personItem) return null;
            if (personItem.Person == null) return null;
            var person = await GetPersonAsync(personItem.Person.Id!);
            if (person == null) return null;
            var key = OrbitUtil.ActivityKey(item);
            var url = UrlUtil.MakeUrl($"{person.OrbitWorkspace}/activities",
                ("activity_tags", OrbitUtil.KeyTag(key)));
            
            try
            {
                var doc = await _orbitClient.GetAsync<List<ActivityBase>>(url);
                return doc.Data.SingleOrDefault();
            }
            catch
            {
                // ignored
            }

            return null;
        }
    }
}