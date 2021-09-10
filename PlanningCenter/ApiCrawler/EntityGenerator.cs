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
            
            foreach (var property in example.Attributes.Properties())
            {
                var pascalCaseName = property.Name.Pascalize();
                var declaration = generator.PropertyDeclaration(
                    pascalCaseName,
                    generator.TypeExpression(SpecialType.System_String),
                    Accessibility.Public);

                clazz = generator.AddMembers(clazz, declaration);
            }
            
            ns = generator.AddMembers(ns, clazz);
            document.ReplaceNode(root!, ns);
            return await _editor.GetTextAsync(document);
        }
    }
}