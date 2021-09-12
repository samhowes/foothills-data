using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.CheckIns;
using Serilog;

namespace Sync
{
    public class Synchronizer
    {
        private readonly ILogger _log;
        private readonly LogDbContext _logDb;
        private readonly PlanningCenterClient _peopleClient;
        private readonly OrbitApiClient _orbitClient;
        private readonly string _workspaceSlug;

        public Synchronizer(ILogger log, LogDbContext logDb, PlanningCenterClient peopleClient,
            OrbitApiClient orbitClient, string workspaceSlug)
        {
            _log = log;
            _logDb = logDb;
            _peopleClient = peopleClient;
            _orbitClient = orbitClient;
            _workspaceSlug = workspaceSlug;
        }

        interface ISync
        {
            
        }
        
        class PeopleToMembersSync : ISync
        {
            
        }
        
        public async Task PeopleToMembers()
        {
            
            var people = await _peopleClient.GetAsync<Person>("people");

            using var stats = new Stats();
            var existing = await _logDb.Mappings.ToDictionaryAsync(m => m.PlanningCenterId);
            for (;;)
            {
                _log.Information("Found {PeopleCount} out of {TotalCount} people to sync: {Url}",
                    people.Data.Count, people.Meta.TotalCount, people.Links.Self);
                using var batchStats = new Stats();
                foreach (var person in people.Data)
                {
                    if (existing.TryGetValue(person.Id, out var mapping))
                    {
                        stats.Skipped++;
                        continue;
                    }

                    mapping = new Mapping()
                    {
                        PlanningCenterId = person.Id,
                    };
                    var tags = new List<string>();
                    if (person.Attributes.Child == "true")
                        tags.Add("child");

                    var member = new UpsertMember()
                    {
                        Birthday = person.Attributes.Birthdate,
                        Name = person.Attributes.Name,
                        Slug = person.Id,
                        TagsToAdd = string.Join(",", tags),
                        Identity = new Identity(source: "planningcenter")
                        {
                            Email = $"{person.Id}@foothillsuu.org",
                            Name = person.Attributes.Name,
                            Uid = person.Id,
                            Url = person.Links.Self,
                        }
                    };

                    try
                    {
                        var created = await _orbitClient.PostAsync<Member>($"{_workspaceSlug}/members", member);
                        mapping.OrbitId = created.Data.Id;
                        batchStats.Success++;
                    }
                    catch (OrbitApiException orbitEx)
                    {
                        mapping.Error = orbitEx.Message;
                        _log.Error("Orbit api error for PlanningCenterId {PlanningCenterId}: {ApiError}", person.Id,
                            mapping.Error);
                        batchStats.Failed++;
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, "Unexpected error for PlanningCenterId {PlanningCenterId}", person.Id);
                        mapping.Error = ex.ToString();
                        batchStats.Failed++;
                    }

                    _logDb.Mappings.Add(mapping);
                    await _logDb.SaveChangesAsync();
                }

                _log.Information(
                    "Uploaded {SuccessCount} members to Orbit in {SecondsElapsed} at {RecordRate}; skipped {SkippedCount}; failed {ErrorCount}",
                    batchStats.Success, batchStats.SecondsElapsed, batchStats.RecordsPerSecond, batchStats.Skipped,
                    batchStats.Failed);

                stats.Accumulate(batchStats);
                _log.Information(
                    "Overall: elapsed: {Elapsed}; total queued {QueueCount}; total processed {TotalProcessed}; success: {Success}; skipped: {Skipped}; failed: {Failed}; RecordsPerSecond: {RecordsPerSecond}",
                    stats.SecondsElapsed, people.Meta.TotalCount, stats.Total, stats.Success, stats.Skipped,
                    stats.Failed, stats.RecordsPerSecond);

                if (!string.IsNullOrEmpty(people.Links.Next))
                {
                    _log.Information("Moving to next page...");
                    people = await _peopleClient.GetAsync<Person>(people.Links.Next);
                }
                else
                {
                    _log.Information("No more pages");
                    break;
                }
            }
        }
    }
}