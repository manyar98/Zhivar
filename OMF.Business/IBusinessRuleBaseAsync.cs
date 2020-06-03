using OMF.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OMF.Business
{
    public interface IBusinessRuleBaseAsync<TEntity> : IBusinessRuleBase<TEntity>, IViewBusinessRuleBase<TEntity>, IDisposable, IViewBusinessRuleBaseAsync<TEntity>
      where TEntity : class, IEntity
    {
        Task DeleteAsync(params object[] keyValues);

        Task<int> SaveChangesAsync();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
