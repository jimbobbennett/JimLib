using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JimBobBennett.JimLib.Extensions;

namespace JimBobBennett.JimLib.Collections
{
    public class ListItemCollection<T> : ReadOnlyObservableCollection<ListItemInnerCollection<T>>
    {
        private readonly object _syncObj = new object();
        private readonly ObservableCollectionEx<ListItemInnerCollection<T>> _list;

        private ListItemCollection(ObservableCollectionEx<ListItemInnerCollection<T>> list) : base(list)
        {
            _list = list;
        }

        public ListItemCollection() : this(new ObservableCollectionEx<ListItemInnerCollection<T>>())
        {
            
        }

        public bool AddGroup(string title, IEnumerable<T> items)
        {
            lock (_syncObj)
            {
                if (_list.Any(l => l.Title == title))
                    return false;

                _list.Add(new ListItemInnerCollection<T>(title, items));

                return true;
            }
        }

        public bool RemoveGroup(string title)
        {
            lock (_syncObj)
            {
                var toRemove = _list.FirstOrDefault(l => l.Title == title);

                if (toRemove == null)
                    return false;

                _list.Remove(toRemove);

                return true;
            }
        }

        public void Clear()
        {
            lock (_syncObj)
                _list.Clear();
        }

        public void Add(string title, T item)
        {
            lock (_syncObj)
            {
                var group = this.FirstOrDefault(g => g.Title == title);

                if (group == null)
                    AddGroup(title, item.AsList());
                else
                    group.Add(item);
            }
        }

        public bool Delete(T item)
        {
            lock (_syncObj)
            {
                var removed = this.Where(i => i.Remove(item)).ToList();

                if (!removed.Any()) return false;

                foreach (var inner in removed.Where(i => !i.Any()))
                    RemoveGroup(inner.Title);

                return true;
            }
        }

        public void AddRange(IEnumerable<Tuple<string, IEnumerable<T>>> items)
        {
            lock (_syncObj)
            {
                _list.AddRange(items.Where(i => _list.All(l => l.Title != i.Item1))
                    .Select(i => new ListItemInnerCollection<T>(i.Item1, i.Item2)).ToList());
            }
        }

        public void AddRange(IEnumerable<ListItemInnerCollection<T>> items)
        {
            lock (_syncObj)
                _list.AddRange(items.Where(i => _list.All(l => l.Title != i.Title)).ToList());
        }

        public void ClearAndAddRange(IEnumerable<Tuple<string, IEnumerable<T>>> items)
        {
            lock (_syncObj)
            {
                _list.ClearAndAddRange(items.Where(i => _list.All(l => l.Title != i.Item1))
                    .Select(i => new ListItemInnerCollection<T>(i.Item1, i.Item2)));
            }
        }

        public void ClearAndAddRange(IEnumerable<ListItemInnerCollection<T>> items)
        {
            lock (_syncObj)
                _list.ClearAndAddRange(items.Where(i => _list.All(l => l.Title != i.Title)));
        }
    }
}
