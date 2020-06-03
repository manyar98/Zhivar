using OMF.Common;
using OMF.EntityFramework.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OMF.EntityFramework.UnitOfWork
{
    public interface IUnitOfWorkAsync : IUnitOfWork, IDisposable
    {
        Task<int> SaveChangesAsync();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class, IEntity;

        IViewRepositoryAsync<TEntity> ViewRepositoryAsync<TEntity>() where TEntity : class, IViewEntity;
    }
}
