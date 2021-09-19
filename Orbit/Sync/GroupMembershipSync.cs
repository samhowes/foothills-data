using System.Threading.Tasks;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.Groups;

namespace Sync
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GroupMembershipConfig
    {
        public decimal Weight { get; set; }
        public string ActivityType { get; set; } = null!;
    }

    public class GroupMembershipSync : GroupSync<Membership>
    {
        private readonly GroupMembershipConfig _membershipConfig;

        public GroupMembershipSync(SyncDeps deps, GroupsClient groupsClient, GroupConfig config,
            GroupMembershipConfig membershipConfig) : base(deps, groupsClient, config)
        {
            _membershipConfig = membershipConfig;
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
                _membershipConfig.ActivityType,
                OrbitUtil.ActivityKey(membership),
                membership.JoinedAt,
                _membershipConfig.Weight,
                $"Joined Group {group.Name}",
                PlanningCenterUtil.GroupLink(group),
                group.Name);

            await UploadActivity(progress, membership, activity, membership.Person.Id!);
        }
    }
}