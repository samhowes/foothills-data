using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JsonApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PlanningCenter.Api
{
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
