using System;
using System.Threading.Tasks;
using JsonApi;
using JsonApiSerializer.JsonApi;
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

    public class GroupMembershipSync : IMultiSync<Group, Membership>
    {
        private readonly SyncDeps _deps;
        private readonly GroupsClient _groupsClient;
        private readonly GroupMembershipConfig _membershipConfig;
        private readonly GroupSync _groupsSync;
        private SyncContext _context = null!;

        public GroupMembershipSync(SyncDeps deps, GroupsClient groupsClient,
            GroupMembershipConfig membershipConfig, GroupSync groupsSync)
        {
            _deps = deps;
            _groupsClient = groupsClient;
            _membershipConfig = membershipConfig;
            _groupsSync = groupsSync;
        }
        
        public async Task<ApiCursor<Group>> InitializeTopLevelAsync(SyncContext context)
        {
            await _groupsSync.InitializeAsync();

            var url = UrlUtil.MakeUrl("groups");
            var cursor = new ApiCursor<Group>(_groupsClient, url);
            return cursor;
        }

        public Task<ApiCursor<Membership>?> InitializeAsync(SyncContext context)
        {
            var group = context.GetData<Group>();
            var url = UrlUtil.MakeUrl($"groups/{group.Id}/memberships", ("order", "-joined_at"));
            var cursor = new ApiCursor<Membership>(_groupsClient, url, $"{group.Name}:Members");
            _context = context;
            return Task.FromResult(cursor)!;
        }

        public async Task<SyncStatus> ProcessItemAsync(Membership membership)
        {
            var group = await _groupsSync.GetGroupInfo(membership.Group.Id!);

            if (group.Ignore)
            {
                return SyncStatus.Ignored;
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

            return await _deps.OrbitSync.UploadActivity<Group, Membership>(membership, activity,
                membership.Person.Id!);
        }
    }
}