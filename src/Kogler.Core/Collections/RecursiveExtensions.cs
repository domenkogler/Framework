using System.Collections.Generic;
using System.Linq;

namespace Kogler.Framework
{
    public static class RecursiveExtensions
    {
        public static IEnumerable<TEntity> Order<TEntity>(this IEnumerable<TEntity> entities)
        {
            var isIPosition = typeof(TEntity).HasInterface<IPosition>();
            var isIModifiable = typeof(TEntity).HasInterface<IModifiable>();
            var w = isIModifiable ? entities.OfType<IModifiable>().Where(e => e != null && !e.Deleted).Cast<TEntity>() : entities;
            return isIPosition ? w.OfType<IPosition>().OrderBy(o => o.Position).Cast<TEntity>() : w;
        }

        //public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> childSelector)
        //{
        //    var stack = new Stack<T>(items);
        //    while (stack.Any())
        //    {
        //        var next = stack.Pop();
        //        foreach (var child in childSelector(next)) stack.Push(child);
        //        yield return next;
        //    }
        //}

        public static IEnumerable<TEntity> Deep<TEntity>(this TEntity entity) where TEntity : IRecursiveBase<TEntity>
        {
            return entity.WithDeep().Skip(1);
        }

        public static IEnumerable<TEntity> WithDeep<TEntity>(this TEntity entity) where TEntity : IRecursiveBase<TEntity>
        {
            if (Equals(entity, null)) yield break;
            yield return entity;
            foreach (var child in entity.ChildrenCollection.SelectMany(s => s.WithDeep()))
            {
                yield return child;
            }
        }

        public static IEnumerable<TEntity> WithSiblings<TEntity>(this TEntity entity) where TEntity : IRecursiveContainer
        {
            if (Equals(entity, null)) return Enumerable.Empty<TEntity>();
            var parent = entity.Container;
            return Equals(parent, null) ? Enumerable.Empty<TEntity>() : parent.Collection.OfType<TEntity>();
        }

        public static IEnumerable<TEntity> Siblings<TEntity>(this TEntity entity) where TEntity : IRecursiveContainer
        {
            if (Equals(entity, null)) return Enumerable.Empty<TEntity>();
            return entity.WithSiblings().Where(e => !Equals(e, entity));
        }

        public static IEnumerable<TEntity> Parents<TEntity>(this TEntity entity) where TEntity : class, IRecursive<TEntity>
        {
            if (Equals(entity, null)) yield break;
            var parent = entity.Parent;
            while (!Equals(parent, default(TEntity)))
            {
                yield return parent;
                parent = parent.Parent;
            }
        }

        public static IEnumerable<TEntity> WithParents<TEntity>(this TEntity entity) where TEntity : class, IRecursive<TEntity>
        {
            if (Equals(entity, null)) return Enumerable.Empty<TEntity>();
            return entity.Parents().Union(new[] { entity });
        }

        public static bool IsParentOf<TEntity>(this TEntity parent, TEntity entity) where TEntity : class, IRecursive<TEntity>
        {
            if (parent == null || entity == null) return false;
            return entity.Parents().Contains(parent);
        }

        public static bool IsChildOf<TEntity>(this TEntity entity, TEntity parent) where TEntity : IRecursiveBase<TEntity>
        {
            if (Equals(parent, null) || Equals(entity, null)) return false;
            return parent.Deep().Contains(entity);
        }

        /// <summary>
        /// Filter items by child presence - return only items without children (remove parents)
        /// </summary>
        /// <typeparam name="TEntity">original IEnumerable</typeparam>
        /// <param name="dics"></param>
        /// <returns>original IEnumerable with only items that have no children</returns>
        public static IEnumerable<TEntity> Flat<TEntity>(this IEnumerable<TEntity> dics) where TEntity : IRecursiveBase<TEntity>
        {
            return dics.Where(d => !d.ChildrenCollection.Any());
        }

        /// <summary>
        /// Filter items by child presence - return parents only (remove items with no children)
        /// </summary>
        /// <typeparam name="TEntity">original IEnumerable</typeparam>
        /// <param name="dics"></param>
        /// <returns>original IEnumerable with child nodes removed</returns>
        public static IEnumerable<TEntity> Roots<TEntity>(this IEnumerable<TEntity> dics) where TEntity : IRecursiveBase<TEntity>
        {
            return dics.Where(d => d.ChildrenCollection.Any());
        }

        public static IEnumerable<TEntity> InLine<TEntity>(this TEntity entity) where TEntity : class, IRecursive<TEntity>
        {
            if (entity == null) return Enumerable.Empty<TEntity>();
            return entity.WithDeep().Union(entity.Parents());
        }

        public static IEnumerable<TEntity> InLineFlat<TEntity>(this TEntity entity) where TEntity : class, IRecursive<TEntity>
        {
            if (entity == null) return Enumerable.Empty<TEntity>();
            return entity.InLine().Union(entity.Parents().SelectMany(p => p.ChildrenCollection)).Distinct().Flat();
        }
    }
}