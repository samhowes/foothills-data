using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JsonApi;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.CheckIns;

namespace Sync
{
    public record CheckInsConfig : IPostProcessConfig
    {
        public List<DateRangeConfig> DateRanges { get; set; } = null!;
        public decimal Weight { get; set; }
        public string ChannelRegex { get; set; }
        
        public string ActivityType { get; set; }
        public List<ActivityOverride> Overrides { get; set; }

        public class ActivityOverride
        {
            public string ActivityType { get; set; }
            public string Channel { get; set; }
        }

        public void PostProcess()
        {
            OverridesDict = Overrides.ToDictionary(o => o.Channel);
        }

        public Dictionary<string,ActivityOverride> OverridesDict { get; set; }
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
        private readonly Regex _eventChannelRegex;
        private SyncContext _context = null!;


        public CheckInsToActivitiesSync(
            SyncDeps deps,
            CheckInsClient checkInsClient,
            CheckInsConfig config)
        {
            _deps = deps;
            _checkInsClient = checkInsClient;
            _config = config;
            _eventChannelRegex = new Regex(config.ChannelRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public async Task<ApiCursor<CheckIn>?> InitializeAsync(SyncContext context)
        {
            _context = context;
            var url = context.NextUrl ?? UrlUtil.MakeUrl("check_ins",
                ("include", "locations,event_times,event"),
                ("order", "-created_at"));
            
            return new ApiCursor<CheckIn>(_checkInsClient, url, "checkins");
        }

        public async Task<SyncStatus> ProcessItemAsync(CheckIn checkIn)
        {
            if (checkIn.Person == null)
            {
                _deps.Log.Warning("No person associated with CheckIn {CheckInId}", checkIn.Id);
                return SyncStatus.Ignored;
            }

            if (checkIn.EventTimes.Data.Count > 1)
                _deps.Log.Error("double event times!");
            if (checkIn.Locations.Data.Count > 1)
                _deps.Log.Error("double locations!");
            
            var @event = checkIn.Event.Data;
            
            var info = await _deps.Cache.GetOrAddEntity(@event.Id!, (_) =>
            {
                var match = _eventChannelRegex.Match(@event.Name!);
                @event.Name = @event.Name![..match.Index].Trim();
                var channel = match.Groups["channel"].Value;
                return Task.FromResult(new EventInfo(@event, channel))!;
            });
            
            if (info?.Channel == null)
            {
                _deps.Log.Debug("Skipping event without channel annotation: {EventName}", @event.Name);
                return SyncStatus.Ignored;
            }
            
            async Task<SyncStatus> CreateActivity(string? checkInName, string type, string? title = null)
            {
                if (title == null)
                {
                    title = "Checked in";
                    checkInName ??= info.Event.Name;
                    title += $" for {checkInName}";
                }
                
                var activity = new UploadActivity(
                    info!.Channel,
                    type,
                    OrbitUtil.ActivityKey(checkIn),
                    OrbitUtil.FormatDate(checkIn.CreatedAt),
                    _config.Weight,
                    title,
                    PlanningCenterUtil.CheckInsLink(checkIn.EventPeriod.Id!, checkIn.Id!),
                    "CheckIn"
                );

                return await _deps.OrbitSync.UploadActivity<CheckIn, CheckIn>(checkIn, activity,
                    checkIn.Person.Id!);
            }

            var activityType = _config.ActivityType;
            if (_config.OverridesDict.TryGetValue(info.Channel!, out var @override))
            {
                activityType = @override.ActivityType;
            }

            foreach (var eventTime in checkIn.EventTimes.Data)
            {
                SyncStatus status;
                if (!checkIn.Locations.Data.Any())
                {
                    if (@override != null)
                    {
                        activityType = $"In Person {activityType}";
                    }
                    status = await CreateActivity(eventTime.Name, activityType);
                    if (status != SyncStatus.Success) return status;
                    _context.BatchProgress.RecordItem(status);
                }
                else
                {
                    foreach (var location in checkIn.Locations.Data)
                    {
                        var dateRangeConfig = _config.DateRanges.Single(
                            dr => dr.StartDate <= checkIn.CreatedAt
                                  && checkIn.CreatedAt <= dr.EndDate);

                        var prefix = dateRangeConfig.Locations ? location.Name : dateRangeConfig.ActivityType;

                        status = await CreateActivity(eventTime.Name!, $"{prefix} {activityType}");
                        if (status != SyncStatus.Success) return status;
                        _context.BatchProgress.RecordItem(status);
                    }
                }
            }

            _context.BatchProgress.Success--;
            return SyncStatus.Success;
        }
    }

    public class EventInfo : EntityBase
    {
        public Event Event { get; }
        public string Channel { get; }

        public EventInfo(Event @event, string channel)
        {
            Id = @event.Id;
            Event = @event;
            Channel = channel;
        }
    }

    public class PlanningCenterException : Exception
    {
        public PlanningCenterException(string message) : base(message)
        {
        }
    }
}