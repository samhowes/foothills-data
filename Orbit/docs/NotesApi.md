# Orbit.Api.Api.NotesApi

All URIs are relative to *https://app.orbit.love/api/v1*

Method | HTTP request | Description
------------- | ------------- | -------------
[**WorkspaceSlugMembersMemberSlugNotesGet**](NotesApi.md#workspaceslugmembersmemberslugnotesget) | **GET** /{workspace_slug}/members/{member_slug}/notes | Get the member&#x27;s notes
[**WorkspaceSlugMembersMemberSlugNotesIdPut**](NotesApi.md#workspaceslugmembersmemberslugnotesidput) | **PUT** /{workspace_slug}/members/{member_slug}/notes/{id} | Update a note
[**WorkspaceSlugMembersMemberSlugNotesPost**](NotesApi.md#workspaceslugmembersmemberslugnotespost) | **POST** /{workspace_slug}/members/{member_slug}/notes | Create a note

<a name="workspaceslugmembersmemberslugnotesget"></a>
# **WorkspaceSlugMembersMemberSlugNotesGet**
> void WorkspaceSlugMembersMemberSlugNotesGet (string workspaceSlug, string memberSlug, string page = null)

Get the member's notes

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugMembersMemberSlugNotesGetExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new NotesApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var memberSlug = memberSlug_example;  // string | 
            var page = page_example;  // string |  (optional) 

            try
            {
                // Get the member's notes
                apiInstance.WorkspaceSlugMembersMemberSlugNotesGet(workspaceSlug, memberSlug, page);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling NotesApi.WorkspaceSlugMembersMemberSlugNotesGet: " + e.Message );
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

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="workspaceslugmembersmemberslugnotesidput"></a>
# **WorkspaceSlugMembersMemberSlugNotesIdPut**
> void WorkspaceSlugMembersMemberSlugNotesIdPut (string workspaceSlug, string memberSlug, string id, Note body = null)

Update a note

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugMembersMemberSlugNotesIdPutExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new NotesApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var memberSlug = memberSlug_example;  // string | 
            var id = id_example;  // string | 
            var body = new Note(); // Note |  (optional) 

            try
            {
                // Update a note
                apiInstance.WorkspaceSlugMembersMemberSlugNotesIdPut(workspaceSlug, memberSlug, id, body);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling NotesApi.WorkspaceSlugMembersMemberSlugNotesIdPut: " + e.Message );
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
 **body** | [**Note**](Note.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="workspaceslugmembersmemberslugnotespost"></a>
# **WorkspaceSlugMembersMemberSlugNotesPost**
> void WorkspaceSlugMembersMemberSlugNotesPost (string workspaceSlug, string memberSlug, Note body = null)

Create a note

### Example
```csharp
using System;
using System.Diagnostics;
using Orbit.Api.Api;
using Orbit.Api.Client;
using Orbit.Api.Model;

namespace Example
{
    public class WorkspaceSlugMembersMemberSlugNotesPostExample
    {
        public void main()
        {
            // Configure API key authorization: bearer
            Configuration.Default.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new NotesApi();
            var workspaceSlug = workspaceSlug_example;  // string | 
            var memberSlug = memberSlug_example;  // string | 
            var body = new Note(); // Note |  (optional) 

            try
            {
                // Create a note
                apiInstance.WorkspaceSlugMembersMemberSlugNotesPost(workspaceSlug, memberSlug, body);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling NotesApi.WorkspaceSlugMembersMemberSlugNotesPost: " + e.Message );
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
 **body** | [**Note**](Note.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

[bearer](../README.md#bearer)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
