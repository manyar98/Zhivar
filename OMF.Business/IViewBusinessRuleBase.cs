using OMF.Common;
using OMF.Common.Security;
using OMF.EntityFramework.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OMF.Business
{
    public interface IViewBusinessRuleBase<TEntity> : IDisposable where TEntity : class, IViewEntity
    {
        OperationAccess CreateOperationAccess();

        IUnitOfWorkAsync UnitOfWork { get; }

        OperationAccess OperationAccess { get; set; }

        TEntity Find(params object[] keyValues);

        IQueryable<TEntity> Queryable(
          bool containsDeletedData = false,
          bool fromCache = true,
          List<Expression<Func<TEntity, object>>> includes = null);

        void LoadReference<TProperty>(
          TEntity entity,
          Expression<Func<TEntity, TProperty>> navigationProperty,
          bool forceToDetach = false)
          where TProperty : class, IEntity;

        TProperty LoadReference<TProperty>(
          Expression<Func<TEntity, TProperty>> navigationProperty,
          params object[] keyValues)
          where TProperty : class, IEntity;

        void LoadCollection<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          bool forceToDetach = false)
          where TElement : class, IEntity;

        void LoadCollection<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          Expression<Func<TElement, bool>> predicate,
          bool forceToDetach = false)
          where TElement : class, IEntity;

        ICollection<TElement> LoadCollection<TElement>(
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          params object[] keyValues)
          where TElement : class, IEntity;

        ICollection<TElement> LoadCollection<TElement>(
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          Expression<Func<TElement, bool>> predicate,
          params object[] keyValues)
          where TElement : class, IEntity;
    }
}
