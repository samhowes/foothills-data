# Orbit.Api.Api.WorkspacesApi

All URIs are relative to *https://app.orbit.love/api/v1*

Method | HTTP request | Description
------------- | ------------- | -------------
[**WorkspacesGet**](WorkspacesApi.md#workspacesget) | **GET** /workspaces | Get all workspaces for the current user
[**WorkspacesWorkspaceSlugGet**](WorkspacesApi.md#workspacesworkspaceslugget) | **GET** /workspaces/{workspace_slug} | Get a workspace

<a name="workspacesget"></a>
# **WorkspacesGet**
> List<Workspace> WorkspacesGet ()

Get all workspaces for the current user

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspacesGetExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new WorkspacesApi();

            try
            {
                // Get all workspaces for the current user
                List&lt;Workspace&gt; result = apiInstance.WorkspacesGet();
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling WorkspacesApi.WorkspacesGet: " + e.Message );
            }
        }
    }
}
```

### Parameters
This endpoint does not need any parameter.

### Return type

[**List<Workspace>**](Workspace.md)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="workspacesworkspaceslugget"></a>
# **WorkspacesWorkspaceSlugGet**
> void WorkspacesWorkspaceSlugGet (string workspaceSlug)

Get a workspace

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspacesWorkspaceSlugGetExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new WorkspacesApi();
            var workspaceSlug = workspaceSlug_example;  // string | 

            try
            {
                // Get a workspace
                apiInstance.WorkspacesWorkspaceSlugGet(workspaceSlug);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling WorkspacesApi.WorkspacesWorkspaceSlugGet: " + e.Message );
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
