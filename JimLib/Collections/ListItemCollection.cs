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
        private IEnumerable<string> _titleSortOrder;
        private DefinedSortOrderComparer<ListItemInnerCollection<T>, string> _comparer;

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

                Sort();

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

                Sort();

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

                Sort();
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

                Sort();

                return true;
            }
        }

        public void AddRange(IEnumerable<Tuple<string, IEnumerable<T>>> items)
        {
            lock (_syncObj)
            {
                _list.AddRange(items.Where(i => _list.All(l => l.Title != i.Item1))
                    .Select(i => new ListItemInnerCollection<T>(i.Item1, i.Item2)).ToList());
                Sort();
            }
        }

        public void AddRange(IEnumerable<ListItemInnerCollection<T>> items)
        {
            lock (_syncObj)
            {
                _list.AddRange(items.Where(i => _list.All(l => l.Title != i.Title)).ToList());
                Sort();
            }
        }

        public void ClearAndAddRange(IEnumerable<Tuple<string, IEnumerable<T>>> items)
        {
            lock (_syncObj)
            {
                _list.ClearAndAddRange(items.Where(i => _list.All(l => l.Title != i.Item1))
                    .Select(i => new ListItemInnerCollection<T>(i.Item1, i.Item2)));
                Sort();
            }
        }

        public void ClearAndAddRange(IEnumerable<ListItemInnerCollection<T>> items)
        {
            lock (_syncObj)
            {
                _list.ClearAndAddRange(items.Where(i => _list.All(l => l.Title != i.Title)));
                Sort();
            }
        }

        public IEnumerable<string> TitleSortOrder
        {
            get { return _titleSortOrder; }
            set
            {
                _titleSortOrder = value.ToList();

                if (_titleSortOrder != null)
                    _comparer = new DefinedSortOrderComparer<ListItemInnerCollection<T>, string>(_titleSortOrder, i => i.Title);
                else
                    _comparer = null;

                Sort();
            }
        }

        private void Sort()
        {
            lock (_syncObj)
            {
                if (_comparer == null) return;

                var list = _list.ToList();
                list.Sort(_comparer);
                _list.ClearAndAddRange(list);
            }
        }
    }
}
