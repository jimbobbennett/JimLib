using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using JimBobBennett.JimLib.Extensions;

namespace JimBobBennett.JimLib.Collections
{
    public class ObservableCollectionEx<T>: ObservableCollection<T>
    {
        public ObservableCollectionEx(IEnumerable<T> collection) : base(collection)
        {
        }

        public ObservableCollectionEx()
        {
        }

        /// <summary> 
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
        /// A collection changed notification is raised at the end of the add, not after each item.
        /// </summary> 
        public void AddRange(IEnumerable<T> collection)
        {
            CheckReentrancy();

            if (collection == null) throw new ArgumentNullException("collection");
            
            var enumerable = collection.ToList();

            if (!enumerable.Any())
                return;

            foreach (var i in enumerable)
                Items.Add(i);

            RaiseAdd(enumerable);
        }

        /// <summary> 
        /// Clears then adds the elements of the specified collection to the end of the ObservableCollection(Of T).
        /// A collection change notification is raised at the end of the clear and add, not at each step. 
        /// </summary> 
        public void ClearAndAddRange(IEnumerable<T> collection)
        {
            CheckReentrancy();

            if (collection == null) throw new ArgumentNullException("collection");

            var enumerable = collection.ToList();

            var hasItems = this.Any();
            var existing = this.ToList();

            Items.Clear();

            if (enumerable.Any())
            {
                foreach (var i in enumerable)
                    Items.Add(i);

                if (hasItems)
                    RaiseReset();
                else
                    RaiseAdd(enumerable);
            }
            else
            {
                if (hasItems)
                    RaiseRemove(existing);
            }
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

            var updated = Items.UpdateToMatch(collection, keyFunc, updateAction);

            if (updated)
                RaiseReset();

            return updated;
        }

        /// <summary>
        /// Raises a collection change event with an action of reset
        /// </summary>
        private void RaiseReset()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Raises a collection change event with an action of add
        /// </summary>
        private void RaiseAdd(IEnumerable<T> items)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList()));
        }

        /// <summary>
        /// Raises a collection change event with an action of remove
        /// </summary>
        private void RaiseRemove(IEnumerable<T> items)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items.ToList()));
        }
    }
}
