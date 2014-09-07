using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JimBobBennett.JimLib.Extensions;

namespace JimBobBennett.JimLib.Events
{
    public class WeakEventManager<TSource, TEventArgs> : IDisposable where TEventArgs : EventArgs
    {
// ReSharper disable once StaticFieldInGenericType
        private static readonly object SyncObj = new object();
        private static readonly Dictionary<TSource, WeakEventManager<TSource, TEventArgs>> WeakEventManagers = new Dictionary<TSource, WeakEventManager<TSource, TEventArgs>>();

        public static WeakEventManager<TSource, TEventArgs> GetWeakEventManager(TSource source)
        {
            lock (SyncObj)
            {
                WeakEventManager<TSource, TEventArgs> manager;
                if (!WeakEventManagers.TryGetValue(source, out manager))
                {
                    manager = new WeakEventManager<TSource, TEventArgs>(source);
                    WeakEventManagers.Add(source, manager);
                }

                return manager;
            }
        }

        private readonly object _syncObj = new object();
        private readonly TSource _source;
        private readonly Dictionary<string, List<Tuple<WeakReference, MethodInfo>>> _eventHandlers = new Dictionary<string, List<Tuple<WeakReference, MethodInfo>>>();

        private WeakEventManager(TSource source)
        {
            if (Equals(source, null))
                throw new ArgumentNullException();

            _source = source;
        }

        public void Dispose()
        {
            
        }

        public void AddEventHandler(string eventName, EventHandler<TEventArgs> value)
        {
            BuildEventHandler(eventName, value.Target, value.GetMethodInfo());
        }

        public void AddEventHandler(string eventName, EventHandler value)
        {
            BuildEventHandler(eventName, value.Target, value.GetMethodInfo());
        }

        private void BuildEventHandler(string eventName, object handlerTarget, MethodInfo methodInfo)
        {
            var sourceEvent = _source.GetType().GetAllEvents().FirstOrDefault(e => e.Name == eventName);

            if (sourceEvent == null)
                throw new ArgumentException("Event " + eventName + " not found", "eventName");

            lock (_syncObj)
            {
                List<Tuple<WeakReference, MethodInfo>> target;
                if (!_eventHandlers.TryGetValue(eventName, out target))
                {
                    target = new List<Tuple<WeakReference, MethodInfo>>();
                    _eventHandlers.Add(eventName, target);
                }

                target.Add(Tuple.Create(new WeakReference(handlerTarget), methodInfo));
            }
        }

        public void HandleEvent(object sender, TEventArgs args, string eventName)
        {
            var toRaise = new List<Tuple<object, MethodInfo>>();

            lock (_syncObj)
            {
                List<Tuple<WeakReference, MethodInfo>> target;
                if (_eventHandlers.TryGetValue(eventName, out target))
                {
                    foreach (var tuple in target.ToList())
                    {
                        var o = tuple.Item1.Target;

                        if (o == null)
                            target.Remove(tuple);
                        else
                            toRaise.Add(Tuple.Create(o, tuple.Item2));
                    }
                }
            }

            foreach (var tuple in toRaise)
                tuple.Item2.Invoke(tuple.Item1, new[] {sender, args});
        }

        public void RemoveEventHandler(string eventName, EventHandler<TEventArgs> value)
        {
            RemoveEventHandlerImpl(eventName, value.Target);
        }

        public void RemoveEventHandler(string eventName, EventHandler value)
        {
            RemoveEventHandlerImpl(eventName, value.Target);
        }

        private void RemoveEventHandlerImpl(string eventName, object handlerTarget)
        {
            lock (_syncObj)
            {
                List<Tuple<WeakReference, MethodInfo>> target;
                if (_eventHandlers.TryGetValue(eventName, out target))
                {
                    foreach (var tuple in target.Where(t => t.Item1.Target == handlerTarget).ToList())
                        target.Remove(tuple);
                }
            }
        }
    }
}
