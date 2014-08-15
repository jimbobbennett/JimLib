using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace JimBobBennett.JimLib.Extensions
{
    /// <summary>
    /// Extensions on Object
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns a new <see cref="List{T}"/> containing the given item
        /// </summary>
        /// <typeparam name="T">The type of the item</typeparam>
        /// <param name="item">The item to add to the list</param>
        /// <returns>A new <see cref="List{T}"/> containing the given item</returns>
        [Pure]
        public static List<T> AsList<T>(this T item)
        {
            return new List<T> { item };
        }

        /// <summary>
        /// Returns a new array of T containing the given item
        /// </summary>
        /// <typeparam name="T">The type of the item</typeparam>
        /// <param name="item">The item to add to the array</param>
        /// <returns>A new array of T containing the given item</returns>
        [Pure]
        public static T[] AsArray<T>(this T item)
        {
            return new T[] {item};
        }

        /// <summary>
        /// Wasits for the given condition to occur, timing out after the given timeout
        /// </summary>
        /// <param name="o"></param>
        /// <param name="condition"></param>
        /// <param name="timeout"></param>
        /// <returns>true if the condition passes within the timeout, otherwise false</returns>
        [Pure]
        public static async Task<bool> WaitForAsync(this object o, Func<bool> condition, int timeout = 1000)
        {
            return await WaitForCondition(condition, timeout);
        }

        private static async Task<bool> WaitForCondition(Func<bool> condition, int timeout)
        {
            var wait = 0;
            const int sleep = 10;

            while (wait < timeout)
            {
                if (condition())
                    return true;

                await Task.Delay(sleep);
                wait += sleep;
            }

            return condition();
        }

        [Pure]
        public static PropertyInfo ExtractPropertyInfo<TValue>(this object o, Expression<Func<TValue>> propertyExpression)
        {
            var memberExpression = (MemberExpression)propertyExpression.Body;
            return (PropertyInfo)memberExpression.Member;
        }

        [Pure]
        public static string ExtractPropertyName<TValue>(this object o, Expression<Func<TValue>> propertyExpression)
        {
            return o.ExtractPropertyInfo(propertyExpression).Name;
        }
    }
}
