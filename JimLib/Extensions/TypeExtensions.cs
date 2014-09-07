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
            var props = type.GetTypeInfo().DeclaredProperties.ToList();

            if (type != typeof(object))
            {
                var baseProps = GetAllProperties(type.GetTypeInfo().BaseType);
                foreach (var propertyInfo in baseProps.Where(pi => props.All(p => p.Name != pi.Name)))
                    props.Add(propertyInfo);
            }

            return props;
        }

        /// <summary>
        /// Gets all the events on the given type and all base types.
        /// This is provided for portable libraries where only the Declared events are available from the TypeInfo
        /// </summary>
        /// <param name="type">The type to get the events on</param>
        /// <returns>An enumerable of <see cref="EventInfo"/> for the events on the type and all base types</returns>
        [Pure]
        public static IEnumerable<EventInfo> GetAllEvents(this Type type)
        {
            var events = type.GetTypeInfo().DeclaredEvents.ToList();

            if (type != typeof(object))
            {
                var baseEvents = GetAllEvents(type.GetTypeInfo().BaseType);
                foreach (var eventInfo in baseEvents.Where(pi => events.All(p => p.Name != pi.Name)))
                    events.Add(eventInfo);
            }

            return events;
        }

        /// <summary>
        /// Gets all the methods on the given type and all base types.
        /// This is provided for portable libraries where only the Declared methods are available from the TypeInfo
        /// </summary>
        /// <param name="type">The type to get the methods on</param>
        /// <returns>An enumerable of <see cref="MethodInfo"/> for the methods on the type and all base types</returns>
        [Pure]
        public static IEnumerable<MethodInfo> GetAllMethods(this Type type)
        {
            var methods = type.GetTypeInfo().DeclaredMethods.ToList();

            if (type != typeof(object))
            {
                var baseMethods = GetAllMethods(type.GetTypeInfo().BaseType);
                foreach (var methodInfo in baseMethods.Where(pi => methods.All(p => p.Name != pi.Name)))
                    methods.Add(methodInfo);
            }

            return methods;
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
