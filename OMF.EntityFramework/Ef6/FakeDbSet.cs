using OMF.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using static OMF.Common.Enums;

namespace OMF.EntityFramework.Ef6
{
    public abstract class FakeDbSet<TEntity> : DbSet<TEntity>, IDbSet<TEntity>, IQueryable<TEntity>, IEnumerable<TEntity>, IEnumerable, IQueryable
      where TEntity : OMF.Common.Entity, new()
    {
        private readonly ObservableCollection<TEntity> _items;
        private readonly IQueryable _query;

        protected FakeDbSet()
        {
            this._items = new ObservableCollection<TEntity>();
            this._query = (IQueryable)this._items.AsQueryable<TEntity>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this._items.GetEnumerator();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return this._items.GetEnumerator();
        }

        public Expression Expression
        {
            get
            {
                return this._query.Expression;
            }
        }

        public Type ElementType
        {
            get
            {
                return this._query.ElementType;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return this._query.Provider;
            }
        }

        public override TEntity Add(TEntity entity)
        {
            this._items.Add(entity);
            return entity;
        }

        public override TEntity Remove(TEntity entity)
        {
            this._items.Remove(entity);
            return entity;
        }

        public override TEntity Attach(TEntity entity)
        {
            switch (entity.ObjectState)
            {
                case ObjectState.Unchanged:
                case ObjectState.Added:
                    this._items.Add(entity);
                    break;
                case ObjectState.Modified:
                    this._items.Remove(entity);
                    this._items.Add(entity);
                    break;
                case ObjectState.Deleted:
                    this._items.Remove(entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return entity;
        }

        public override TEntity Create()
        {
            return new TEntity();
        }

        public override TDerivedEntity Create<TDerivedEntity>()
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public override ObservableCollection<TEntity> Local
        {
            get
            {
                return this._items;
            }
        }
    }
}
