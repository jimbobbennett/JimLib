using System;
using System.Collections.Generic;
using System.Linq;

namespace JimBobBennett.JimLib.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Updates the collection to match the passed in collection.  Matching is done using the key for each item,
        /// generated using the <param name="keyFunc"></param>.
        /// If the item in the passed in collection is not in this collection, they are added.
        /// If there are any items in this collection that are not in the passed in collection, they are removed.
        /// If there are any items in the passed in collection that are also in this collection, they are updated using the
        /// <param name="updateAction"> if this is set.</param>
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <typeparam name="TKey">The type of the key</typeparam>
        /// <param name="originalCollection">The list to update</param>
        /// <param name="collection">The collection to update this to match</param>
        /// <param name="keyFunc">A func that returns the key for the item</param>
        /// <param name="updateAction">An action called for each item in the collections that match to update the values.  If this is null, no updates happen</param>
        /// <returns>True if any items are added/removed or updated, otherwise false.</returns>
        public static bool UpdateToMatch<T, TKey>(this ICollection<T> originalCollection, ICollection<T> collection, Func<T, TKey> keyFunc, Func<T, T, bool> updateAction = null)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (keyFunc == null) throw new ArgumentNullException("keyFunc");

            var updated = false;

            var newKeys = new HashSet<TKey>(collection.Select(keyFunc));
            var oldKeys = new HashSet<TKey>(originalCollection.Select(keyFunc));

            var toDelete = new HashSet<TKey>(oldKeys);
            toDelete.ExceptWith(newKeys);

            var toAdd = new HashSet<TKey>(newKeys);
            toAdd.ExceptWith(oldKeys);

            foreach (var item in originalCollection.Where(i => toDelete.Contains(keyFunc(i))).ToList())
            {
                originalCollection.Remove(item);
                updated = true;
            }

            foreach (var item in collection.Where(i => toAdd.Contains(keyFunc(i))).ToList())
            {
                originalCollection.Add(item);
                updated = true;
            }

            if (updateAction != null)
            {
                var toUpdate = new HashSet<TKey>(oldKeys);
                toUpdate.IntersectWith(newKeys);

                foreach (var key in toUpdate)
                {
                    var oldItem = originalCollection.FirstOrDefault(i => Equals(keyFunc(i), key));
                    var newItem = collection.FirstOrDefault(i => Equals(keyFunc(i), key));

                    if (!Equals(oldItem, default(T)) && !Equals(newItem, default(T)))
                        updated = updateAction(oldItem, newItem) | updated;
                }
            }

            return updated;
        }
    }
}
