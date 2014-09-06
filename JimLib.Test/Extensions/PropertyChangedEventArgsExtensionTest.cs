using System.ComponentModel;
using FluentAssertions;
using JimBobBennett.JimLib.Extensions;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Extensions
{
    [TestFixture]
    public class PropertyChangedEventArgsExtensionTest
    {
        private bool Foo { get; set; }

        [TestCase("Foo", true)]
        [TestCase("", true)]
        [TestCase("Bar", false)]
        public void PropertyNameMatches(string propertyName, bool expected)
        {
            new PropertyChangedEventArgs(propertyName).PropertyNameMatches(() => Foo).Should().Be(expected);
        }
    }
}
