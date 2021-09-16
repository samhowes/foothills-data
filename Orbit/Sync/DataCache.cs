using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlanningCenter.Api;
using PlanningCenter.Api.Groups;

namespace Sync
{
    public class DataCache
    {
        private readonly Dictionary<string, object> _cache = new();

        private const string Mapping = "mapping";
        private const string Entity = "entity";
        private string Key<T>(string prefix, string key) => $"{typeof(T).Name}:{prefix}:{key}";
        
        public bool TryGetMapping<T>(string key, out string? value)
            => GetValue(Key<T>(Mapping, key), out value);
        
        public bool TryGetEntity<T>(string key, out T? value) where T : class 
            => GetValue(Key<T>(Entity, key), out value);

        private bool GetValue<T>(string key, out T? value) where T : class
        {
            var exists = _cache.TryGetValue(key, out var cachedValue);
            value = cachedValue as T;
            return exists;
        }

        public void SetMapping<T>(string key, string value)
        {
            _cache[Key<T>(Mapping, key)] = value;
        }
        
        public void SetEntity<T>(T value) where T : EntityBase
        {
            _cache[Key<T>(Entity, value.Id)] = value!;
        }

        public async Task<T> GetOrAddEntity<T>(string key, Func<string, Task<T>> func) where T : EntityBase
        {
            if (!TryGetEntity<T>(key, out var entity))
            {
                entity = await func(key);
                SetEntity(entity);
            }

            return entity!;
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

        public static string GroupLink(Group group)
            => $"https://groups.planningcenteronline.com/groups/{group.Id}";

    }
}