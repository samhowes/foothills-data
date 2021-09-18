using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Humanizer;
using JsonApi;
using PlanningCenter.Api;

namespace Sync
{
    public class DataCache
    {
        private readonly Dictionary<string, object> _cache = new(StringComparer.OrdinalIgnoreCase);

        private const string Mapping = "mapping";
        private const string Entity = "entity";
        private string Key<T>(string prefix, string key) => Key(typeof(T).Name, prefix, key);
        private string Key(string entityName, string prefix, string key) => $"{entityName}:{prefix}:{key}";
        
        public bool TryGetMapping<T>(string key, out string? value)
            => GetValue(Key<T>(Mapping, key), out value);
        
        public bool TryGetMapping<T>(T entity, out string? value) where T : EntityBase
            => GetValue(Key<T>(Mapping, entity.Id!), out value);
        
        public bool TryGetEntity<T>(string key, out T? value) where T : class 
            => GetValue(Key<T>(Entity, key), out value);

        private bool GetValue<T>(string key, out T? value) where T : class
        {
            lock (_cache)
            {
                var exists = _cache.TryGetValue(key, out var cachedValue);
                value = cachedValue as T;
                return exists;
            }
        }

        public void SetMapping<T>(string key, string value) => SetMapping(typeof(T).Name, key, value);
        
        public void SetMapping(string entityName, string key, string value)
        {
            lock (_cache)
            {
                _cache[Key(entityName, Mapping, key)] = value;    
            }
        }
        
        public void SetEntity<T>(T value) where T : EntityBase
        {
            lock (_cache)
            {
                _cache[Key<T>(Entity, value.Id!)] = value!;    
            }
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
        
        public async Task<string?> GetOrAddMapping<T>(string entityId, Func<string, Task<string?>> func) where T : EntityBase
        {
            if (!TryGetMapping<T>(entityId, out var mapped))
            {
                mapped = await func(entityId);
                if (mapped != null)
                    SetMapping<T>(entityId, mapped);
            }

            return mapped!;
        }
    }
}