using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JimLib.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            return type == typeof(object) ? new List<PropertyInfo>() : type.GetTypeInfo().DeclaredProperties.Union(type.GetTypeInfo().BaseType.GetAllProperties()).ToList();
        }
    }
}
