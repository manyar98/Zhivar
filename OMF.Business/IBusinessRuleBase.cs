using OMF.Common;
using System;
using System.Collections.Generic;

namespace OMF.Business
{
    public interface IBusinessRuleBase<TEntity> : IViewBusinessRuleBase<TEntity>, IDisposable
      where TEntity : class, IEntity
    {
        bool UseForAnonymousUser { get; set; }

        OMF.Common.Validation.EntityValidator<TEntity> EntityValidator { get; }

        List<string> Validate(TEntity entity);

        void Insert(TEntity entity);

        void Insert(IEnumerable<TEntity> entities);

        void InsertOrUpdateGraph(TEntity entity);

        void Update(TEntity entity);

        void Update(IEnumerable<TEntity> entities);

        void Delete(params object[] keyValues);

        void Delete(TEntity entity);

        void Delete(IEnumerable<TEntity> entities);

        int SaveChanges();
    }
}
