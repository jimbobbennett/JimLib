using System;
using System.Threading.Tasks;
using FluentAssertions;
using JimBobBennett.JimLib.Events;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Events
{
    [TestFixture]
    public class WeakEventManagerTest
    {
        public class EventFirer
        {
            public event EventHandler<EventArgs<string>> MyEvent
            {
                add
                {
                    var manager = WeakEventManager.GetWeakEventManager(this);
                    manager.AddEventHandler("MyEvent", value);
                }
                remove
                {
                    var manager = WeakEventManager.GetWeakEventManager(this);
                    manager.RemoveEventHandler("MyEvent", value);
                }
            }

            public void OnMyEvent(string s)
            {
                var manager = WeakEventManager.GetWeakEventManager(this);
                manager.HandleEvent(this, new EventArgs<string>(s), "MyEvent");    
            }


            public event EventHandler MySimpleEvent
            {
                add
                {
                    var manager = WeakEventManager.GetWeakEventManager(this);
                    manager.AddEventHandler("MySimpleEvent", value);
                }
                remove
                {
                    var manager = WeakEventManager.GetWeakEventManager(this);
                    manager.RemoveEventHandler("MySimpleEvent", value);
                }
            }

            public void OnMySimpleEvent()
            {
                var manager = WeakEventManager.GetWeakEventManager(this);
                manager.HandleEvent(this, EventArgs.Empty, "MySimpleEvent");
            }
        }

        public class Handler<TEventArgs>
            where TEventArgs : EventArgs
        {
            public void HandleEvent(object sender, TEventArgs args)
            {

            }
        }

        [Test]
        public void FiringEventFiresToHandler()
        {
            string result = null;

            var eventFirer = new EventFirer();

            eventFirer.MyEvent += (s, e) => { result = e.Value; };
            eventFirer.OnMyEvent("FooBar");

            result.Should().Be("FooBar");
        }

        [Test]
        public void FiringEventFiresToMultipleHandlers()
        {
            string result1 = null;
            string result2 = null;

            var eventFirer = new EventFirer();

            eventFirer.MyEvent += (s, e) => { result1 = e.Value; };
            eventFirer.MyEvent += (s, e) => { result2 = e.Value; };
            eventFirer.OnMyEvent("FooBar");

            result1.Should().Be("FooBar");
            result2.Should().Be("FooBar");
        }

        [Test]
        public void FiringSimpleEventFiresToHandler()
        {
            var fired = false;

            var eventFirer = new EventFirer();

            eventFirer.MySimpleEvent += (s, e) => { fired = true; };
            eventFirer.OnMySimpleEvent();

            fired.Should().BeTrue();
        }

        [Test]
        public void FiringRemovedEventDoesntFireToHandler()
        {
            string result = null;

            var eventFirer = new EventFirer();
            EventHandler<EventArgs<string>> handler = (sender, args) => result = args.Value;

            eventFirer.MyEvent += handler;
            eventFirer.MyEvent -= handler;
            eventFirer.OnMyEvent("FooBar");

            result.Should().BeNull();
        }

        [Test]
        public void FiringRemovedSimpleEventDoesntFireToHandler()
        {
            var fired = false;

            var eventFirer = new EventFirer();
            EventHandler handler = (sender, args) => fired = true;

            eventFirer.MySimpleEvent += handler;
            eventFirer.MySimpleEvent -= handler;
            eventFirer.OnMyEvent("FooBar");

            fired.Should().BeFalse();
        }

        [Test]
        public async Task EventHoldsWeakReference()
        {
            var eventFirer = new EventFirer();

            WeakReference reference;

            {
                var handler = new Handler<EventArgs<string>>();
                reference = new WeakReference(handler);

                eventFirer.MyEvent += handler.HandleEvent;
                eventFirer.OnMyEvent("FooBar");
            }

            var pos = 0;
            while (reference.Target != null && pos < 10)
            {
                await Task.Delay(1000);
                GC.Collect();
                GC.WaitForPendingFinalizers();

                eventFirer.OnMyEvent("HelloWorld");

                pos++;
            }

            reference.Target.Should().BeNull();

            GC.KeepAlive(eventFirer);
        }

        [Test]
        public async Task SimpleEventHoldsWeakReference()
        {
            var eventFirer = new EventFirer();

            WeakReference reference;

            {
                var handler = new Handler<EventArgs>();
                reference = new WeakReference(handler);

                eventFirer.MySimpleEvent += handler.HandleEvent;
                eventFirer.OnMySimpleEvent();
            }

            var pos = 0;
            while (reference.Target != null && pos < 10)
            {
                await Task.Delay(1000);
                GC.Collect();
                GC.WaitForPendingFinalizers();

                eventFirer.OnMyEvent("HelloWorld");

                pos++;
            }

            reference.Target.Should().BeNull();

            GC.KeepAlive(eventFirer);
        }

        [Test]
        public async Task UsingWeakEventManagerDoesntKeepAlive()
        {
            WeakReference reference;

            {
                var eventFirer = new EventFirer();
                reference = new WeakReference(eventFirer);

                var handler = new Handler<EventArgs>();
                eventFirer.MySimpleEvent += handler.HandleEvent;
            }
            
            var pos = 0;
            while (reference.Target != null && pos < 10)
            {
                await Task.Delay(1000);
                GC.Collect();
                GC.WaitForPendingFinalizers();

                pos++;
            }

            reference.Target.Should().BeNull();
        }
    }
}
