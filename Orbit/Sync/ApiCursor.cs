using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonApi;
using JsonApiSerializer.JsonApi;

namespace Sync
{
    public abstract class ApiCursor
    {
        public Meta Meta { get; protected set; } = null!;

        public string? NextUrl { get; protected set; }
        public abstract Task InitializeAsync();
    }
    
    public class ApiCursor<T> : ApiCursor
    {

        private readonly ApiClientBase _client;
        private readonly Func<T, bool> _shouldContinue;
        private DocumentRoot<List<T>>? _batch;

        public ApiCursor(ApiClientBase client, string nextUrl, string? name= null, Func<T, bool>? shouldContinue = null)
        {
            NextUrl = nextUrl;
            _client = client;
            _shouldContinue = shouldContinue ?? ((_) => true);
            Name = name ?? typeof(T).Name;
        }

        public string Name { get; }
        
        public override async Task InitializeAsync()
        {
            _batch = await _client.GetAsync<List<T>>(NextUrl);
            Meta = _batch.Meta;
        }

        public IEnumerable<T>? Data => _batch?.Data.Where(_shouldContinue);

        public Task<bool> FetchNextAsync() => FetchAsync(_batch?.Links.Next());

        public Task<bool> FetchPreviousAsync() => FetchAsync(_batch?.Links.Prev());

        private async Task<bool> FetchAsync(string? url)
        {
            if (_batch == null)
            {
                await InitializeAsync();
                return true;
            }
            NextUrl = url;
            if (string.IsNullOrEmpty(NextUrl)) return false;

            _batch = await _client.GetAsync<List<T>>(NextUrl);
            return true;
        }
        
        public async IAsyncEnumerable<T> GetAllAsync()
        {
            for (;;)
            {
                if (!await FetchNextAsync()) yield break;
                foreach (var item in Data!)
                {
                    yield return item;
                }
            }
        }
    }
}