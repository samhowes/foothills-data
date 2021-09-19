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

    }

    public class SyncDeps
    {
        public SyncImplConfig Config { get; }
        public OrbitApiClient OrbitClient { get; }
        public DataCache Cache { get; }
        public ILogger Log { get; }
        public LogDbContext LogDb { get; }
        public PeopleClient PeopleClient { get; set; }

        public SyncDeps(SyncImplConfig config, OrbitApiClient orbitClient, DataCache cache, ILogger log,
            LogDbContext logDb, PeopleClient peopleClient)
        {
            Config = config;
            OrbitClient = orbitClient;
            Cache = cache;
            Log = log;
            LogDb = logDb;
            PeopleClient = peopleClient;
        }
    }

    public abstract class Sync<TSource> where TSource : EntityBase
    {
        private readonly SyncImplConfig _config;
        private readonly OrbitApiClient _orbitClient;
        private readonly DataCache _cache;
        protected readonly ILogger Log;
        private readonly LogDbContext _logDb;

        protected Sync(SyncDeps deps, PlanningCenterClient planningCenterClient)
        {
            Deps = deps;
            _config = deps.Config;
            _orbitClient = deps.OrbitClient;
            _cache = deps.Cache;
            Log = deps.Log;
            _logDb = deps.LogDb;
            PlanningCenterClient = planningCenterClient;
        }
        
        public string LastDate { get; set; }
        public virtual SyncType SyncType => SyncType.TopLevel;

        protected SyncDeps Deps { get; }
        public string From => typeof(TSource).Name;
        public virtual string To => "Activity";

        public PlanningCenterClient PlanningCenterClient { get; }

        protected Dictionary<string, ActivityLoader> OrbitInfo { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public virtual async Task<DocumentRoot<List<TSource>>> GetInitialDataAsync(string? nextUrl)
        {
            if (_config.Mode == SyncMode.Update)
            {
                var type = typeof(TSource).Name;
                var loader = new ActivityLoader(type, Deps, _orbitClient);
                
                OrbitInfo[type] = loader;
                await loader.InitAsync();
            }

            return new DocumentRoot<List<TSource>>();
        }

        public abstract Task ProcessBatchAsync(Progress progress, TSource item);

        protected async Task UploadActivity<TActivitySource>(
            Progress progress, TActivitySource source, UploadActivity activity, string personId)
            where TActivitySource : EntityBase
        {
            LastDate = activity.OccurredAt;
            try
            {
                var occurredAt = DateTime.Parse(activity.OccurredAt);
                var identity = new OtherIdentity(source: Constants.PlanningCenterSource)
                {
                    Uid = personId
                };
                OrbitInfo? info = null;
                if (OrbitInfo.TryGetValue(typeof(TSource).Name, out var loader))
                {
                    info = await loader.GetUntil(occurredAt);
                }

                if (_config.Mode == SyncMode.Create ||
                    (info != null && info.MaxDate < DateTime.Parse(activity.OccurredAt)))
                {
                    await _orbitClient.CreateActivity(activity, identity);
                    progress.Success++;
                }
                else
                {
                    if (await UpsertActivity(source, activity, personId, identity))
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
                                Log.Information("Key {ActivityKey} already exists, assuming upload complete",
                                    activity.Key);
                                progress.Complete = true;
                                return;
                            
                            case KeyExistsMode.Skip:
                            default:
                                Log.Debug("Skipping already uploaded activity");
                                progress.Skipped++;
                                return;
                        }
                        
                    }

                    Log.Error("Orbit api error for PlanningCenterId {PlanningCenterId}: {ApiError}", source.Id,
                        orbitException.Message);
                    errorMessage = orbitException.Message;
                }
                else
                {
                    Log.Error(ex, "Unexpected error for PlanningCenterId {PlanningCenterId}", source.Id);
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

        private async Task<bool> UpsertActivity<TActivitySource>(TActivitySource source, UploadActivity activity,
            string personId, OtherIdentity identity)
            where TActivitySource : EntityBase
        {
            var memberSlug = await _cache.GetOrAddMapping<Person>(personId, async (_) =>
            {
                var person = await Deps.PeopleClient.GetAsync<Person>($"people/{personId}");
                var slug = await CreateMemberAsync(person.Data);
                return slug;
            });
            if (memberSlug == null) return false;

            if (!_cache.TryGetMapping(source, out var activityId))
            {
                await _orbitClient.CreateActivity(activity, identity!);
            }
            else
            {
                // if we put these tags on when we create, then the api will duplicate them with its own tags
                // if we leave these tags off of the PUT, then the api will delete the tags
                activity.Tags.AddRange(new []{
                    $"custom_type:{activity.Type.Kebaberize()}",
                    $"custom_title:{activity.Title.Kebaberize()}"});
                
                var updated = await _orbitClient.PutAsync<DocumentRoot<UploadActivity>>(
                    $"members/{memberSlug}/activities/{activityId}", activity);
            }

            return true;
        }

        protected async Task<string?> CreateMemberAsync(Person person)
        {
            var tags = new List<string>();
            if (person.Membership.Contains("(Child)", StringComparison.OrdinalIgnoreCase))
            {
                person.Child = true;
            }
            
            var member = new UpsertMember()
            {
                Birthday = person.Birthdate,
                Name = person.Name,
                Slug = person.Id!,
                TagsToAdd = string.Join(",", tags),
                Identity = new OtherIdentity(source: Constants.PlanningCenterSource)
                {
                    Name = person.Name,
                    Uid = person.Id,
                    Url = PlanningCenterUtil.PersonLink(person.Id!),
                }
            };

            try
            {
                var created = await Deps.OrbitClient.PostAsync<Member>("members", member);
                return created.Data.Slug;
            }
            catch (ApiException orbitEx)
            {
                Deps.Log.Error("Orbit api error for PlanningCenterId {PlanningCenterId}: {ApiError}", person.Id,
                    orbitEx.Message);
                return null;
            }
            catch (Exception ex)
            {
                Deps.Log.Error(ex, "Unexpected error for PlanningCenterId {PlanningCenterId}", person.Id);
                return null;
            }
        }

        public async Task CleanActivitiesAsync(string channel)
        {
            channel = OrbitUtil.ChannelTag(channel);
            
            Log.Information("Cleaning {Channel}", channel);
            for (;;)
            {
                var batch = await _orbitClient.GetAsync<List<CustomActivity>>(
                    $"activities?items=100&direction=DESC&sort=occurred_at&activity_tags={channel}");
                if (!batch.Data.Any()) break;
                Log.Information("{Date:MM/dd/yyyy}", DateTime.Parse(batch.Data.First().OccurredAt));
                foreach (var activity in batch.Data)
                {
                    await _orbitClient.Delete($"members/{activity.Member.Slug}/activities/{activity.Id}");
                }
                
            }
        }
        
        public virtual Task AfterEachBatchAsync() => Task.CompletedTask;
    }
}