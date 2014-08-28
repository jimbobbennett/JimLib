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

            list.AddRange(new List<ListItemInnerCollection<string>>
            {
                new ListItemInnerCollection<string>("Bob", toAdd),
                new ListItemInnerCollection<string>("Dave", toAddSecond),
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
        public void AddRangeAddsARangeEnumerableOfTuple()
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

            list.ClearAndAddRange(new List<ListItemInnerCollection<string>>
            {
                new ListItemInnerCollection<string>("Bob", toAdd),
                new ListItemInnerCollection<string>("Dave", toAddSecond),
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
        public void ClearAndAddRangeClearsAndAddsARangeFromEnumerableOfTuple()
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

        [Test]
        public void ClearAndAddRangeWithExistingWorks()
        {
            var list = new ListItemCollection<string>();
            list.AddGroup("Hello", new List<string> {"Foo"});

            var items = new List<ListItemInnerCollection<string>>();
            items.Add(new ListItemInnerCollection<string>("Hello", new List<string> { "Bar" }));

            list.ClearAndAddRange(items);

            list.Count.Should().Be(1);
        }

        [Test]
        public void AddAddsToNewGroupIfGroupDoesntExist()
        {
            var list = new ListItemCollection<string>();
            list.AddGroup("Hello", new List<string> { "Foo" });

            list.Add("World", "Bar");

            list.Should().HaveCount(2);
            list.Should().Contain(g => g.Title == "Hello" && g.Count == 1 && g[0] == "Foo");
            list.Should().Contain(g => g.Title == "World" && g.Count == 1 && g[0] == "Bar");
        }

        [Test]
        public void AddAddsToExistingGroupIfGroupDoesExist()
        {
            var list = new ListItemCollection<string>();
            list.AddGroup("Hello", new List<string> { "Foo" });

            list.Add("Hello", "Bar");

            list.Should().HaveCount(1);
            list.Should().Contain(g => g.Title == "Hello" && g.Count == 2 && 
                g.Contains("Foo") && g.Contains("Bar"));
        }

        [Test]
        public void DeleteDeletesItem()
        {
            var list = new ListItemCollection<string>();
            list.AddGroup("Hello", new List<string> { "Foo", "Bar" });
            list.Delete("Foo");

            list.Should().HaveCount(1);
            list.Should().Contain(g => g.Title == "Hello" && g.Count == 1 && g[0] == "Bar");
        }

        [Test]
        public void DeleteReturnsTrueIfItDeletesTheItem()
        {
            var list = new ListItemCollection<string>();
            list.AddGroup("Hello", new List<string> { "Foo", "Bar" });
            list.Delete("Foo").Should().BeTrue();
        }

        [Test]
        public void DeleteReturnsFalseIfTheItemDoesnExist()
        {
            var list = new ListItemCollection<string>();
            list.AddGroup("Hello", new List<string> { "Foo", "Bar" });
            list.Delete("World").Should().BeFalse();

            list.Should().HaveCount(1);
            list.Should().Contain(g => g.Title == "Hello" && g.Count == 2 &&
                g.Contains("Foo") && g.Contains("Bar"));
        }

        [Test]
        public void DeleteRemovesEmptyGroups()
        {
            var list = new ListItemCollection<string>();
            list.AddGroup("Hello", new List<string> { "Foo", "Bar" });
            list.AddGroup("World", new List<string> { "FooBar" });

            list.Should().HaveCount(2);

            list.Delete("FooBar").Should().BeTrue();

            list.Should().HaveCount(1);
            list.Should().Contain(g => g.Title == "Hello" && g.Count == 2 &&
                g.Contains("Foo") && g.Contains("Bar"));
        }

        [Test]
        public void GroupsAreSorted()
        {
            var list = new ListItemCollection<string>();
            
            list.AddGroup("Bob", new List<string>
            {
                "Foo",
                "Bar"
            });

            list.AddGroup("Dave", new List<string>
            {
                "Hello",
                "World"
            });

            list[0].Title.Should().Be("Bob");
            list[1].Title.Should().Be("Dave");

            list.TitleSortOrder = new List<string>
            {
                "Dave",
                "Colin",
                "Bob"
            };

            list[0].Title.Should().Be("Dave");
            list[1].Title.Should().Be("Bob");
            
            list.AddGroup("Colin", new List<string>
            {
                "FooBar",
                "HelloWorld"
            });

            list[0].Title.Should().Be("Dave");
            list[1].Title.Should().Be("Colin");
            list[2].Title.Should().Be("Bob");
        }
    }
}
