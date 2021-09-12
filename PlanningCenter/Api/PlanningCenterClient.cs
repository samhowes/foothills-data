﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PlanningCenter.Api
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
        public string? SafeGet(string key)
        {
            TryGetValue(key, out var value);
            return value;
        }

        public string Self => SafeGet("self")!;
        public string? Next => SafeGet("next");
    }


    public static class Constants
    {
        public const string CheckInsPrefix = "check-ins";
        public const string PeoplePrefix = "people";
    }

    public class PlanningCenterClient
    {
        private readonly JsonSerializerSettings _jsonSettings;
        public HttpClient HttpClient { get; }

        public static PlanningCenterClient Create(string apiPrefix, string? applicationId = null, string? secret = null)
        {
            applicationId ??= Environment.GetEnvironmentVariable("PCO_APPLICATION_ID"); 
            secret ??= Environment.GetEnvironmentVariable("PCO_SECRET");

            var headerValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{applicationId}:{secret}"));
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", headerValue);
            return new PlanningCenterClient(client, apiPrefix);
        }
        public PlanningCenterClient(HttpClient httpClient, string apiPrefix)
        {
            HttpClient = httpClient;
            HttpClient.BaseAddress = new Uri($"https://api.planningcenteronline.com/{apiPrefix}/v2/");
            _jsonSettings = new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        }

        public Task<Response<List<Resource<Dictionary<string, string>>>>> GetGenericAsync(string url)
            => GetAsync<Dictionary<string, string>>(url);

        public async Task<Response<List<Resource<T>>>> GetAsync<T>(string url)
        {
            var response = await HttpClient.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            var typed = JsonConvert.DeserializeObject<Response<List<Resource<T>>>>(body, _jsonSettings)!;
            return typed;
        }
    }
}
