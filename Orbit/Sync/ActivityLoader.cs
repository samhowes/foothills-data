using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonApi;
using JsonApiSerializer.JsonApi;
using Microsoft.EntityFrameworkCore;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api.People;

namespace Sync
{
    public class OrbitInfo
    {
        public string? NextUrl;
        public string? PrevUrl;
        public DateTime MaxDate = DateTime.MinValue;
        public DateTime MinDate = DateTime.MaxValue;
    }
    
    public class ActivityLoader
    {
        private readonly string _activityType;
        private readonly SyncDeps _deps;
        private readonly OrbitApiClient _orbitClient;
        private string? _url;
        private readonly OrbitInfo _info;
        private Progress _progress = null!;

        public ActivityLoader(string activityType, OrbitApiClient orbitClient, string? url = null)
        {
            _activityType = activityType;
            _orbitClient = orbitClient;
            _url = url;
            _info = new OrbitInfo();
        }

        public async Task InitAsync()
        {
            var type = $"{nameof(CustomActivity)}:{_activityType}";
            _progress = await _deps.LogDb.Progress.SingleOrDefaultAsync(p => p.Type == type);

            if (_progress == null)
            {
                _url ??= $"activities?items=100&direction=DESC&sort=occurred_at";
                _progress = new Progress(type, _url);
                _deps.LogDb.Add(_progress);
            }

            _info.NextUrl = _progress.NextUrl!;
            _info.PrevUrl = _progress.NextUrl!;
        }

        public async Task<OrbitInfo> GetUntil(DateTime date)
        {
            string? url = null!;
            int ShouldContinue()
            {
                int direction = 0;
                if (date < _info.MinDate)
                {
                    if (_info.NextUrl == null) return 0;
                    url = _info.NextUrl;
                    direction = 1;
                }
                
                if (date > _info.MaxDate)
                {
                    if (_info.PrevUrl == null) return 0;
                    url = _info.PrevUrl;
                    direction = -1;
                }
                
                return direction;    
            }

            var direction = ShouldContinue();
            if (direction == 0) return _info;
            for (;;)
            {
                var batch = await LoadBatch(url);
                if (direction < 0) _info.PrevUrl = batch.Links.Prev();
                else if (direction > 0) _info.NextUrl = batch.Links.Next();
                
                direction = ShouldContinue();
                if (direction == 0) break;
            }

            return _info;
        }

        private DateTime MinDate(DateTime a, DateTime b) => a < b ? a : b;

        private DateTime MaxDate(DateTime a, DateTime b) => a > b ? a : b;

        private async Task<DocumentRoot<List<CustomActivity>>> LoadBatch(string url)
        {
            DocumentRoot<List<CustomActivity>> batch;
            batch = await _orbitClient.GetAsync<List<CustomActivity>>(url);
            foreach (var activity in batch.Data)
            {
                var entityInfo = OrbitUtil.EntityId(activity.Key);
                if (entityInfo == null) continue;

                if (_deps.Config.ShouldDelete && string.Equals(_activityType, activity.CustomType,
                        StringComparison.OrdinalIgnoreCase) &&
                    !activity.Tags.Any(t => t.StartsWith("custom")))
                {
                    _deps.Log.Debug("Deleting activity {ActivityType} missing custom tags: {ActivityLink}",
                        activity.CustomType, activity.OrbitUrl);

                    if (await _orbitClient.Delete(
                        $"members/{activity.Member.Slug}/activities/{activity.Id}"))
                        continue;
                }

                var planningCenterId =
                    activity.Member.Identities.Data.FirstOrDefault(i => i.Source == Constants.PlanningCenterSource);

                if (planningCenterId != null)
                {
                    _deps.Cache.SetMapping<Person>(planningCenterId.Uid!, activity.Member.Slug);
                }

                _deps.Cache.SetMapping(entityInfo.Value.entityName, entityInfo.Value.id, activity.Id);
                var planningCenterIdentity =
                    activity.Member?.Identities?.Data.FirstOrDefault(i =>
                        i.Source == Constants.PlanningCenterSource);

                if (planningCenterIdentity != null)
                {
                    _deps.Cache.SetMapping<Person>(planningCenterIdentity.Uid!, activity.Member!.Slug);
                }
            }
            var max = batch.Data.First().OccurredAt;
            var min = batch.Data.Last().OccurredAt;
            _info.MinDate = MinDate(_info.MinDate, min);
            _info.MaxDate = MaxDate(_info.MaxDate, max);
            _progress.NextUrl = url;

            return batch;
        }
    }
}