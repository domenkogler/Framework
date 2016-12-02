using System.Globalization;

namespace Kogler.Standard
{
    public static class RecursiveCollectionHelper
    {
        public static void Adopt<TEntity>(this IRecursiveCollection collection, TEntity item) where TEntity : class
        {
            var recursive = item as IRecursiveContainer;
            if (recursive != null) recursive.Container = collection.Container;
            //collection.Added(item);
        }

        public static void Discard<TEntity>(this IRecursiveCollection collection, TEntity item) where TEntity : class
        {
            //collection.Deleteted(item);
            var recursive = item as IRecursiveContainer;
            if (recursive == null) return;
            recursive.RecursiveCollection = null;
        }

        public static void RecalculatePositions<TEntity>(this IRecursiveCollection collection) where TEntity : class
        {
            if (!typeof(TEntity).HasInterface<IPosition>()) return;
            //if (IsNotifying) return;
            for (int i = 0; i < collection.Count; i++)
            {
                var iPosition = ((IPosition)collection[i]);
                if (collection.Container == null) iPosition.Position = AlphaNumString(i);
                else
                {
                    var oldPos = iPosition.Position;
                    iPosition.Position = (collection.Container == null ? string.Empty : ((IPosition)collection.Container).Position + ".") + AlphaNumString(i);
                    var recursive = collection[i] as IRecursiveContainer;
                    if (recursive == null) continue;
                    if (oldPos != iPosition.Position) recursive.RecursiveCollection?.RecalculatePositions<TEntity>();
                }
            }
        }

        private static string AlphaNumString(int i, int significance = 2)
        {
            var i_s = i.ToString(CultureInfo.InvariantCulture);
            while (i_s.Length < significance)
            {
                i_s = "0" + i_s;
            }
            return i_s;
        }
    }
}