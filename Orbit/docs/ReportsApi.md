# Orbit.Api.Api.ReportsApi

All URIs are relative to *https://app.orbit.love/api/v1*

Method | HTTP request | Description
------------- | ------------- | -------------
[**WorkspaceSlugReportsGet**](ReportsApi.md#workspaceslugreportsget) | **GET** /{workspace_slug}/reports | Get a workspace stats

<a name="workspaceslugreportsget"></a>
# **WorkspaceSlugReportsGet**
> void WorkspaceSlugReportsGet (string workspaceSlug, string startDate = null, string endDate = null, string group = null, string activityType = null, string type = null)

Get a workspace stats

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugReportsGetExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new ReportsApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var startDate = startDate_example;  // string |  (optional) 
            var endDate = endDate_example;  // string |  (optional) 
            var group = group_example;  // string |  (optional) 
            var activityType = activityType_example;  // string |  (optional) 
            var type = type_example;  // string |  (optional) 

            try
            {
                // Get a workspace stats
                apiInstance.WorkspaceSlugReportsGet(workspaceSlug, startDate, endDate, group, activityType, type);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ReportsApi.WorkspaceSlugReportsGet: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **workspaceSlug** | **string**|  | 
 **startDate** | **string**|  | [optional] 
 **endDate** | **string**|  | [optional] 
 **group** | **string**|  | [optional] 
 **activityType** | **string**|  | [optional] 
 **type** | **string**|  | [optional] 

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
