using System;
using System.Text.RegularExpressions;
using Humanizer;
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
            return $"{typeof(TEntity).Name.ToLower()}:{entity.Id}";
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

        public const string DateFormatString = "yyyy-MM-ddTHH:mm:ssZ";
        
        public static string FormatDate(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString(DateFormatString);
        }

        private static readonly Regex Cleaner = new(@"([\W])" , RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static string CleanChannelName(string channel)
        {
            var cleaned = Cleaner.Replace(channel, " ");
            return cleaned.Trim().Pascalize();
        }

        public static string KeyTag(string key) => $"key:{key.Replace('/', ':')}";

        public static string ActivityTypeTag(string activityType)
        {
            return $"custom_type:{activityType.Kebaberize()}";
        }
    }
}