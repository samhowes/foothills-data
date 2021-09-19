using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonApi;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.CheckIns;

namespace Sync
{
    public record CheckInsConfig 
    {
        public List<DateRangeConfig> DateRanges { get; set; } = null!;
        public string Channel { get; set; } = null!;
        public decimal Weight { get; set; }
    }

    public class DateRangeConfig
    {
        public DateTime StartDate { get; set; } = DateTime.MinValue;
        public DateTime EndDate { get; set; } = DateTime.MaxValue;
        public string ActivityType { get; set; }
        public bool Locations { get; set; }
    }

    public class CheckInsToActivitiesSync : ISync<CheckIn>
    {
        private readonly SyncDeps _deps;
        private readonly CheckInsClient _checkInsClient;
        private readonly CheckInsConfig _config;
        private Event _worship = null!;
        private SyncContext _context = null!;

        public CheckInsToActivitiesSync(
            SyncDeps deps,
            CheckInsClient checkInsClient,
            CheckInsConfig config)
        {
            _deps = deps;
            _checkInsClient = checkInsClient;
            _config = config;
        }

        public async Task<ApiCursor<CheckIn>?> InitializeAsync(SyncContext context)
        {
            _context = context;
            var worships = await _checkInsClient.GetAsync<List<Event>>(UrlUtil.MakeUrl("events",
                ("where[name]", "Sunday Attendance")));

            if (worships.Data.Count != 1)
            {
                throw new PlanningCenterException(
                    $"Expected to find 1 event titled 'Sunday Attendance', but found {worships.Meta.TotalCount()} " +
                    $"instead: {string.Join(",", worships.Data.Select(w => w.Name))}");
            }
            
            _worship = worships.Data.Single();
            _worship = (await _checkInsClient.GetAsync<Event>(_worship.Links.Self())).Data;

            var url = context.NextUrl ?? UrlUtil.MakeUrl(UrlUtil.MakeUrl(_worship.Links!["check_ins"].Href,
                ("include", "locations,event_times"),
                ("order", "-created_at")));
            
            return new ApiCursor<CheckIn>(_checkInsClient, url, _worship.Name);
        }

        public async Task ProcessItemAsync(CheckIn checkIn)
        {
            var progress = _context.BatchProgress;
            if (checkIn.Person == null)
            {
                _deps.Log.Warning("No person associated with CheckIn {CheckInId}", checkIn.Id);
                progress.Skipped++;
                _deps.LogDb.Mappings.Add(new Mapping()
                {
                    PlanningCenterId = checkIn.Id,
                    Type = nameof(CheckIn),
                    Error = $"Missing person for {checkIn.Links.Self()}"
                });
                await _deps.LogDb.SaveChangesAsync();
                return;
            }

            if (checkIn.EventTimes.Data.Count > 1) 
                _deps.Log.Error("double event times!");
            if (checkIn.Locations.Data.Count > 1) 
                _deps.Log.Error("double locations!");
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

                    await _deps.OrbitSync.UploadActivity<CheckIn, CheckIn>(progress, checkIn, activity, checkIn.Person.Id!);
                    if (progress.Complete) return;
                }
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