using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JsonApi;
using JsonApiSerializer.JsonApi;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Orbit.Api.Model;
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
    
    
    public class OrbitApiClient : ApiClientBase
    {
        public OrbitApiClient(HttpClient httpClient, ILogger log) : base(log, httpClient)
        {
        }
        
        public async Task<DocumentRoot<T>> PostAsync<T>(string url, object data)
        {
            return await UploadAsync<T>(url, data, HttpMethod.Post);
        }
        
        public async Task<DocumentRoot<T>> PutAsync<T>(string url, object data)
        {
            return await UploadAsync<T>(url, data, HttpMethod.Put);
        }

        private async Task<DocumentRoot<T>> UploadAsync<T>(string url, object data, HttpMethod? method)
        {
            var body = JsonConvert.SerializeObject(data, WriteJsonSettings);

            var response = await Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(5, retryAttempt =>
                    {
                        var waitTime = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)); // exponential backoff  

                        Log.Information("{ApiName} Got {HttpStatusCode}, waiting to retry for {WaitTime} seconds",
                            nameof(OrbitApiClient), HttpStatusCode.TooManyRequests, waitTime.Seconds);
                        return waitTime;
                    }
                )
                .ExecuteAsync(async () =>
                {
                    // create this inside the executeAsync because you can't send the same request twice
                    var request = new HttpRequestMessage(method, url)
                    {
                        Content = new StringContent(body)
                    };
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"); // don't forget this!
                    return await HttpClient.SendAsync(request);
                });

            return await ReadResponse<DocumentRoot<T>>(response);
        }

        public record AddActivity(UploadActivity Activity, Identity Identity);
        public async Task CreateActivity(UploadActivity uploadActivity, Identity identity)
        {
            var created = await PostAsync<DocumentRoot<UploadActivity>>("activities", 
                new AddActivity(uploadActivity, identity));
        }
    }
}