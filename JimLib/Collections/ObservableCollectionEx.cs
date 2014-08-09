using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace JimLib.Collections
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
                Remove(item);
                updated = true;
            }

            foreach (var item in collection.Where(i => toAdd.Contains(keyFunc(i))).ToList())
            {
                Add(item);
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

            return updated;
        }

        public bool UpdateToMatch<TKey>(T item, Func<T, TKey> keyFunc, Func<T, T, bool> updateAction = null)
        {
            return UpdateToMatch(new List<T> {item}, keyFunc, updateAction);
        }
    }
}
