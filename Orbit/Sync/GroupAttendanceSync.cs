using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    
    public class GroupAttendanceSync : GroupSync<Event>
    {
        private readonly GroupAttendanceConfig _attendanceConfig;

        public GroupAttendanceSync(SyncDeps deps, GroupsClient groupsClient, GroupConfig config, GroupAttendanceConfig attendanceConfig)
            : base(deps, groupsClient, config)
        {
            _attendanceConfig = attendanceConfig;
        }

        public override string To => "Activity";
        // start with the most recent events, but don't get events in the future
        protected override string Endpoint 
            => $"events?order=-starts_at&where[starts_at][lt]={OrbitUtil.FormatDate(DateTime.Now)})";

        public override async Task ProcessBatchAsync(Progress progress, Event @event)
        {
            if (@event.Canceled)
            {
                progress.Skipped++;
                return;
            }
            
            var group = await GetGroupInfo(@event.Group.Id!);
            if (group.Ignore)
            {
                progress.Skipped++;
                return;
            }
            
            var eventAppLink = $"{PlanningCenterUtil.GroupLink(group)}/events/{@event.Id}";
            
            var batches = GroupsClient.GetAllAsync<List<Attendance>>($"events/{@event.Id}/attendances");

            var titleSuffix = @event.Name ?? $"A {@event.Group.Name} Event";
            
            await foreach (var batch in batches)
            {
                foreach (var attendance in batch.Data)
                {
                    if (!attendance.Attended) continue;
                    
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

                    await UploadActivity(progress, attendance, activity, attendance.Person.Id!);
                }
            }
        }
    }
}