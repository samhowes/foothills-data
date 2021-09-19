using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using JsonApiSerializer;
using JsonApiSerializer.ContractResolvers;
using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using Serilog;

namespace JsonApi
{
    public static class UrlUtil
    {
        public static string MakeUrl(string baseUrl, params (string key, string value)[] parameters)
        {
            var url = new StringBuilder(baseUrl);
            if (parameters.Length > 0)
            {
                url.Append("?");
                foreach (var param in parameters)
                {
                    url.AppendFormat("{0}={1}",
                        HttpUtility.UrlEncode(param.key),
                        HttpUtility.UrlEncode(param.value));
                }
            }

            return url.ToString();
        }
    }

    public class ApiClientBase
    {
        public static JsonSerializerSettings CreateJsonSettings()
        {
            var jsonSettings = new JsonApiSerializerSettings();
            ((JsonApiContractResolver)jsonSettings.ContractResolver)!.NamingStrategy = new SnakeCaseNamingStrategy();
            return jsonSettings;
        }
        
        protected readonly ILogger Log;
        protected readonly HttpClient HttpClient;
        protected readonly JsonSerializerSettings ReadJsonSettings;
        protected readonly JsonSerializerSettings WriteJsonSettings;

        protected ApiClientBase(ILogger log, HttpClient httpClient)
        {
            Log = log;
            HttpClient = httpClient;
            ReadJsonSettings = CreateJsonSettings();
            WriteJsonSettings = new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        }

        public async IAsyncEnumerable<DocumentRoot<T>> GetAllAsync<T>(string url)
        {
            var data = await GetAsync<T>(url);
            yield return data;
            while (!string.IsNullOrEmpty(data.Links.Next()))
            {
                data = await GetAsync<T>(data.Links.Next());
                yield return data;
            }
        }
        
        public async Task<DocumentRoot<T>> GetAsync<T>(string url)
        {
            var response = await Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(5, retryAttempt =>
                    {
                        var waitTime = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                        Log.Information("Got {HttpStatusCode}, waiting to retry for {WaitTime} seconds", 
                            HttpStatusCode.TooManyRequests, waitTime.Seconds);
                        return waitTime;
                    }
                    // exponential backoff 
                )
                .ExecuteAsync(async () => await HttpClient.GetAsync(url));
            
            var typed = await ReadResponse<DocumentRoot<T>>(response);
            return typed;
        }

        protected async Task<T> ReadResponse<T>(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException($"Http failure for {response.RequestMessage!.RequestUri}: {body}");
            }
            
            var typed = JsonConvert.DeserializeObject<T>(body, ReadJsonSettings)!;
            return typed;
        }
    }
    
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message)
        {
        }
    }
}