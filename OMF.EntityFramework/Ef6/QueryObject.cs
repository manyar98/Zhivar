using OMF.EntityFramework.Repositories;
using LinqKit;
using System;
using System.Linq.Expressions;

namespace OMF.EntityFramework.Ef6
{
    public class QueryObject<TEntity> : IQueryObject<TEntity>
    {
        private Expression<Func<TEntity, bool>> _query;

        public virtual Expression<Func<TEntity, bool>> Query()
        {
            return this._query;
        }

        public Expression<Func<TEntity, bool>> And(Expression<Func<TEntity, bool>> query)
        {
            if (query == null)
                return this._query;
            return this._query = this._query == null ? query : this._query.And<TEntity>(query.Expand<Func<TEntity, bool>>());
        }

        public Expression<Func<TEntity, bool>> Or(Expression<Func<TEntity, bool>> query)
        {
            if (query == null)
                return this._query;
            return this._query = this._query == null ? query : this._query.Or<TEntity>(query.Expand<Func<TEntity, bool>>());
        }

        public Expression<Func<TEntity, bool>> And(IQueryObject<TEntity> queryObject)
        {
            return this.And(queryObject.Query());
        }

        public Expression<Func<TEntity, bool>> Or(IQueryObject<TEntity> queryObject)
        {
            return this.Or(queryObject.Query());
        }
    }
}
