using System;
using System.Linq;
using System.Threading.Tasks;
using ApiCrawler;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ApiCrawlerTests
{
    public class EntityGeneratorTests
    {
        private readonly EntityGenerator _generator;

        public EntityGeneratorTests()
        {
            _generator = new EntityGenerator(new Editor());
        }
        
        [Fact]
        public async Task GenerateEntity_Works()
        {
            _generator.RegisterEntity(new ExampleObject()
            {
                Type = "bar",
                Attributes = new JObject(
                    new JProperty("multi_word", "string"))
            });

            await _generator.GenerateEntitiesAsync("foo", (_, text) =>
            {
                text.Should().Be(@"namespace PlanningCenter.Api.foo
{
    public class bar : EntityBase
    {
        public string MultiWord { get; set; }
    }
}");
                return Task.CompletedTask;
            });

        }

        [Fact]
        public async Task PropertyName_SameAsEntity_IsReplaced()
        {
            _generator.RegisterEntity(new ExampleObject()
            {
                Type = "Bar",
                Attributes = new JObject(
                    new JProperty("bar", "string"))
            });

            await _generator.GenerateEntitiesAsync("foo", (_, text) =>
            {
                text.Should().Be(@"using Newtonsoft.Json;

namespace PlanningCenter.Api.foo
{
    public class Bar : EntityBase
    {
        [JsonProperty(""bar"")]
        public string Value { get; set; }
    }
}");
                return Task.CompletedTask;
            });
        }
        
        [Fact]
        public async Task NavigationName_SameAsEntity_IsParent()
        {
            _generator.RegisterEntity(new ExampleObject()
            {
                Type = "Bar",
                Attributes = new JObject(
                    new JProperty("bar_id", "string")),
                Relationships = JObject.Parse(@"{
    ""bar"": {
      ""data"": {
        ""type"": ""Bar"",
        ""id"": ""1""
      }
    }
  }")
            });
            var model = _generator._modelBuilder.Model;
            var entities = model.GetEntityTypes().ToList();
            entities.Count.Should().Be(1);
            var bar = entities.Single();
            var properties = bar.GetProperties().ToList();
            properties.Count.Should().Be(2);
            var navigations = bar.GetNavigations().ToList();
            navigations.Count.Should().Be(1);
            var navigation = navigations.Single();
            navigation.Name.Should().Be("Parent");
        }

        [Fact]
        public async Task Relationships_Works()
        {
            _generator.RegisterEntity(new ExampleObject()
            {
                Type = "Foo",
                Attributes = new JObject(new JProperty("value", "string"))
            });
            
            _generator.RegisterEntity(new ExampleObject()
            {
                Type = "Bar",
                Attributes = new JObject(
                    new JProperty("value", "string"),
                    new JProperty("bar_id", "string")
                    ),
                Relationships = JObject.Parse(@"{
    ""foo"": {
      ""data"": {
        ""type"": ""Foo"",
        ""id"": ""1""
      }
    },
    ""bar"": {
      ""data"": {
        ""type"": ""Bar"",
        ""id"": ""1""
      }
    }
  }")
            });
            
            var debugString = _generator._modelBuilder.Model.ToDebugString(MetadataDebugStringOptions.LongDefault);
            debugString.Should().Be(@"Model: 
  EntityType: Bar
    Properties: 
      Id (no field, string) Shadow Required PK AfterSave:Throw
      BarId (no field, string) Shadow FK
        Annotations: 
          PropertyOrder: 1
      FooId (no field, string) Shadow FK
        Annotations: 
          PropertyOrder: 3
      Value (no field, string) Shadow
        Annotations: 
          PropertyOrder: 0
    Navigations: 
      Bar (no field, object) ToPrincipal Bar
        Annotations: 
          PropertyOrder: 2
      Foo (no field, object) ToPrincipal Foo
        Annotations: 
          PropertyOrder: 3
    Keys: 
      Id PK
    Foreign keys: 
      Bar {'BarId'} -> Bar {'Id'} ToPrincipal: Bar ClientSetNull
      Bar {'FooId'} -> Foo {'Id'} ToPrincipal: Foo ClientSetNull
  EntityType: Foo
    Properties: 
      Id (no field, string) Shadow Required PK AfterSave:Throw
      Value (no field, string) Shadow
        Annotations: 
          PropertyOrder: 0
    Keys: 
      Id PK");

            bool found = false;
            await _generator.GenerateEntitiesAsync("foo", (name, text) =>
            {
                if (name != "Bar") return Task.CompletedTask;
                found = true;
                text.Should().Be(@"namespace PlanningCenter.Api.foo
{
    public class Bar : EntityBase
    {
        public string Value { get; set; }
        public string BarId { get; set; }
        public Bar Bar { get; set; }
        public string FooId { get; set; }
        public Foo Foo { get; set; }
    }
}");
                return Task.CompletedTask;
            });

            found.Should().Be(true);
        }
        
    }
}