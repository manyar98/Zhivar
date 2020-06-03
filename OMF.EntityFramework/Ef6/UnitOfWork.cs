using OMF.Common;
using OMF.EntityFramework.DataContext;
using OMF.EntityFramework.Repositories;
using OMF.EntityFramework.UnitOfWork;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using OMF.EntityFramework;
using OMF.EntityFramework.Ef6;

namespace OMF.EntityFramework.Ef6
{
    public class UnitOfWork : IUnitOfWorkAsync, IUnitOfWork, IDisposable
    {
        // Fields
        private IDataContextAsync _dataContext;
        private bool _disposed;
        private ObjectContext _objectContext;
        private DbTransaction _transaction;
        private Dictionary<string, object> _repositories;

        public IDataContextAsync DataContext
        {
            get
            {
                return this._dataContext;
            }
        }


        public UnitOfWork()
        {
            this._dataContext = OMFAppContext.DataContextCreator();
            this._repositories = new Dictionary<string, object>();
        }

        public UnitOfWork(IDataContextAsync dataContext)
        {
            this._dataContext = dataContext;
            this._repositories = new Dictionary<string, object>();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    try
                    {
                        if ((this._objectContext != null) && (this._objectContext.Connection.State == ConnectionState.Open))
                        {
                            this._objectContext.Connection.Close();
                        }
                        if (this._dataContext != null)
                        {
                            this._dataContext.Dispose();
                            this._dataContext = null;
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }
            }
            else
            {
                return;
            }
            this._disposed = true;
        }
        public int SaveChanges()
        {
            return this._dataContext.SaveChanges();
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity
        {
            if (ServiceLocator.IsLocationProviderSet)
                return ServiceLocator.Current.GetInstance<IRepository<TEntity>>();
            return (IRepository<TEntity>)this.RepositoryAsync<TEntity>();
        }

        public IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class, IEntity
        {
            if (ServiceLocator.IsLocationProviderSet)
                return ServiceLocator.Current.GetInstance<IRepositoryAsync<TEntity>>();
            if (this._repositories == null)
                this._repositories = new Dictionary<string, object>();
            string key = string.Format("Repository_{0}", (object)typeof(TEntity).Name);
            if (this._repositories.ContainsKey(key))
            {
                if (o__15<TEntity>.p__0 == null)
                {
                    o__15<TEntity>.p__0 = CallSite<Func<CallSite, object, IRepositoryAsync<TEntity>>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IRepositoryAsync<TEntity>), typeof(UnitOfWork)));
                }

                return o__15<TEntity>.p__0.Target((CallSite)o__15<TEntity>.p__0, this._repositories[key]);
            }
            Type type = typeof(Repository<>);
            this._repositories.Add(key, Activator.CreateInstance(type.MakeGenericType(typeof(TEntity)), (object)this));

            if (o__15<TEntity>.p__1 == null)
            {
                o__15<TEntity>.p__1 = CallSite<Func<CallSite, object, IRepositoryAsync<TEntity>>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(IRepositoryAsync<TEntity>), typeof(UnitOfWork)));
            }

            return o__15<TEntity>.p__1.Target((CallSite)o__15<TEntity>.p__1, this._repositories[key]);
        }
        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (this._dataContext is DbContext)
            {
                DbContext context = (DbContext)this._dataContext;
                if (this._transaction != null)
                {
                    context.Database.UseTransaction(this._transaction);
                }
                else
                {
                    DbContextTransaction transaction = context.Database.BeginTransaction(isolationLevel);
                    this._transaction = transaction.UnderlyingTransaction;
                }
            }
            else
            {
                this._objectContext = ((IObjectContextAdapter)this._dataContext).ObjectContext;
                if (this._objectContext.Connection.State != ConnectionState.Open)
                {
                    this._objectContext.Connection.Open();
                }
                this._transaction = this._objectContext.Connection.BeginTransaction(isolationLevel);
            }
        }

        public bool Commit()
        {
            this._transaction.Commit();
            return true;
        }







        public void Rollback()
        {
            this._transaction.Rollback();
            this._dataContext.SyncObjectsStatePostCommit();
        }


        public Task<int> SaveChangesAsync()
        {
            return this._dataContext.SaveChangesAsync();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return this._dataContext.SaveChangesAsync(cancellationToken);
        }

        public IViewRepositoryAsync<TEntity> ViewRepositoryAsync<TEntity>() where TEntity : class, IViewEntity
        {
            if (ServiceLocator.IsLocationProviderSet)
                return ServiceLocator.Current.GetInstance<IViewRepositoryAsync<TEntity>>();
            if (this._repositories == null)
                this._repositories = new Dictionary<string, object>();
            string key = string.Format("ViewRepository_{0}", (object)typeof(TEntity).Name);
            if (this._repositories.ContainsKey(key))
            {
                // ISSUE: reference to a compiler-generated field
                if (o__16<TEntity>.p__0 == null)
                {
                    // ISSUE: reference to a compiler-generated field
                    o__16<TEntity>.p__0 = CallSite<Func<CallSite, object, IViewRepositoryAsync<TEntity>>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IViewRepositoryAsync<TEntity>), typeof(UnitOfWork)));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                return o__16<TEntity>.p__0.Target((CallSite)o__16<TEntity>.p__0, this._repositories[key]);
            }
            Type type = typeof(ViewRepository<>);
            this._repositories.Add(key, Activator.CreateInstance(type.MakeGenericType(typeof(TEntity)), (object)this));
            // ISSUE: reference to a compiler-generated field
            if (o__16<TEntity>.p__1 == null)
            {
                // ISSUE: reference to a compiler-generated field
                o__16<TEntity>.p__1 = CallSite<Func<CallSite, object, IViewRepositoryAsync<TEntity>>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(IViewRepositoryAsync<TEntity>), typeof(UnitOfWork)));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            return o__16<TEntity>.p__1.Target((CallSite)o__16<TEntity>.p__1, this._repositories[key]);
        }

        private static class o__15<TEntity> where TEntity : class, IEntity
        {
            public static CallSite<Func<CallSite, object, IRepositoryAsync<TEntity>>> p__0;
            public static CallSite<Func<CallSite, object, IRepositoryAsync<TEntity>>> p__1;
        }

        private static class o__16<TEntity> where TEntity : class, IViewEntity
        {

            public static CallSite<Func<CallSite, object, IViewRepositoryAsync<TEntity>>> p__0;
            public static CallSite<Func<CallSite, object, IViewRepositoryAsync<TEntity>>> p__1;
        }
    }


}

