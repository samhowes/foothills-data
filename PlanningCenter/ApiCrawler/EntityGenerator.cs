using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;

namespace ApiCrawler
{
    public class EntityGenerator
    {
        private readonly Editor _editor;

        public EntityGenerator(Editor editor)
        {
            _editor = editor;
        }

        public async Task<string> GenerateEntityAsync(string namespaceName, ExampleObject example)
        {
            var (document, root) = await _editor.CreateDocument(example.Type);
            var generator = document.Generator;

            var ns = generator.NamespaceDeclaration($"PlanningCenter.Api.{namespaceName}");
            
            var clazz = generator.ClassDeclaration(example.Type, accessibility: Accessibility.Public);

            var usings = new HashSet<string>();
            foreach (var property in example.Attributes.Properties())
            {
                var pascalCaseName = property.Name.Pascalize();
                var attrs = new List<SyntaxNode>();
                if (pascalCaseName == example.Type)
                {
                    usings.Add("Newtonsoft.Json");
                    pascalCaseName = "Value";
                    attrs.Add(generator.Attribute(nameof(JsonProperty), generator.LiteralExpression(property.Name)));
                }   
                
                var declaration = generator.PropertyDeclaration(
                    pascalCaseName,
                    generator.TypeExpression(SpecialType.System_String),
                    Accessibility.Public);

                if (attrs.Any())
                {
                    declaration = generator.AddAttributes(declaration, attrs);
                }

                clazz = generator.AddMembers(clazz, declaration);
            }

            ns = generator.AddMembers(ns, clazz);
            var usingNodes = usings.Select(u => generator.NamespaceImportDeclaration(u));
            root = generator.AddNamespaceImports(root, usingNodes);
            root = generator.AddMembers(root, ns);
            
            document.ReplaceNode(document.OriginalRoot, root);
            return await _editor.GetTextAsync(document);
        }
    }
}