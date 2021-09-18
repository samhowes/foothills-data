using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using JsonApi;
using JsonApiSerializer.JsonApi;
using PlanningCenter.Api;
using PlanningCenter.Api.Groups;

namespace Sync
{
    public class GroupInfo : Group
    {
        public string Type => "group";
        public string? Channel { get; set; }
        public List<Tag> Tags { get; set; } = null!;
        public bool Ignore { get; set; }
    }

    public class GroupsConfig
    {
        public string DefaultChannel { get; set; } = null!;
        public string IgnoreTagName { get; set; } = null!;
        public int ChannelTagGroupId { get; set; }
        public bool Clean { get; set; }
    }

    public abstract class GroupSync<TSource> : Sync<TSource> where TSource : EntityBase
    {
        protected readonly GroupsClient GroupsClient;
        private readonly GroupsConfig _groupsConfig;

        protected GroupSync(SyncDeps deps, GroupsClient groupsClient, GroupsConfig config) : base(deps, groupsClient)
        {
            GroupsClient = groupsClient;
            _groupsConfig = config;
        }

        protected abstract string Endpoint { get; }

        public override async Task<DocumentRoot<List<TSource>>> GetInitialDataAsync(string? nextUrl)
        {
            var channelTags =
                await GroupsClient.GetAsync<List<Tag>>($"tag_groups/{_groupsConfig.ChannelTagGroupId}/tags");
            
            Log.Information("Found {ChannelCount} channel tags: {ChannelTags}", channelTags.Data.Count,
                channelTags.Data.Select(t => t.Name));

            _channels = channelTags.Data.ToDictionary(t => t.Id!);

            if (_groupsConfig.Clean)
            {
                foreach (var channel in _channels.Values)
                {
                    await CleanActivitiesAsync(channel.Name.Kebaberize());
                }
            }

            _ignoreTag = _channels.Values.SingleOrDefault(t => t.Name == _groupsConfig.IgnoreTagName)!;
            if (_ignoreTag == null)
            {
                throw new PlanningCenterException(
                    $"Failed to find the `{_groupsConfig.IgnoreTagName}` tag. Found {string.Join(",", _channels.Values.Select(t => t.Name))}");
            }

            return await GroupsClient.GetAsync<List<TSource>>(nextUrl ?? Endpoint);
        }

        private Tag _ignoreTag = null!;

        private Dictionary<string, Tag> _channels = null!;

        protected async Task<GroupInfo> GetGroupInfo(string groupId)
        {
            return await Deps.Cache.GetOrAddEntity(groupId, async (_) =>
            {
                var groupDocument = await GroupsClient.GetAsync<GroupInfo>($"groups/{groupId}");
                var group = groupDocument.Data!;
                var tagsDocument = await GroupsClient.GetAsync<List<Tag>>(group.Links!["tags"].Href);
                group.Tags = tagsDocument.Data;
                foreach (var tag in group.Tags)
                {
                    if (tag.Id == _ignoreTag.Id)
                        group.Ignore = true;
                    if (_channels.TryGetValue(tag.Id!, out var chanelTag))
                    {
                        group.Channel = chanelTag.Name.Trim();
                        break;
                    }
                }

                group.Channel ??= _groupsConfig.DefaultChannel;
                return group;
            });
        }
    }
}