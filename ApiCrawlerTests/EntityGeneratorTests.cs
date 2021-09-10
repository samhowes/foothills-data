using System;
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
        public async Task ModelBuilder_Works()
        {
            var modelBuilder = new ModelBuilder();
            var foo = modelBuilder.Entity("foo");
            foo.Property(typeof(string), "fooId");
            foo.HasKey("fooId");
            var bar = modelBuilder.Entity("bar");
            bar.Property(typeof(string), "barId");
            bar.HasKey("barId");
            foo.HasOne("bar");

            var debugString = modelBuilder.Model.ToDebugString(MetadataDebugStringOptions.LongDefault);
            debugString.Should().Be(@"Model: 
  EntityType: bar
    Properties: 
      barId (no field, string) Shadow Required PK AfterSave:Throw
    Keys: 
      barId PK
  EntityType: foo
    Properties: 
      fooId (no field, string) Shadow Required PK AfterSave:Throw
      barId (no field, string) Shadow FK
    Keys: 
      fooId PK
    Foreign keys: 
      foo {'barId'} -> bar {'barId'} ClientSetNull");
        }
    }
}