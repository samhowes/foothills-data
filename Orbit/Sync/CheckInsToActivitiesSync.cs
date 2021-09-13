using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonApi;
using JsonApiSerializer.JsonApi;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.CheckIns;
using Serilog;

namespace Sync
{
    public record AddActivity(Activity Activity, Identity Identity);
    
    public class CheckInsToActivitiesSync : ISync
    {
        private readonly CheckInsClient _checkInsClient;
        private DocumentRoot<List<CheckIn>> _checkIns;
        private LogDbContext _logDb;
        private Dictionary<string,Mapping> _existing;
        private readonly OrbitApiClient _orbitClient;
        private ILogger _log;


        public CheckInsToActivitiesSync(CheckInsClient checkInsClient, LogDbContext logDb, OrbitApiClient orbitClient, ILogger log)
        {
            _checkInsClient = checkInsClient;
            _logDb = logDb;
            _orbitClient = orbitClient;
            _log = log;
        }

        public string From => "CheckIns";
        public string To => "Activities";
        public async Task<(Meta Meta, Links Links)> GetInitialDataAsync()
        {
            // var urlBase = "events";
            // var worships = await _checkInsClient.GetAsync<Event>(urlBase, 
            //     ("where[name]", "Sunday Attendance"),
            //     ("include", "event_periods"));
            //
            // if (worships.Meta.TotalCount != 1)
            // {
            //     throw new PlanningCenterException(
            //         $"Expected to find 1 event titled 'Sunday Attendance', but found {worships.Meta.TotalCount} " +
            //         $"instead: {string.Join(",", worships.Data.Select(w => w.Attributes.Name))}");
            // }
            //
            // var worship = worships.Data.Single();
            //
            // var eventPeriods = await _checkInsClient.GetAsync<EventPeriod>(
            //     worship.Links.Self + "/event_periods", 
            //     ("order", "-starts_at"));
            //
            // var builder = new StringBuilder().AppendLine();
            // foreach (var period in eventPeriods.Data)
            // {
            //     builder.AppendLine($"----- {period.Attributes.StartsAt}");
            //     var times = await _checkInsClient.GetAsync<EventTime>(period.Links.Self + "/event_times");
            //
            //     foreach (var time in times.Data.OrderBy(e => e.Attributes.StartsAt))
            //     {
            //         builder.AppendLine($"{time.Attributes.Name}:\t\t {time.Attributes.ShowsAt} => {time.Attributes.StartsAt} => {time.Attributes.HidesAt}");
            //         var checkIns = await _checkInsClient.GetAsync<CheckIn>(
            //             time.Links.Self + "/check_ins");
            //         builder.AppendLine($"\tCheckIn count: {checkIns.Meta.TotalCount}");
            //     }    
            // }
            //
            //
            // _log.Information(builder.ToString());
            
            _checkIns = await _checkInsClient.GetAsync<List<CheckIn>>("check_ins", ("include", "event_times"));

            foreach (var checkIn in _checkIns.Data)
            {
            }
            
            _existing = await _logDb.Mappings
                .Where(m => m.Type == nameof(CheckIn))
                .ToDictionaryAsync(m => m.PlanningCenterId);
            return (_checkIns.Meta, _checkIns.Links);
        }

        public async Task ProcessBatchAsync(Stats stats)
        {
            foreach (var checkIn in _checkIns.Data)
            {
                if (_existing.TryGetValue(checkIn.Id, out var mapping))
                {
                    stats.Skipped++;
                    continue;
                }

                mapping = new Mapping()
                {
                    PlanningCenterId = checkIn.Id,
                };

                var activity = new AddActivity(
                    new Activity("activity title")
                    {
                        
                    },
                    new Identity(source: Constants.PlanningCenterSource)
                    {
                        // Uid = checkIn.Attributes.PersonId
                    });
                
                var member = new Activity()
                {
                    
                };

                try
                {
                    var created = await _orbitClient.PostAsync<Activity>("members", member);
                    mapping.OrbitId = created.Data.Id;
                    stats.Success++;
                }
                catch (OrbitApiException orbitEx)
                {
                    mapping.Error = orbitEx.Message;
                    _log.Error("Orbit api error for PlanningCenterId {PlanningCenterId}: {ApiError}", checkIn.Id,
                        mapping.Error);
                    stats.Failed++;
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "Unexpected error for PlanningCenterId {PlanningCenterId}", checkIn.Id);
                    mapping.Error = ex.ToString();
                    stats.Failed++;
                }

                _logDb.Mappings.Add(mapping);
            }
            await _logDb.SaveChangesAsync();
        }

        public async Task<bool> GetNextBatchAsync()
        {
            if (string.IsNullOrEmpty(_checkIns.Links.Prev())) return true;
            if (string.IsNullOrEmpty(_checkIns.Links.Next())) return false;
            _checkIns = await _checkInsClient.GetAsync<List<CheckIn>>(_checkIns.Links.Next()!);
            return true;
        }
    }
}