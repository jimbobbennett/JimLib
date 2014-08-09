using FluentAssertions;
using JimBobBennett.JimLib.Extensions;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Extensions
{
    [TestFixture]
    public class StringExtensionsTest
    {
        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase("FooBar", false)]
        public void IsNullOrEmptyReturnsCorrectValue(string s, bool expected)
        {
            s.IsNullOrEmpty().Should().Be(expected);
        }

        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase(" ", true)]
        [TestCase("  ", true)]
        [TestCase("\t", true)]
        [TestCase("\t\t", true)]
        [TestCase(" FooBar", false)]
        [TestCase("FooBar ", false)]
        [TestCase("\tFooBar", false)]
        [TestCase("FooBar\t", false)]
        public void IsNullOrWhiteSpaceReturnsCorrectValue(string s, bool expected)
        {
            s.IsNullOrWhiteSpace().Should().Be(expected);
        }
    }
}
