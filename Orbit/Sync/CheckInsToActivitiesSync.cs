using System;
using System.Collections.Generic;
using System.Linq;
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
        private DocumentRoot<List<CheckIn>> _checkIns = null!;
        private LogDbContext _logDb;
        private Dictionary<string,Mapping> _existing = null!;
        private readonly OrbitApiClient _orbitClient;
        private ILogger _log;
        private Event _worship = null!;
        private HashSet<string> _ignoredEventIds = null!;
        private Blob _blob;


        public CheckInsToActivitiesSync(CheckInsClient checkInsClient, LogDbContext logDb, OrbitApiClient orbitClient, ILogger log)
        {
            _checkInsClient = checkInsClient;
            _logDb = logDb;
            _orbitClient = orbitClient;
            _log = log;
        }

        public string From => "CheckIns";
        public string To => "Activities";
        public async Task<BatchInfo> GetInitialDataAsync(string? nextUrl)
        {
            var urlBase = "events";
            var worships = await _checkInsClient.GetAsync<List<Event>>(urlBase, 
                ("where[name]", "Sunday Attendance"),
                ("include", "event_periods"));
            
            if (worships.Data.Count != 1)
            {
                throw new PlanningCenterException(
                    $"Expected to find 1 event titled 'Sunday Attendance', but found {worships.Meta.TotalCount()} " +
                    $"instead: {string.Join(",", worships.Data.Select(w => w.Name))}");
            }
            
            _worship = worships.Data.Single();
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

            if (nextUrl != null)
            {
                _checkIns = await _checkInsClient.GetAsync<List<CheckIn>>(nextUrl);
            }
            else
            {
                _checkIns = await _checkInsClient.GetAsync<List<CheckIn>>("check_ins", 
                    ("include", "event_times"),
                    ("order", "created_at"));
            }
            
            
            const string skippedKey = "skippedEvents";
            _blob = await _logDb.Blobs.SingleOrDefaultAsync(b => b.Key == skippedKey);
            if (_blob == null)
            {
                _blob = new Blob() {Key = skippedKey, Value = ""};
            }

            _ignoredEventIds = new HashSet<string>(_blob.Value.Split(","));
            
            return new BatchInfo(_checkIns.Links.Self(), _checkIns.Meta, _checkIns.Links);
        }

        public async Task ProcessBatchAsync(Progress progress)
        {
            foreach (var checkIn in _checkIns.Data)
            {
                foreach (var eventTime in checkIn.EventTimes.Data)
                {
                    var eventId = eventTime.Event.Id;
                    if (eventId != _worship.Id)
                    {
                        if (_ignoredEventIds.Add(eventId))
                        {
                            var details = await _checkInsClient.GetAsync<Event>($"events/{eventId}");
                            var self = details.Data.Links.Self();
                            details.Data.Links = null!;
                            _log.Information("Ignoring event {EventLink}: {EventJson}", self, JsonConvert.SerializeObject(details.Data));
                            _blob.Value = string.Join(",", _ignoredEventIds);
                            await _logDb.SaveChangesAsync();
                        }

                        progress.Skipped++;
                        continue;
                    }

                    if (checkIn.Person == null)
                    {
                        _log.Warning("No person associated with CheckIn {CheckInId}", checkIn.Id);
                        _logDb.Mappings.Add(new Mapping()
                        {
                            PlanningCenterId = checkIn.Id,
                            Type = nameof(CheckIn),
                            Error = $"Missing person for {checkIn.Links.Self()}"
                        });
                        await _logDb.SaveChangesAsync();
                        continue;
                    }

                    var activity = new AddActivity(
                        new Activity(title: $"Checked in for Worship for {eventTime.Name}")
                        {
                            Key = $"check_ins/{checkIn.Id}",
                            Link = checkIn.Links.Self()!,
                            LinkText = "CheckIn",
                            ActivityType = "Online Worship Attendance",
                            OccurredAt = checkIn.CreatedAt,
                            Description = "Important Description from CheckIn",
                            Tags = new List<string>()
                            {
                                "channel:Worship"
                            },
                            Weight = "5"
                        },
                        new Identity(source: Constants.PlanningCenterSource)
                        {
                            Uid = checkIn.Person.Id,
                        });
                    
                    try
                    {
                        var created = await _orbitClient.PostAsync<DocumentRoot<Activity>>("activities", activity);
                        progress.Success++;
                    }
                    catch (Exception ex)
                    {
                        string errorMessage;
                        if (ex is OrbitApiException orbitException)
                        {
                            if (ex.Message.Contains("has already been taken"))
                            {
                                _log.Debug("Skipping already uploaded activity");
                                progress.Skipped++;
                                continue;
                            }
                            _log.Error("Orbit api error for PlanningCenterId {PlanningCenterId}: {ApiError}", checkIn.Id, orbitException.Message);
                            errorMessage = orbitException.Message;
                        }
                        else
                        {
                            _log.Error(ex, "Unexpected error for PlanningCenterId {PlanningCenterId}", checkIn.Id);
                            errorMessage = ex.StackTrace;
                        }

                        var mapping = new Mapping()
                        {
                            PlanningCenterId = checkIn.Id,
                            Type = nameof(CheckIn),
                            Error = errorMessage
                        };

                        _logDb.Add(mapping);
                        await _logDb.SaveChangesAsync();
                        
                        progress.Failed++;
                    }

                }
            }
            await _logDb.SaveChangesAsync();
        }

        public async Task<string?> GetNextBatchAsync()
        {
            if (string.IsNullOrEmpty(_checkIns.Links.Next())) return null;
            _checkIns = await _checkInsClient.GetAsync<List<CheckIn>>(_checkIns.Links.Next()!);
            return _checkIns.Links.Self();
        }
    }
}