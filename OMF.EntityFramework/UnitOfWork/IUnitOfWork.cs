using OMF.Common;
using OMF.EntityFramework.DataContext;
using OMF.EntityFramework.Repositories;
using System;
using System.Data;

namespace OMF.EntityFramework.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IDataContextAsync DataContext { get; }

        int SaveChanges();

        void Dispose(bool disposing);

        IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity;

        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        bool Commit();

        void Rollback();
    }
}
