using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.CheckIns;
using Serilog;
using Serilog.Core;
using Constants = PlanningCenter.Api.Constants;

namespace Sync
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var peopleClient = PlanningCenterClient.Create(Constants.PeoplePrefix);
            // var checkinsClient = PlanningCenterClient.Create(Constants.CheckInsPrefix);
            var orbitClient = OrbitApiClient.Create("");
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            
            var workspaceSlug = "sam-workspace";
            var logDb = await GetLog();
            
            await DoSync(log, peopleClient, logDb, orbitClient, workspaceSlug);
        }

        private static async Task DoSync(Logger log, PlanningCenterClient peopleClient, LogDbContext logDb,
            OrbitApiClient orbitClient, string workspaceSlug)
        {
            log.Information("Starting sync to workspace {WorkspaceSlug}...", workspaceSlug);
            var people = await peopleClient.GetAsync<Person>("people");

            using var stats = new Stats();
            var existing = await logDb.Mappings.ToDictionaryAsync(m => m.PlanningCenterId);
            for (;;)
            {
                log.Information("Found {PeopleCount} out of {TotalCount} people to sync: {Url}",
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
                        var created = await orbitClient.PostAsync<Member>($"{workspaceSlug}/members", member);
                        mapping.OrbitId = created.Data.Id;
                        batchStats.Success++;
                    }
                    catch (OrbitApiException orbitEx)
                    {
                        mapping.Error = orbitEx.Message;
                        log.Error("Orbit api error for PlanningCenterId {PlanningCenterId}: {ApiError}", person.Id,
                            mapping.Error);
                        batchStats.Failed++;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, "Unexpected error for PlanningCenterId {PlanningCenterId}", person.Id);
                        mapping.Error = ex.ToString();
                        batchStats.Failed++;
                    }

                    logDb.Mappings.Add(mapping);
                    await logDb.SaveChangesAsync();
                }

                log.Information("Uploaded {SuccessCount} members to Orbit in {SecondsElapsed} at {RecordRate}; skipped {SkippedCount}; failed {ErrorCount}",
                    batchStats.Success, batchStats.SecondsElapsed, batchStats.RecordsPerSecond, batchStats.Skipped, batchStats.Failed);
                
                stats.Accumulate(batchStats);
                log.Information("Overall: elapsed: {Elapsed}; total queued {QueueCount}; total processed {TotalProcessed}; success: {Success}; skipped: {Skipped}; failed: {Failed}; RecordsPerSecond: {RecordsPerSecond}",
                    stats.SecondsElapsed, people.Meta.TotalCount, stats.Total, stats.Success, stats.Skipped, stats.Failed, stats.RecordsPerSecond);
                
                if (!string.IsNullOrEmpty(people.Links.Next))
                {
                    log.Information("Moving to next page...");
                    people = await peopleClient.GetAsync<Person>(people.Links.Next);
                }
                else
                {
                    log.Information("No more pages");
                    break;
                }
            }
        }

        public class Stats : IDisposable
        {
            public Stats()
            {
                Timer = new Stopwatch();
                Timer.Start();
            }
            public Stopwatch Timer { get; }
            public int Skipped { get; set; }
            public int Success { get; set; }
            public int Failed { get; set; }
            public int Total => Skipped + Success + Failed;
            public decimal SecondsElapsed => Timer.ElapsedMilliseconds / 1000m;
            public decimal RecordsPerSecond => ((decimal)Total / Math.Max(Timer.ElapsedMilliseconds, 1)) * 1000m;

            public void Accumulate(Stats other)
            {
                Skipped += other.Skipped;
                Success += other.Success;
                Failed += other.Failed;
            }

            public void Dispose()
            {
                Timer.Stop();
            }
        }
        
        private static async Task<LogDbContext> GetLog()
        {
            var root = Directory.GetCurrentDirectory();
            while (!Directory.Exists(Path.Combine(root!, ".git")))
                root = Path.GetDirectoryName(root)!;
            var dbPath = Path.Combine(root, "log.db");

            var log = new LogDbContext(new DbContextOptionsBuilder<LogDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options);
            await log.Database.EnsureCreatedAsync();
            return log;
        }
    }


    public class UpsertMember
    {
        public string Birthday { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string TagsToAdd { get; set; }
        public Identity Identity { get; set; }
    }
}
