using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using Orbit.Api;
using Orbit.Api.Api;
using Orbit.Api.Client;
using PlanningCenter.Api;
using PlanningCenter.Api.CheckIns;

namespace Sync
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var checkinsClient = PlanningCenterClient.Create(Constants.CheckInsPrefix);
            // Configure API key authorization: bearer
            Configuration.Default.ApiKey.Add("Authorization", "Bearer " + Environment.GetEnvironmentVariable("ORBIT_API_TOKEN"));
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.ApiKeyPrefix.Add("Authorization", "Bearer");

            var apiInstance = new ActivitiesApi();
            var workspaceSlug = "sam-workspace";  // string | 
            // var activityTags = activityTags_example;  // string |  (optional) 
            // var affiliation = affiliation_example;  // string |  (optional) 
            // var memberTags = memberTags_example;  // string |  (optional) 
            // var orbitLevel = orbitLevel_example;  // string |  (optional) 
            // var activityType = activityType_example;  // string |  (optional) 
            // var weight = weight_example;  // string |  (optional) 
            // var identity = identity_example;  // string |  (optional) 
            // var location = location_example;  // string |  (optional) 
            // var company = company_example;  // string |  (optional) 
            // var startDate = startDate_example;  // string |  (optional) 
            // var endDate = endDate_example;  // string |  (optional) 
            // var page = page_example;  // string |  (optional) 
            // var direction = direction_example;  // string |  (optional) 
            // var items = items_example;  // string |  (optional) 
            // var sort = sort_example;  // string |  (optional) 
            // var type = type_example;  // string |  (optional) 

            // List activities for a workspace
            
            var result = await apiInstance.WorkspaceSlugActivitiesGetAsync(workspaceSlug);

            // var checkins = await checkinsClient.GetAsync<CheckIn>("check_ins");
            
            
        }
    }
}
