using OMF.Business;
using OMF.Common;
using OMF.Common.Cache;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Http;
using System.Data.Entity;

namespace OMF.Enterprise.MVC
{
    public class NewApiControllerBaseAsync<TEntity, TEntityVM> : ApiController
      where TEntity : class, IEntity, new()
      where TEntityVM : class, new()
    {
        private bool needSetHasChildren = false;
        private bool addChildrenToFilteredList = false;
        private IBusinessRuleBaseAsync<TEntity> businessRule;
        private WebApiBusinessManager<TEntity> businessManager;

        public IBusinessRuleBaseAsync<TEntity> BusinessRule
        {
            get
            {
                return this.businessRule;
            }
        }

        public NewApiControllerBaseAsync()
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
                TEntityVM entityVm = this.TranslateEntityToEntityVM(entity1);
                Expression<Func<TEntityVM, bool>> defaultExpr = this.CreateDefaultSearchExpression();
             //   if ((object)entityVm != null && defaultExpr != null )//&& !defaultExpr.Invoke<TEntityVM, bool>(entityVm))
             //   {
            //        entityVm = default(TEntityVM);
            //    }
            //    else
            //    {
                    bool async = await this.HasAccessToAsync((object)id);
                    entityVm = async ? this.PrepareResponseAfterGetByID(entityVm) : default(TEntityVM);
             //   }
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
            entityVm.AsUnicode1256<TEntityVM>();
            return entityVm;
        }

        public async Task<HttpResponseMessage> GetKeyValues(
          [FromUri] string parentEntityName)
        {
            try
            {
                try
                {
                    IQueryable<TEntity> query = this.CreateQuery(new FilterInfo());
                    IQueryable<TEntityVM> getAllQuery = this.CreateSearchQuery(query);
                    Expression<Func<TEntityVM, bool>> defaultExpression = this.CreateDefaultSearchExpression();
                    if (defaultExpression != null)
                        getAllQuery = getAllQuery.Where<TEntityVM>(defaultExpression);
                    getAllQuery = (IQueryable<TEntityVM>)this.CreateOrderedQuery(getAllQuery, (List<SortInfo>)null);
                    List<TEntityVM> getAllQueryListVm = await getAllQuery.ToListAsync2<TEntityVM>();
                    List<KeyValueVM> keyValues = getAllQueryListVm.ConvertAll<KeyValueVM>((Converter<TEntityVM, KeyValueVM>)(vm =>
                    {
                        vm.AsUnicode1256<TEntityVM>();
                        return this.ConvertEntityVMToKeyValue(vm);
                    }));
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 0,
                        data = keyValues
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

        protected virtual KeyValueVM ConvertEntityVMToKeyValue(TEntityVM entityVM)
        {
            return (KeyValueVM)null;
        }

        public async Task<HttpResponseMessage> GetAll([FromUri] string parentEntityName)
        {
            try
            {
                try
                {
                    IQueryable<TEntity> query = this.CreateQuery(new FilterInfo());
                    IQueryable<TEntityVM> getAllQuery = this.CreateSearchQuery(query);
                    Expression<Func<TEntityVM, bool>> defaultExpression = this.CreateDefaultSearchExpression();
                    if (defaultExpression != null)
                        getAllQuery = getAllQuery.Where<TEntityVM>(defaultExpression);
                    getAllQuery = (IQueryable<TEntityVM>)this.CreateOrderedQuery(getAllQuery, (List<SortInfo>)null);
                    List<TEntityVM> getAllQueryListVm = await getAllQuery.ToListAsync2<TEntityVM>();
                    getAllQueryListVm = await this.PostExecuteQueryAsync(getAllQueryListVm, (QueryInfo)null);
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 0,
                        data = getAllQueryListVm
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
                    IQueryable<TEntity> query;
                    IQueryable<TEntityVM> searchQuery;
                    int numberOfRecords;
                    if (searchRequestInfo.Filter == null || searchRequestInfo.Filter.Filters == null || searchRequestInfo.Filter.Filters.Count == 0)
                    {
                        query = this.CreateQuery(searchRequestInfo.Filter);
                        searchQuery = this.CreateSearchQuery(query);
                        Expression<Func<TEntityVM, bool>> defaultExpression = this.CreateDefaultSearchExpression();
                        if (defaultExpression != null)
                            searchQuery = searchQuery.Where<TEntityVM>(defaultExpression);
                        numberOfRecords = await searchQuery.CountAsync2<TEntityVM>();
                        defaultExpression = (Expression<Func<TEntityVM, bool>>)null;
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
                            FilterData addChildrenToFilteredListFilterData = searchRequestInfo.Filter.Filters.FirstOrDefault<FilterData>((Func<FilterData, bool>)(fd => fd.Field.ToLower() == "addChildrenToFilteredList".ToLower()));
                            if (addChildrenToFilteredListFilterData != null)
                            {
                                this.addChildrenToFilteredList = addChildrenToFilteredListFilterData.Value.ConvertTo<bool>();
                                searchRequestInfo.Filter.Filters.Remove(addChildrenToFilteredListFilterData);
                            }
                            needSetHasChildrenFilterData = (FilterData)null;
                            addChildrenToFilteredListFilterData = (FilterData)null;
                        }
                        query = this.CreateQuery(searchRequestInfo.Filter);
                        searchQuery = this.CreateSearchQuery(query);
                        Expression<Func<TEntityVM, bool>> searchExpression = this.CreateSearchExpression(searchRequestInfo.Filter);
                        if (searchExpression != null)
                            searchQuery = searchQuery.Where<TEntityVM>(searchExpression);
                        numberOfRecords = await searchQuery.CountAsync2<TEntityVM>();
                        searchExpression = (Expression<Func<TEntityVM, bool>>)null;
                    }
                    searchQuery = (IQueryable<TEntityVM>)this.CreateOrderedQuery(searchQuery, searchRequestInfo.Sort);
                    if (searchRequestInfo.PageSize > 0)
                        searchQuery = searchQuery.Skip<TEntityVM>(searchRequestInfo.Skip).Take<TEntityVM>(searchRequestInfo.PageSize);
                    List<TEntityVM> searchQueryListVm = await searchQuery.ToListAsync2<TEntityVM>();
                    searchQueryListVm = await this.PostExecuteQueryAsync(searchQueryListVm, searchRequestInfo);
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 0,
                        data = new
                        {
                            records = searchQueryListVm,
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
            return this.businessManager.GetByFilter((Expression<Func<TEntity, bool>>)null, (List<Expression<Func<TEntity, object>>>)null);
        }

        protected virtual async Task<List<TEntityVM>> PostExecuteQueryAsync(
          List<TEntityVM> list,
          QueryInfo searchRequestInfo)
        {
            list.ForEach((Action<TEntityVM>)(entityVm => entityVm.AsUnicode1256<TEntityVM>()));
            bool isISelfReferenceEntity = (object)new TEntity() is ISelfReferenceEntity;
            if (!isISelfReferenceEntity || searchRequestInfo == null || (searchRequestInfo.Filter == null || searchRequestInfo.Filter.Filters == null) || !searchRequestInfo.Filter.Filters.Any<FilterData>())
                return list;
            if (this.needSetHasChildren)
            {
                PropertyInfo hasChildrenPropInfo = ((IEnumerable<PropertyInfo>)typeof(TEntityVM).GetProperties()).FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>)(pInfo => pInfo.Name.ToLower() == "haschildren"));
                PropertyInfo idProp = ((IEnumerable<PropertyInfo>)typeof(TEntityVM).GetProperties()).FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>)(pInfo => pInfo.Name.ToLower() == "id"));
                if (hasChildrenPropInfo != (PropertyInfo)null && idProp != (PropertyInfo)null)
                {
                    TEntityVM[] entityVmArray = list.ToArray();
                    for (int index = 0; index < entityVmArray.Length; ++index)
                    {
                        TEntityVM item = entityVmArray[index];
                        object id = idProp.GetValue((object)item);
                        FilterInfo filterInfo = new FilterInfo()
                        {
                            Filters = new List<FilterData>()
              {
                new FilterData()
                {
                  Field = "ParentId",
                  Operator = "eq",
                  Value = id.ToString()
                }
              },
                            Logic = "and"
                        };
                        Expression<Func<TEntity, bool>> expression = filterInfo.TranslateFilter<TEntity>(true);
                        hasChildrenPropInfo.SetValue((object)item, (object)this.BusinessRule.Queryable(false, true, (List<Expression<Func<TEntity, object>>>)null).Where<TEntity>(expression).Any<TEntity>());
                        id = (object)null;
                        filterInfo = (FilterInfo)null;
                        expression = (Expression<Func<TEntity, bool>>)null;
                        item = default(TEntityVM);
                    }
                    entityVmArray = (TEntityVM[])null;
                }
                hasChildrenPropInfo = (PropertyInfo)null;
                idProp = (PropertyInfo)null;
            }
            if (searchRequestInfo.Filter.Filters.Count<FilterData>() == 1 && searchRequestInfo.Filter.Filters.Any<FilterData>((Func<FilterData, bool>)(fd => fd.Field.ToLower() == "parentid")))
                return list;
            TEntityVM[] entityVmArray1 = list.ToArray();
            for (int index = 0; index < entityVmArray1.Length; ++index)
            {
                TEntityVM item = entityVmArray1[index];
                if (this.addChildrenToFilteredList)
                    await this.AddChildrenToListAsync(item, list);
                await this.AddParentToListAsync(item, list);
                item = default(TEntityVM);
            }
            entityVmArray1 = (TEntityVM[])null;
            return list;
        }

        private async Task AddChildrenToListAsync(TEntityVM item, List<TEntityVM> list)
        {
         
            PropertyInfo idProp = ((IEnumerable<PropertyInfo>)typeof(TEntityVM).GetProperties()).FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>)(pInfo => pInfo.Name.ToLower() == "id"));
            object id = idProp.GetValue((object)item);
            FilterInfo filterInfo = new FilterInfo()
            {
                Filters = new List<FilterData>()
        {
          new FilterData()
          {
            Field = "ParentId",
            Operator = "eq",
            Value = id.ToString()
          }
        },
                Logic = "and"
            };
            Expression<Func<TEntity, bool>> expression = filterInfo.TranslateFilter<TEntity>(true);
            List<TEntity> children = await this.BusinessRule.Queryable(false, true, (List<Expression<Func<TEntity, object>>>)null).Where<TEntity>(expression).ToListAsync<TEntity>();
            foreach (TEntity entity in children)
            {
                TEntity child = entity;
                if (!list.Any<TEntityVM>((Func<TEntityVM, bool>)(e => (int)idProp.GetValue((object)e) == child.ID)))
                {
                    TEntityVM childEntityVm = this.TranslateEntityToEntityVM(child);
                    childEntityVm.AsUnicode1256<TEntityVM>();
                    list.Add(childEntityVm);
                    await this.AddChildrenToListAsync(childEntityVm, list);
                    childEntityVm = default(TEntityVM);
                }
            }
        }

        protected async Task AddParentToListAsync(TEntityVM item, List<TEntityVM> list)
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
            Expression<Func<TEntityVM, bool>> expression = filterInfo.TranslateFilter<TEntityVM>(true);
            if (list.Any<TEntityVM>(expression.Compile()))
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
                TEntityVM parentEntityVm = this.TranslateEntityToEntityVM(parentEntity);
                parentEntityVm.AsUnicode1256<TEntityVM>();
                list.Add(parentEntityVm);
                await this.AddParentToListAsync(parentEntityVm, list);
            }
        }

        protected Expression<Func<TEntityVM, bool>> CreateSearchExpression(
          FilterInfo filterInfo)
        {
            QueryObject<TEntityVM> queryObject = new QueryObject<TEntityVM>();
            Expression<Func<TEntityVM, bool>> searchExpression = this.CreateDefaultSearchExpression();
            if (searchExpression != null)
                queryObject.And(searchExpression);
            Expression<Func<TEntityVM, bool>> expressionByFilterInfo = this.CreateSearchExpressionByFilterInfo(filterInfo);
            if (expressionByFilterInfo != null)
                queryObject.And(expressionByFilterInfo);
            return queryObject.Query();
        }

        protected virtual Expression<Func<TEntityVM, bool>> CreateDefaultSearchExpression()
        {
            return (Expression<Func<TEntityVM, bool>>)null;
        }

        protected virtual Expression<Func<TEntityVM, bool>> CreateSearchExpressionByFilterInfo(
          FilterInfo filterInfo)
        {
            if (ConfigurationController.P1Enabled)
            {
                foreach (FilterData filter in filterInfo.Filters)
                {
                    if (filter.Field.EndsWith("P1"))
                    {
                        FilterData filterData = filter;
                        string input = filter.Value;
                        string str = input != null ? input.ToP1() : (string)null;
                        filterData.Value = str;
                    }
                }
            }
            return filterInfo.TranslateFilter<TEntityVM>(true);
        }

        protected virtual IOrderedQueryable<TEntityVM> CreateOrderedQuery(
          IQueryable<TEntityVM> query,
          List<SortInfo> sortInfoList)
        {
            if (sortInfoList == null || sortInfoList.Count == 0)
                return query.OrderByDescending<TEntityVM>("ID");
            return sortInfoList.CreateOrderedQuery<TEntityVM>(query);
        }

        protected virtual IQueryable<TEntityVM> CreateSearchQuery(IQueryable<TEntity> query)
        {
            Type type = typeof(TEntityVM);
            string key = string.Format("SelectExpression_Of_{0}", (object)type.FullName);
            Expression<Func<TEntity, TEntityVM>> selector = CacheManager.GetData(key) as Expression<Func<TEntity, TEntityVM>>;
            if (selector == null)
            {
                selector = this.CreateSelectExpression(type.GetProperties());
                CacheManager.Add(key, (object)selector);
            }
            return query.Select<TEntity, TEntityVM>(selector);
        }

        private Expression<Func<TEntity, TEntityVM>> CreateSelectExpression(
          PropertyInfo[] entityVmProperties)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "en");
            NewExpression newExpression = Expression.New(typeof(TEntityVM));
            List<MemberAssignment> memberAssignmentList = new List<MemberAssignment>();
            foreach (PropertyInfo entityVmProperty in entityVmProperties)
            {
                PropertyInfo property = typeof(TEntity).GetProperty(entityVmProperty.Name);
                if (!(property == (PropertyInfo)null) && (!property.PropertyType.IsInterface && !((IEnumerable<Type>)property.PropertyType.GetInterfaces()).Any<Type>((Func<Type, bool>)(x => x.GetType() == typeof(IObjectState))) && (!property.PropertyType.IsGenericType || !((IEnumerable<Type>)property.PropertyType.GetInterfaces()).Any<Type>((Func<Type, bool>)(x =>
                {
                    if (x.IsGenericType)
                        return x.GetGenericTypeDefinition() == typeof(IEnumerable<>);
                    return false;
                })))))
                {
                    MemberExpression memberExpression = Expression.Property((Expression)parameterExpression, property);
                    memberAssignmentList.Add(Expression.Bind((MemberInfo)entityVmProperty, (Expression)memberExpression));
                }
            }
            return Expression.Lambda<Func<TEntity, TEntityVM>>((Expression)Expression.MemberInit(newExpression, (IEnumerable<MemberBinding>)memberAssignmentList), parameterExpression);
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
                Expression<Func<TEntityVM, bool>> defaultExpr = this.CreateDefaultSearchExpression();
                //if (defaultExpr != null)// && !defaultExpr.Invoke<TEntityVM, bool>(entityVM))
                //    return this.Request.CreateResponse(HttpStatusCode.OK, new
                //    {
                //        resultCode = 4,
                //        message = "اطلاعات یافت نشد"
                //    });
                entity = this.TranslateEntityVMToEntity(entityVM);
                bool async = await this.HasAccessToAsync(entity.GetID());
                if (!async)
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
                Expression<Func<TEntityVM, bool>> defaultExpr = this.CreateDefaultSearchExpression();
                if (defaultExpr != null )//&& !defaultExpr.Invoke<TEntityVM, bool>(entityVM))
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 4,
                        message = "اطلاعات یافت نشد"
                    });
                entity = this.TranslateEntityVMToEntity(entityVM);
                bool async = await this.HasAccessToAsync(entity.GetID());
                if (!async)
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
                TEntityVM entityVm = this.TranslateEntityToEntityVM(entity2);
                Expression<Func<TEntityVM, bool>> defaultExpr = this.CreateDefaultSearchExpression();
                if ((object)entityVm == null || defaultExpr != null )//&& !defaultExpr.Invoke<TEntityVM, bool>(entityVm))
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 4,
                        message = "اطلاعات یافت نشد"
                    });
                bool async = await this.HasAccessToAsync((object)id);
                if (!async)
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 4,
                        message = "اطلاعات یافت نشد"
                    });
                this.businessManager.Delete((object)id);
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

        protected virtual async Task<bool> HasAccessToAsync(params object[] keyValues)
        {
            if (keyValues == null || keyValues.Length == 0)
                return false;
            QueryObject<TEntityVM> queryObject = new QueryObject<TEntityVM>();
            Expression<Func<TEntityVM, bool>> defaultSearchExpr = this.CreateDefaultSearchExpression();
            if (defaultSearchExpr != null)
                queryObject.And(defaultSearchExpr);
            Expression<Func<TEntityVM, bool>> idExpr = new FilterInfo()
            {
                Filters = {
          new FilterData()
          {
            Field = "ID",
            Operator = "eq",
            Value = keyValues[0].ToString()
          }
        }
            }.TranslateFilter<TEntityVM>(false);
            if (idExpr != null)
                queryObject.And(idExpr);
            IQueryable<TEntity> query = this.BusinessRule.Queryable(false, true, (List<Expression<Func<TEntity, object>>>)null);
            IQueryable<TEntityVM> searchQuery = this.CreateSearchQuery(query);
            if (queryObject.Query() != null)
                searchQuery = searchQuery.Where<TEntityVM>(queryObject.Query());
            bool hasAccess = false;
            if ((object)new TEntity() is ICacheable)
                hasAccess = await Task.Run<bool>((Func<bool>)(() => searchQuery.Any<TEntityVM>()));
            else
                hasAccess = await searchQuery.AnyAsync<TEntityVM>();
            if (!hasAccess)
                await ExceptionManager.SaveExceptionAsync((Exception)new OperationAccessException(string.Format("id: {0}, controller: {1}, userName: {2}", keyValues[0], (object)this.GetType().Name, (object)SecurityManager.CurrentUserContext.UserName)));
            return hasAccess;
        }

        protected virtual bool HasAccessTo(params object[] keyValues)
        {
            if (keyValues == null || keyValues.Length == 0)
                return false;
            QueryObject<TEntityVM> queryObject = new QueryObject<TEntityVM>();
            Expression<Func<TEntityVM, bool>> searchExpression = this.CreateDefaultSearchExpression();
            if (searchExpression != null)
                queryObject.And(searchExpression);
            Expression<Func<TEntityVM, bool>> query = new FilterInfo()
            {
                Filters = {
          new FilterData()
          {
            Field = "ID",
            Operator = "eq",
            Value = keyValues[0].ToString()
          }
        }
            }.TranslateFilter<TEntityVM>(false);
            if (query != null)
                queryObject.And(query);
            IQueryable<TEntityVM> source = this.CreateSearchQuery(this.BusinessRule.Queryable(false, true, (List<Expression<Func<TEntity, object>>>)null));
            if (queryObject.Query() != null)
                source = source.Where<TEntityVM>(queryObject.Query());
            bool flag = source.Any<TEntityVM>();
            if (!flag)
                ExceptionManager.SaveException((Exception)new OperationAccessException(string.Format("id: {0}, controller: {1}, userName: {2}", keyValues[0], (object)this.GetType().Name, (object)SecurityManager.CurrentUserContext.UserName)));
            return flag;
        }

        protected virtual TEntityVM TranslateEntityToEntityVM(TEntity entity)
        {
            return entity.Translate<TEntityVM>();
        }

        protected virtual TEntity TranslateEntityVMToEntity(TEntityVM entityVM)
        {
            if (ConfigurationController.P1Enabled)
                entityVM.AsP1<TEntityVM>();
            return entityVM.Translate<TEntity>();
        }

        protected override void Dispose(bool disposing)
        {
            this.BusinessRule.Dispose();
            base.Dispose(disposing);
        }
    }
}
