using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Api
{
    public class PlanningCenterResponse<T>
    {
        public T Data { get; set; } 
    }

    public class PlaningCenterLinks : Dictionary<string, string>
    {
        public string Self => this["self"];
    }

    public abstract class PlanningCenterObject
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public PlaningCenterLinks Links { get; set; }
    }

    public class GenericPlanningCenterObject : PlanningCenterObject
    {
        public Dictionary<string, string> Attributes { get; set; }
    }

    public class PlanningCenterClient
    {
        public HttpClient HttpClient { get; }

        public static PlanningCenterClient Create(string? applicationId = null, string? secret = null)
        {
            applicationId ??= Environment.GetEnvironmentVariable("PC_APPLICATION_ID"); 
            secret ??= Environment.GetEnvironmentVariable("PC_SECRET");

            var headerValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{applicationId}:{secret}"));
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", headerValue);
            return new PlanningCenterClient(client);
        }
        public PlanningCenterClient(HttpClient httpHttpClient)
        {
            HttpClient = httpHttpClient;
            HttpClient.BaseAddress = new Uri("https://api.planningcenteronline.com");
        }

        public async Task<PlanningCenterResponse<GenericPlanningCenterObject>> GetGenericAsync(string url)
        {
            var response = await HttpClient.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            var typed = JsonConvert.DeserializeObject<PlanningCenterResponse<GenericPlanningCenterObject>>(body)!;
            return typed;
        }
    }
}
