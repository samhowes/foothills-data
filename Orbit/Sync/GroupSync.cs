using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public const string DefaultChannel = "Belonging & Community";
        public const string IgnoreTag = "Belonging & Community";
    }

    public abstract class GroupSync<TSource> : Sync<TSource> where TSource : EntityBase
    {
        protected readonly GroupsClient _groupsClient;
        private readonly GroupsConfig _groupsConfig;

        protected GroupSync(SyncDeps deps, GroupsClient groupsClient, GroupsConfig config) : base(deps, groupsClient)
        {
            _groupsClient = groupsClient;
            _groupsConfig = config;
        }

        protected abstract string Endpoint { get; }
        
        public override async Task<DocumentRoot<List<TSource>>> GetInitialDataAsync(string? nextUrl)
        {
            var channelTags = await _groupsClient.GetAsync<List<Tag>>("tag_groups/417552/tags");

            Channels = channelTags.Data.ToDictionary(t => t.Id);

            IgnoreTag = Channels.Values.SingleOrDefault(t => t.Name == "Ignore")!;
            if (IgnoreTag == null)
            {
                throw new PlanningCenterException(
                    $"Failed to find the `{GroupsConfig.IgnoreTag}` tag. Found {string.Join(",", Channels.Values.Select(t => t.Name))}");
            }
            
            return await _groupsClient.GetAsync<List<TSource>>(nextUrl ?? Endpoint);
        }

        protected Tag IgnoreTag;

        protected Dictionary<string,Tag> Channels { get; set; }

        protected async Task<GroupInfo> GetGroupInfo(string groupId)
        {
            return await Deps.Cache.GetOrAddEntity(groupId, async (_) =>
            {
                var groupDocument = await _groupsClient.GetAsync<GroupInfo>($"groups/{groupId}");
                var group = groupDocument.Data!;
                var tagsDocument = await _groupsClient.GetAsync<List<Tag>>(group.Links["tags"].Href);
                group.Tags = tagsDocument.Data;
                foreach (var tag in group.Tags)
                {
                    if (tag.Id == IgnoreTag.Id)
                        group.Ignore = true;
                    if (Channels.TryGetValue(tag.Id, out var chanelTag))
                    {
                        group.Channel = chanelTag.Name.Trim();
                        break;
                    }
                }

                group.Channel ??= GroupsConfig.DefaultChannel;
                return group;
            });
        }
    }
}