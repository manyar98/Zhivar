using OMF.Common;
using OMF.EntityFramework.DataContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace OMF.EntityFramework.Ef6
{
    public abstract class FakeDbContext : IFakeDbContext, IDataContextAsync, IDataContext, IDisposable
    {
        private readonly Dictionary<Type, object> _fakeDbSets;

        protected FakeDbContext()
        {
            this._fakeDbSets = new Dictionary<Type, object>();
        }

        public int SaveChanges()
        {
            return 0;
        }

        public void SyncObjectState<TEntity>(TEntity entity) where TEntity : class, IObjectState
        {
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return new Task<int>((Func<int>)(() => 0));
        }

        public Task<int> SaveChangesAsync()
        {
            return new Task<int>((Func<int>)(() => 0));
        }

        public void Dispose()
        {
        }

        public DbSet<T> Set<T>() where T : class
        {
            return (DbSet<T>)this._fakeDbSets[typeof(T)];
        }

        public void AddFakeDbSet<TEntity, TFakeDbSet>()
          where TEntity : OMF.Common.Entity, new()
          where TFakeDbSet : FakeDbSet<TEntity>, IDbSet<TEntity>, new()
        {
            this._fakeDbSets.Add(typeof(TEntity), (object)new TFakeDbSet());
        }

        public void SyncObjectsStatePostCommit()
        {
        }
    }
}
