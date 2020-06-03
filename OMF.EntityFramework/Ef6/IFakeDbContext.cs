using OMF.EntityFramework.DataContext;
using System;
using System.Data.Entity;

namespace OMF.EntityFramework.Ef6
{
    public interface IFakeDbContext : IDataContextAsync, IDataContext, IDisposable
    {
        DbSet<T> Set<T>() where T : class;

        void AddFakeDbSet<TEntity, TFakeDbSet>()
          where TEntity : OMF.Common.Entity, new()
          where TFakeDbSet : FakeDbSet<TEntity>, IDbSet<TEntity>, new();
    }
}
