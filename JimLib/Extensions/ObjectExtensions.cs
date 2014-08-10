using System.Collections.Generic;

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
        public static T[] AsArray<T>(this T item)
        {
            return new T[] {item};
        }
    }
}
