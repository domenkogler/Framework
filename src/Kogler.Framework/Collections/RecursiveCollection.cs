using System.Linq;
using Caliburn.Micro;

namespace Kogler.Framework
{
    public class RecursiveCollectionBase<TEntity> : BindableCollection<TEntity>, IRecursiveCollection where TEntity : class//, IRecursive
    {
        public RecursiveCollectionBase(TEntity entity = null)
        {
            var recursiveEntity = entity as IRecursiveContainer;
            if (recursiveEntity == null) return;
            Container = recursiveEntity;
            Execute.OnUIThread(() =>
            {
                if (recursiveEntity.Collection != null) this.Add(recursiveEntity.Collection.OfType<TEntity>().Order());
                this.RecalculatePositions<TEntity>();
            });
        }

        public bool IsIPosition => typeof(TEntity).HasInterface<IPosition>();
        public bool IsIModifiable => typeof(TEntity).HasInterface<IModifiable>();
        public IRecursiveContainer Container { get; }
        //internal void Deleteted(TEntity item) { }
        //internal void Added(TEntity item) { }

        protected override void InsertItemBase(int index, TEntity item)
        {
            this.Adopt(item);
            base.InsertItemBase(index, item);
            this.RecalculatePositions<TEntity>();
        }

        protected override void RemoveItemBase(int index)
        {
            this.Discard(this[index]);
            base.RemoveItemBase(index);
            this.RecalculatePositions<TEntity>();
        }

        protected override void ClearItemsBase()
        {
            foreach (var item in this) this.Discard(item);
            base.ClearItemsBase();
            this.RecalculatePositions<TEntity>();
        }

        protected override void SetItemBase(int index, TEntity item)
        {
            this.Adopt(item);
            base.SetItemBase(index, item);
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