using System;
using System.Collections.Generic;
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

    public abstract class ApiCursor
    {
        public Meta Meta { get; protected set; } = null!;

        public string? NextUrl { get; protected set; }
        public abstract Task InitializeAsync();
    }

    public class ApiCursor<T> : ApiCursor
    {

        private readonly ApiClientBase _client;
        private DocumentRoot<List<T>>? _batch;

        public ApiCursor(ApiClientBase client, string nextUrl, string? name= null)
        {
            NextUrl = nextUrl;
            _client = client;
            Name = name ?? typeof(T).Name;
        }

        public string Name { get; }
        
        public override async Task InitializeAsync()
        {
            _batch = await _client.GetAsync<List<T>>(NextUrl);
            Meta = _batch.Meta;
        }

        public List<T>? Data => _batch?.Data;

        public async Task<bool> FetchNextAsync()
        {
            if (_batch == null)
            {
                await InitializeAsync();
                return true;
            }
            NextUrl = _batch?.Links.Next();
            if (string.IsNullOrEmpty(NextUrl)) return false;

            _batch = await _client.GetAsync<List<T>>(NextUrl);
            return true;
        }
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
            _context = context;
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
            return Task.FromResult(cursor)!;
        }

        public async Task ProcessItemAsync(Membership membership)
        {
            var progress = _context.BatchProgress;
            var group = await _groupsSync.GetGroupInfo(membership.Group.Id!);

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

            await _deps.OrbitSync.UploadActivity<Group, Membership>(progress, membership, activity,
                membership.Person.Id!);
        }
    }
}