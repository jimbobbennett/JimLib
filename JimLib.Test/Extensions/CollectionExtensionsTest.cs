using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using JimBobBennett.JimLib.Extensions;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Extensions
{
    [TestFixture]
    public class CollectionExtensionsTest
    {
        class CollectionItem
        {
            public CollectionItem(string key, int value)
            {
                Key = key;
                Value = value;
            }

            public string Key { get; private set; }
            public int Value { get; set; }
        }

        #region UpdateToMatch

        [Test]
        public void UpdateToMatchAddsNewItems()
        {
            var oc = new List<string>
            {
                "Existing",
                "Items"
            };

            var toAdd = new List<string>
            {
                "Existing",
                "Items",
                "Foo",
                "Bar"
            };

            oc.UpdateToMatch(toAdd, s => s);

            oc.Should().ContainInOrder(toAdd);
            oc.Should().OnlyContain(s => toAdd.Contains(s));
            oc.Count.Should().Be(4);
        }

        [Test]
        public void UpdateToMatchReturnsTrueIfItemsAreAdded()
        {
            var oc = new List<string>
            {
                "Existing",
                "Items"
            };

            var toAdd = new List<string>
            {
                "Existing",
                "Items",
                "Foo",
                "Bar"
            };

            oc.UpdateToMatch(toAdd, s => s).Should().BeTrue();
        }
        
        [Test]
        public void UpdateToMatchRemovesMissingItems()
        {
            var oc = new List<string>
            {
                "Existing",
                "Items"
            };

            var toRemove = new List<string>
            {
                "Items"
            };

            oc.UpdateToMatch(toRemove, s => s);

            oc.Should().ContainInOrder(toRemove);
            oc.Should().OnlyContain(s => toRemove.Contains(s));
            oc.Count.Should().Be(1);
        }

        [Test]
        public void UpdateToMatchReturnsTrueIfItemsAreRemoved()
        {
            var oc = new List<string>
            {
                "Existing",
                "Items"
            };

            var toRemove = new List<string>
            {
                "Items"
            };

            oc.UpdateToMatch(toRemove, s => s).Should().BeTrue();
        }
        
        [Test]
        public void UpdateToMatchUpdatesChangedItems()
        {
            var oldFirst = new CollectionItem("First", 1);

            var oc = new List<CollectionItem>
            {
                oldFirst,
                new CollectionItem("Second", 2),
                new CollectionItem("Third", 3)
            };

            var newFirst = new CollectionItem("First", 100);

            var toChange = new List<CollectionItem>
            {
                newFirst,
                new CollectionItem("Second", 200),
                new CollectionItem("Third", 3)
            };

            Func<CollectionItem, CollectionItem, bool> updateAction = (i1, i2) =>
            {
                if (i1.Value != i2.Value)
                {
                    i1.Value = i2.Value;
                    return true;
                }

                return false;
            };

            oc.UpdateToMatch(toChange, i => i.Key, updateAction);

            oc.Single(i => i.Key == "First").Value.Should().Be(100);
            oc.Single(i => i.Key == "Second").Value.Should().Be(200);
            oc.Single(i => i.Key == "Third").Value.Should().Be(3);

            oc.Single(i => i.Key == "First").Should().BeSameAs(oldFirst);
            oc.Single(i => i.Key == "First").Should().NotBeSameAs(newFirst);
        }

        [Test]
        public void UpdateToMatchReturnsTrueIfItemsAreUpdated()
        {
            var oc = new List<CollectionItem>
            {
                new CollectionItem("First", 1),
                new CollectionItem("Second", 2),
                new CollectionItem("Third", 3)
            };

            var toChange = new List<CollectionItem>
            {
                new CollectionItem("First", 1),
                new CollectionItem("Second", 200),
                new CollectionItem("Third", 3)
            };

            Func<CollectionItem, CollectionItem, bool> updateAction = (i1, i2) =>
            {
                if (i1.Value != i2.Value)
                {
                    i1.Value = i2.Value;
                    return true;
                }

                return false;
            };

            oc.UpdateToMatch(toChange, i => i.Key, updateAction).Should().BeTrue();
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateToMatchWithANullCollectionThrows()
        {
            var oc = new List<string>();
            oc.UpdateToMatch(null, s => s);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateToMatchWithANullKeyFuncThrows()
        {
            var oc = new List<string>();
            var toAdd = new List<string>
            {
                "Existing",
                "Items",
                "Foo",
                "Bar"
            };

            oc.UpdateToMatch<string, string>(toAdd, null);
        }

        #endregion UpdateTopMatch
    }
}
