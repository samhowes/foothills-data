using System;
using System.Threading.Tasks;
using JsonApi;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.Groups;

namespace Sync
{
    public class MembershipSync : GroupSync<Membership>
    {
        public MembershipSync(SyncDeps deps, GroupsClient groupsClient, GroupsConfig config) : base(deps, groupsClient, config)
        {
        }

        protected override string Endpoint => "membership";

        public override async Task ProcessBatchAsync(Progress progress, Membership membership)
        {
            var group = await GetGroupInfo(membership.Group.Id!);

            if (group.Ignore)
            {
                progress.Skipped++;
                return;
            }

            var activity = new UploadActivity(
                group.Channel!,
                "Group Attendance",
                OrbitUtil.ActivityKey(membership),
                membership.JoinedAt,
                6m,
                $"Joined Group {group.Name}",
                OrbitUtil.GroupLink(group),
                group.Name);

            await UploadActivity(progress, membership, activity, membership.Person.Id!);
        }
    }
}