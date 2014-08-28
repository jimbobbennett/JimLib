using System;
using System.Collections.Generic;
using System.Linq;

namespace JimBobBennett.JimLib.Collections
{
    public class DefinedSortOrderComparer<T, TKey> : IComparer<T>
    {
        private readonly IList<TKey> _sortedKeys;
        private readonly Func<T, TKey> _keyFunc;

        public DefinedSortOrderComparer(IEnumerable<TKey> sortedKeys, Func<T, TKey> keyFunc)
        {
            _sortedKeys = sortedKeys.ToList();
            _keyFunc = keyFunc;
        }
        
        public int Compare(T x, T y)
        {
            var xKey = _keyFunc(x);
            var yKey = _keyFunc(y);

            var xPos = _sortedKeys.IndexOf(xKey);
            var yPos = _sortedKeys.IndexOf(yKey);

            if (xPos > -1)
                return yPos == -1 ? -1 : xPos.CompareTo(yPos);

            return yPos > -1 ? 1 : 0;
        }
    }
}
