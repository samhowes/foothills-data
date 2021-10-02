using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using JsonApi;
using JsonApiSerializer.JsonApi;
using PlanningCenter.Api;
using PlanningCenter.Api.Groups;
using Serilog;

namespace Sync
{
    public class GroupInfo : Group
    {
        public string Type => "group";
        public string? Channel { get; set; }
        public List<Tag> Tags { get; set; } = null!;
        public bool Ignore { get; set; }
    }

    public class GroupConfig
    {
        public string DefaultChannel { get; set; } = null!;
        public string IgnoreTagName { get; set; } = null!;
        public int ChannelTagGroupId { get; set; }
        public bool Clean { get; set; }
    }

    public class GroupSync 
    {
        private readonly GroupsClient _groupsClient;
        private readonly GroupConfig _groupConfig;
        private Tag _ignoreTag = null!;
        private Dictionary<string, Tag> _channels = null!;
        private readonly ILogger _log;
        private readonly DataCache _cache;

        public GroupSync(GroupsClient groupsClient, GroupConfig config, ILogger log, DataCache cache)
        {
            _groupsClient = groupsClient;
            _groupConfig = config;
            _log = log;
            _cache = cache;
        }

        public async Task InitializeAsync()
        {
            var channelTags =
                await _groupsClient.GetAsync<List<Tag>>($"tag_groups/{_groupConfig.ChannelTagGroupId}/tags");
            
            _log.Information("Found {ChannelCount} channel tags: {ChannelTags}", channelTags.Data.Count,
                channelTags.Data.Select(t => t.Name));

            _channels = new Dictionary<string, Tag>();
            foreach (var tag in channelTags.Data)
            {
                _channels[tag.Id!] = tag;
                tag.Name = OrbitUtil.CleanChannelName(tag.Name);

                if (tag.Name == _groupConfig.IgnoreTagName)
                    _ignoreTag = tag;
            }
            
            if (_ignoreTag == null)
            {
                throw new PlanningCenterException(
                    $"Failed to find the `{_groupConfig.IgnoreTagName}` tag. Found {string.Join(",", _channels.Values.Select(t => t.Name))}");
            }
        }

        public async Task<GroupInfo> GetGroupInfo(string groupId)
        {
            var info = await _cache.GetOrAddEntity(groupId, async (_) =>
            {
                var groupDocument = await _groupsClient.GetAsync<GroupInfo>($"groups/{groupId}");
                var group = groupDocument.Data!;
                var tagsDocument = await _groupsClient.GetAsync<List<Tag>>(group.Links!["tags"].Href);
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

                group.Channel ??= _groupConfig.DefaultChannel;
                return group;
            });
            return info!;
        }
    }
}