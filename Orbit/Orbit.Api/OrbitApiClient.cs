using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JsonApiSerializer.JsonApi;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using Serilog;

namespace Orbit.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOrbitApi(this IServiceCollection services, string workspaceSlug,
            string? apiToken = null)
        {
            apiToken ??= Environment.GetEnvironmentVariable("ORBIT_API_TOKEN");
            services.AddHttpClient<OrbitApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https://app.orbit.love/api/v1/{workspaceSlug}/");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiToken);
            });
            return services;
        }
    }

    public class OrbitApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _jsonSettings;
        private ILogger _log;


        public OrbitApiClient(HttpClient httpClient, ILogger log)
        {
            _httpClient = httpClient;
            _log = log;
            _jsonSettings = new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        }

        public async Task<DocumentRoot<T>> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            return await ReadResponse<T>(response);
        }

        private async Task<DocumentRoot<T>> ReadResponse<T>(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new OrbitApiException($"Http failure for {response.RequestMessage!.RequestUri}: {body}");
            }

            var typed = JsonConvert.DeserializeObject<DocumentRoot<T>>(body, _jsonSettings)!;
            return typed;
        }

        public async Task<DocumentRoot<T>> PostAsync<T>(string url, object data)
        {
            var body = JsonConvert.SerializeObject(data, _jsonSettings);
            
            var response = await Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(5, retryAttempt =>
                    {
                        var waitTime = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)); // exponential backoff  
                        
                        _log.Information("{ApiName} Got {HttpStatusCode}, waiting to retry for {WaitTime} seconds",
                            nameof(OrbitApiClient), HttpStatusCode.TooManyRequests, waitTime.Seconds);
                        return waitTime;
                    }
                )
                .ExecuteAsync(async () =>
                {
                    // create this inside the executeAsync because you can't send the same request twice
                    var request = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new StringContent(body)
                    };
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"); // don't forget this!
                    return await _httpClient.SendAsync(request);
                });
            
            return await ReadResponse<T>(response);
        }
    }

    public class OrbitApiException : Exception
    {
        public OrbitApiException(string message) : base(message)
        {
        }
    }
}