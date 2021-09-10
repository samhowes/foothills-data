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
        [Fact]
        public async Task GenerateEntity_Works()
        {
            var generator = new EntityGenerator(new Editor());
            var text = await generator.GenerateEntityAsync("foo", new ExampleObject()
            {
                Type = "bar",
                Attributes = new JObject(
                    new JProperty("multi_word", "string"))
            });

            text.Should().Be(@"namespace PlanningCenter.Api.foo
{
    public class bar
    {
        public string MultiWord { get; set; }
    }
}");

        }
    }
}