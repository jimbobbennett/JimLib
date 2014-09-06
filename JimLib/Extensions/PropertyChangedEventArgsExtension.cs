using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace JimBobBennett.JimLib.Extensions
{
    public static class PropertyChangedEventArgsExtension
    {
        /// <summary>
        /// Gets if a property change matches the given property name.
        /// This will also match any property to string.Empty as this is the standard way
        /// to indicate all properties have changed
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="args"></param>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        [Pure]
        public static bool PropertyNameMatches<TValue>(this PropertyChangedEventArgs args, Expression<Func<TValue>> propertyExpression)
        {
            return args.PropertyName == string.Empty || args.PropertyName == args.ExtractPropertyName(propertyExpression);
        }
    }
}
