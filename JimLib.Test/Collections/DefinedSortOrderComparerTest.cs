using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using JimBobBennett.JimLib.Collections;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Collections
{
    [TestFixture]
    public class DefinedSortOrderComparerTest
    {
        class ToBeSorted
        {
            public ToBeSorted(string key, int value)
            {
                Key = key;
                Value = value;
            }

            public string Key { get; private set; }
            public int Value { get; private set; }
        }

        [Test]
        public void SortingBasedOnKeys()
        {
            var toBeSorted = new List<ToBeSorted>
            {
                new ToBeSorted("FooBar", 1),
                new ToBeSorted("Third", 2),
                new ToBeSorted("Fourth", 3),
                new ToBeSorted("Second", 4),
                new ToBeSorted("First", 5),
                new ToBeSorted("Fifth", 6),
                new ToBeSorted("FooBar", 7)
            };

            var sortOrder = new List<string>
            {
                "First",
                "Second",
                "Third",
                "Fourth",
                "Fifth"
            };

            var comparer = new DefinedSortOrderComparer<ToBeSorted, string>(sortOrder, s => s.Key);

            toBeSorted.Sort(comparer);

            toBeSorted[0].Key.Should().Be("First");
            toBeSorted[0].Value.Should().Be(5);

            toBeSorted[1].Key.Should().Be("Second");
            toBeSorted[1].Value.Should().Be(4);

            toBeSorted[2].Key.Should().Be("Third");
            toBeSorted[2].Value.Should().Be(2);

            toBeSorted[3].Key.Should().Be("Fourth");
            toBeSorted[3].Value.Should().Be(3);

            toBeSorted[4].Key.Should().Be("Fifth");
            toBeSorted[4].Value.Should().Be(6);

            toBeSorted[5].Key.Should().Be("FooBar");
            toBeSorted[5].Value.Should().BeOneOf(1, 7);

            toBeSorted[6].Key.Should().Be("FooBar");
            toBeSorted[6].Value.Should().BeOneOf(1, 7);
        }
    }
}
