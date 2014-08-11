using System.ComponentModel;

namespace JimBobBennett.JimLib.Mvvm
{
    public abstract class ViewModelBase<T> : NotificationObject
    {
        private T _model;

        protected T Model
        {
            get { return _model; }
            set
            {
                if (Equals(value, _model)) return;

                var npc = _model as INotifyPropertyChanged;
                if (npc != null)
                    npc.PropertyChanged -= ModelOnPropertyChanged;

                _model = value;

                npc = _model as INotifyPropertyChanged;
                if (npc != null)
                    npc.PropertyChanged += ModelOnPropertyChanged;

                RaisePropertyChangedForAll();

                OnModelChanged();
            }
        }

        protected ViewModelBase(T model)
        {
            Model = model;
        }
        
        protected virtual void OnModelChanged()
        {
            
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertiesByName.ContainsKey(e.PropertyName))
                RaisePropertyChanged(e.PropertyName);
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
