using System.Collections.Generic;

namespace JimBobBennett.JimLib.Collections
{
    public class ListItemInnerCollection<T> : ObservableCollectionEx<T>
    {
        public ListItemInnerCollection(string title)
        {
            Title = title;
        }

        public ListItemInnerCollection(string title, IEnumerable<T> items)
            : base(items)
        {
            Title = title;
        }

        public string Title { get; private set; }
    }
}
