using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Extensions
{
    [TestFixture]
    public class EnumerableExtensionsTest
    {
        [Test]
        public void ForEachRunsActionFOrAllItems()
        {
            var strings = new List<string>
            {
                "Hello",
                "World",
                "Foo",
                "Bar"
            };

            var newList = new List<string>();

            strings.ForEach(newList.Add);

            newList.Should().BeEquivalentTo(strings);
        }
    }
}
