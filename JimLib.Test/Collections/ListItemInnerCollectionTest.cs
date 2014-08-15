using System.Collections.Generic;
using FluentAssertions;
using JimBobBennett.JimLib.Collections;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Collections
{
    [TestFixture]
    public class ListItemInnerCollectionTest
    {
        [Test]
        public void TitlSetter()
        {
            var v = new ListItemInnerCollection<string>("Hello");
            v.Title.Should().Be("Hello");
        }

        [Test]
        public void SettingItemsInTheConstructor()
        {
            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            var v = new ListItemInnerCollection<string>("Hello", toAdd);
            v.Title.Should().Be("Hello");
            
            v.Should().ContainInOrder(toAdd);
            v.Should().OnlyContain(s => toAdd.Contains(s));
            v.Count.Should().Be(2);
        }
    }
}
