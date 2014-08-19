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
        private class ClassPropertyMap
        {
            public ClassPropertyMap(ReadOnlyDictionary<string, PropertyInfo> propertiesByName, ReadOnlyDictionary<string, List<string>> dependentProperties)
            {
                PropertiesByName = propertiesByName;
                DependentProperties = dependentProperties;
            }

            public ReadOnlyDictionary<string, PropertyInfo> PropertiesByName { get; private set; }
            public ReadOnlyDictionary<string, List<string>> DependentProperties { get; private set; }
        }

        private static readonly Dictionary<Type, ClassPropertyMap> PropertyMaps = new Dictionary<Type, ClassPropertyMap>();
        
        private readonly object _syncObj = new object();
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected internal void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            RaisePropertyChangedEvent(propertyName);
        }

        protected internal void RaisePropertyChanged<TValue>(Expression<Func<TValue>> propertyExpression)
        {
            RaisePropertyChangedEvent(this.ExtractPropertyName(propertyExpression));
        }

        protected internal void RaisePropertyChangedForAll()
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

        private IEnumerable<string> GetDependentProperties(string propertyName)
        {
            var classPropertyMap = LookUpProperties();

            List<string> retVal;
            return classPropertyMap.DependentProperties.TryGetValue(propertyName, out retVal) ? retVal : null;
        }

        protected ReadOnlyDictionary<string, PropertyInfo> PropertiesByName
        {
            get
            {
                var classPropertyMap = LookUpProperties();
                return classPropertyMap.PropertiesByName;
            }
        } 

        private ClassPropertyMap LookUpProperties()
        {
            lock (_syncObj)
            {
                ClassPropertyMap classPropertyMap;
                var type = GetType();

                if (PropertyMaps.TryGetValue(type, out classPropertyMap))
                    return classPropertyMap;

                var dict = new Dictionary<string, List<string>>();

                var properties = new ReadOnlyCollection<PropertyInfo>(type.GetAllProperties().ToList());

                var propertiesByName = new ReadOnlyDictionary<string, PropertyInfo>(properties.ToDictionary(p => p.Name, p => p));

                foreach (var propertyInfo in properties)
                {
                    var attributes = propertyInfo.GetCustomAttributes<NotifyPropertyChangeDependencyAttribute>().ToList();

                    if (attributes.Any())
                        dict[propertyInfo.Name] = attributes.Select(a => a.DependentPropertyName).ToList();
                }

                var dependentProperties = new ReadOnlyDictionary<string, List<string>>(dict);
                
                classPropertyMap = new ClassPropertyMap(propertiesByName, dependentProperties);
                PropertyMaps.Add(type, classPropertyMap);

                return classPropertyMap;
            }
        }
    }
}
