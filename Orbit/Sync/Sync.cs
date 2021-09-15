using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonApi;
using JsonApiSerializer.JsonApi;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api;
using Serilog;
using Person = PlanningCenter.Api.Calendar.Person;

namespace Sync
{
        
    public enum SyncMode
    {
        Create,
        Update
    }

    public record SyncImplConfig(SyncMode Mode = SyncMode.Create);

    public abstract class Sync<TSource> where TSource : EntityBase
    {
        private readonly SyncImplConfig _config;
        private readonly OrbitApiClient _orbitClient;
        private readonly DataCache _cache;
        protected readonly ILogger Log;
        private readonly LogDbContext _logDb;

        protected Sync(SyncImplConfig config, OrbitApiClient orbitClient, DataCache cache, ILogger log, LogDbContext logDb, PlanningCenterClient planningCenterClient)
        {
            _config = config;
            _orbitClient = orbitClient;
            _cache = cache;
            Log = log;
            _logDb = logDb;
            PlanningCenterClient = planningCenterClient;
        }
        
        public abstract string From { get; }
        public abstract string To { get; }
        
        public PlanningCenterClient PlanningCenterClient { get; }

        public abstract Task<DocumentRoot<List<TSource>>> GetInitialDataAsync(string? nextUrl);
        public abstract Task ProcessBatchAsync(Progress progress, TSource item);

        protected async Task UploadActivity<TActivitySource>(
            Progress progress, TActivitySource source, UploadActivity activity, string personId) where TActivitySource : EntityBase
        {
            try
            {
                var identity = new Identity(source: Constants.PlanningCenterSource)
                {
                    Uid = personId
                };
                
                if (_config.Mode == SyncMode.Create)
                {
                    await _orbitClient.CreateActivity(activity, identity);
                }
                else
                {
                    await UpsertActivity(source, activity, personId, identity);
                }

                progress.Success++;
            }
            catch (Exception ex)
            {
                string? errorMessage;
                if (ex is ApiException orbitException)
                {
                    if (ex.Message.Contains("has already been taken"))
                    {
                        Log.Debug("Skipping already uploaded activity");
                        progress.Skipped++;
                        return;
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

        private async Task UpsertActivity<TActivitySource>(TActivitySource source, UploadActivity activity, string personId, Identity? identity)
            where TActivitySource : EntityBase
        {
            if (!_cache.TryGetValue<Person>(personId, out var memberSlug))
            {
                var member = await _orbitClient.GetAsync<Member>("members/find",
                    ("source", Constants.PlanningCenterSource),
                    ("uid", personId));

                memberSlug = member.Data.Slug;
                _cache.Set<Person>(personId, memberSlug);
            }

            if (!_cache.TryGetValue<TActivitySource>(source.Id, out var activityId))
            {
                string? nextUrl = null;
                for (;;)
                {
                    var batch = await _orbitClient.GetAsync<List<ActivityBase>>(
                        nextUrl ?? $"members/{memberSlug}/activities");
                    foreach (var maybe in batch.Data)
                    {
                        if (string.Equals(maybe.Key, activity.Key, StringComparison.OrdinalIgnoreCase))
                            activityId = maybe.Id;

                        var sourceId = OrbitUtil.EntityId<TActivitySource>(maybe.Key);
                        if (sourceId == null) continue;

                        _cache.Set<TActivitySource>(sourceId, maybe.Id);
                    }

                    nextUrl = OrbitUtil.TrimLink(batch.Links.Next());
                    if (activityId != null || string.IsNullOrEmpty(nextUrl)) break;
                }
            }

            if (activityId == null)
            {
                await _orbitClient.CreateActivity(activity, identity);
            }
            else
            {
                activity.Description = "";
                var updated = await _orbitClient.PutAsync<DocumentRoot<UploadActivity>>(
                    $"members/{memberSlug}/activities/{activityId}", activity);
            }
        }
    }
}