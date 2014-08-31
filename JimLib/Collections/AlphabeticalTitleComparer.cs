using System.Collections.Generic;

namespace JimBobBennett.JimLib.Collections
{
    public class AlphabeticalTitleComparer<T> : IComparer<ListItemInnerCollection<T>>
    {
        public int Compare(ListItemInnerCollection<T> x, ListItemInnerCollection<T> y)
        {
            return System.String.Compare(x.Title, y.Title, System.StringComparison.Ordinal);
        }
    }
}
