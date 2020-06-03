using OMF.Common;
using System.Collections.Generic;

namespace OMF.EntityFramework.Repositories
{
    public interface IRepository<TEntity> : IViewRepository<TEntity> where TEntity : class, IEntity
    {
        void Insert(TEntity entity);

        void InsertRange(IEnumerable<TEntity> entities);

        void InsertOrUpdateGraph(TEntity entity);

        void InsertGraphRange(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void Delete(object id);

        void Delete(TEntity entity);
    }
}
