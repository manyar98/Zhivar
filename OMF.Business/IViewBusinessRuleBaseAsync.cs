using OMF.Common;
using OMF.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OMF.Business
{
    public interface IViewBusinessRuleBaseAsync<TEntity> : IViewBusinessRuleBase<TEntity>, IDisposable
      where TEntity : class, IViewEntity
    {
        Task<OperationAccess> CreateOperationAccessAsync();

        Task<TEntity> FindAsync(params object[] keyValues);

        Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);

        Task LoadReferenceAsync<TProperty>(
          TEntity entity,
          Expression<Func<TEntity, TProperty>> navigationProperty,
          bool forceToDetach = false)
          where TProperty : class, IEntity;

        Task<TProperty> LoadReferenceAsync<TProperty>(
          Expression<Func<TEntity, TProperty>> navigationProperty,
          params object[] keyValues)
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

        Task<ICollection<TElement>> LoadCollectionAsync<TElement>(
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          params object[] keyValues)
          where TElement : class, IEntity;

        Task<ICollection<TElement>> LoadCollectionAsync<TElement>(
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          Expression<Func<TElement, bool>> predicate,
          params object[] keyValues)
          where TElement : class, IEntity;
    }
}
