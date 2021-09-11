# Orbit.Api.Api.ActivityTypesApi

All URIs are relative to *https://app.orbit.love/api/v1*

Method | HTTP request | Description
------------- | ------------- | -------------
[**WorkspaceSlugActivityTypesGet**](ActivityTypesApi.md#workspaceslugactivitytypesget) | **GET** /{workspace_slug}/activity_types | List all activity types for a workspace

<a name="workspaceslugactivitytypesget"></a>
# **WorkspaceSlugActivityTypesGet**
> void WorkspaceSlugActivityTypesGet (string workspaceSlug)

List all activity types for a workspace

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugActivityTypesGetExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new ActivityTypesApi();
            var workspaceSlug = workspaceSlug_example;  // string | 

            try
            {
                // List all activity types for a workspace
                apiInstance.WorkspaceSlugActivityTypesGet(workspaceSlug);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ActivityTypesApi.WorkspaceSlugActivityTypesGet: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **workspaceSlug** | **string**|  | 

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
