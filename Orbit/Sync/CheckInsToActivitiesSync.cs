using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonApi;
using JsonApiSerializer.JsonApi;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.CheckIns;

namespace Sync
{
    public record CheckInsConfig 
    {
        public List<DateRangeConfig> DateRanges { get; set; }
        public string Channel { get; set; }
        public decimal Weight { get; set; }
        public string ActivityTypeSuffix { get; set; }
        public bool Clean { get; set; }
    }

    public class DateRangeConfig
    {
        public DateTime StartDate { get; set; } = DateTime.MinValue;
        public DateTime EndDate { get; set; } = DateTime.MaxValue;
        public string ActivityType { get; set; }
        public bool Locations { get; set; }
    }

    public class CheckInsToActivitiesSync : Sync<CheckIn>
    {
        private readonly CheckInsClient _checkInsClient;
        private readonly CheckInsConfig _config;
        private Event _worship = null!;

        public CheckInsToActivitiesSync(
            SyncDeps deps,
            CheckInsClient checkInsClient,
            CheckInsConfig config)
            : base(deps, checkInsClient)
        {
            _checkInsClient = checkInsClient;
            _config = config;
        }

        public override async Task<DocumentRoot<List<CheckIn>>> GetInitialDataAsync(string? nextUrl)
        {
            var worships = await _checkInsClient.GetAsync<List<Event>>("events",
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

            if (_config.Clean)
            {
                await CleanActivitiesAsync(_config.Channel);
            }

            return batch;
        }

        public override async Task ProcessBatchAsync(Progress progress, CheckIn checkIn)
        {
            if (checkIn.Person == null)
            {
                Log.Warning("No person associated with CheckIn {CheckInId}", checkIn.Id);
                progress.Skipped++;
                Deps.LogDb.Mappings.Add(new Mapping()
                {
                    PlanningCenterId = checkIn.Id,
                    Type = nameof(CheckIn),
                    Error = $"Missing person for {checkIn.Links.Self()}"
                });
                await Deps.LogDb.SaveChangesAsync();
                return;
            }

            if (checkIn.EventTimes.Data.Count > 1) 
                Log.Error("double event times!");
            if (checkIn.Locations.Data.Count > 1) 
                Log.Error("double locations!");
            foreach (var eventTime in checkIn.EventTimes.Data)
            {
                foreach (var location in checkIn.Locations.Data)
                {
                    var dateRangeConfig = _config.DateRanges.Single(
                        dr => dr.StartDate <= checkIn.CreatedAt
                              && checkIn.CreatedAt <= dr.EndDate);

                    var prefix = dateRangeConfig.Locations ? location.Name : dateRangeConfig.ActivityType;
                    
                    var activity = new UploadActivity(
                        _config.Channel,
                        $"{prefix} {_worship.Name}",
                        OrbitUtil.ActivityKey(checkIn),
                        OrbitUtil.FormatDate(checkIn.CreatedAt),
                        _config.Weight,
                        $"Checked in for {eventTime.Name}",
                        PlanningCenterUtil.CheckInsLink(checkIn.EventPeriod.Id!, checkIn.Id!),
                        "CheckIn"
                    );

                    await UploadActivity(progress, checkIn, activity, checkIn.Person.Id!);
                    if (progress.Complete) return;
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
    }

    public class PlanningCenterException : Exception
    {
        public PlanningCenterException(string message) : base(message)
        {
        }
    }
}