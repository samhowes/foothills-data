# Orbit.Api.Api.ActivitiesApi

All URIs are relative to *https://app.orbit.love/api/v1*

Method | HTTP request | Description
------------- | ------------- | -------------
[**WorkspaceSlugActivitiesGet**](ActivitiesApi.md#workspaceslugactivitiesget) | **GET** /{workspace_slug}/activities | List activities for a workspace
[**WorkspaceSlugActivitiesIdGet**](ActivitiesApi.md#workspaceslugactivitiesidget) | **GET** /{workspace_slug}/activities/{id} | Get an activity in the workspace
[**WorkspaceSlugActivitiesPost**](ActivitiesApi.md#workspaceslugactivitiespost) | **POST** /{workspace_slug}/activities | Create a Custom or a Content activity for a new or existing member
[**WorkspaceSlugMembersMemberSlugActivitiesGet**](ActivitiesApi.md#workspaceslugmembersmemberslugactivitiesget) | **GET** /{workspace_slug}/members/{member_slug}/activities | List activities for a member
[**WorkspaceSlugMembersMemberSlugActivitiesIdDelete**](ActivitiesApi.md#workspaceslugmembersmemberslugactivitiesiddelete) | **DELETE** /{workspace_slug}/members/{member_slug}/activities/{id} | Delete a post activity
[**WorkspaceSlugMembersMemberSlugActivitiesIdPut**](ActivitiesApi.md#workspaceslugmembersmemberslugactivitiesidput) | **PUT** /{workspace_slug}/members/{member_slug}/activities/{id} | Update a custom activity for a member
[**WorkspaceSlugMembersMemberSlugActivitiesPost**](ActivitiesApi.md#workspaceslugmembersmemberslugactivitiespost) | **POST** /{workspace_slug}/members/{member_slug}/activities | Create a Custom or a Content activity for a member

<a name="workspaceslugactivitiesget"></a>
# **WorkspaceSlugActivitiesGet**
> List<Activity> WorkspaceSlugActivitiesGet (string workspaceSlug, string activityTags = null, string affiliation = null, string memberTags = null, string orbitLevel = null, string activityType = null, string weight = null, string identity = null, string location = null, string company = null, string startDate = null, string endDate = null, string page = null, string direction = null, string items = null, string sort = null, string type = null)

List activities for a workspace

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugActivitiesGetExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new ActivitiesApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var activityTags = activityTags_example;  // string |  (optional) 
            var affiliation = affiliation_example;  // string |  (optional) 
            var memberTags = memberTags_example;  // string |  (optional) 
            var orbitLevel = orbitLevel_example;  // string |  (optional) 
            var activityType = activityType_example;  // string |  (optional) 
            var weight = weight_example;  // string |  (optional) 
            var identity = identity_example;  // string |  (optional) 
            var location = location_example;  // string |  (optional) 
            var company = company_example;  // string |  (optional) 
            var startDate = startDate_example;  // string |  (optional) 
            var endDate = endDate_example;  // string |  (optional) 
            var page = page_example;  // string |  (optional) 
            var direction = direction_example;  // string |  (optional) 
            var items = items_example;  // string |  (optional) 
            var sort = sort_example;  // string |  (optional) 
            var type = type_example;  // string |  (optional) 

            try
            {
                // List activities for a workspace
                List&lt;Activity&gt; result = apiInstance.WorkspaceSlugActivitiesGet(workspaceSlug, activityTags, affiliation, memberTags, orbitLevel, activityType, weight, identity, location, company, startDate, endDate, page, direction, items, sort, type);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ActivitiesApi.WorkspaceSlugActivitiesGet: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **workspaceSlug** | **string**|  | 
 **activityTags** | **string**|  | [optional] 
 **affiliation** | **string**|  | [optional] 
 **memberTags** | **string**|  | [optional] 
 **orbitLevel** | **string**|  | [optional] 
 **activityType** | **string**|  | [optional] 
 **weight** | **string**|  | [optional] 
 **identity** | **string**|  | [optional] 
 **location** | **string**|  | [optional] 
 **company** | **string**|  | [optional] 
 **startDate** | **string**|  | [optional] 
 **endDate** | **string**|  | [optional] 
 **page** | **string**|  | [optional] 
 **direction** | **string**|  | [optional] 
 **items** | **string**|  | [optional] 
 **sort** | **string**|  | [optional] 
 **type** | **string**|  | [optional] 

### Return type

[**List<Activity>**](Activity.md)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="workspaceslugactivitiesidget"></a>
# **WorkspaceSlugActivitiesIdGet**
> void WorkspaceSlugActivitiesIdGet (string workspaceSlug, string id)

Get an activity in the workspace

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugActivitiesIdGetExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new ActivitiesApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var id = id_example;  // string | 

            try
            {
                // Get an activity in the workspace
                apiInstance.WorkspaceSlugActivitiesIdGet(workspaceSlug, id);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ActivitiesApi.WorkspaceSlugActivitiesIdGet: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **workspaceSlug** | **string**|  | 
 **id** | **string**|  | 

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="workspaceslugactivitiespost"></a>
# **WorkspaceSlugActivitiesPost**
> void WorkspaceSlugActivitiesPost (string workspaceSlug, ActivityAndIdentity body = null)

Create a Custom or a Content activity for a new or existing member

Use this method when you know an identity of the member (github, email, twitter, etc.) but not their Orbit ID. Pass fields in the member object to update the member in addition to creating the activity.

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugActivitiesPostExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new ActivitiesApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var body = new ActivityAndIdentity(); // ActivityAndIdentity |  (optional) 

            try
            {
                // Create a Custom or a Content activity for a new or existing member
                apiInstance.WorkspaceSlugActivitiesPost(workspaceSlug, body);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ActivitiesApi.WorkspaceSlugActivitiesPost: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **workspaceSlug** | **string**|  | 
 **body** | [**ActivityAndIdentity**](ActivityAndIdentity.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="workspaceslugmembersmemberslugactivitiesget"></a>
# **WorkspaceSlugMembersMemberSlugActivitiesGet**
> void WorkspaceSlugMembersMemberSlugActivitiesGet (string workspaceSlug, string memberSlug, string page = null, string direction = null, string items = null, string sort = null, string activityType = null, string type = null)

List activities for a member

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugMembersMemberSlugActivitiesGetExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new ActivitiesApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var memberSlug = memberSlug_example;  // string | 
            var page = page_example;  // string |  (optional) 
            var direction = direction_example;  // string |  (optional) 
            var items = items_example;  // string |  (optional) 
            var sort = sort_example;  // string |  (optional) 
            var activityType = activityType_example;  // string |  (optional) 
            var type = type_example;  // string |  (optional) 

            try
            {
                // List activities for a member
                apiInstance.WorkspaceSlugMembersMemberSlugActivitiesGet(workspaceSlug, memberSlug, page, direction, items, sort, activityType, type);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ActivitiesApi.WorkspaceSlugMembersMemberSlugActivitiesGet: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **workspaceSlug** | **string**|  | 
 **memberSlug** | **string**|  | 
 **page** | **string**|  | [optional] 
 **direction** | **string**|  | [optional] 
 **items** | **string**|  | [optional] 
 **sort** | **string**|  | [optional] 
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
<a name="workspaceslugmembersmemberslugactivitiesiddelete"></a>
# **WorkspaceSlugMembersMemberSlugActivitiesIdDelete**
> void WorkspaceSlugMembersMemberSlugActivitiesIdDelete (string workspaceSlug, string memberSlug, string id)

Delete a post activity

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugMembersMemberSlugActivitiesIdDeleteExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new ActivitiesApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var memberSlug = memberSlug_example;  // string | 
            var id = id_example;  // string | 

            try
            {
                // Delete a post activity
                apiInstance.WorkspaceSlugMembersMemberSlugActivitiesIdDelete(workspaceSlug, memberSlug, id);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ActivitiesApi.WorkspaceSlugMembersMemberSlugActivitiesIdDelete: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **workspaceSlug** | **string**|  | 
 **memberSlug** | **string**|  | 
 **id** | **string**|  | 

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="workspaceslugmembersmemberslugactivitiesidput"></a>
# **WorkspaceSlugMembersMemberSlugActivitiesIdPut**
> void WorkspaceSlugMembersMemberSlugActivitiesIdPut (string workspaceSlug, string memberSlug, string id, Activity body = null)

Update a custom activity for a member

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugMembersMemberSlugActivitiesIdPutExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new ActivitiesApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var memberSlug = memberSlug_example;  // string | 
            var id = id_example;  // string | 
            var body = new Activity(); // Activity |  (optional) 

            try
            {
                // Update a custom activity for a member
                apiInstance.WorkspaceSlugMembersMemberSlugActivitiesIdPut(workspaceSlug, memberSlug, id, body);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ActivitiesApi.WorkspaceSlugMembersMemberSlugActivitiesIdPut: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **workspaceSlug** | **string**|  | 
 **memberSlug** | **string**|  | 
 **id** | **string**|  | 
 **body** | [**Activity**](Activity.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="workspaceslugmembersmemberslugactivitiespost"></a>
# **WorkspaceSlugMembersMemberSlugActivitiesPost**
> void WorkspaceSlugMembersMemberSlugActivitiesPost (string workspaceSlug, string memberSlug, CustomOrPostActivity body = null)

Create a Custom or a Content activity for a member

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugMembersMemberSlugActivitiesPostExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new ActivitiesApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var memberSlug = memberSlug_example;  // string | 
            var body = new CustomOrPostActivity(); // CustomOrPostActivity |  (optional) 

            try
            {
                // Create a Custom or a Content activity for a member
                apiInstance.WorkspaceSlugMembersMemberSlugActivitiesPost(workspaceSlug, memberSlug, body);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ActivitiesApi.WorkspaceSlugMembersMemberSlugActivitiesPost: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **workspaceSlug** | **string**|  | 
 **memberSlug** | **string**|  | 
 **body** | [**CustomOrPostActivity**](CustomOrPostActivity.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
