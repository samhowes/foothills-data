using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Orbit.Api
{
    public abstract class Resource
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public Links Links { get; set; }
    }

    public class Resource<T> : Resource
    {
        public T Attributes { get; set; }
    }

    public class GenericResource : Resource
    {
        public Dictionary<string, string> Attributes { get; set; }
    }
    
    public class Response<T>
    {
        public Links Links { get; set; }
        
        public T Data { get; set; } 
        
        public Metadata Meta { get; set; }
    }
 
    public class Next
    {
        public int Offset { get; set; }
    }

    public class Parent
    {
        public string Id { get; set; }
        public string Type { get; set; }
    }

    public class Metadata
    {
        public int TotalCount { get; set; }
        public int Count { get; set; }
        public Next Next { get; set; }
        public List<string> CanOrderBy { get; set; }
        public List<string> CanQueryBy { get; set; }
        public List<string> CanInclude { get; set; }
        public List<string> CanFilter { get; set; }
        public Parent Parent { get; set; }
    }

    public class Links : Dictionary<string, string>
    {
        public string Self => this["self"];
        public string Next => this["self"];
    }
    
    public class OrbitApiClient
    {
        private readonly JsonSerializerSettings _jsonSettings;
        public HttpClient HttpClient { get; }

        public static OrbitApiClient Create(string apiPrefix, string? apiToken = null)
        {
            apiToken ??= Environment.GetEnvironmentVariable("ORBIT_API_TOKEN");
            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiToken);
            return new OrbitApiClient(client, apiPrefix);
        }

        public OrbitApiClient(HttpClient httpClient, string apiPrefix)
        {
            HttpClient = httpClient;
            HttpClient.BaseAddress = new Uri($"https://app.orbit.love/api/v1/");
            _jsonSettings = new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        }

        public async Task<Response<List<Resource<T>>>> GetAsync<T>(string url)
        {
            var response = await HttpClient.GetAsync(url);
            return await ReadResponse<Response<List<Resource<T>>>>(response);
        }

        private async Task<T> ReadResponse<T>(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new OrbitApiException($"Http failure for {response.RequestMessage!.RequestUri}: {body}");
            }
            var typed = JsonConvert.DeserializeObject<T>(body, _jsonSettings)!;
            return typed;
        }

        public async Task<Response<Resource<T>>> PostAsync<T>(string url, object data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(JsonConvert.SerializeObject(data, _jsonSettings));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await HttpClient.SendAsync(request);
            return await ReadResponse<Response<Resource<T>>>(response);
        }
    }

    public class OrbitApiException : Exception
    {
        public OrbitApiException(string message) : base(message)
        {
            
        }
    }
}