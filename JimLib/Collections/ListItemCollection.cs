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
        private IComparer<ListItemInnerCollection<T>> _comparer;
        private bool _sortTitleAlphabetically;

        private ListItemCollection(ObservableCollectionEx<ListItemInnerCollection<T>> list)
            : base(list)
        {
            _list = list;
        }

        public ListItemCollection()
            : this(new ObservableCollectionEx<ListItemInnerCollection<T>>())
        {

        }

        public bool AddGroup(string title, IEnumerable<T> items)
        {
            lock (_syncObj)
            {
                if (_list.Any(l => l.Title == title))
                    return false;

                var comparer = _comparer;

                if (comparer != null)
                    _list.StopEvents = true;

                try
                {
                    _list.Add(new ListItemInnerCollection<T>(title, items));

                    if (comparer != null)
                        Sort();
                }
                finally
                {
                    if (comparer != null)
                        _list.StopEvents = false;
                }

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
                var comparer = _comparer;

                if (comparer != null)
                    _list.StopEvents = true;

                try
                {
                    var group = this.FirstOrDefault(g => g.Title == title);

                    if (group == null)
                        AddGroup(title, item.AsList());
                    else
                        group.Add(item);

                    if (comparer != null)
                        Sort();
                }
                finally
                {
                    if (comparer != null)
                        _list.StopEvents = false;
                }
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
                var comparer = _comparer;

                if (comparer != null)
                    _list.StopEvents = true;

                try
                {
                    _list.AddRange(items.Where(i => _list.All(l => l.Title != i.Item1))
                        .Select(i => new ListItemInnerCollection<T>(i.Item1, i.Item2)).ToList());

                    if (comparer != null)
                        Sort();
                }
                finally
                {
                    if (comparer != null)
                        _list.StopEvents = false;
                }
            }
        }

        public void AddRange(IEnumerable<ListItemInnerCollection<T>> items)
        {
            lock (_syncObj)
            {
                var comparer = _comparer;

                if (comparer != null)
                    _list.StopEvents = true;

                try
                {
                    _list.AddRange(items.Where(i => _list.All(l => l.Title != i.Title)).ToList());

                    if (comparer != null)
                        Sort();
                }
                finally
                {
                    if (comparer != null)
                        _list.StopEvents = false;
                }
            }
        }

        public void ClearAndAddRange(IEnumerable<Tuple<string, IEnumerable<T>>> items)
        {
            lock (_syncObj)
            {
                var comparer = _comparer;

                if (comparer != null)
                    _list.StopEvents = true;

                try
                {
                    _list.ClearAndAddRange(items.Where(i => _list.All(l => l.Title != i.Item1))
                        .Select(i => new ListItemInnerCollection<T>(i.Item1, i.Item2)));

                    if (comparer != null)
                        Sort();
                }
                finally
                {
                    if (comparer != null)
                        _list.StopEvents = false;
                }
            }
        }

        public void ClearAndAddRange(IEnumerable<ListItemInnerCollection<T>> items)
        {
            lock (_syncObj)
            {
                var comparer = _comparer;

                if (comparer != null)
                    _list.StopEvents = true;

                try
                {
                    _list.ClearAndAddRange(items.Where(i => _list.All(l => l.Title != i.Title)));

                    if (comparer != null)
                        Sort();
                }
                finally
                {
                    if (comparer != null)
                        _list.StopEvents = false;
                }
            }
        }

        public void ClearAndAddRange(IEnumerable<T> items, Func<T, string> titleFunc)
        {
            lock (_syncObj)
            {
                var comparer = _comparer;

                if (comparer != null)
                    _list.StopEvents = true;

                try
                {
                    foreach (var item in items)
                    {
                        var title = titleFunc(item);

                        var group = this.FirstOrDefault(g => g.Title == title);

                        if (group == null)
                            AddGroup(title, item.AsList());
                        else
                            group.Add(item);
                    }

                    if (comparer != null)
                        Sort();
                }
                finally
                {
                    if (comparer != null)
                        _list.StopEvents = false;
                }
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
                else if (SortTitleAlphabetically)
                    _comparer = new AlphabeticalTitleComparer<T>();
                else
                    _comparer = null;

                Sort();
            }
        }

        public bool SortTitleAlphabetically
        {
            get { return _sortTitleAlphabetically; }
            set
            {
                if (_sortTitleAlphabetically == value) return;

                _sortTitleAlphabetically = value;
                _comparer = new AlphabeticalTitleComparer<T>();
            }
        }

        private void Sort()
        {
            lock (_syncObj)
            {
                if (_comparer == null) return;

                var list = _list.ToList();
                list.Sort(_comparer);

                var needSort = false;
                for (var i = 0; i < list.Count && !needSort; i++)
                {
                    if (list[i] != _list[i])
                        needSort = true;
                }

                if (needSort)
                    _list.ClearAndAddRange(list);
            }
        }
    }
}
