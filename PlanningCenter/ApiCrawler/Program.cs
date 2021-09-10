using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var apiSections = await docs.GetSectionsAsync();
            
            apiSections.Count.Should().Be(8);

            var editor = new Editor();
            var entityGenerator = new EntityGenerator(editor);
            
            foreach (var apiSection in apiSections.Skip(1))
            {
                Section(apiSection.Name);
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
                    
                    var filename = example.Type + ".cs";
                    var filePath = Path.Combine(sectionFolder, filename);
                    var text = await entityGenerator.GenerateEntityAsync(sectionName, example);
                    await File.WriteAllTextAsync(filePath, text);
                }
            }

            return 0;
        }
    }


    public class ExampleObject
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public JObject Attributes { get; set; }
        public JObject Relationships { get; set; }
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