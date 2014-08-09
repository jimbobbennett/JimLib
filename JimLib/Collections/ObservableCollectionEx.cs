using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace JimBobBennett.JimLib.Collections
{
    public class ObservableCollectionEx<T>: ObservableCollection<T>
    {
        /// <summary> 
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
        /// A collection changed notification is raised at the end of the add, not after each item.
        /// </summary> 
        public void AddRange(IEnumerable<T> collection)
        {
            CheckReentrancy();

            if (collection == null) throw new ArgumentNullException("collection");

            var enumerable = collection as IList<T> ?? collection.ToList();

            foreach (var i in enumerable)
                Items.Add(i);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary> 
        /// Clears then adds the elements of the specified collection to the end of the ObservableCollection(Of T).
        /// A collection change notification is raised at the end of the clear and add, not at each step. 
        /// </summary> 
        public void ClearAndAddRange(IEnumerable<T> collection)
        {
            CheckReentrancy();

            if (collection == null) throw new ArgumentNullException("collection");

            var enumerable = collection as IList<T> ?? collection.ToList();

            Items.Clear();

            foreach (var i in enumerable)
                Items.Add(i);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Updates the collection to match the passed in collection.  Matching is done using the key for each item,
        /// generated using the <param name="keyFunc"></param>.
        /// If the item in the passed in collection is not in this collection, they are added.
        /// If there are any items in this collection that are not in the passed in collection, they are removed.
        /// If there are any items in the passed in collection that are also in this collection, they are updated using the
        /// <param name="updateAction"> if this is set.</param>
        /// If any items are added/removed/updated then a single collection change notification is raised with an action of reset.
        /// </summary>
        /// <typeparam name="TKey">The type of the key</typeparam>
        /// <param name="collection">The collection to update this to match</param>
        /// <param name="keyFunc">A func that returns the key for the item</param>
        /// <param name="updateAction">An action called for each item in the collections that match to update the values.  If this is null, no updates happen</param>
        /// <returns>True if any items are added/removed or updated, otherwise false.</returns>
        public bool UpdateToMatch<TKey>(ICollection<T> collection, Func<T, TKey> keyFunc, Func<T, T, bool> updateAction = null)
        {
            CheckReentrancy();

            var updated = false;

            var newKeys = new HashSet<TKey>(collection.Select(keyFunc));
            var oldKeys = new HashSet<TKey>(Items.Select(keyFunc));

            var toDelete = new HashSet<TKey>(oldKeys);
            toDelete.ExceptWith(newKeys);

            var toAdd = new HashSet<TKey>(newKeys);
            toAdd.ExceptWith(oldKeys);

            foreach (var item in Items.Where(i => toDelete.Contains(keyFunc(i))).ToList())
            {
                Items.Remove(item);
                updated = true;
            }

            foreach (var item in collection.Where(i => toAdd.Contains(keyFunc(i))).ToList())
            {
                Items.Add(item);
                updated = true;
            }

            if (updateAction == null) 
                return updated;

            var toUpdate = new HashSet<TKey>(oldKeys);
            toUpdate.IntersectWith(newKeys);
            
            foreach (var key in toUpdate)
            {
                var oldItem = Items.FirstOrDefault(i => Equals(keyFunc(i), key));
                var newItem = collection.FirstOrDefault(i => Equals(keyFunc(i), key));

                if (!Equals(oldItem, default(T)) && !Equals(newItem, default(T)))
                    updated = updateAction(oldItem, newItem) | updated;
            }

            if (updated)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            return updated;
        }
    }
}
