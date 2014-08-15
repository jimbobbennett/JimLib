using System.Collections.Generic;
using FluentAssertions;
using JimBobBennett.JimLib.Extensions;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Extensions
{
    [TestFixture]
    public class ObjectExtensionsTest
    {
        [Test]
        public void AsListAddsTheItemToTheList()
        {
            const string s = "Foo";

            s.AsList().Should().OnlyContain(i => i == s);
            s.AsList().Should().HaveCount(1);
            s.AsList().Should().BeOfType<List<string>>();
        }

        [Test]
        public void AsArrayAddsTheItemToTheArray()
        {
            const string s = "Foo";

            s.AsArray().Should().OnlyContain(i => i == s);
            s.AsArray().Should().HaveCount(1);
            s.AsArray().Should().BeOfType<string[]>();
        }

        [Test]
        public void ExtractPropertyNameExtractsThePropertyName()
        {
            var s = new List<string>();
            this.ExtractPropertyName(() => s.Count).Should().Be("Count");
        }

        [Test]
        public void ExtractPropertyInfoExtractsThePropertyInfo()
        {
            var s = new List<string>();
            var propertyInfo = this.ExtractPropertyInfo(() => s.Count);
            propertyInfo.Name.Should().Be("Count");
            propertyInfo.PropertyType.Should().Be(typeof(int));
        }
    }
}
