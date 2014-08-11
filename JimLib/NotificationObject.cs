using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JimBobBennett.JimLib.Annotations;
using JimBobBennett.JimLib.Extensions;

namespace JimBobBennett.JimLib
{
    public abstract class NotificationObject : INotifyPropertyChanged
    {
        private static ReadOnlyDictionary<string, List<string>> _dependentProperties;
        private readonly static object SyncObj = new object();
 
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            RaisePropertyChangeEvent(propertyName);
        }

        protected void RaisePropertyChangeForAll()
        {
            RaisePropertyChangeEvent(string.Empty);
        }

        private void RaisePropertyChangeEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));

                var dependent = GetDependentProperties(propertyName);
                if (dependent != null)
                   dependent.ForEach(s => handler(this, new PropertyChangedEventArgs(s)));
            }
        }

        private IEnumerable<string> GetDependentProperties(string propertyName)
        {
            lock (SyncObj)
            {
                if (_dependentProperties == null)
                {
                    var dict = new Dictionary<string, List<string>>();

                    var props = GetType().GetAllProperties();

                    foreach (var propertyInfo in props)
                    {
                        var attributes = propertyInfo.GetCustomAttributes<NotifyPropertyChangeDependencyAttribute>().ToList();

                        if (attributes.Any())
                            dict[propertyInfo.Name] = attributes.Select(a => a.DependentPropertyName).ToList();
                    }

                    _dependentProperties = new ReadOnlyDictionary<string, List<string>>(dict);
                }
            }

            List<string> retVal;
            return _dependentProperties.TryGetValue(propertyName, out retVal) ? retVal : null;
        }
    }
}
