using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OMF.EntityFramework.Repositories
{
    public interface IViewRepositoryAsync<TEntity> : IViewRepository<TEntity> where TEntity : class, IViewEntity
    {
        Task<TEntity> FindAsync(params object[] keyValues);

        Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);

        Task LoadReferenceAsync<TProperty>(
          TEntity entity,
          Expression<Func<TEntity, TProperty>> navigationProperty,
          bool forceToDetach = false)
          where TProperty : class, IEntity;

        Task LoadCollectionAsync<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          bool forceToDetach = false)
          where TElement : class, IEntity;

        Task LoadCollectionAsync<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          Expression<Func<TElement, bool>> predicate,
          bool forceToDetach = false)
          where TElement : class, IEntity;
    }
}
