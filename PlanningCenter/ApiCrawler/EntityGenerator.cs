using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using static ApiCrawler.Logger;

namespace ApiCrawler
{
    public class Relationship
    {
        public string Type { get; set; }
        public string Id { get; set; }
    }

    public static class Annotations
    {
        public const string PropertyOrder = nameof(PropertyOrder);
    }
    
    public class EntityGenerator
    {
        private readonly Editor _editor;
        public readonly ModelBuilder _modelBuilder;
        private readonly HashSet<string> _createdEntities;
        const string IdName = "Id";
        

        public EntityGenerator(Editor editor)
        {
            _editor = editor;
            _createdEntities = new HashSet<string>();
            _modelBuilder = new ModelBuilder();
        }

        public void RegisterEntity(ExampleObject example)
        {
            var entity = EnsureEntity(example.Type);
            var relationships = example.Relationships?.Properties().ToList();
            var relationshipDict = relationships != null
                ? relationships.ToDictionary(r => r.Name.Pascalize())
                : new Dictionary<string, JProperty>();

            var propertyIndex = 0;
            foreach (var jsonProperty in example.Attributes.Properties())
            {
                var pascalCaseName = jsonProperty.Name.Pascalize();
                
                var annotations = new List<(string, object)>()
                {
                    (Annotations.PropertyOrder, propertyIndex++)
                };
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

                if (pascalCaseName.EndsWith(IdName))
                {
                    var navigationName = pascalCaseName[0..^IdName.Length];
                    if (relationshipDict.TryGetValue(navigationName, out var relationship))
                    {
                        relationshipDict.Remove(navigationName);
                        AddRelationship(relationship, entity, propertyIndex++, property);
                    }
                }
            }

            if (relationships != null)
            {
                foreach (var relationship in relationships)
                {
                    if (!relationshipDict.ContainsKey(relationship.Name.Pascalize())) continue;
                    AddRelationship(relationship, entity, propertyIndex++);
                }    
            }
            
        }

        private void AddRelationship(JProperty relationship, EntityTypeBuilder entity, int propertyOrder, 
            PropertyBuilder? foreignKeyProperty = null)
        {
            var propertyName = relationship.Name.Pascalize();
            var relationShipData = ((relationship.Value as JObject)!).Property("data")!.Value;
            if (relationShipData.Type == JTokenType.Array)
            {
                Warn($"Detected suspicious relationship array for relationship {relationship.Name}. Skipping.");
                return;
            }

            var typedRelationship = relationShipData.ToObject<Relationship>();

            // todo: something with a relationship on a property that was not defined in attributes

            if (!_createdEntities.Contains(typedRelationship!.Type))
            {
                EnsureEntity(typedRelationship.Type);
            }

            if (foreignKeyProperty == null)
            {
                var foreignKeyName = propertyName + IdName;
                foreignKeyProperty = entity.Property(typeof(string), foreignKeyName)
                    .HasAnnotation(Annotations.PropertyOrder, propertyOrder);
            }

            if (propertyName == entity.Metadata.Name)
            {
                propertyName = "Parent";
            }

            entity.HasOne(typedRelationship!.Type, propertyName)
                .WithMany()
                .HasForeignKey(foreignKeyProperty.Metadata.Name)
                .HasPrincipalKey("Id");

            entity.Navigation(propertyName).HasAnnotation(Annotations.PropertyOrder, propertyOrder);
        }

        private EntityTypeBuilder EnsureEntity(string name)
        {
            var entity = _modelBuilder.Entity(name);
            if (!_createdEntities.Contains(name))
            {
                _createdEntities.Add(name);
                entity.Property(typeof(string), "Id");
                entity.HasKey("Id");    
            }
            
            return entity;
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
                var properties = new List<SyntaxNode>();
                var propertyOrdering = new Dictionary<SyntaxNode, int>();
                // todo: order properties by the order we read them from the docs 
                foreach (var property in entity.GetProperties().Where(p => p.Name != "Id"))
                {
                    var declaration = generator.PropertyDeclaration(
                        property.Name,
                        generator.TypeExpression(SpecialType.System_String),
                        Accessibility.Public);
                    
                    var annotations = property.GetAnnotations().ToList();
                    var attributes = new List<SyntaxNode>();
                    var propertyOrder = 0;
                    for (var i = 0; i < annotations.Count; i++)
                    {
                        var annotation = annotations[i];
                        switch (annotation.Name)
                        {
                            case Annotations.PropertyOrder:
                                propertyOrder = (int)annotation.Value;
                                continue;
                        }
                        
                        var lastPeriod = annotation.Name.LastIndexOf('.');
                        var attributeNamespace = annotation.Name[0..lastPeriod];
                        var attributeName = annotation.Name[(lastPeriod + 1)..];
                        const string attribute = "Attribute";
                        if (attributeName.EndsWith(attribute))
                        {
                            attributeName = attributeName[0..^attribute.Length];
                        }

                        usings.Add(attributeNamespace);
                        attributes.Add(
                            generator.Attribute(attributeName, generator.LiteralExpression(annotation.Value)));
                    }
                    declaration = generator.AddAttributes(declaration, attributes);
                    properties.Add(declaration);
                    propertyOrdering[declaration] = propertyOrder;
                    
                    if (property.IsForeignKey())
                    {
                        var fks = property.AsProperty().ForeignKeys;
                        fks.Count.Should().Be(1);
                        var fk = fks.Single();

                        var navigation = ((IForeignKey) fk).GetNavigation(true);
                        var navigationDeclaration = generator.PropertyDeclaration(
                            navigation.Name,
                            SyntaxFactory.IdentifierName(navigation.TargetEntityType.Name),
                            Accessibility.Public
                        );
                        properties.Add(navigationDeclaration);
                        propertyOrdering[navigationDeclaration] = propertyOrder;
                    }
                }

                properties = properties.OrderBy(p => propertyOrdering[p]).ToList();
                clazz = generator.AddMembers(clazz, properties);
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