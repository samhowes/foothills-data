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
    public record CheckInsConfig 
    {
        public List<DateRangeConfig> DateRanges { get; set; } = null!;
        public decimal Weight { get; set; }
        public string ChannelRegex { get; set; }
        
        public string ActivityType { get; set; }
        public HashSet<string> ActivityTypeFilters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }

    public class DateRangeConfig
    {
        public DateTime StartDate { get; set; } = DateTime.MinValue;
        public DateTime EndDate { get; set; } = DateTime.MaxValue;
        public string ActivityType { get; set; }
        public bool Locations { get; set; }
    }

    public class CheckInsToActivitiesSync : IMultiSync<Event, CheckIn>
    {
        private readonly SyncDeps _deps;
        private readonly CheckInsClient _checkInsClient;
        private readonly CheckInsConfig _config;
        private SyncContext? _childContext;
        private readonly Regex _eventChannelRegex;
        

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

        public async Task<ApiCursor<Event>> InitializeTopLevelAsync(SyncContext context)
        {
            if (context.NextUrl?.Contains("check_ins") == true)
            {
                var data = await _checkInsClient.GetAsync<List<CheckIn>>(context.NextUrl);
                var eventId = data.Data[0].EventTimes.Data[0].Event.Id;
                var search = new ApiCursor<Event>(_checkInsClient, UrlUtil.MakeUrl("events"), "Events");
                await foreach (var ev in search.GetAllAsync())
                {
                    if (ev.Id == eventId) break;
                }

                context.NextUrl = search.NextUrl;
            }
            var url = context.NextUrl ?? UrlUtil.MakeUrl("events");
            var cursor = new ApiCursor<Event>(_checkInsClient, url, "Events");
            // return Task.FromResult(cursor);
            return cursor;
        }

        public async Task<ApiCursor<CheckIn>?> InitializeAsync(SyncContext context)
        {
            _childContext = context;
            var @event = context.GetData<Event>();

            var match = _eventChannelRegex.Match(@event.Name);
            if (!match.Success)
            {
                _deps.Log.Debug("Skipping event without channel annotation: {EventName}", @event.Name);
                return null;
            }

            @event.Name = @event.Name.Substring(0, match.Index).Trim();
            
            var channel = match.Groups["channel"].Value;
            
            var details = await _checkInsClient.GetAsync<Event>(@event.Links.Self());
            
            var url = context.NextUrl ?? UrlUtil.MakeUrl(details.Data.Links!["check_ins"].Href,
                ("include", "locations,event_times"),
                ("order", "-created_at"));

            context.SetData(new EventInfo(@event, channel));
            return new ApiCursor<CheckIn>(_checkInsClient, url, $"{@event.Name}:{channel}");
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

            var info = _childContext!.GetData<EventInfo>();
            async Task<SyncStatus> CreateActivity(string? checkInName, string type)
            {
                var title = "Checked in";
                if (checkInName != null)
                {
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

            if (!_config.ActivityTypeFilters.Contains(info.Channel))
            {
                return await CreateActivity(info.Event.Name, _config.ActivityType);
            }
            
            foreach (var eventTime in checkIn.EventTimes.Data)
            {
                SyncStatus status;
                if (!checkIn.Locations.Data.Any())
                {
                    status = await CreateActivity(eventTime.Name, info.Event.Name);
                    _childContext.BatchProgress.RecordItem(status);
                }
                else
                {
                    foreach (var location in checkIn.Locations.Data)
                    {
                        var dateRangeConfig = _config.DateRanges.Single(
                            dr => dr.StartDate <= checkIn.CreatedAt
                                  && checkIn.CreatedAt <= dr.EndDate);

                        var prefix = dateRangeConfig.Locations ? location.Name : dateRangeConfig.ActivityType;

                        status = await CreateActivity(eventTime.Name!, $"{prefix} {info.Event.Name}");
                        _childContext.BatchProgress.RecordItem(status);
                    }
                }
            }

            _childContext.BatchProgress.Success--;
            return SyncStatus.Success;
        }
    }

    public class EventInfo
    {
        public Event Event { get; }
        public string Channel { get; }

        public EventInfo(Event @event, string channel)
        {
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