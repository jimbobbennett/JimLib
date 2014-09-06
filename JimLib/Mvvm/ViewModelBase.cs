using System;
using System.ComponentModel;
using System.Threading.Tasks;
using JimBobBennett.JimLib.Events;
using JimBobBennett.JimLib.Extensions;

namespace JimBobBennett.JimLib.Mvvm
{
    public abstract class ViewModelBase<T> : NotificationObject, IBusy
    {
        private T _model;
        private bool _isBusy;
        private string _busyMessage;

        public T Model
        {
            get { return _model; }
            set
            {
                if (Equals(value, _model)) return;

                var npc = _model as INotifyPropertyChanged;
                if (npc != null)
                    npc.PropertyChanged -= ModelOnPropertyChanged;

                var old = _model;

                _model = value;

                npc = _model as INotifyPropertyChanged;
                if (npc != null)
                    npc.PropertyChanged += ModelOnPropertyChanged;

                RaisePropertyChangedForAll();

                OnModelChanged(old, _model);
            }
        }

        public bool HasModel { get { return !Equals(Model, null); } }

        protected ViewModelBase(T model)
        {
            Model = model;
        }

        protected ViewModelBase()
            : this(default(T))
        {
            
        }
        
        protected virtual void OnModelChanged(T oldModel, T newModel)
        {
            
        }

        protected virtual void OnModelPropertyChanged(string propertyName)
        {

        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertiesByName.ContainsKey(e.PropertyName))
                RaisePropertyChanged(e.PropertyName);

            if (e.PropertyName.IsNullOrEmpty())
                RaisePropertyChangedForAll();

            OnModelPropertyChanged(e.PropertyName);
        }

        protected void FireEvent(EventHandler handler)
        {
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected TArgs FireEvent<TArgs>(EventHandler<TArgs> handler, TArgs args)
            where TArgs : EventArgs
        {
            if (handler != null)
                handler(this, args);

            return args;
        }

        protected TValue FireEvent<TValue>(EventHandler<EventArgs<TValue>> handler, TValue value)
        {
            var eventArgs = new EventArgs<TValue>(value);

            if (handler != null)
                handler(this, eventArgs);

            return eventArgs.Value;
        }

        protected internal async Task RunWithBusyIndicatorAsync(Func<Task> action, string message = default(string))
        {
            try
            {
                IsBusy = true;
                BusyMessage = message;
                await action();
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected internal async Task<TResult> RunWithBusyIndicatorAsync<TResult>(Func<Task<TResult>> action, string message = default(string))
        {
            try
            {
                IsBusy = true;
                BusyMessage = message;
                return await action();
            }
            finally
            {
                IsBusy = false;
            }
        }

        [NotifyPropertyChangeDependency("IsActive")]
        public bool IsBusy
        {
            get { return _isBusy; }
            protected internal set
            {
                if (value.Equals(_isBusy)) return;
                _isBusy = value;
                RaisePropertyChanged();
            }
        }

        public string BusyMessage
        {
            get { return _busyMessage; }
            set
            {
                if (Equals(_busyMessage, value)) return;
                _busyMessage = value;
                RaisePropertyChanged();
            }
        }

        public bool IsActive { get { return !IsBusy; } }
    }

    public abstract class ViewModelBase : ViewModelBase<object>
    {
        protected ViewModelBase(object model) : base(model)
        {
        }

        protected ViewModelBase()
            : base(null)
        {
        }
    }
}
