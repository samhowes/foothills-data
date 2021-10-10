using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api.CheckIns;
using Serilog;

namespace Sync
{
    public class Cleaner
    {
        private readonly ILogger _log;
        private readonly OrbitApiClient _orbitClient;
        private readonly SyncImplConfig _config;
        private readonly NotesConfig _notesConfig;

        public Cleaner(ILogger log, OrbitApiClient orbitClient, IOptions<SyncImplConfig> config, NotesConfig notesConfig)
        {
            _log = log;
            _orbitClient = orbitClient;
            _config = config.Value;
            _notesConfig = notesConfig;
        }

        public async Task<int> Clean(params string[] activityTags)
        {
            activityTags = new[]
                {
                    OrbitUtil.SourceTag<CheckIn>(),
                }.Concat(_notesConfig.Categories.Select(c => OrbitUtil.ActivityTypeTag(c.ActivityType)))
                .ToArray();
            
            foreach (var activityTag in activityTags)
            {
                _log.Information("Cleaning activities of type: {ActivityType}", activityTag);

                foreach (var workspace in new[] {_config.ChildWorkspace, _config.DefaultWorkspace})
                {
                    for (;;)
                    {
                        var batch = await _orbitClient.GetAsync<List<CustomActivity>>(
                            $"{workspace}/activities?items=100&direction=ASC&sort=occurred_at&activity_tags={activityTag}");
                        if (!batch.Data.Any()) break;
                        _log.Information("{Date:MM/dd/yyyy}", batch.Data.First().OccurredAt);
                        foreach (var activity in batch.Data)
                        {
                            await _orbitClient.Delete(
                                $"{workspace}/members/{activity.Member.Slug}/activities/{activity.Id}");
                        }
                    }
                }
            }

            return 0;
        }
    }
}