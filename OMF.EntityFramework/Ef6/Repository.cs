using OMF.Common;
using OMF.EntityFramework.Repositories;
using OMF.EntityFramework.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace OMF.EntityFramework.Ef6
{
    public class Repository<TEntity> : ViewRepository<TEntity>, IRepositoryAsync<TEntity>, IViewRepositoryAsync<TEntity>, IViewRepository<TEntity>, IRepository<TEntity>
      where TEntity : class, IEntity
    {
        private HashSet<object> _entitesChecked;

        public Repository(IUnitOfWorkAsync unitOfWork)
          : base(unitOfWork)
        {
        }

        public virtual void Insert(TEntity entity)
        {
            entity.ObjectState = ObjectState.Added;
            this._dbSet.Attach(entity);
            this._context.SyncObjectState<TEntity>(entity);
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
            try
            {
                if (this._context is DbContext)
                    ((DbContext)this._context).Configuration.AutoDetectChangesEnabled = false;
                foreach (TEntity entity in entities)
                    this.Insert(entity);
            }
            finally
            {
                if (this._context is DbContext)
                    ((DbContext)this._context).Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public virtual void InsertGraphRange(IEnumerable<TEntity> entities)
        {
            this._dbSet.AddRange(entities);
        }

        public virtual void InsertOrUpdateGraph(TEntity entity)
        {
            this.SyncObjectGraph((object)entity);
            this._entitesChecked = (HashSet<object>)null;
            this._dbSet.Attach(entity);
        }

        public virtual void Update(TEntity entity)
        {
            if (entity.ObjectState == ObjectState.Added)
                entity.ObjectState = ObjectState.Added;
            else
                entity.ObjectState = ObjectState.Modified;
            if (this._context is DbContext)
            {
                DbEntityEntry<TEntity> dbEntityEntry = (this._context as DbContext).ChangeTracker.Entries<TEntity>().FirstOrDefault<DbEntityEntry<TEntity>>((Func<DbEntityEntry<TEntity>, bool>)(en => ((IEnumerable<object>)en.Entity.GetID()).Select<object, string>((Func<object, string>)(id => id.ToString())).SequenceEqual<string>(((IEnumerable<object>)entity.GetID()).Select<object, string>((Func<object, string>)(id => id.ToString())))));
                if (dbEntityEntry != null)
                    dbEntityEntry.State = EntityState.Detached;
            }
            this._dbSet.Attach(entity);
            this._context.SyncObjectState<TEntity>(entity);
        }

        public virtual void Delete(object id)
        {
            this.Delete(this._dbSet.Find(id));
        }

        public virtual void Delete(TEntity entity)
        {
            if (entity.ObjectState == ObjectState.Added)
            {
                this._dbSet.Remove(entity);
                entity.ObjectState = ObjectState.Detached;
                this._context.SyncObjectState<TEntity>(entity);
            }
            else
            {
                entity.ObjectState = ObjectState.Deleted;
                this._dbSet.Attach(entity);
                this._context.SyncObjectState<TEntity>(entity);
            }
        }

        public virtual async Task<bool> DeleteAsync(params object[] keyValues)
        {
            bool flag = await this.DeleteAsync(CancellationToken.None, keyValues);
            return flag;
        }

        public virtual async Task<bool> DeleteAsync(
          CancellationToken cancellationToken,
          params object[] keyValues)
        {
            TEntity entity1 = await this.FindAsync(cancellationToken, keyValues);
            TEntity entity2 = entity1;
            entity1 = default(TEntity);
            if ((object)entity2 == null)
                return false;
            if (entity2.ObjectState == ObjectState.Added)
            {
                this._dbSet.Remove(entity2);
                entity2.ObjectState = ObjectState.Detached;
                this._context.SyncObjectState<TEntity>(entity2);
            }
            else
            {
                entity2.ObjectState = ObjectState.Deleted;
                this._dbSet.Attach(entity2);
                this._context.SyncObjectState<TEntity>(entity2);
            }
            return true;
        }

        private void SyncObjectGraph(object entity)
        {
            if (this._entitesChecked == null)
                this._entitesChecked = new HashSet<object>();
            if (this._entitesChecked.Contains(entity))
                return;
            this._entitesChecked.Add(entity);
            IObjectState objectState1 = entity as IObjectState;
            if (objectState1 != null && objectState1.ObjectState == ObjectState.Added)
                this._context.SyncObjectState<IObjectState>((IObjectState)entity);
            foreach (PropertyInfo property in entity.GetType().GetProperties())
            {
                if (!((IEnumerable<ParameterInfo>)property.GetIndexParameters()).Any<ParameterInfo>())
                {
                    IObjectState objectState2 = property.GetValue(entity, (object[])null) as IObjectState;
                    if (objectState2 != null)
                    {
                        if (objectState2.ObjectState == ObjectState.Added)
                            this._context.SyncObjectState<IObjectState>((IObjectState)entity);
                        this.SyncObjectGraph((object)objectState2);
                    }
                    else
                    {
                        IEnumerable<IObjectState> objectStates = property.GetValue(entity, (object[])null) as IEnumerable<IObjectState>;
                        if (objectStates != null)
                        {
                            foreach (object entity1 in objectStates)
                                this.SyncObjectGraph(entity1);
                        }
                    }
                }
            }
        }
    }
}
