using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kogler.Framework
{
    public static class IEnumerableExtensions
    {
        public static TCollection[] ToArrayLock<TCollection>(this IEnumerable<TCollection> collection)
        {
            if (!(collection is ICollection)) return collection.ToArray();
            lock (((ICollection)collection).SyncRoot) { return collection.ToArray(); }
        }
    }
}