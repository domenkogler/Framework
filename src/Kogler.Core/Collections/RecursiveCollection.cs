using System.Collections.ObjectModel;
using System.Linq;

namespace Kogler.Framework
{
    public class RecursiveCollectionBase<TEntity> : ObservableCollection<TEntity>, IRecursiveCollection where TEntity : class//, IRecursive
    {
        public RecursiveCollectionBase(TEntity entity = null)
        {
            var recursiveEntity = entity as IRecursiveContainer;
            if (recursiveEntity == null) return;
            Container = recursiveEntity;
            if (recursiveEntity.Collection != null) this.Add(recursiveEntity.Collection.OfType<TEntity>().Order());
            this.RecalculatePositions<TEntity>();
        }

        public bool IsIPosition => typeof(TEntity).HasInterface<IPosition>();
        public bool IsIModifiable => typeof (TEntity).HasInterface<IModifiable>();
        public IRecursiveContainer Container { get; }
        //internal void Deleteted(TEntity item) { }
        //internal void Added(TEntity item) { }

        protected override void InsertItem(int index, TEntity item)
        {
            this.Adopt(item);
            base.InsertItem(index, item);
            this.RecalculatePositions<TEntity>();
        }
        
        protected override void RemoveItem(int index)
        {
            this.Discard(this[index]);
            base.RemoveItem(index);
            this.RecalculatePositions<TEntity>();
        }

        protected override void ClearItems()
        {
            foreach (var item in this) this.Discard(item);
            base.ClearItems();
            this.RecalculatePositions<TEntity>();
        }

        protected override void SetItem(int index, TEntity item)
        {
            this.Adopt(item);
            base.SetItem(index, item);
            this.RecalculatePositions<TEntity>();
        }
    }

    public class RecursiveCollection<TEntity> : RecursiveCollectionBase<TEntity>, IRecursiveCollection<TEntity> where TEntity : class, IRecursive
    {
        public RecursiveCollection() { }
        public RecursiveCollection(TEntity entity) : base(entity) { }

        public new IRecursiveContainer<TEntity> Container => base.Container as IRecursiveContainer<TEntity>;
    }
}