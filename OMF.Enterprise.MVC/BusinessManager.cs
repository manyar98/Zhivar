using OMF.Business;
using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OMF.Enterprise.MVC
{
    public class BusinessManager<TEntity> where TEntity : class, IEntity
    {
        private IBusinessRuleBaseAsync<TEntity> businessRule;

        public IBusinessRuleBaseAsync<TEntity> BusinessRule
        {
            get
            {
                return this.businessRule;
            }
        }

        public BusinessManager(IBusinessRuleBaseAsync<TEntity> businessRule)
        {
            this.businessRule = businessRule;
        }

        public IQueryable<TEntity> GetByFilter(
          Expression<Func<TEntity, bool>> filter,
          List<Expression<Func<TEntity, object>>> includes = null)
        {
            if (filter != null)
                return this.businessRule.Queryable(false, true, includes).Where<TEntity>(filter);
            return this.businessRule.Queryable(false, true, includes);
        }

        public TEntity GetByID(params object[] keyValues)
        {
            return this.businessRule.Find(keyValues);
        }

        public async Task<TEntity> GetByIDAsync(params object[] keyValues)
        {
            TEntity async = await this.businessRule.FindAsync(keyValues);
            return async;
        }

        public void Insert(TEntity entity)
        {
            this.businessRule.Insert(entity);
            this.businessRule.SaveChanges();
        }

        public async Task InsertAsync(TEntity entity)
        {
            try
            {
                this.businessRule.Insert(entity);
                int num = await this.businessRule.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public void Update(TEntity entity)
        {
            this.businessRule.Update(entity);
            this.businessRule.SaveChanges();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            this.businessRule.Update(entity);
            int num = await this.businessRule.SaveChangesAsync();
        }

        public void Delete(params object[] keyValues)
        {
            this.businessRule.Delete(keyValues);
            this.businessRule.SaveChanges();
        }

        public void Delete(TEntity entity)
        {
            this.businessRule.Delete(entity);
            this.businessRule.SaveChanges();
        }

        public async Task DeleteAsync(params object[] keyValues)
        {
            this.businessRule.Delete(keyValues);
            int num = await this.businessRule.SaveChangesAsync();
        }
    }
}
