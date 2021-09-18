using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.Groups;

namespace Sync
{
    public class GroupAttendanceSync : GroupSync<Event>
    {
        public GroupAttendanceSync(SyncDeps deps, GroupsClient groupsClient, GroupsConfig config)
            : base(deps, groupsClient, config)
        {
        }

        public override string To => "Activity";
        // start with the most recent events, but don't get events in the future
        protected override string Endpoint 
            => $"events?order=-starts_at&where[starts_at][lt]={DateTime.Now.ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ})";

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
                        "Group Attendance",
                        OrbitUtil.ActivityKey(attendance),
                        @event.StartsAt,
                        isLeader ? 6m : 3m, 
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