using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OMF.EntityFramework.Repositories
{
    public interface IViewRepository<TEntity> where TEntity : class, IViewEntity
    {
        TEntity Find(params object[] keyValues);

        IQueryable<TEntity> SelectQuery(string query, params object[] parameters);

        IQueryable<TEntity> Queryable(
          bool containsDeletedData = false,
          bool fromCache = true,
          List<Expression<Func<TEntity, object>>> includes = null);

        void LoadReference<TProperty>(
          TEntity entity,
          Expression<Func<TEntity, TProperty>> navigationProperty,
          bool forceToDetach = false)
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
    }
}
