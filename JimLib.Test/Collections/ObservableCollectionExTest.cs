using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using FluentAssertions;
using JimBobBennett.JimLib.Collections;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Collections
{
    [TestFixture]
    public class ObservableCollectionExTest
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

        #region AddRange

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
        public void AddRangeDoesntRaiseAnEventIfNothingIsAdded()
        {
            var oc = new ObservableCollectionEx<string>
            {
                "Foo",
                "Bar"
            };

            var toAdd = new List<string>();

            oc.MonitorEvents();
            oc.AddRange(toAdd);
            oc.ShouldNotRaise("CollectionChanged");
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
            IList<string> addedItems = null;

            oc.CollectionChanged += (sender, args) =>
                {
                    ++eventCount;
                    itemCount = oc.Count;
                    action = args.Action;
                    addedItems = args.NewItems.OfType<string>().ToList();
                };

            oc.AddRange(toAdd);

            itemCount.Should().Be(2);
            eventCount.Should().Be(1);
            action.Should().Be(NotifyCollectionChangedAction.Add);
            addedItems.Should().NotBeNull();
            addedItems.ShouldBeEquivalentTo(toAdd);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void AddRangeWithANullCollectionThrows()
        {
            var oc = new ObservableCollectionEx<string>();
            oc.AddRange(null);
        }
           
        #endregion AddRange

        #region ClearAndAddRange
        
        [Test]
        public void ClearAndAddRangeClearsTheCollectionThenAddsAllTheItems()
        {
            var oc = new ObservableCollectionEx<string>
            {
                "Existing",
                "Items"
            };

            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            oc.ClearAndAddRange(toAdd);

            oc.Should().ContainInOrder(toAdd);
            oc.Should().OnlyContain(s => toAdd.Contains(s));
            oc.Count.Should().Be(2);
        }

        [Test]
        public void ClearAddRangeDoesntRaiseAnEventIfNothingIsAddedOrCleared()
        {
            var oc = new ObservableCollectionEx<string>();

            var toAdd = new List<string>();

            oc.MonitorEvents();
            oc.ClearAndAddRange(toAdd);
            oc.ShouldNotRaise("CollectionChanged");
        }

        [Test]
        public void ClearAndAddRangeOnlyRaisesOneResetCollectionChangedEventAtTheEndIfItemsAreClearedAndAdded()
        {
            var oc = new ObservableCollectionEx<string>
            {
                "Existing",
                "Items"
            };

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

            oc.ClearAndAddRange(toAdd);

            itemCount.Should().Be(2);
            eventCount.Should().Be(1);
            action.Should().Be(NotifyCollectionChangedAction.Reset);
        }

        [Test]
        public void ClearAndAddRangeOnlyRaisesOneAddCollectionChangedEventAtTheEndIfNoItemsAreClearedAndSomeAreAdded()
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
            IList<string> addedItems = null;

            oc.CollectionChanged += (sender, args) =>
            {
                ++eventCount;
                itemCount = oc.Count;
                action = args.Action;
                addedItems = args.NewItems.OfType<string>().ToList();
            };

            oc.ClearAndAddRange(toAdd);

            itemCount.Should().Be(2);
            eventCount.Should().Be(1);
            action.Should().Be(NotifyCollectionChangedAction.Add);
            addedItems.ShouldBeEquivalentTo(toAdd);
        }

        [Test]
        public void ClearAndAddRangeOnlyRaisesOneRemoveCollectionChangedEventAtTheEndIfNoItemsAreAddedAndSomeAreCleared()
        {
            var oc = new ObservableCollectionEx<string>
            {
                "Foo",
                "Bar"
            };

            var toAdd = new List<string>();

            var itemCount = 0;
            var eventCount = 0;
            var action = NotifyCollectionChangedAction.Add;
            IList<string> removedItems = null;

            oc.CollectionChanged += (sender, args) =>
            {
                ++eventCount;
                itemCount = oc.Count;
                action = args.Action;
                removedItems = args.OldItems.OfType<string>().ToList();
            };

            oc.ClearAndAddRange(toAdd);

            itemCount.Should().Be(0);
            eventCount.Should().Be(1);
            action.Should().Be(NotifyCollectionChangedAction.Remove);
            removedItems.ShouldBeEquivalentTo(new List<string>
            {
                "Foo",
                "Bar"
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ClearAndAddRangeWithANullCollectionThrows()
        {
            var oc = new ObservableCollectionEx<string>();
            oc.ClearAndAddRange(null);
        }

        #endregion ClearAndAddRange

        #region UpdateToMatch

        [Test]
        public void UpdateToMatchAddsNewItems()
        {
            var oc = new ObservableCollectionEx<string>
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
            var oc = new ObservableCollectionEx<string>
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
        public void UpdateToMatchRaisesCollectionChangeOnlyOnceWithResetIfItemsAreAdded()
        {
            var oc = new ObservableCollectionEx<string>
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
            
            var itemCount = 0;
            var eventCount = 0;
            var action = NotifyCollectionChangedAction.Add;

            oc.CollectionChanged += (sender, args) =>
            {
                ++eventCount;
                itemCount = oc.Count;
                action = args.Action;
            };

            oc.UpdateToMatch(toAdd, s => s);

            itemCount.Should().Be(4);
            eventCount.Should().Be(1);
            action.Should().Be(NotifyCollectionChangedAction.Reset);
        }

        [Test]
        public void UpdateToMatchRemovesMissingItems()
        {
            var oc = new ObservableCollectionEx<string>
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
            var oc = new ObservableCollectionEx<string>
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
        public void UpdateToMatchRaisesCollectionChangeOnlyOnceWithResetIfItemsAreRemoved()
        {
            var oc = new ObservableCollectionEx<string>
            {
                "Existing",
                "Items"
            };

            var toRemove = new List<string>
            {
                "Items"
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

            oc.UpdateToMatch(toRemove, s => s);

            itemCount.Should().Be(1);
            eventCount.Should().Be(1);
            action.Should().Be(NotifyCollectionChangedAction.Reset);
        }

        [Test]
        public void UpdateToMatchUpdatesChangedItems()
        {
            var oc = new ObservableCollectionEx<CollectionItem>
            {
                new CollectionItem("First", 1),
                new CollectionItem("Second", 2),
                new CollectionItem("Third", 3)
            };

            var toChange = new List<CollectionItem>
            {
                new CollectionItem("First", 100),
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
        }

        [Test]
        public void UpdateToMatchReturnsTrueIfItemsAreUpdated()
        {
            var oc = new ObservableCollectionEx<CollectionItem>
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
        public void UpdateToMatchRaisesCollectionChangeOnlyOnceWithResetIfItemsAreUpdated()
        {
            var oc = new ObservableCollectionEx<CollectionItem>
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
            
            var itemCount = 0;
            var eventCount = 0;
            var action = NotifyCollectionChangedAction.Add;

            oc.CollectionChanged += (sender, args) =>
            {
                ++eventCount;
                itemCount = oc.Count;
                action = args.Action;
            };

            oc.UpdateToMatch(toChange, i => i.Key, updateAction).Should().BeTrue();

            itemCount.Should().Be(3);
            eventCount.Should().Be(1);
            action.Should().Be(NotifyCollectionChangedAction.Reset);
        }

        [Test]
        public void UpdateToMatchRaisesCollectionChangeOnlyOnceWithResetIfItemsAreAddedRemovedAndUpdated()
        {
            var oc = new ObservableCollectionEx<CollectionItem>
            {
                new CollectionItem("First", 1),
                new CollectionItem("Second", 2),
                new CollectionItem("Third", 3)
            };

            var toChange = new List<CollectionItem>
            {
                new CollectionItem("First", 1),
                new CollectionItem("Second", 200),
                new CollectionItem("Fourth", 4)
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

            var itemCount = 0;
            var eventCount = 0;
            var action = NotifyCollectionChangedAction.Add;

            oc.CollectionChanged += (sender, args) =>
            {
                ++eventCount;
                itemCount = oc.Count;
                action = args.Action;
            };

            oc.UpdateToMatch(toChange, i => i.Key, updateAction).Should().BeTrue();

            itemCount.Should().Be(3);
            eventCount.Should().Be(1);
            action.Should().Be(NotifyCollectionChangedAction.Reset);

            oc.Single(i => i.Key == "First").Value.Should().Be(1);
            oc.Single(i => i.Key == "Second").Value.Should().Be(200);
            oc.Single(i => i.Key == "Fourth").Value.Should().Be(4);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateToMatchWithANullCollectionThrows()
        {
            var oc = new ObservableCollectionEx<string>();
            oc.UpdateToMatch(null, s => s);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateToMatchWithANullKeyFuncThrows()
        {
            var oc = new ObservableCollectionEx<string>();
            var toAdd = new List<string>
            {
                "Existing",
                "Items",
                "Foo",
                "Bar"
            };

            oc.UpdateToMatch<string>(toAdd, null);
        }

        #endregion UpdateToMatch

        #region Constructor

        [Test]
        public void ConstructorWithItemsSetsTheItems()
        {
            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            var oc = new ObservableCollectionEx<string>(toAdd);

            oc.Should().ContainInOrder(toAdd);
            oc.Should().OnlyContain(s => toAdd.Contains(s));
            oc.Count.Should().Be(2);
        }

        #endregion Constructor
    }
}
