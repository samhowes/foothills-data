using System;
using System.Threading.Tasks;
using JsonApi;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.Groups;

namespace Sync
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GroupAttendanceConfig
    {
        public string ActivityType { get; set; } = null!;
        public decimal LeadershipWeight { get; set; }
        public decimal NormalWeight { get; set; }
    }
    
    public class GroupAttendanceSync : IMultiSync<Event, Attendance>
    {
        private readonly SyncDeps _deps;
        private readonly GroupsClient _groupsClient;
        private readonly GroupAttendanceConfig _attendanceConfig;
        private readonly GroupSync _groupSync;
        private SyncContext _context = null!;

        public GroupAttendanceSync(SyncDeps deps, GroupsClient groupsClient, GroupAttendanceConfig attendanceConfig,
            GroupSync groupSync)
        {
            _deps = deps;
            _groupsClient = groupsClient;
            _attendanceConfig = attendanceConfig;
            _groupSync = groupSync;
        }

        
        public async Task<ApiCursor<Event>> InitializeTopLevelAsync(SyncContext context)
        {
            _context = context;
            await _groupSync.InitializeAsync();
            var url = context.NextUrl ?? UrlUtil.MakeUrl("events",
                ("order", "-starts_at"),
                ("where[starts_at][lt]", OrbitUtil.FormatDate(DateTime.Now)));
            return new ApiCursor<Event>(_groupsClient, url);
        }
        
        public async Task<ApiCursor<Attendance>?> InitializeAsync(SyncContext context)
        {
            var @event = context.GetData<Event>();
            if (@event.Canceled)
            {
                _deps.Log.Debug("Ignoring cancelled event: {EventName}", @event.Name);
                return null;
            }
            
            var group = await _groupSync.GetGroupInfo(@event.Group.Id!);
            if (group.Ignore)
            {
                _deps.Log.Debug("Ignoring group marked as ignored: {GroupName}", group.Name);
                return null;
            }

            _context.SetData(group);
            return new ApiCursor<Attendance>(_groupsClient, $"events/{@event.Id}/attendances", $"{group.Name}:Attendance");
        }

        public async Task ProcessItemAsync(Attendance attendance)
        {
            var progress = _context.BatchProgress;
            if (!attendance.Attended)
            {
                progress.Skipped++;
                return;
            }
            
            var group = _context.GetData<GroupInfo>();
            var @event = _context.GetData<Event>();
            
            var eventAppLink = $"{PlanningCenterUtil.GroupLink(group)}/events/{@event.Id}";
            var titleSuffix = @event.Name ?? $"A {@event.Group.Name} Event";
            
            var isLeader = attendance.Role == "leader"; 
            
            var activity = new UploadActivity(
                group.Channel!,
                _attendanceConfig.ActivityType,
                OrbitUtil.ActivityKey(attendance),
                @event.StartsAt,
                isLeader ? _attendanceConfig.LeadershipWeight : _attendanceConfig.NormalWeight, 
                $"{(isLeader ? "Led" : "Attended")} {titleSuffix}", 
                eventAppLink,
                "Event"
            );

            await _deps.OrbitSync.UploadActivity<Group, Attendance>(progress, attendance, activity, attendance.Person.Id!);
            
        }
    }
}