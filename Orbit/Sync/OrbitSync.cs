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
using Serilog;
using Person = PlanningCenter.Api.People.Person;

namespace Sync
{
    public enum SyncMode
    {
        Create,
        Update
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

        public async Task UploadActivity<TSource, TActivitySource>(
            Progress progress, TActivitySource source, UploadActivity activity, string personId)
            where TActivitySource : EntityBase
        {
            LastDate = activity.OccurredAt;
            var person = await _cache.GetOrAddEntity(personId, async (_) =>
            {
                var doc = await _peopleClient.GetAsync<Person>($"people/{personId}");
                SetChild(doc.Data);
                SetWorkspace(doc.Data);
                return doc.Data;
            });

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
                    progress.Success++;
                }
                else
                {
                    if (await UpsertActivity(person, source, activity, identity))
                        progress.Success++;
                }
            }
            catch (Exception ex)
            {
                string? errorMessage;
                if (ex is ApiException orbitException)
                {
                    if (ex.Message.Contains("has already been taken"))
                    {
                        switch (_config.KeyExistsMode)
                        {
                            case KeyExistsMode.Stop:
                                _log.Information("Key {ActivityKey} already exists, assuming upload complete",
                                    activity.Key);
                                progress.Complete = true;
                                return;

                            case KeyExistsMode.Skip:
                            default:
                                _log.Debug("Skipping already uploaded activity");
                                progress.Skipped++;
                                return;
                        }
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

                progress.Failed++;
            }
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

        private async Task<bool> UpsertActivity<TActivitySource>(
            Person person, TActivitySource source, UploadActivity activity, OtherIdentity identity)
            where TActivitySource : EntityBase
        {
            if (!_cache.TryGetMapping(source, out var activityId))
            {
                await _orbitClient.CreateActivity(person.OrbitWorkspace, activity, identity!);
            }
            else
            {
                // if we put these tags on when we create, then the api will duplicate them with its own tags
                // if we leave these tags off of the PUT, then the api will delete the tags
                activity.Tags.AddRange(new[]
                {
                    $"custom_type:{activity.Type.Kebaberize()}",
                    $"custom_title:{activity.Title.Kebaberize()}"
                });

                var updated = await _orbitClient.PutAsync<DocumentRoot<UploadActivity>>(
                    $"{person.OrbitWorkspace}/members/{person.Id}/activities/{activityId}", activity);
            }

            return true;
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

            var member = new UpsertMember()
            {
                Member = new Member()
                {
                    Url = PlanningCenterUtil.PersonLink(person.Id!),
                    Email = $"{person.Id}@foothillsuu.org",
                    Birthday = person.Birthdate,
                    Name = person.Name,
                    Slug = person.Id!,
                    TagsToAdd = string.Join(",", tags),
                },
                Identity = new OtherIdentity(source: Constants.PlanningCenterSource)
                {
                    Name = person.Name,
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
    }
}