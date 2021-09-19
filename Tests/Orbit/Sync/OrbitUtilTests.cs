using System;
using FluentAssertions;
using Xunit;

namespace Sync
{
    public class OrbitUtilTests
    {
        [Theory]
        [InlineData("foo & bar", "FooBar")]
        public void CleanChannelName_Works(string input, string expected)
        {
            var actual = OrbitUtil.CleanChannelName(input);
            actual.Should().Be(expected);
        }
    }
}
