using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JsonApi;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PlanningCenter.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPlanningCenter(this IServiceCollection services,
            string? applicationId = null, string? secret = null)
        {
            applicationId ??= Environment.GetEnvironmentVariable("PCO_APPLICATION_ID");
            secret ??= Environment.GetEnvironmentVariable("PCO_SECRET");

            var headerValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{applicationId}:{secret}"));
            var header = new AuthenticationHeaderValue("Basic", headerValue);

            Action<HttpClient> ConfigureClient(string apiPrefix)
            {
                return (client) =>
                {
                    client.BaseAddress = new Uri($"https://api.planningcenteronline.com/{PeopleClient.ApiPrefix}/v2/");
                    client.DefaultRequestHeaders.Authorization = header;
                };
            }
            
            services.AddHttpClient<PeopleClient>(ConfigureClient(PeopleClient.ApiPrefix));
            services.AddHttpClient<CheckInsClient>(ConfigureClient(CheckInsClient.ApiPrefix));
            return services;
        }
    }

    
    public class CheckInsClient : PlanningCenterClient
    {
        public const string ApiPrefix = "check-ins";

        public CheckInsClient(HttpClient httpClient) : base(httpClient)
        {
        }
    }
    
    public class PeopleClient : PlanningCenterClient
    {
        public const string ApiPrefix = "people";

        public PeopleClient(HttpClient httpClient) : base(httpClient)
        {
        }
    }

    public class PlanningCenterClient
    {
        private readonly JsonSerializerSettings _jsonSettings;
        public HttpClient HttpClient { get; }

        public PlanningCenterClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
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
