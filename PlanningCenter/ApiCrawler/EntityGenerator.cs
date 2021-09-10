using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace ApiCrawler
{
    public class EntityGenerator
    {
        private readonly Editor _editor;
        private readonly ModelBuilder _modelBuilder;

        public EntityGenerator(Editor editor)
        {
            _editor = editor;
            _modelBuilder = new ModelBuilder();
        }

        public void RegisterEntity(ExampleObject example)
        {
            var entity = _modelBuilder.Entity(example.Type);
            
            foreach (var jsonProperty in example.Attributes.Properties())
            {
                var pascalCaseName = jsonProperty.Name.Pascalize();
                
                var annotations = new List<(string, string)>();
                if (pascalCaseName == example.Type)
                {
                    annotations.Add((typeof(JsonPropertyAttribute).FullName!, jsonProperty.Name));
                    pascalCaseName = "Value";
                }   
                
                var property = entity.Property(typeof(string), pascalCaseName);
                
                if (annotations.Any())
                {
                    foreach (var (name, value) in annotations)
                    {
                        property.HasAnnotation(name, value);
                    }
                }
            }
        }

        public async Task GenerateEntitiesAsync(string namespaceName, Func<string, string, Task> saveEntityAsync)
        {
            foreach (var entity in _modelBuilder.Model.GetEntityTypes())  
            {
                var (document, root) = await _editor.CreateDocument(entity.Name);
                var generator = document.Generator;

                var ns = generator.NamespaceDeclaration($"PlanningCenter.Api.{namespaceName}");
            
                var clazz = generator.ClassDeclaration(entity.Name, 
                    accessibility: Accessibility.Public, 
                    baseType: generator.IdentifierName("EntityBase"));
                
                
                var usings = new HashSet<string>();
                foreach (var property in entity.GetProperties())
                {
                    var declaration = generator.PropertyDeclaration(
                        property.Name,
                        generator.TypeExpression(SpecialType.System_String),
                        Accessibility.Public);

                    var annotations = property.GetAnnotations().ToList();
                    var attributes = new SyntaxNode[annotations.Count];
                    for (var i = 0; i < annotations.Count; i++)
                    {
                        var annotation = annotations[i];
                        var lastPeriod = annotation.Name.LastIndexOf('.');
                        var attributeNamespace = annotation.Name[0..lastPeriod];
                        var attributeName = annotation.Name[(lastPeriod + 1)..];
                        const string attribute = "Attribute";
                        if (attributeName.EndsWith(attribute))
                        {
                            attributeName = attributeName[0..^attribute.Length];
                        }

                        usings.Add(attributeNamespace);
                        attributes[i] =
                            generator.Attribute(attributeName, generator.LiteralExpression(annotation.Value));
                    }
                    declaration = generator.AddAttributes(declaration, attributes);

                    clazz = generator.AddMembers(clazz, declaration);
                }
                
                ns = generator.AddMembers(ns, clazz);
                var usingNodes = usings.Select(u => generator.NamespaceImportDeclaration(u));
                root = generator.AddNamespaceImports(root, usingNodes);
                root = generator.AddMembers(root, ns);
            
                document.ReplaceNode(document.OriginalRoot, root);
                var text = await _editor.GetTextAsync(document);
                await saveEntityAsync(entity.Name, text);
            }
        }
    }
}