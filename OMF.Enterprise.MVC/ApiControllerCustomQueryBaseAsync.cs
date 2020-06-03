using OMF.Business;
using OMF.Common;
using OMF.Common.ExceptionManagement;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.Query;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;

namespace OMF.Enterprise.MVC
{
    public class ApiControllerCustomQueryBaseAsync<TEntity, TCustomQueryEntity, TEntityVM> : ApiController
      where TEntity : class, IEntity, new()
      where TCustomQueryEntity : class, new()
      where TEntityVM : class, new()
    {
        private bool needSetHasChildren = false;
        private IBusinessRuleBaseAsync<TEntity> businessRule;
        private WebApiBusinessManager<TEntity> businessManager;

        public IBusinessRuleBaseAsync<TEntity> BusinessRule
        {
            get
            {
                return this.businessRule;
            }
        }

        public ApiControllerCustomQueryBaseAsync()
        {
            try
            {
                this.businessRule = this.CreateBusinessRule();
                this.businessManager = new WebApiBusinessManager<TEntity>(this.businessRule);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ex);
            }
        }

        protected virtual IBusinessRuleBaseAsync<TEntity> CreateBusinessRule()
        {
            return BusinessContext.GetBusinessRule<TEntity>();
        }

        public async Task<HttpResponseMessage> GetOperationAccess()
        {
            try
            {
                OperationAccess oprAccess = await this.BusinessRule.CreateOperationAccessAsync();
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 0,
                    data = oprAccess
                });
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = await this.HandleExceptionAsync(ex);
                return httpResponseMessage;
            }
            HttpResponseMessage httpResponseMessage1;
            return httpResponseMessage1;
        }

        public async Task<HttpResponseMessage> GetByID(int id)
        {
            try
            {
                if (id == 0)
                    return this.Request.CreateResponse(HttpStatusCode.BadRequest);
                TEntity entity1 = default(TEntity);
                TEntity entity2 = await this.businessManager.GetByIDAsync((object)id);
                entity1 = entity2;
                entity2 = default(TEntity);
                if ((object)entity1 == null)
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 0,
                        data = entity1
                    });
                TEntityVM entityVm = default(TEntityVM);
                Expression<Func<TEntity, bool>> defaultExpr = this.CreateDefaultSearchExpression();
                if (defaultExpr != null )//&& !defaultExpr.Invoke<TEntity, bool>(entity1))
                {
                    entityVm = default(TEntityVM);
                }
                else
                {
                    bool flag = await this.HasAccessTo((object)id);
                    if (!flag)
                    {
                        entityVm = default(TEntityVM);
                    }
                    else
                    {
                        TCustomQueryEntity queryEntity = this.TranslateEntityToCustomQueryEntity(entity1);
                        entityVm = this.TranslateCustomQueryEntityToEntityVM(queryEntity);
                        entityVm = this.PrepareResponseAfterGetByID(entityVm);
                        queryEntity = default(TCustomQueryEntity);
                    }
                }
                return (object)entityVm != null ? this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 0,
                    data = entityVm
                }) : this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 4,
                    message = "اطلاعات یافت نشد"
                });
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = await this.HandleExceptionAsync(ex);
                return httpResponseMessage;
            }
            HttpResponseMessage httpResponseMessage1;
            return httpResponseMessage1;
        }

        protected virtual TEntityVM PrepareResponseAfterGetByID(TEntityVM entityVm)
        {
            return entityVm;
        }

        public async Task<HttpResponseMessage> GetAll(
          [FromUri] OperationAccess operationAccess)
        {
            try
            {
                try
                {
                    Expression<Func<TEntity, bool>> expression = this.CreateDefaultSearchExpression();
                    if (operationAccess != null)
                        this.BusinessRule.OperationAccess = operationAccess;
                    IQueryable<TEntity> query = this.businessManager.GetByFilter(expression, (List<Expression<Func<TEntity, object>>>)null);
                    List<TEntity> list = await query.ToListAsync2<TEntity>();
                    List<TCustomQueryEntity> customQueryList = list.ConvertAll<TCustomQueryEntity>((Converter<TEntity, TCustomQueryEntity>)(entity => this.TranslateEntityToCustomQueryEntity(entity)));
                    customQueryList = await this.PostExecuteQueryAsync(customQueryList, (QueryInfo)null);
                    List<TEntityVM> listVM = customQueryList.ConvertAll<TEntityVM>((Converter<TCustomQueryEntity, TEntityVM>)(entity => this.TranslateCustomQueryEntityToEntityVM(entity)));
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 0,
                        data = listVM
                    });
                }
                catch (Exception ex)
                {
                    HttpResponseMessage httpResponseMessage = await this.HandleExceptionAsync(ex);
                    return httpResponseMessage;
                }
                HttpResponseMessage httpResponseMessage1;
                return httpResponseMessage1;
            }
            finally
            {
                if (this.businessRule != null)
                    this.businessRule.Dispose();
            }
        }

        public async Task<HttpResponseMessage> Get([FromUri] QueryInfo searchRequestInfo)
        {
            try
            {
                try
                {
                    if (searchRequestInfo == null)
                        return this.Request.CreateResponse(HttpStatusCode.BadRequest);
                    if (searchRequestInfo.OperationAccess != null)
                        this.BusinessRule.OperationAccess = searchRequestInfo.OperationAccess;
                    IQueryable<TEntity> query;
                    IQueryable<TCustomQueryEntity> joinQuery;
                    int numberOfRecords;
                    if (searchRequestInfo.Filter == null || searchRequestInfo.Filter.Filters == null || searchRequestInfo.Filter.Filters.Count == 0)
                    {
                        query = this.CreateQuery((FilterInfo)null);
                        joinQuery = this.CreateCustomQuery(query, searchRequestInfo);
                        numberOfRecords = await joinQuery.CountAsync2<TCustomQueryEntity>();
                    }
                    else
                    {
                        if ((object)new TEntity() is ISelfReferenceEntity)
                        {
                            FilterData needSetHasChildrenFilterData = searchRequestInfo.Filter.Filters.FirstOrDefault<FilterData>((Func<FilterData, bool>)(fd => fd.Field.ToLower() == "needSetHasChildren".ToLower()));
                            if (needSetHasChildrenFilterData != null)
                            {
                                this.needSetHasChildren = needSetHasChildrenFilterData.Value.ConvertTo<bool>();
                                searchRequestInfo.Filter.Filters.Remove(needSetHasChildrenFilterData);
                            }
                            needSetHasChildrenFilterData = (FilterData)null;
                        }
                        query = this.CreateQuery(searchRequestInfo.Filter);
                        joinQuery = this.CreateCustomQuery(query, searchRequestInfo);
                        numberOfRecords = await joinQuery.CountAsync2<TCustomQueryEntity>();
                    }
                    joinQuery = (IQueryable<TCustomQueryEntity>)this.CreateOrderedQuery(joinQuery, searchRequestInfo.Sort);
                    if (searchRequestInfo.PageSize > 0)
                        joinQuery = joinQuery.Skip<TCustomQueryEntity>(searchRequestInfo.Skip).Take<TCustomQueryEntity>(searchRequestInfo.PageSize);
                    List<TCustomQueryEntity> customQueryList = await joinQuery.ToListAsync2<TCustomQueryEntity>();
                    customQueryList = await this.PostExecuteQueryAsync(customQueryList, searchRequestInfo);
                    List<TEntityVM> listVM = customQueryList.ConvertAll<TEntityVM>((Converter<TCustomQueryEntity, TEntityVM>)(entity => this.TranslateCustomQueryEntityToEntityVM(entity)));
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 0,
                        data = new
                        {
                            records = listVM,
                            count = numberOfRecords
                        }
                    });
                }
                catch (Exception ex)
                {
                    HttpResponseMessage httpResponseMessage = await this.HandleExceptionAsync(ex);
                    return httpResponseMessage;
                }
                HttpResponseMessage httpResponseMessage1;
                return httpResponseMessage1;
            }
            finally
            {
                if (this.businessRule != null)
                    this.businessRule.Dispose();
            }
        }

        protected virtual IQueryable<TEntity> CreateQuery(FilterInfo filter)
        {
            return filter != null && filter.Filters != null && filter.Filters.Count != 0 ? this.businessManager.GetByFilter(this.CreateSearchExpression(filter), (List<Expression<Func<TEntity, object>>>)null) : this.businessManager.GetByFilter(this.CreateDefaultSearchExpression(), (List<Expression<Func<TEntity, object>>>)null);
        }

        protected virtual async Task<List<TCustomQueryEntity>> PostExecuteQueryAsync(
          List<TCustomQueryEntity> list,
          QueryInfo searchRequestInfo)
        {
            bool isISelfReferenceEntity = (object)new TEntity() is ISelfReferenceEntity;
            if (!isISelfReferenceEntity || searchRequestInfo == null || (searchRequestInfo.Filter == null || searchRequestInfo.Filter.Filters == null) || !searchRequestInfo.Filter.Filters.Any<FilterData>() || searchRequestInfo.Filter.Filters.Count<FilterData>() == 1 && searchRequestInfo.Filter.Filters.Any<FilterData>((Func<FilterData, bool>)(fd => fd.Field.ToLower() == "parentid")))
                return list;
            TCustomQueryEntity[] customQueryEntityArray = list.ToArray();
            for (int index = 0; index < customQueryEntityArray.Length; ++index)
            {
                TCustomQueryEntity item = customQueryEntityArray[index];
                await this.AddParentToListAsync(item, list);
                item = default(TCustomQueryEntity);
            }
            customQueryEntityArray = (TCustomQueryEntity[])null;
            return list;
        }

        protected async Task AddParentToListAsync(
          TCustomQueryEntity item,
          List<TCustomQueryEntity> list)
        {
            PropertyInfo parentIdProp = item.GetType().GetProperty("ParentId");
            if (parentIdProp == (PropertyInfo)null)
                return;
            object parentIdValue = parentIdProp.GetValue((object)item);
            if (parentIdValue == null)
                return;
            FilterInfo filterInfo = new FilterInfo()
            {
                Filters = new List<FilterData>()
        {
          new FilterData()
          {
            Field = "ID",
            Operator = "eq",
            Value = parentIdValue.ToString()
          }
        }
            };
            Expression<Func<TCustomQueryEntity, bool>> expression = filterInfo.TranslateFilter<TCustomQueryEntity>(true);
            if (list.Any<TCustomQueryEntity>(expression.Compile()))
                return;
            int parentIDIntVal = parentIdValue.ConvertTo<int>();
            TEntity entity = await this.BusinessRule.Queryable(false, true, (List<Expression<Func<TEntity, object>>>)null).Where<TEntity>((Expression<Func<TEntity, bool>>)(en => en.ID == parentIDIntVal)).SingleOrDefaultAsync2<TEntity>();
            TEntity parentEntity = entity;
            entity = default(TEntity);
            if ((object)parentEntity == null)
            {
                parentIdProp.SetValue((object)item, (object)null);
            }
            else
            {
                TCustomQueryEntity customQueryEntity = this.TranslateEntityToCustomQueryEntity(parentEntity);
                list.Add(customQueryEntity);
                await this.AddParentToListAsync(customQueryEntity, list);
            }
        }

        protected Expression<Func<TEntity, bool>> CreateSearchExpression(
          FilterInfo filterInfo)
        {
            QueryObject<TEntity> queryObject = new QueryObject<TEntity>();
            Expression<Func<TEntity, bool>> searchExpression = this.CreateDefaultSearchExpression();
            if (searchExpression != null)
                queryObject.And(searchExpression);
            Expression<Func<TEntity, bool>> expressionByFilterInfo = this.CreateSearchExpressionByFilterInfo(filterInfo);
            if (expressionByFilterInfo != null)
                queryObject.And(expressionByFilterInfo);
            return queryObject.Query();
        }

        protected virtual Expression<Func<TEntity, bool>> CreateDefaultSearchExpression()
        {
            return (Expression<Func<TEntity, bool>>)null;
        }

        protected virtual Expression<Func<TEntity, bool>> CreateSearchExpressionByFilterInfo(
          FilterInfo filterInfo)
        {
            return filterInfo.TranslateFilter<TEntity>(true);
        }

        protected virtual IOrderedQueryable<TCustomQueryEntity> CreateOrderedQuery(
          IQueryable<TCustomQueryEntity> query,
          List<SortInfo> sortInfoList)
        {
            if (sortInfoList == null || sortInfoList.Count == 0)
                return query.OrderByDescending<TCustomQueryEntity>("ID");
            return sortInfoList.CreateOrderedQuery<TCustomQueryEntity>(query);
        }

        protected virtual IQueryable<TCustomQueryEntity> CreateCustomQuery(
          IQueryable<TEntity> query,
          QueryInfo searchRequestInfo)
        {
            if (typeof(TEntity) == typeof(TCustomQueryEntity))
                return Queryable.OfType<TCustomQueryEntity>(query);
            throw new ShouldImplemented(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
        }

        public async Task<HttpResponseMessage> Post(TEntityVM entityVM)
        {
            if ((object)entityVM == null)
                return this.Request.CreateResponse(HttpStatusCode.BadRequest);
            TEntity entity = default(TEntity);
            try
            {
                entity = this.TranslateEntityVMToEntity(entityVM);
                await this.businessManager.InsertAsync(entity);
                object responseData = this.PrepareResponseAfterInsert(entity);
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 0,
                    data = responseData
                });
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = await this.HandleExceptionAsync(ex);
                return httpResponseMessage;
            }
            entity = default(TEntity);
            HttpResponseMessage httpResponseMessage1;
            return httpResponseMessage1;
        }

        protected virtual object PrepareResponseAfterInsert(TEntity entity)
        {
            return (object)entity.ID;
        }

        public async Task<HttpResponseMessage> Put(TEntityVM entityVM)
        {
            if ((object)entityVM == null)
                return this.Request.CreateResponse(HttpStatusCode.BadRequest);
            TEntity entity = default(TEntity);
            try
            {
                entity = this.TranslateEntityVMToEntity(entityVM);
                Expression<Func<TEntity, bool>> defaultExpr = this.CreateDefaultSearchExpression();
                if (defaultExpr != null)// && !defaultExpr.Invoke<TEntity, bool>(entity))
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 4,
                        message = "اطلاعات یافت نشد"
                    });
                bool flag = await this.HasAccessTo(entity.GetID());
                if (!flag)
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 4,
                        message = "اطلاعات یافت نشد"
                    });
                await this.businessManager.UpdateAsync(entity);
                object responseData = this.PrepareResponseAfterUpdate(entity);
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 0,
                    data = responseData
                });
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = await this.HandleExceptionAsync(ex);
                return httpResponseMessage;
            }
            entity = default(TEntity);
            HttpResponseMessage httpResponseMessage1;
            return httpResponseMessage1;
        }

        protected virtual object PrepareResponseAfterUpdate(TEntity entity)
        {
            return (object)entity.ID;
        }

        public async Task<HttpResponseMessage> Delete(TEntityVM entityVM)
        {
            if ((object)entityVM == null)
                return this.Request.CreateResponse(HttpStatusCode.BadRequest);
            TEntity entity = default(TEntity);
            try
            {
                entity = this.TranslateEntityVMToEntity(entityVM);
                Expression<Func<TEntity, bool>> defaultExpr = this.CreateDefaultSearchExpression();
                if (defaultExpr != null )//&& !defaultExpr.Invoke<TEntity, bool>(entity))
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 4,
                        message = "اطلاعات یافت نشد"
                    });
                bool flag = await this.HasAccessTo(entity.GetID());
                if (!flag)
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 4,
                        message = "اطلاعات یافت نشد"
                    });
                this.businessManager.Delete(entity);
                object responseData = this.PrepareResponseAfterDelete(entity);
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 0,
                    data = responseData
                });
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = await this.HandleExceptionAsync(ex);
                return httpResponseMessage;
            }
            entity = default(TEntity);
            HttpResponseMessage httpResponseMessage1;
            return httpResponseMessage1;
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                if (id == 0)
                    return this.Request.CreateResponse(HttpStatusCode.BadRequest);
                TEntity entity1 = await this.BusinessRule.FindAsync((object)id);
                TEntity entity2 = entity1;
                entity1 = default(TEntity);
                Expression<Func<TEntity, bool>> defaultExpr = this.CreateDefaultSearchExpression();
                if ((object)entity2 == null || defaultExpr != null )//&& !defaultExpr.Invoke<TEntity, bool>(entity2))
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 4,
                        message = "اطلاعات یافت نشد"
                    });
                bool flag = await this.HasAccessTo(entity2.GetID());
                if (!flag)
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 4,
                        message = "اطلاعات یافت نشد"
                    });
                this.businessManager.Delete(entity2);
                object responseData = this.PrepareResponseAfterDelete(entity2);
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 0,
                    data = responseData
                });
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = await this.HandleExceptionAsync(ex);
                return httpResponseMessage;
            }
            HttpResponseMessage httpResponseMessage1;
            return httpResponseMessage1;
        }

        protected virtual object PrepareResponseAfterDelete(TEntity entity)
        {
            return (object)entity.ID;
        }

        protected virtual async Task<bool> HasAccessTo(params object[] keyValues)
        {
            if (keyValues == null || keyValues.Length == 0)
                return false;
            QueryObject<TEntity> queryObject = new QueryObject<TEntity>();
            Expression<Func<TEntity, bool>> defaultSearchExpr = this.CreateDefaultSearchExpression();
            if (defaultSearchExpr != null)
                queryObject.And(defaultSearchExpr);
            Expression<Func<TEntity, bool>> idExpr = new FilterInfo()
            {
                Filters = {
          new FilterData()
          {
            Field = "ID",
            Operator = "eq",
            Value = keyValues[0].ToString()
          }
        }
            }.TranslateFilter<TEntity>(false);
            if (idExpr != null)
                queryObject.And(idExpr);
            IQueryable<TEntity> query = this.BusinessRule.Queryable(false, true, (List<Expression<Func<TEntity, object>>>)null);
            if (queryObject.Query() != null)
                query = query.Where<TEntity>(queryObject.Query());
            IQueryable<TCustomQueryEntity> customSearchQuery = this.CreateCustomQuery(query, new QueryInfo());
            bool hasAccess = false;
            if ((object)new TEntity() is ICacheable)
                hasAccess = await Task.Run<bool>((Func<bool>)(() => customSearchQuery.Any<TCustomQueryEntity>()));
            else
                hasAccess = await customSearchQuery.AnyAsync<TCustomQueryEntity>();
            if (!hasAccess)
                await ExceptionManager.SaveExceptionAsync((Exception)new OperationAccessException(string.Format("id: {0}, controller: {1}, userName: {2}", keyValues[0], (object)this.GetType().Name, (object)SecurityManager.CurrentUserContext.UserName)));
            return hasAccess;
        }

        protected virtual TCustomQueryEntity TranslateEntityToCustomQueryEntity(TEntity entity)
        {
            if ((object)entity is TCustomQueryEntity)
                return (object)entity as TCustomQueryEntity;
            return entity.Translate<TCustomQueryEntity>();
        }

        protected virtual TEntityVM TranslateCustomQueryEntityToEntityVM(
          TCustomQueryEntity customQueryEntity)
        {
            TEntityVM entityVm1 = default(TEntityVM);
            TEntityVM entityVm2 = !((object)customQueryEntity is TEntityVM) ? customQueryEntity.Translate<TEntityVM>() : (object)customQueryEntity as TEntityVM;
            if ((object)new TEntity() is ISelfReferenceEntity && this.needSetHasChildren)
            {
                PropertyInfo propertyInfo1 = ((IEnumerable<PropertyInfo>)typeof(TEntityVM).GetProperties()).FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>)(pInfo => pInfo.Name.ToLower() == "haschildren"));
                if (propertyInfo1 != (PropertyInfo)null)
                {
                    PropertyInfo propertyInfo2 = ((IEnumerable<PropertyInfo>)typeof(TEntityVM).GetProperties()).FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>)(pInfo => pInfo.Name.ToLower() == "id"));
                    if (propertyInfo2 != (PropertyInfo)null)
                    {
                        object obj = propertyInfo2.GetValue((object)entityVm2);
                        Expression<Func<TEntity, bool>> predicate = new FilterInfo()
                        {
                            Filters = new List<FilterData>()
              {
                new FilterData()
                {
                  Field = "ParentId",
                  Operator = "eq",
                  Value = obj.ToString()
                }
              },
                            Logic = "and"
                        }.TranslateFilter<TEntity>(true);
                        propertyInfo1.SetValue((object)entityVm2, (object)this.BusinessRule.Queryable(false, true, (List<Expression<Func<TEntity, object>>>)null).Where<TEntity>(predicate).Any<TEntity>());
                    }
                }
            }
            return entityVm2;
        }

        protected virtual TEntity TranslateEntityVMToEntity(TEntityVM entityVM)
        {
            if ((object)entityVM is TEntity)
                return (object)entityVM as TEntity;
            return entityVM.Translate<TEntity>();
        }

        protected override void Dispose(bool disposing)
        {
            this.BusinessRule.Dispose();
            base.Dispose(disposing);
        }
    }
}
