using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Playwright;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using static ApiCrawler.Logger;

namespace ApiCrawler
{
    public record ApiSection(string Name, IElementHandle Element);

    public class ApiDocs : IAsyncDisposable
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IPage _page;

        public static async Task<ApiDocs> CreateAsync()
        {
            var docs = new ApiDocs();
            
            try
            {
                await docs.InitAsync();
                return docs;
            }
            catch
            {
                await docs.DisposeAsync();
                throw;
            }
        }

        private async Task InitAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
            }); 
            var context = await _browser.NewContextAsync();
            
            _page = await context.NewPageAsync();
            
        }

        public async ValueTask DisposeAsync()
        {
            _playwright?.Dispose();
            if (_browser != null) await _browser.DisposeAsync();
        }

        public async Task<List<ApiSection>> GetSectionsAsync()
        {
            // Go to https://developer.planning.center/docs/#/apps/api
            await _page.GotoAsync("https://developer.planning.center/docs/#/apps/api");
            var apiSections = await _page.QuerySelectorAllAsync("css=li > a >> text=API >> xpath=../.. >> css=li");
            var records = new List<ApiSection>();
            foreach (var element in apiSections)
            {
                var name = await element.TextContentAsync();
                const string beta = "Beta";
                if (name!.EndsWith(beta, StringComparison.OrdinalIgnoreCase))
                {
                    name = name[0..^beta.Length];
                }
                records.Add(new ApiSection(name!, element));
            }
            return records;
        }

        public async Task<IReadOnlyList<IElementHandle>> GetEntities(ApiSection apiSection)
        {
            await apiSection.Element.ClickAsync();
            await _page.WaitForSelectorAsync($@"text=/Planning Center {apiSection.Name}.*/i");
            var versions = await _page.QuerySelectorAllAsync("css=a > h1");
            versions.Count.Should().BeGreaterThan(0);

            var latestVersion = versions.First();
            var versionName = await latestVersion.TextContentAsync();
            versionName.Should().MatchRegex(@"\d\d\d-\d\d-\d\d");

            await latestVersion.ClickAsync();

            var entityListSelector = "css=li > ul";
            var entityList = await _page.WaitForSelectorAsync(entityListSelector)!;
            entityList.Should().NotBeNull();
            var entities = await entityList!.QuerySelectorAllAsync("li");
            return entities;
        }

        public async Task<string> GetExampleObjectJson(IElementHandle entity)
        {
            await entity.ClickAsync();

            var jsonExampleElement = await _page.WaitForSelectorAsync("css=#example-object pre");

            jsonExampleElement.Should().NotBeNull();
            var json = await jsonExampleElement!.TextContentAsync();
            return json!;
        }
    }

    public class GraphVersion : Node<GraphVersion.Attributes>
    {
        public class Attributes
        {
            public bool Beta { get; set; }
            public string Details { get; set; }
        }

        public class Relationships
        {
            public Wrapper<JObject> PreviousVersion { get; set; }
            public Wrapper<JObject> NextVersion { get; set; }
            
        }
        
    }
    
    public class Graph
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
    
    public class Node<T>
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public T Attributes { get; set; }
        public JObject Relationships { get; set; }
    }

    public class ApiVersion
    {
        public bool Beta { get; set; }
        public string Details { get; set; }
    }
    
    public class ApiDocsClient
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerSettings _jsonSettings;

        public ApiDocsClient()
        {
            _http = new HttpClient()
            {
                BaseAddress = new Uri("https://api.planningcenteronline.com/")
            };
            _jsonSettings = new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        }

        public async Task GetSectionAsync(string sectionName)
        {
            var graph = await GetJsonAsync<Node<Graph>>($"{sectionName}/v2/documentation");

            var versions = graph!.Relationships.Property("versions")!.Value.ToObject<Wrapper<List<Node<ApiVersion>>>>()!.Data;

            var latestVersion = versions!.First();

            var entities = latestVersion.Relationships.Property("vertices")!.ToObject<Wrapper<List<Node<EntityInfo>>>>();
            


        }

        private async Task<T?> GetJsonAsync<T>(string url)
        {
            var response = await _http.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get {url}: {content}");
            }

            return JsonConvert.DeserializeObject<Wrapper<T>>(content)!.Data;
        }
    }

    public class EntityInfo
    {
        public string Name { get; set;  }
        public bool Deprecated { get; set; }
    }

    public class Wrapper<T>
    {
        public T Data { get; set; }
    }

    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // assume we're in the bin folder
            var path = Directory.GetCurrentDirectory();
            var binDex = path!.IndexOf("bin", StringComparison.Ordinal);
            var thisProjectDirectory = path[0..(binDex - 1)];
            var apiProjectDirectory = Path.Combine(
                Path.GetDirectoryName(thisProjectDirectory)!,
                "Api");
            if (!Directory.Exists(apiProjectDirectory))
            {
                Console.Error.WriteLine("Failed to find the ApiProject Directory");
                return -1;
            }
            
            var docs = await ApiDocs.CreateAsync();
            var docsClient = new ApiDocsClient();
            

            var apiSections = await docs.GetSectionsAsync();
            
            apiSections.Count.Should().Be(8);

            var editor = new Editor();
            
            foreach (var apiSection in apiSections.Skip(1))
            {
                var entityGenerator = new EntityGenerator(editor);
                Section(apiSection.Name);

                await docsClient.GetSectionAsync(apiSection.Name.ToLower());
                
                var sectionName = apiSection.Name.Pascalize().Replace("-", "");
                var sectionFolder = Path.Combine(apiProjectDirectory, sectionName);
                if (Directory.Exists(sectionFolder))
                    Directory.Delete(sectionFolder, true);
                Directory.CreateDirectory(sectionFolder);
                
                var entities = await docs.GetEntities(apiSection);
                entities.Count.Should().BeGreaterThan(0);
                foreach (var entity in entities)
                {
                    var exampleJson = await docs.GetExampleObjectJson(entity);
                    var example = JsonConvert.DeserializeObject<ExampleObject>(exampleJson);
                    Info(example!.Type);

                    entityGenerator.RegisterEntity(example);
                }

                await entityGenerator.GenerateEntitiesAsync(sectionName, async (entityName, text) =>
                {
                    var filename = entityName + ".cs";
                    var filePath = Path.Combine(sectionFolder, filename);
                    await File.WriteAllTextAsync(filePath, text);
                });
            }

            return 0;
        }
    }


    public class ExampleObject
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public JObject Attributes { get; set; }
        public JObject? Relationships { get; set; }
    }

    public class Editor
    {
        private readonly AdhocWorkspace _workspace;
        private readonly Project _newProject;

        public Editor()
        {
            _workspace = new AdhocWorkspace();
            var projectId = ProjectId.CreateNewId();
            var versionStamp = VersionStamp.Create();
            var projectInfo = ProjectInfo.Create(projectId, versionStamp, "NewProject", "projName", LanguageNames.CSharp);
            _newProject = _workspace.AddProject(projectInfo);
        }

        public async Task<(SyntaxNode root, DocumentEditor editor)> LoadDocument(string path)
        {
            var sourceText = SourceText.From(await File.ReadAllTextAsync(path));
            var document = _workspace.AddDocument(_newProject.Id, "NewFile.cs", sourceText);
            var root = await document.GetSyntaxRootAsync()!;
            var editor = await DocumentEditor.CreateAsync(document);
            return (root!, editor);
        }

        public async Task<(DocumentEditor editor, SyntaxNode?)> CreateDocument(string name)
        {
            var document = _workspace.AddDocument(_newProject.Id, name, SourceText.From(""));
            var editor = await DocumentEditor.CreateAsync(document);
            return (editor, await document.GetSyntaxRootAsync());
        }

        public async Task<string> GetTextAsync(DocumentEditor editor)
        {
            var document = await Formatter.FormatAsync(editor.GetChangedDocument());
            var text = await document.GetTextAsync();
            return text.ToString();
        }
    }
}