using System;
using System.Threading.Tasks;
using ApiCrawler;
using FluentAssertions;
using Microsoft.CodeAnalysis;
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
            var text = await _generator.GenerateEntityAsync("foo", new ExampleObject()
            {
                Type = "bar",
                Attributes = new JObject(
                    new JProperty("multi_word", "string"))
            });

            text.Should().Be(@"namespace PlanningCenter.Api.foo
{
    public class bar : EntityBase
    {
        public string MultiWord { get; set; }
    }
}");
        }

        [Fact]
        public async Task PropertyName_SameAsEntity_IsReplaced()
        {
            var text = await _generator.GenerateEntityAsync("foo", new ExampleObject()
            {
                Type = "Bar",
                Attributes = new JObject(
                    new JProperty("bar", "string"))
            });

            text.Should().Be(@"using Newtonsoft.Json;

namespace PlanningCenter.Api.foo
{
    public class Bar : EntityBase
    {
        [JsonProperty(""bar"")]
        public string Value { get; set; }
    }
}");
        }
    }
}