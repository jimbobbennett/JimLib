using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace JimBobBennett.JimLib.Extensions
{
    /// <summary>
    /// Extension methods for Type
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets all the properties on the given type and all base types.
        /// This is provided for portable libraries where only the Declared properties are available from the TypeInfo
        /// </summary>
        /// <param name="type">The type to get the properties on</param>
        /// <returns>An enumerable of <see cref="PropertyInfo"/> for the properties on the type and all base types</returns>
        [Pure]
        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            return type == typeof(object) ? new List<PropertyInfo>() : type.GetTypeInfo().DeclaredProperties.Union(type.GetTypeInfo().BaseType.GetAllProperties()).ToList();
        }

        /// <summary>
        /// Checks a type to see if it derives from a raw generic (e.g. List[[]])
        /// </summary>
        /// <param name="toCheck">The type to check</param>
        /// <param name="generic">The raw generic type</param>
        /// <returns>True if toCheck is derived from generic, otherwise false</returns>
        [Pure]
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != typeof(object))
            {
                var cur = toCheck.GetTypeInfo().IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                    return true;
                
                toCheck = toCheck.GetTypeInfo().BaseType;
            }

            return false;
        }
    }
}
