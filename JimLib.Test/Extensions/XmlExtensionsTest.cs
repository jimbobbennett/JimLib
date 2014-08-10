using FluentAssertions;
using NUnit.Framework;
using JimBobBennett.JimLib.Extensions;

namespace JimBobBennett.JimLib.Test.Extensions
{
    [TestFixture]
    public class XmlExtensionsTest
    {
        [Test]
        public void AsNamespacedAddsTheNamespace()
        {
            const string name = "Foo";
            const string @namespace = "Bar";

            var namspaced = name.AsNamespaced(@namespace);
            namspaced.ToString().Should().Be("{Bar}Foo");
        }

        [Test]
        public void AsNamespacedDoesntAddTheNamespaceIfItsNull()
        {
            const string name = "Foo";

            var namspaced = name.AsNamespaced(null);
            namspaced.ToString().Should().Be("Foo");
        }
    }
}
