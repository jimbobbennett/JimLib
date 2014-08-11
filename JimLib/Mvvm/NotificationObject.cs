using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using JimBobBennett.JimLib.Annotations;
using JimBobBennett.JimLib.Extensions;

namespace JimBobBennett.JimLib.Mvvm
{
    public abstract class NotificationObject : INotifyPropertyChanged
    {
        private static ReadOnlyDictionary<string, PropertyInfo> _propertiesByName;
        private static ReadOnlyCollection<PropertyInfo> _properties; 
        private static ReadOnlyDictionary<string, List<string>> _dependentProperties;
        private static bool _needPropertyLookup = true;

        private readonly static object SyncObj = new object();
        
        protected static PropertyInfo ExtractPropertyInfo<TValue>(Expression<Func<TValue>> propertyExpression)
        {
            var memberExpression = (MemberExpression)propertyExpression.Body;
            return (PropertyInfo)memberExpression.Member;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            RaisePropertyChangedEvent(propertyName);
        }

        protected void RaisePropertyChanged<TValue>(Expression<Func<TValue>> propertyExpression)
        {
            RaisePropertyChangedEvent(ExtractPropertyInfo(propertyExpression).Name);
        }

        protected void RaisePropertyChangedForAll()
        {
            RaisePropertyChangedEvent(string.Empty);
        }

        private void RaisePropertyChangedEvent(string propertyName)
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

        protected internal IEnumerable<string> GetDependentProperties(string propertyName)
        {
            LookUpProperties();

            List<string> retVal;
            return _dependentProperties.TryGetValue(propertyName, out retVal) ? retVal : null;
        }

        protected internal ReadOnlyDictionary<string, PropertyInfo> PropertiesByName
        {
            get
            {
                LookUpProperties();
                return _propertiesByName;
            }
        }

        protected internal IEnumerable<PropertyInfo> Properties
        {
            get
            {
                LookUpProperties();
                return _properties;
            }
        } 

        private void LookUpProperties()
        {
            lock (SyncObj)
            {
                if (!_needPropertyLookup) return;

                var dict = new Dictionary<string, List<string>>();

                _properties = new ReadOnlyCollection<PropertyInfo>(GetType().GetAllProperties().ToList());

                _propertiesByName = new ReadOnlyDictionary<string, PropertyInfo>(_properties.ToDictionary(p => p.Name, p => p));

                foreach (var propertyInfo in _properties)
                {
                    var attributes = propertyInfo.GetCustomAttributes<NotifyPropertyChangeDependencyAttribute>().ToList();

                    if (attributes.Any())
                        dict[propertyInfo.Name] = attributes.Select(a => a.DependentPropertyName).ToList();
                }

                _dependentProperties = new ReadOnlyDictionary<string, List<string>>(dict);
                _needPropertyLookup = false;
            }
        }
    }
}
