using System.Text.RegularExpressions;
using JsonApi;

namespace Sync
{
    public static class OrbitUtil
    {
        public static string? TrimLink(string? href)
        {
            if (href == null) return href;
            var index = href.IndexOf('/');
            index = href.IndexOf('/', index + 1);
            return href[(index + 1)..];
        }
        
        public static string ActivityKey<TEntity>(TEntity entity) where TEntity : EntityBase
        {
            return $"{typeof(TEntity).Name.ToLower()}/{entity.Id}";
        }
        public static (string entityName, string id)? EntityId(string? activityKey)
        {
            if (activityKey == null) return null;
            var parts = activityKey.Split('/');
            if (parts.Length < 2) return null;
            return (parts[0], parts[1]);
        }

        public static string ChannelTag(string channel)
        {
            var cleaned = Regex.Replace(channel.Replace(' ', '-'), @"([&])", "");
            return $"channel:{cleaned}";
        }
    }
}