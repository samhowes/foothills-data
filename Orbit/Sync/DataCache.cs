using System;
using System.Collections.Generic;
using PlanningCenter.Api;

namespace Sync
{
    public class DataCache
    {
        private readonly Dictionary<Type, Dictionary<string, string>> _caches = new();

        public bool TryGetValue<T>(string key, out string? value)
        {
            var cache = GetCache<T>();

            return cache.TryGetValue(key, out value);
        }

        private Dictionary<string, string> GetCache<T>()
        {
            if (_caches.TryGetValue(typeof(T), out var cache)) return cache;
            
            cache = new Dictionary<string, string>();
            _caches[typeof(T)] = cache;

            return cache;
        }

        public void Set<T>(string key, string value)
        {
            var cache = GetCache<T>();
            cache[key] = value;
        }
    }
    
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
        public static string? EntityId<TEntity>(string activityKey) where TEntity : EntityBase
        {
            var name = typeof(TEntity).Name.ToLower();

            return !activityKey.StartsWith(name)
                ? null
                : activityKey[(name.Length + 1)..];
        }
        
    }
}