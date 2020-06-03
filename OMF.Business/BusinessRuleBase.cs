using OMF.Common;
using OMF.Common.Cache;
using OMF.Common.Configuration;
//using OMF.Common.Excel;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.Common.Validation;
using OMF.EntityFramework.DataContext;
using OMF.EntityFramework.Query;
using OMF.EntityFramework.Repositories;
using OMF.EntityFramework.UnitOfWork;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static OMF.Common.Enums;
using OMF.Common.Excel;
using BPJ.Common.ExceptionManagement.Exceptions;

namespace OMF.Business
{
    public class BusinessRuleBase<TEntity> : ViewBusinessRuleBase<TEntity>, IBusinessRuleBaseAsync<TEntity>, IBusinessRuleBase<TEntity>, IViewBusinessRuleBase<TEntity>, IDisposable, IViewBusinessRuleBaseAsync<TEntity>
      where TEntity : class, IEntity, new()
    {
        private IRepositoryAsync<TEntity> repository;

        public BusinessRuleBase()
        {
        }

        public BusinessRuleBase(IDataContextAsync dataContext)
          : base(dataContext)
        {
        }

        public BusinessRuleBase(IUnitOfWorkAsync unitOfWork)
          : base(unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.businessOwnsUnitOfWork = false;
            this.oprKeys = this.CreateOperationKeys();
        }

        protected IRepositoryAsync<TEntity> Repository
        {
            get
            {
                if (this.repository == null)
                    this.repository = this.CreateRepository(this.unitOfWork);
                return this.repository;
            }
        }

        protected virtual IRepositoryAsync<TEntity> CreateRepository(
          IUnitOfWorkAsync unitOfWork)
        {
            return unitOfWork.RepositoryAsync<TEntity>();
        }

        protected override TEntity FindEntity(params object[] keyValues)
        {
            return this.Repository.Find(keyValues);
        }

        protected override async Task<TEntity> FindEntityAsync(
          CancellationToken cancellationToken,
          params object[] keyValues)
        {
            TEntity entity1 = await this.ViewRepository.FindAsync(cancellationToken, keyValues);
            TEntity entity2 = entity1;
            entity1 = default(TEntity);
            return entity2;
        }

        protected override IQueryable<TEntity> GetQueryableData(
          bool containsDeletedData = false,
          bool fromCache = true,
          List<Expression<Func<TEntity, object>>> includes = null)
        {
            return this.Repository.Queryable(containsDeletedData, fromCache, includes);
        }

        protected override void LoadReferenceEntity<TProperty>(
          TEntity entity,
          Expression<Func<TEntity, TProperty>> navigationProperty,
          bool forceToDetach)
        {
            this.Repository.LoadReference<TProperty>(entity, navigationProperty, forceToDetach);
        }

        protected override async Task LoadReferenceEntityAsync<TProperty>(
          TEntity entity,
          Expression<Func<TEntity, TProperty>> navigationProperty,
          bool forceToDetach)
        {
            await this.Repository.LoadReferenceAsync<TProperty>(entity, navigationProperty, forceToDetach);
        }

        protected override void LoadCollectionEntity<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          Expression<Func<TElement, bool>> predicate,
          bool forceToDetach)
        {
            this.Repository.LoadCollection<TElement>(entity, navigationProperty, predicate, forceToDetach);
        }

        protected override async Task LoadCollectionEntityAsync<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          Expression<Func<TElement, bool>> predicate,
          bool forceToDetach)
        {
            await this.Repository.LoadCollectionAsync<TElement>(entity, navigationProperty, predicate, forceToDetach);
        }

        public void Insert(TEntity entity)
        {
            if (!this.UseForAnonymousUser)
            {
                SecurityManager.ThrowIfUserContextNull();
                if (!this.OperationAccess.CanInsert)
                    throw new OperationAccessException(this.InsertKey);
            }
            entity.ObjectState = ObjectState.Added;
            List<string> failureList = new List<string>();
            failureList.AddRange((IEnumerable<string>)this.CheckCommonRules(entity));
            failureList.AddRange((IEnumerable<string>)this.CheckInsertRules(entity));
            if (failureList.Count > 0)
                throw new OMFValidationException(failureList);
            this.InsertEntity(entity);
        }

        public void Insert(IEnumerable<TEntity> entityList)
        {
            if (!this.UseForAnonymousUser)
            {
                SecurityManager.ThrowIfUserContextNull();
                if (!this.OperationAccess.CanInsert)
                    throw new OperationAccessException(this.InsertKey);
            }
            List<string> failureList = new List<string>();
            foreach (TEntity entity in entityList)
            {
                entity.ObjectState = ObjectState.Added;
                failureList.AddRange((IEnumerable<string>)this.CheckCommonRules(entity));
                failureList.AddRange((IEnumerable<string>)this.CheckInsertRules(entity));
            }
            if (failureList.Count > 0)
                throw new OMFValidationException(failureList);
            foreach (TEntity entity in entityList)
                this.InsertEntity(entity);
        }

        public void InsertOrUpdateGraph(TEntity entity)
        {
            if (!this.UseForAnonymousUser)
            {
                SecurityManager.ThrowIfUserContextNull();
                if (entity.ObjectState == ObjectState.Added && !this.OperationAccess.CanInsert)
                    throw new OperationAccessException(this.InsertKey);
                if (entity.ObjectState == ObjectState.Modified && !this.OperationAccess.CanUpdate)
                    throw new OperationAccessException(this.UpdateKey);
            }
            List<string> stringList = new List<string>();
            if (entity.ObjectState == ObjectState.Added)
                stringList.AddRange((IEnumerable<string>)this.CheckInsertRules(entity));
            else if (entity.ObjectState == ObjectState.Modified)
                stringList.AddRange((IEnumerable<string>)this.CheckUpdateRules(entity));
            stringList.AddRange((IEnumerable<string>)this.ValidateGraph((IEntity)entity));
            if (stringList.Any<string>())
                throw new OMFValidationException(stringList);
            this.InsertOrUpdateGraphEntity(entity);
        }

        protected virtual void InsertOrUpdateGraphEntity(TEntity entity)
        {
            this.Repository.InsertOrUpdateGraph(entity);
        }

        protected virtual void InsertEntity(TEntity entity)
        {
            this.Repository.Insert(entity);
        }

        public void Update(TEntity entity)
        {
            if (!this.UseForAnonymousUser)
            {
                SecurityManager.ThrowIfUserContextNull();
                if (!this.OperationAccess.CanUpdate)
                    throw new OperationAccessException(this.UpdateKey);
            }
            if (entity.ObjectState == ObjectState.Added)
                entity.ObjectState = ObjectState.Added;
            else
                entity.ObjectState = ObjectState.Modified;
            List<string> failureList = new List<string>();
            failureList.AddRange((IEnumerable<string>)this.CheckCommonRules(entity));
            if (entity.ObjectState == ObjectState.Modified)
                failureList.AddRange((IEnumerable<string>)this.CheckUpdateRules(entity));
            else if (entity.ObjectState == ObjectState.Added)
                failureList.AddRange((IEnumerable<string>)this.CheckInsertRules(entity));
            if (failureList.Count > 0)
                throw new OMFValidationException(failureList);
            if (entity.ObjectState == ObjectState.Modified)
            {
                this.UpdateEntity(entity);
            }
            else
            {
                if (entity.ObjectState != ObjectState.Added)
                    return;
                this.InsertEntity(entity);
            }
        }

        public void Update(IEnumerable<TEntity> entityList)
        {
            if (!this.UseForAnonymousUser)
            {
                SecurityManager.ThrowIfUserContextNull();
                if (!this.OperationAccess.CanUpdate)
                    throw new OperationAccessException(this.UpdateKey);
            }
            List<string> failureList = new List<string>();
            foreach (TEntity entity in entityList)
            {
                if (entity.ObjectState == ObjectState.Added)
                    entity.ObjectState = ObjectState.Added;
                else
                    entity.ObjectState = ObjectState.Modified;
                failureList.AddRange((IEnumerable<string>)this.CheckCommonRules(entity));
                if (entity.ObjectState == ObjectState.Modified)
                    failureList.AddRange((IEnumerable<string>)this.CheckUpdateRules(entity));
                else if (entity.ObjectState == ObjectState.Added)
                    failureList.AddRange((IEnumerable<string>)this.CheckInsertRules(entity));
            }
            if (failureList.Count > 0)
                throw new OMFValidationException(failureList);
            foreach (TEntity entity in entityList)
            {
                if (entity.ObjectState == ObjectState.Modified)
                    this.UpdateEntity(entity);
                else if (entity.ObjectState == ObjectState.Added)
                    this.InsertEntity(entity);
            }
        }

        protected virtual void UpdateEntity(TEntity entity)
        {
            if ((object)entity is ISelfReferenceEntity && (object)entity is IActivatable)
            {
                if (!((IActivatable)(object)entity).IsActive)
                {
                    Expression<Func<TEntity, bool>> predicate = new FilterInfo()
                    {
                        Filters = new List<FilterData>()
            {
              new FilterData()
              {
                Field = "ParentId",
                Operator = "eq",
                Value = entity.ID.ToString()
              },
              new FilterData()
              {
                Field = "IsActive",
                Operator = "eq",
                Value = (!((IActivatable) (object) entity).IsActive).ToString()
              }
            },
                        Logic = "and"
                    }.TranslateFilter<TEntity>(true);
                    if (this.Queryable(false, true, (List<Expression<Func<TEntity, object>>>)null).Count<TEntity>(predicate) > 0)
                    {
                        foreach (TEntity entity1 in this.Queryable(false, true, (List<Expression<Func<TEntity, object>>>)null).Where<TEntity>(predicate).ToList<TEntity>())
                        {
                            ((IActivatable)(object)entity1).IsActive = false;
                            this.UpdateEntity(entity1);
                        }
                    }
                }
                else
                {
                    int? parentId = entity["ParentId"].ConvertTo<int?>();
                    int num1;
                    if (parentId.HasValue)
                    {
                        int? nullable = parentId;
                        int num2 = 0;
                        num1 = nullable.GetValueOrDefault() == num2 ? (!nullable.HasValue ? 1 : 0) : 1;
                    }
                    else
                        num1 = 0;
                    if (num1 != 0)
                    {
                        TEntity entity1 = this.Queryable(false, true, (List<Expression<Func<TEntity, object>>>)null).SingleOrDefault<TEntity>((Expression<Func<TEntity, bool>>)(en => (int?)en.ID == parentId));
                        ((IActivatable)(object)entity1).IsActive = true;
                        this.UpdateEntity(entity1);
                    }
                }
            }
            this.Repository.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            if (!this.UseForAnonymousUser)
            {
                SecurityManager.ThrowIfUserContextNull();
                if (!this.OperationAccess.CanDelete)
                    throw new OperationAccessException(this.DeleteKey);
            }
            if (entity.ObjectState == ObjectState.Added)
                entity.ObjectState = ObjectState.Detached;
            else
                entity.ObjectState = ObjectState.Deleted;
            if (entity.ObjectState == ObjectState.Detached)
                return;
            List<string> failureList = new List<string>();
            failureList.AddRange((IEnumerable<string>)this.CheckDeleteRules(entity));
            if (failureList.Count > 0)
                throw new OMFValidationException(failureList);
            this.DeleteEntity(entity);
        }

        public void Delete(IEnumerable<TEntity> entityList)
        {
            if (!this.UseForAnonymousUser)
            {
                SecurityManager.ThrowIfUserContextNull();
                if (!this.OperationAccess.CanDelete)
                    throw new OperationAccessException(this.DeleteKey);
            }
            List<string> failureList = new List<string>();
            foreach (TEntity entity in entityList)
            {
                if (entity.ObjectState == ObjectState.Added)
                    entity.ObjectState = ObjectState.Detached;
                else
                    entity.ObjectState = ObjectState.Deleted;
                if (entity.ObjectState != ObjectState.Detached)
                    failureList.AddRange((IEnumerable<string>)this.CheckDeleteRules(entity));
            }
            if (failureList.Count > 0)
                throw new OMFValidationException(failureList);
            foreach (TEntity entity in entityList)
            {
                if (entity.ObjectState != ObjectState.Detached)
                    this.DeleteEntity(entity);
            }
        }

        public void Delete(params object[] keyValues)
        {
            this.Delete(this.Repository.Find(keyValues));
        }

        protected virtual void DeleteEntity(TEntity entity)
        {
            if ((object)entity is ISelfReferenceEntity)
            {
                Expression<Func<TEntity, bool>> predicate = new FilterInfo()
                {
                    Filters = new List<FilterData>()
          {
            new FilterData()
            {
              Field = "ParentId",
              Operator = "eq",
              Value = entity.ID.ToString()
            },
            new FilterData()
            {
              Field = "IsDeleted",
              Operator = "eq",
              Value = false.ToString()
            }
          },
                    Logic = "and"
                }.TranslateFilter<TEntity>(true);
                if (this.Queryable(false, true, (List<Expression<Func<TEntity, object>>>)null).Count<TEntity>(predicate) > 0)
                {
                    foreach (TEntity entity1 in this.Queryable(false, true, (List<Expression<Func<TEntity, object>>>)null).Where<TEntity>(predicate).ToList<TEntity>())
                        this.DeleteEntity(entity1);
                }
            }
            if ((object)entity is ILogicalDeletable)
            {
                ((ILogicalDeletable)(object)entity).IsDeleted = true;
                if ((object)entity is IActivatable)
                    ((IActivatable)(object)entity).IsActive = false;
                this.Repository.Update(entity);
            }
            else
                this.Repository.Delete(entity);
        }

        public int SaveChanges()
        {
            return this.unitOfWork.SaveChanges();
        }

        public async Task DeleteAsync(params object[] keyValues)
        {
            TEntity entity1 = await this.Repository.FindAsync(keyValues);
            TEntity entity2 = entity1;
            entity1 = default(TEntity);
            this.Delete(entity2);
        }

        public async Task<int> SaveChangesAsync()
        {
            int num = await this.SaveChangesAsync(CancellationToken.None);
            return num;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (!this.UseForAnonymousUser)
                SecurityManager.ThrowIfUserContextNull();
            int num = await this.unitOfWork.SaveChangesAsync(cancellationToken);
            return num;
        }

        public virtual void Import(IEnumerable<TEntity> entityList)
        {
            if (!this.UseForAnonymousUser)
            {
                SecurityManager.ThrowIfUserContextNull();
                if (!this.OperationAccess.CanImport)
                    throw new OperationAccessException(this.ImportKey);
            }
            string message = this.ValidateImport(entityList);
            if (!string.IsNullOrEmpty(message))
                throw new ImportExcelException(message);
            this.OperationAccess = new OperationAccess()
            {
                CanDelete = true,
                CanExport = true,
                CanImport = true,
                CanInsert = true,
                CanPrint = true,
                CanUpdate = true,
                CanView = true
            };
            foreach (TEntity entity in entityList)
            {
                if (this.ExistsForImport(entity))
                    this.Update(entity);
                else
                    this.Insert(entity);
            }
        }

        public object ListDataForImport(EntityImportItem item)
        {
            this.OperationAccess = new OperationAccess()
            {
                CanDelete = true,
                CanExport = true,
                CanImport = true,
                CanInsert = true,
                CanPrint = true,
                CanUpdate = true,
                CanView = true
            };
            return this.ListDataForImportExcel(item);
        }

        protected virtual object ListDataForImportExcel(EntityImportItem item)
        {
            return (object)this.Queryable(false, true, (List<Expression<Func<TEntity, object>>>)null);
        }

        protected virtual bool ExistsForImport(TEntity entityInfo)
        {
            return false;
        }

        protected virtual string ValidateImport(IEnumerable<TEntity> entityList)
        {
            return (string)null;
        }

        public OMF.Common.Validation.EntityValidator<TEntity> EntityValidator
        {
            get
            {
                return ValidationManager.GetEntityValidator<TEntity>();
            }
        }

        public List<string> Validate(TEntity entity)
        {
            return this.CheckCommonRules(entity);
        }

        protected virtual List<string> CheckCommonRules(TEntity entity)
        {
            return this.Validate((IEntityValidator)this.EntityValidator, (IObjectState)entity);
        }

        private List<string> Validate(IEntityValidator validator, IObjectState entity)
        {
            if (validator == null)
                return new List<string>();
            ValidationResult validationResult = validator.Validate((object)entity);
            if (validationResult.IsValid)
                return new List<string>();
            return validationResult.Errors.Select<ValidationFailure, string>((Func<ValidationFailure, string>)(failure => failure.ErrorMessage)).ToList<string>();
        }

        protected virtual List<string> ValidateGraph(IEntity entity)
        {
            if (entity.ObjectState != ObjectState.Added && entity.ObjectState != ObjectState.Modified)
                return new List<string>();
            List<string> stringList = new List<string>();
            IEntityValidator entityValidator = ValidationManager.GetEntityValidator(entity.GetType());
            stringList.AddRange((IEnumerable<string>)this.Validate(entityValidator, (IObjectState)entity));
            foreach (PropertyInfo property in entity.GetType().GetProperties())
            {
                if (!((IEnumerable<ParameterInfo>)property.GetIndexParameters()).Any<ParameterInfo>())
                {
                    object obj = property.GetValue((object)entity, (object[])null);
                    if (obj is IEntity)
                        stringList.AddRange((IEnumerable<string>)this.ValidateGraph(obj as IEntity));
                    else if (obj is IEnumerable<IEntity>)
                    {
                        foreach (IEntity entity1 in property.GetValue((object)entity, (object[])null) as IEnumerable<IEntity>)
                            stringList.AddRange((IEnumerable<string>)this.ValidateGraph(entity1));
                    }
                }
            }
            return stringList;
        }

        protected virtual List<string> CheckInsertRules(TEntity entity)
        {
            return new List<string>();
        }

        protected virtual List<string> CheckUpdateRules(TEntity entity)
        {
            return new List<string>();
        }

        protected virtual List<string> CheckDeleteRules(TEntity entity)
        {
            return new List<string>();
        }

        public string InsertKey
        {
            get
            {
                return this.oprKeys.InsertKey;
            }
            set
            {
                this.oprKeys.InsertKey = value;
            }
        }

        public string UpdateKey
        {
            get
            {
                return this.oprKeys.UpdateKey;
            }
        }

        public string DeleteKey
        {
            get
            {
                return this.oprKeys.DeleteKey;
            }
        }

        public string ImportKey
        {
            get
            {
                return this.oprKeys.ImportKey;
            }
        }

        public override OperationAccess CreateOperationAccess()
        {
            if (this.UseForAnonymousUser)
                return new OperationAccess()
                {
                    CanDelete = true,
                    CanExport = true,
                    CanImport = true,
                    CanInsert = true,
                    CanPrint = true,
                    CanUpdate = true,
                    CanView = true
                };
            SecurityManager.ThrowIfUserContextNull();
            if (!ConfigurationController.EnableSecurityCheck)
                return new OperationAccess()
                {
                    CanDelete = true,
                    CanExport = true,
                    CanImport = true,
                    CanInsert = true,
                    CanPrint = true,
                    CanUpdate = true,
                    CanView = true
                };
            OperationAccess operationAccess = SessionManager.GetData(this.OprAccessSessionKey) as OperationAccess;
            if (operationAccess == null)
            {
                operationAccess = new OperationAccess();
                operationAccess.CanImport = this.HasAccess(this.ImportKey);
                operationAccess.CanView = this.HasAccess(this.ViewKey);
                operationAccess.CanInsert = this.HasAccess(this.InsertKey);
                operationAccess.CanDelete = this.HasAccess(this.DeleteKey);
                operationAccess.CanUpdate = this.HasAccess(this.UpdateKey);
                operationAccess.CanExport = this.HasAccess(this.ExportKey);
                operationAccess.CanPrint = this.HasAccess(this.PrintKey);
                SessionManager.Add(this.OprAccessSessionKey, (object)operationAccess);
            }
            return operationAccess;
        }

        public override async Task<OperationAccess> CreateOperationAccessAsync()
        {
            if (this.UseForAnonymousUser)
                return new OperationAccess()
                {
                    CanDelete = true,
                    CanExport = true,
                    CanImport = true,
                    CanInsert = true,
                    CanPrint = true,
                    CanUpdate = true,
                    CanView = true
                };
            SecurityManager.ThrowIfUserContextNull();
            if (!ConfigurationController.EnableSecurityCheck)
                return new OperationAccess()
                {
                    CanDelete = true,
                    CanExport = true,
                    CanImport = true,
                    CanInsert = true,
                    CanPrint = true,
                    CanUpdate = true,
                    CanView = true
                };
            OperationAccess oprAccess = SessionManager.GetData(this.OprAccessSessionKey) as OperationAccess;
            if (oprAccess == null)
            {
                oprAccess = new OperationAccess();
                OperationAccess operationAccess1 = oprAccess;
                bool flag1 = await this.HasAccessAsync(this.ImportKey);
                operationAccess1.CanImport = flag1;
                operationAccess1 = (OperationAccess)null;
                OperationAccess operationAccess2 = oprAccess;
                bool flag2 = await this.HasAccessAsync(this.ViewKey);
                operationAccess2.CanView = flag2;
                operationAccess2 = (OperationAccess)null;
                OperationAccess operationAccess3 = oprAccess;
                bool flag3 = await this.HasAccessAsync(this.InsertKey);
                operationAccess3.CanInsert = flag3;
                operationAccess3 = (OperationAccess)null;
                OperationAccess operationAccess4 = oprAccess;
                bool flag4 = await this.HasAccessAsync(this.DeleteKey);
                operationAccess4.CanDelete = flag4;
                operationAccess4 = (OperationAccess)null;
                OperationAccess operationAccess5 = oprAccess;
                bool flag5 = await this.HasAccessAsync(this.UpdateKey);
                operationAccess5.CanUpdate = flag5;
                operationAccess5 = (OperationAccess)null;
                OperationAccess operationAccess6 = oprAccess;
                bool flag6 = await this.HasAccessAsync(this.ExportKey);
                operationAccess6.CanExport = flag6;
                operationAccess6 = (OperationAccess)null;
                OperationAccess operationAccess7 = oprAccess;
                bool flag7 = await this.HasAccessAsync(this.PrintKey);
                operationAccess7.CanPrint = flag7;
                operationAccess7 = (OperationAccess)null;
                SessionManager.Add(this.OprAccessSessionKey, (object)oprAccess);
            }
            return oprAccess;
        }
    }
}
