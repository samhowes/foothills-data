using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JsonApi;
using JsonApiSerializer.JsonApi;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.CheckIns;

namespace Sync
{
    public record CheckInsConfig 
    {
        public List<DateRangeConfig> DateRanges { get; set; }
    }

    public class DateRangeConfig
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } = DateTime.MaxValue;
        public string ActivityType { get; set; }
        public bool Locations { get; set; }
    }

    public class CheckInsToActivitiesSync : Sync<CheckIn>
    {
        private readonly CheckInsClient _checkInsClient;
        private readonly CheckInsConfig _config;
        private Event _worship = null!;
        private HashSet<string> _ignoredEventIds = null!;
        private Blob _blob = null!;
        private FilesConfig _filesConfig;
        private StreamWriter _csv;

        public CheckInsToActivitiesSync(
            SyncDeps deps,
            CheckInsClient checkInsClient,
            CheckInsConfig config, FilesConfig filesConfig)
            : base(deps, checkInsClient)
        {
            _checkInsClient = checkInsClient;
            _config = config;
            _filesConfig = filesConfig;
        }

        public override async Task<DocumentRoot<List<CheckIn>>> GetInitialDataAsync(string? nextUrl)
        {
            var csv = new FileInfo(Path.Combine(_filesConfig.Root, "checkins.csv"));
            if (csv.Exists)
                csv.Delete();
            _csv = csv.CreateText();
            await _csv.WriteLineAsync(
                "Firstname,LastName,Time,EventTimeName,ShowsAt,HidesAt,LocationName");
        
            var urlBase = "events";
            var worships = await _checkInsClient.GetAsync<List<Event>>(urlBase,
                ("where[name]", "Sunday Attendance"));

            if (worships.Data.Count != 1)
            {
                throw new PlanningCenterException(
                    $"Expected to find 1 event titled 'Sunday Attendance', but found {worships.Meta.TotalCount()} " +
                    $"instead: {string.Join(",", worships.Data.Select(w => w.Name))}");
            }

            _worship = worships.Data.Single();
            _worship = (await _checkInsClient.GetAsync<Event>(_worship.Links.Self())).Data;

            DocumentRoot<List<CheckIn>> batch;
            if (nextUrl != null)
            {
                batch = await _checkInsClient.GetAsync<List<CheckIn>>(nextUrl);
            }
            else
            {
                batch = await _checkInsClient.GetAsync<List<CheckIn>>(_worship.Links!["check_ins"].Href,
                    ("include", "locations,event_times"),
                    ("order", "-created_at"));
            }

            const string skippedKey = "skippedEvents";
            _blob = await Deps.LogDb.Blobs.SingleOrDefaultAsync(b => b.Key == skippedKey);
            if (_blob == null)
            {
                _blob = new Blob() {Key = skippedKey, Value = ""};
            }

            _ignoredEventIds = new HashSet<string>(_blob.Value.Split(","));

            return batch;
        }

        public override async Task ProcessBatchAsync(Progress progress, CheckIn checkIn)
        {
            if (checkIn.Person == null)
            {
                Log.Warning("No person associated with CheckIn {CheckInId}", checkIn.Id);
                Deps.LogDb.Mappings.Add(new Mapping()
                {
                    PlanningCenterId = checkIn.Id,
                    Type = nameof(CheckIn),
                    Error = $"Missing person for {checkIn.Links.Self()}"
                });
                await Deps.LogDb.SaveChangesAsync();
                return;
            }

            DateTime ToMountainTime(DateTime original) => original.ToLocalTime().AddHours(-2);
            
            if (checkIn.EventTimes.Data.Count > 1) 
                Log.Error("double event times!");
            if (checkIn.Locations.Data.Count > 1) 
                Log.Error("double locations!");
            foreach (var eventTime in checkIn.EventTimes.Data)
            {
                var eventId = eventTime.Event.Id;
                foreach (var location in checkIn.Locations.Data)
                {
                    var dateRangeConfig = _config.DateRanges.Single(
                        dr => dr.StartDate <= checkIn.CreatedAt
                              && checkIn.CreatedAt <= dr.EndDate);
                    
                    var activity = new UploadActivity(
                        "Worship",
                        "Online Worship Attendance",
                        OrbitUtil.ActivityKey(checkIn),
                        OrbitUtil.FormatDate(checkIn.CreatedAt),
                        5m,
                        $"Checked in for Worship for {eventTime.Name}",
                        $"https://check-ins.planningcenteronline.com/event_periods/{checkIn.EventPeriod.Id}/check_ins/{checkIn.Id}",
                        "CheckIn"
                    );
                    
                    var data = new[]
                    {
                        checkIn.FirstName,
                        checkIn.LastName,
                        OrbitUtil.FormatDate(checkIn.CreatedAt),
                        eventTime.Name,
                        ToMountainTime(eventTime.ShowsAt).ToString("HH:mm"),
                        ToMountainTime(eventTime.HidesAt).ToString("HH:mm"),
                        location.Name
                    };

                    await _csv.WriteLineAsync(string.Join(",", data));

                    Log.Debug("{Firstname:0,10} {LastName:0,10} at {Time}\t for {EventTimeName} ({ShowsAt:HH:mm} - {HidesAt:HH:mm}):\t {LocationName}",
                        checkIn.FirstName, checkIn.LastName, checkIn.CreatedAt, eventTime.Name, ToMountainTime(eventTime.ShowsAt), ToMountainTime(eventTime.HidesAt), location.Name);

                    // await UploadActivity(progress, checkIn, activity, checkIn.Person.Id!);
                }
                
                // if (eventId != _worship.Id)
                // {
                //     if (_ignoredEventIds.Add(eventId!))
                //     {
                //         var details = await _checkInsClient.GetAsync<Event>($"events/{eventId}");
                //         var self = details.Data.Links.Self();
                //         details.Data.Links = null!;
                //         Log.Information("Ignoring event {EventLink}: {EventJson}", self,
                //             JsonConvert.SerializeObject(details.Data));
                //         _blob.Value = string.Join(",", _ignoredEventIds);
                //         await Deps.LogDb.SaveChangesAsync();
                //     }
                //
                //     progress.Skipped++;
                //     continue;
                // }
            }
        }

        public override async Task AfterEachBatchAsync()
        {
             await base.AfterEachBatchAsync();
             await _csv.FlushAsync();
        }
    }

    public class PlanningCenterException : Exception
    {
        public PlanningCenterException(string message) : base(message)
        {
        }
    }
}