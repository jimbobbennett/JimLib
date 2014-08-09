using System.Collections.Generic;
using System.Collections.Specialized;
using FluentAssertions;
using JimBobBennett.JimLib.Collections;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Collections
{
    [TestFixture]
    public class ObservableCollectionExTest
    {
        [Test]
        public void AddRangeAddsAllTheItems()
        {
            var oc = new ObservableCollectionEx<string>();

            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            oc.AddRange(toAdd);

            oc.Should().ContainInOrder(toAdd);
            oc.Should().OnlyContain(s => toAdd.Contains(s));
            oc.Count.Should().Be(2);
        }

        [Test]
        public void AddRangeOnlyRaisesOneCollectionChangedEventAtTheEnd()
        {

            var oc = new ObservableCollectionEx<string>();

            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            var itemCount = 0;
            var eventCount = 0;
            var action = NotifyCollectionChangedAction.Add;

            oc.CollectionChanged += (sender, args) =>
                {
                    ++eventCount;
                    itemCount = oc.Count;
                    action = args.Action;
                };

            oc.AddRange(toAdd);

            itemCount.Should().Be(2);
            eventCount.Should().Be(1);
            action.Should().Be(NotifyCollectionChangedAction.Reset);
        }
    }
}
