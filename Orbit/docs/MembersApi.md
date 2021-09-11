# Orbit.Api.Api.MembersApi

All URIs are relative to *https://app.orbit.love/api/v1*

Method | HTTP request | Description
------------- | ------------- | -------------
[**WorkspaceSlugMembersFindGet**](MembersApi.md#workspaceslugmembersfindget) | **GET** /{workspace_slug}/members/find | Find a member by an identity
[**WorkspaceSlugMembersGet**](MembersApi.md#workspaceslugmembersget) | **GET** /{workspace_slug}/members | List members in a workspace
[**WorkspaceSlugMembersMemberSlugDelete**](MembersApi.md#workspaceslugmembersmemberslugdelete) | **DELETE** /{workspace_slug}/members/{member_slug} | Delete a member
[**WorkspaceSlugMembersMemberSlugGet**](MembersApi.md#workspaceslugmembersmemberslugget) | **GET** /{workspace_slug}/members/{member_slug} | Get a member
[**WorkspaceSlugMembersMemberSlugIdentitiesDelete**](MembersApi.md#workspaceslugmembersmemberslugidentitiesdelete) | **DELETE** /{workspace_slug}/members/{member_slug}/identities | Remove identity from a member
[**WorkspaceSlugMembersMemberSlugIdentitiesPost**](MembersApi.md#workspaceslugmembersmemberslugidentitiespost) | **POST** /{workspace_slug}/members/{member_slug}/identities | Add identity to a member
[**WorkspaceSlugMembersMemberSlugPut**](MembersApi.md#workspaceslugmembersmemberslugput) | **PUT** /{workspace_slug}/members/{member_slug} | Update a member
[**WorkspaceSlugMembersPost**](MembersApi.md#workspaceslugmemberspost) | **POST** /{workspace_slug}/members | Create or update a member

<a name="workspaceslugmembersfindget"></a>
# **WorkspaceSlugMembersFindGet**
> void WorkspaceSlugMembersFindGet (string workspaceSlug, string source = null, string sourceHost = null, string uid = null, string username = null, string email = null, string github = null)

Find a member by an identity

Provide a source and one of username/uid/email params to return a member with that identity, if one exists. Common values for source include github, twitter, and email.

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugMembersFindGetExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new MembersApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var source = source_example;  // string |  (optional) 
            var sourceHost = sourceHost_example;  // string |  (optional) 
            var uid = uid_example;  // string |  (optional) 
            var username = username_example;  // string |  (optional) 
            var email = email_example;  // string |  (optional) 
            var github = github_example;  // string | Deprecated, please use source=github and username=<username> instead (optional) 

            try
            {
                // Find a member by an identity
                apiInstance.WorkspaceSlugMembersFindGet(workspaceSlug, source, sourceHost, uid, username, email, github);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling MembersApi.WorkspaceSlugMembersFindGet: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **workspaceSlug** | **string**|  | 
 **source** | **string**|  | [optional] 
 **sourceHost** | **string**|  | [optional] 
 **uid** | **string**|  | [optional] 
 **username** | **string**|  | [optional] 
 **email** | **string**|  | [optional] 
 **github** | **string**| Deprecated, please use source&#x3D;github and username&#x3D;&lt;username&gt; instead | [optional] 

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="workspaceslugmembersget"></a>
# **WorkspaceSlugMembersGet**
> void WorkspaceSlugMembersGet (string workspaceSlug, string activityTags = null, string affiliation = null, string memberTags = null, string orbitLevel = null, string activityType = null, string weight = null, string identity = null, string location = null, string company = null, string startDate = null, string endDate = null, string query = null, string page = null, string direction = null, string items = null, string activitiesCountMin = null, string activitiesCountMax = null, string sort = null, string type = null)

List members in a workspace

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugMembersGetExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new MembersApi();
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
            var query = query_example;  // string |  (optional) 
            var page = page_example;  // string |  (optional) 
            var direction = direction_example;  // string |  (optional) 
            var items = items_example;  // string |  (optional) 
            var activitiesCountMin = activitiesCountMin_example;  // string |  (optional) 
            var activitiesCountMax = activitiesCountMax_example;  // string |  (optional) 
            var sort = sort_example;  // string |  (optional) 
            var type = type_example;  // string |  (optional) 

            try
            {
                // List members in a workspace
                apiInstance.WorkspaceSlugMembersGet(workspaceSlug, activityTags, affiliation, memberTags, orbitLevel, activityType, weight, identity, location, company, startDate, endDate, query, page, direction, items, activitiesCountMin, activitiesCountMax, sort, type);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling MembersApi.WorkspaceSlugMembersGet: " + e.Message );
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
 **query** | **string**|  | [optional] 
 **page** | **string**|  | [optional] 
 **direction** | **string**|  | [optional] 
 **items** | **string**|  | [optional] 
 **activitiesCountMin** | **string**|  | [optional] 
 **activitiesCountMax** | **string**|  | [optional] 
 **sort** | **string**|  | [optional] 
 **type** | **string**|  | [optional] 

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="workspaceslugmembersmemberslugdelete"></a>
# **WorkspaceSlugMembersMemberSlugDelete**
> void WorkspaceSlugMembersMemberSlugDelete (string workspaceSlug, string memberSlug)

Delete a member

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugMembersMemberSlugDeleteExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new MembersApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var memberSlug = memberSlug_example;  // string | 

            try
            {
                // Delete a member
                apiInstance.WorkspaceSlugMembersMemberSlugDelete(workspaceSlug, memberSlug);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling MembersApi.WorkspaceSlugMembersMemberSlugDelete: " + e.Message );
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

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="workspaceslugmembersmemberslugget"></a>
# **WorkspaceSlugMembersMemberSlugGet**
> void WorkspaceSlugMembersMemberSlugGet (string workspaceSlug, string memberSlug)

Get a member

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugMembersMemberSlugGetExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new MembersApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var memberSlug = memberSlug_example;  // string | 

            try
            {
                // Get a member
                apiInstance.WorkspaceSlugMembersMemberSlugGet(workspaceSlug, memberSlug);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling MembersApi.WorkspaceSlugMembersMemberSlugGet: " + e.Message );
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

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="workspaceslugmembersmemberslugidentitiesdelete"></a>
# **WorkspaceSlugMembersMemberSlugIdentitiesDelete**
> void WorkspaceSlugMembersMemberSlugIdentitiesDelete (string workspaceSlug, string memberSlug, Identity body = null)

Remove identity from a member

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugMembersMemberSlugIdentitiesDeleteExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new MembersApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var memberSlug = memberSlug_example;  // string | 
            var body = new Identity(); // Identity |  (optional) 

            try
            {
                // Remove identity from a member
                apiInstance.WorkspaceSlugMembersMemberSlugIdentitiesDelete(workspaceSlug, memberSlug, body);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling MembersApi.WorkspaceSlugMembersMemberSlugIdentitiesDelete: " + e.Message );
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
 **body** | [**Identity**](Identity.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="workspaceslugmembersmemberslugidentitiespost"></a>
# **WorkspaceSlugMembersMemberSlugIdentitiesPost**
> void WorkspaceSlugMembersMemberSlugIdentitiesPost (string workspaceSlug, string memberSlug, Identity body = null)

Add identity to a member

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugMembersMemberSlugIdentitiesPostExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new MembersApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var memberSlug = memberSlug_example;  // string | 
            var body = new Identity(); // Identity |  (optional) 

            try
            {
                // Add identity to a member
                apiInstance.WorkspaceSlugMembersMemberSlugIdentitiesPost(workspaceSlug, memberSlug, body);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling MembersApi.WorkspaceSlugMembersMemberSlugIdentitiesPost: " + e.Message );
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
 **body** | [**Identity**](Identity.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="workspaceslugmembersmemberslugput"></a>
# **WorkspaceSlugMembersMemberSlugPut**
> void WorkspaceSlugMembersMemberSlugPut (string workspaceSlug, string memberSlug, Member body = null)

Update a member

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugMembersMemberSlugPutExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new MembersApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var memberSlug = memberSlug_example;  // string | 
            var body = new Member(); // Member |  (optional) 

            try
            {
                // Update a member
                apiInstance.WorkspaceSlugMembersMemberSlugPut(workspaceSlug, memberSlug, body);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling MembersApi.WorkspaceSlugMembersMemberSlugPut: " + e.Message );
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
 **body** | [**Member**](Member.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="workspaceslugmemberspost"></a>
# **WorkspaceSlugMembersPost**
> void WorkspaceSlugMembersPost (string workspaceSlug, MemberAndIdentity body = null)

Create or update a member

This method is useful when you know a member's identity in another system and want to create or update the corresponding Orbit member. Identities can be specified in the identity object or member attributes like member.github. If no member exists, a new member will be created and linked to any provided identities.

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugMembersPostExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new MembersApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var body = new MemberAndIdentity(); // MemberAndIdentity |  (optional) 

            try
            {
                // Create or update a member
                apiInstance.WorkspaceSlugMembersPost(workspaceSlug, body);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling MembersApi.WorkspaceSlugMembersPost: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **workspaceSlug** | **string**|  | 
 **body** | [**MemberAndIdentity**](MemberAndIdentity.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
