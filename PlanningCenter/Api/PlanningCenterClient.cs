using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using JsonApiSerializer;
using JsonApiSerializer.ContractResolvers;
using JsonApiSerializer.JsonApi;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using Serilog;

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
                    client.BaseAddress = new Uri($"https://api.planningcenteronline.com/{apiPrefix}/v2/");
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

        public CheckInsClient(HttpClient httpClient, ILogger log) : base(httpClient, log)
        {
        }
    }
    
    public class PeopleClient : PlanningCenterClient
    {
        public const string ApiPrefix = "people";

        public PeopleClient(HttpClient httpClient, ILogger log) : base(httpClient, log)
        {
        }
    }

    public class PlanningCenterClient
    {
        public readonly JsonSerializerSettings _jsonSettings;
        private ILogger _log;
        public HttpClient HttpClient { get; }

        public PlanningCenterClient(HttpClient httpClient, ILogger log)
        {
            HttpClient = httpClient;
            _log = log;
            _jsonSettings = new JsonApiSerializerSettings();
            ((JsonApiContractResolver) _jsonSettings.ContractResolver!).NamingStrategy = new SnakeCaseNamingStrategy();
        }
        
        public async Task<DocumentRoot<T>> GetAsync<T>(string url, params (string key, string value)[] parameters)
        {
            if (parameters.Length > 0)
            {
                var query = string.Join("&",
                    parameters.Select(p => $"{HttpUtility.UrlEncode(p.key)}={HttpUtility.UrlEncode(p.value)}"));
                url += "?" + query;
            }
            
            var response = await Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(5, retryAttempt =>
                    {
                        var waitTime = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                        _log.Information("Got {HttpStatusCode}, waiting to retry for {WaitTime} seconds", 
                            HttpStatusCode.TooManyRequests, waitTime.Seconds);
                        return waitTime;
                    }
                    // exponential backoff 
                )
                .ExecuteAsync(async () => await HttpClient.GetAsync(url));
            
            var typed = await ReadResponse<DocumentRoot<T>>(response);
            return typed;
        }
        
        private async Task<T> ReadResponse<T>(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new PlanningCenterException($"Http failure for {response.RequestMessage!.RequestUri}: {body}");
            }
            
            var typed = JsonConvert.DeserializeObject<T>(body, _jsonSettings)!;
            return typed;
        }
        
        // public async Task<List<Resource<T>>> GetAllAsync<T>(string url, params (string key, string value)[] parameters)
        // {
        //     var current = await GetAsync<T>(url, parameters);
        //     var data = current.Data;
        //     while (!string.IsNullOrEmpty(current.Links.Next))
        //     {
        //         current = await GetAsync<T>(current.Links.Next);
        //         data.AddRange(current.Data);
        //     }
        //
        //     return data;
        // }
    }
    
    public class PlanningCenterException : Exception
    {
        public PlanningCenterException(string message) : base(message)
        {
        }
    }
}
