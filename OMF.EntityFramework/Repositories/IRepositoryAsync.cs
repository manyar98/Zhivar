using OMF.Common;
using System.Threading;
using System.Threading.Tasks;

namespace OMF.EntityFramework.Repositories
{
    public interface IRepositoryAsync<TEntity> : IViewRepositoryAsync<TEntity>, IViewRepository<TEntity>, IRepository<TEntity>
      where TEntity : class, IEntity
    {
        Task<bool> DeleteAsync(params object[] keyValues);

        Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues);
    }
}
