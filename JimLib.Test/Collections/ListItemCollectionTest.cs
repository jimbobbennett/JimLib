﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using FluentAssertions;
using JimBobBennett.JimLib.Collections;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Collections
{
    [TestFixture]
    public class ListItemCollectionTest
    {
        [Test]
        public void AddGroupSetsItemsAndTitle()
        {
            var list = new ListItemCollection<string>();
            
            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            list.AddGroup("Bob", toAdd);

            list.Count.Should().Be(1);
            list[0].Title.Should().Be("Bob");

            list[0].Should().ContainInOrder(toAdd);
            list[0].Should().OnlyContain(s => toAdd.Contains(s));
            list[0].Count.Should().Be(2);
        }

        [Test]
        public void AddGroupReturnsTrueIfGroupIsAdded()
        {
            var list = new ListItemCollection<string>();

            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            list.AddGroup("Bob", toAdd).Should().BeTrue();

            list.Count.Should().Be(1);
            list[0].Title.Should().Be("Bob");

            list[0].Should().ContainInOrder(toAdd);
            list[0].Should().OnlyContain(s => toAdd.Contains(s));
            list[0].Count.Should().Be(2);
        }

        [Test]
        public void AddGroupReturnsFalseIfGroupAlreadyExists()
        {
            var list = new ListItemCollection<string>();

            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            var toAddSecond = new List<string>
            {
                "Foo",
                "Bar",
                "Hello"
            };

            list.AddGroup("Bob", toAdd).Should().BeTrue();
            list.AddGroup("Bob", toAddSecond).Should().BeFalse();

            list.Count.Should().Be(1);
            list[0].Title.Should().Be("Bob");

            list[0].Should().ContainInOrder(toAdd);
            list[0].Should().OnlyContain(s => toAdd.Contains(s));
            list[0].Count.Should().Be(2);
        }

        [Test]
        public void RemoveGroupRemovesGroupIfItIsThere()
        {
            var list = new ListItemCollection<string>();

            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            list.AddGroup("Bob", toAdd);
            list.AddGroup("Dave", toAdd);

            list.Count.Should().Be(2);

            list.RemoveGroup("Dave");

            list.Count.Should().Be(1);
            list[0].Title.Should().Be("Bob");
        }

        [Test]
        public void RemoveGroupReturnsTrueIfTheGroupIsRemoved()
        {
            var list = new ListItemCollection<string>();

            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            list.AddGroup("Bob", toAdd);
            list.AddGroup("Dave", toAdd);

            list.Count.Should().Be(2);

            list.RemoveGroup("Dave").Should().BeTrue();

            list.Count.Should().Be(1);
            list[0].Title.Should().Be("Bob");
        }

        [Test]
        public void RemoveGroupReturnsFalseIfTheGroupIsNotPresent()
        {
            var list = new ListItemCollection<string>();

            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            list.AddGroup("Bob", toAdd);
            list.AddGroup("Dave", toAdd);

            list.Count.Should().Be(2);

            list.RemoveGroup("Trevor").Should().BeFalse();

            list.Count.Should().Be(2);
            list[0].Title.Should().Be("Bob");
            list[1].Title.Should().Be("Dave");
        }

        [Test]
        public void ClearClearsTheList()
        {
            var list = new ListItemCollection<string>();

            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            list.AddGroup("Bob", toAdd);
            list.Clear();

            list.Count.Should().Be(0);
        }

        [Test]
        public void AddRangeAddsARange()
        {
            var list = new ListItemCollection<string>();

            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            var toAddSecond = new List<string>
            {
                "Foo",
                "Bar",
                "Hello"
            };

            list.AddRange(new List<Tuple<string, IEnumerable<string>>>
            {
                Tuple.Create("Bob", (IEnumerable<string>) toAdd),
                Tuple.Create("Dave", (IEnumerable<string>) toAddSecond),
            });

            list.Count.Should().Be(2);

            list[0].Title.Should().Be("Bob");
            list[0].Should().ContainInOrder(toAdd);
            list[0].Should().OnlyContain(s => toAdd.Contains(s));
            list[0].Count.Should().Be(2);

            list[1].Title.Should().Be("Dave");
            list[1].Should().ContainInOrder(toAddSecond);
            list[1].Should().OnlyContain(s => toAddSecond.Contains(s));
            list[1].Count.Should().Be(3);
        }

        [Test]
        public void AddRangeDoesntAddExistingGroups()
        {
            var list = new ListItemCollection<string>();

            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            var toAddSecond = new List<string>
            {
                "Foo",
                "Bar",
                "Hello"
            };

            list.AddGroup("Bob", toAdd);

            list.AddRange(new List<Tuple<string, IEnumerable<string>>>
            {
                Tuple.Create("Bob", (IEnumerable<string>) toAdd),
                Tuple.Create("Dave", (IEnumerable<string>) toAddSecond),
            });

            list.Count.Should().Be(2);

            list[0].Title.Should().Be("Bob");
            list[0].Should().ContainInOrder(toAdd);
            list[0].Should().OnlyContain(s => toAdd.Contains(s));
            list[0].Count.Should().Be(2);

            list[1].Title.Should().Be("Dave");
            list[1].Should().ContainInOrder(toAddSecond);
            list[1].Should().OnlyContain(s => toAddSecond.Contains(s));
            list[1].Count.Should().Be(3);
        }

        [Test]
        public void AddRangeOnlyRaisesOneCollectionChangeEventForTheAdd()
        {
            var list = new ListItemCollection<string>();

            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            var toAddSecond = new List<string>
            {
                "Foo",
                "Bar",
                "Hello"
            };

            var eventCount = 0;
            IEnumerable<ListItemInnerCollection<string>> itemsAdded = null;
            var action = NotifyCollectionChangedAction.Reset;

            ((INotifyCollectionChanged) list).CollectionChanged += (s, e) =>
                {
                    ++eventCount;
                    itemsAdded = e.NewItems.OfType<ListItemInnerCollection<string>>();
                    action = e.Action;
                };

            list.AddRange(new List<Tuple<string, IEnumerable<string>>>
            {
                Tuple.Create("Bob", (IEnumerable<string>) toAdd),
                Tuple.Create("Dave", (IEnumerable<string>) toAddSecond),
            });

            eventCount.Should().Be(1);
            action.Should().Be(NotifyCollectionChangedAction.Add);
            itemsAdded.Should().HaveCount(2);
        }

        [Test]
        public void ClearAndAddRangeClearsAndAddsARange()
        {
            var list = new ListItemCollection<string>();

            var initial = new List<string>
            {
                "A",
                "B"
            };

            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            var toAddSecond = new List<string>
            {
                "Foo",
                "Bar",
                "Hello"
            };
            
            list.AddGroup("FooBar", initial);

            list.ClearAndAddRange(new List<Tuple<string, IEnumerable<string>>>
            {
                Tuple.Create("Bob", (IEnumerable<string>) toAdd),
                Tuple.Create("Dave", (IEnumerable<string>) toAddSecond),
            });

            list.Count.Should().Be(2);

            list[0].Title.Should().Be("Bob");
            list[0].Should().ContainInOrder(toAdd);
            list[0].Should().OnlyContain(s => toAdd.Contains(s));
            list[0].Count.Should().Be(2);

            list[1].Title.Should().Be("Dave");
            list[1].Should().ContainInOrder(toAddSecond);
            list[1].Should().OnlyContain(s => toAddSecond.Contains(s));
            list[1].Count.Should().Be(3);
        }

        [Test]
        public void ClearAndAddRangeOnlyRaisesOneCollectionChangeEventForTheAdd()
        {
            var list = new ListItemCollection<string>();

            var initial = new List<string>
            {
                "A",
                "B"
            };

            var toAdd = new List<string>
            {
                "Foo",
                "Bar"
            };

            var toAddSecond = new List<string>
            {
                "Foo",
                "Bar",
                "Hello"
            };

            list.AddGroup("FooBar", initial);

            var eventCount = 0;
            var action = NotifyCollectionChangedAction.Add;

            ((INotifyCollectionChanged) list).CollectionChanged += (s, e) =>
                {
                    ++eventCount;
                    action = e.Action;
                };

            list.ClearAndAddRange(new List<Tuple<string, IEnumerable<string>>>
            {
                Tuple.Create("Bob", (IEnumerable<string>) toAdd),
                Tuple.Create("Dave", (IEnumerable<string>) toAddSecond),
            });

            eventCount.Should().Be(1);
            action.Should().Be(NotifyCollectionChangedAction.Reset);
        }
    }
}