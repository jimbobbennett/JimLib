using System;

namespace JimBobBennett.JimLib.Events
{
    public interface IWeakEventManager
    {
        void AddEventHandler<TEventArgs>(object source, string eventName, EventHandler<TEventArgs> value)
            where TEventArgs : EventArgs;

        void AddEventHandler(object source, string eventName, EventHandler value);

        void HandleEvent<TEventArgs>(object sender, TEventArgs args, string eventName) 
            where TEventArgs : EventArgs;

        void RemoveEventHandler<TEventArgs>(string eventName, EventHandler<TEventArgs> value)
            where TEventArgs : EventArgs;

        void RemoveEventHandler(string eventName, EventHandler value);
    }
}