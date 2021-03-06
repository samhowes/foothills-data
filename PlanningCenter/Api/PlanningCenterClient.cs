using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using JsonApi;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddHttpClient<GivingClient>(ConfigureClient(GivingClient.ApiPrefix));
            services.AddHttpClient<GroupsClient>(ConfigureClient(GroupsClient.ApiPrefix));
            return services;
        }
    }

    public interface IPerson
    {
        public string? Id { get; set; }
    }
    
    public interface IHavePerson
    {
        IPerson? Person { get; }
    }
    public class PlanningCenterClient : ApiClientBase
    {
        public PlanningCenterClient(HttpClient httpClient, ILogger log) : base(log, httpClient)
        {
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
    
    public class GivingClient : PlanningCenterClient
    {
        public const string ApiPrefix = "giving";

        public GivingClient(HttpClient httpClient, ILogger log) : base(httpClient, log)
        {
        }
    }
    
    public class GroupsClient : PlanningCenterClient
    {
        public const string ApiPrefix = "groups";

        public GroupsClient(HttpClient httpClient, ILogger log) : base(httpClient, log)
        {
        }
    }
}
