using FluentAssertions;
using JimBobBennett.JimLib.Events;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Events
{
    [TestFixture]
    public class EventArgsTest
    {
        [Test]
        public void ValueComesFromConstructor()
        {
            var args = new EventArgs<string>("FooBar");
            args.Value.Should().Be("FooBar");
        }
    }
}
