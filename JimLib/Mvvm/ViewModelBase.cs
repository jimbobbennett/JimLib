using System;
using System.ComponentModel;

namespace JimBobBennett.JimLib.Mvvm
{
    public abstract class ViewModelBase<T> : NotificationObject
    {
        private T _model;

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

            OnModelPropertyChanged(e.PropertyName);
        }

        protected void FireEvent(EventHandler handler)
        {
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected void FireEvent<TArgs>(EventHandler<TArgs> handler, TArgs args)
            where TArgs : EventArgs
        {
            if (handler != null) handler(this, args);
        }
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
