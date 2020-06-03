using OMF.Common;
using OMF.Common.ActivityLog;
using OMF.Common.Cache;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.DataContext;
using OMF.EntityFramework.Repositories;
using OMF.EntityFramework.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace OMF.Business
{
    public class ViewBusinessRuleBase<TEntity> : IViewBusinessRuleBaseAsync<TEntity>, IViewBusinessRuleBase<TEntity>, IDisposable
      where TEntity : class, IViewEntity, new()
    {
        protected bool businessOwnsUnitOfWork = true;
        private IViewRepositoryAsync<TEntity> viewRepository;
        protected IUnitOfWorkAsync unitOfWork;
        protected OperationKeys oprKeys;
        protected OperationAccess oprAccess;

        public IUnitOfWorkAsync UnitOfWork
        {
            get
            {
                return this.unitOfWork;
            }
        }

        public ViewBusinessRuleBase()
        {
            this.unitOfWork = (IUnitOfWorkAsync)new OMF.EntityFramework.Ef6.UnitOfWork();
            this.oprKeys = this.CreateOperationKeys();
        }

        public ViewBusinessRuleBase(IDataContextAsync dataContext)
        {
            this.unitOfWork = (IUnitOfWorkAsync)new OMF.EntityFramework.Ef6.UnitOfWork(dataContext);
            this.oprKeys = this.CreateOperationKeys();
        }

        public ViewBusinessRuleBase(IUnitOfWorkAsync unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.businessOwnsUnitOfWork = false;
            this.oprKeys = this.CreateOperationKeys();
        }

        public bool UseForAnonymousUser { get; set; } = false;

        protected virtual OperationKeys CreateOperationKeys()
        {
            return OperationKeysManager.GetOperationKeys<TEntity>();
        }

        protected IViewRepositoryAsync<TEntity> ViewRepository
        {
            get
            {
                if (this.viewRepository == null)
                    this.viewRepository = this.CreateViewRepository(this.unitOfWork);
                return this.viewRepository;
            }
        }

        protected virtual IViewRepositoryAsync<TEntity> CreateViewRepository(
          IUnitOfWorkAsync unitOfWork)
        {
            return unitOfWork.ViewRepositoryAsync<TEntity>();
        }

        public IQueryable<TEntity> Queryable(
          bool containsDeletedData = false,
          bool fromCache = true,
          List<Expression<Func<TEntity, object>>> includes = null)
        {
            if (!this.UseForAnonymousUser)
            {
                SecurityManager.ThrowIfUserContextNull();
                if (!this.OperationAccess.CanView)
                    throw new OperationAccessException(this.ViewKey);
            }
            return this.GetQueryableData(containsDeletedData, fromCache, includes);
        }

        protected virtual IQueryable<TEntity> GetQueryableData(
          bool containsDeletedData = false,
          bool fromCache = true,
          List<Expression<Func<TEntity, object>>> includes = null)
        {
            return this.ViewRepository.Queryable(containsDeletedData, fromCache, includes);
        }

        public virtual TEntity InitializeEmptyValue()
        {
            return default(TEntity);
        }

        public TEntity Find(params object[] keyValues)
        {
            if (!this.UseForAnonymousUser)
            {
                SecurityManager.ThrowIfUserContextNull();
                if (!this.OperationAccess.CanView)
                    throw new OperationAccessException(this.ViewKey);
            }
            TEntity entity1 = this.FindEntity(keyValues);
            if ((object)entity1 is IActivityLoggable)
            {
                IActivityLoggable entity2 = (IActivityLoggable)(object)entity1;
                if (entity2.ActionsToLog == ActionLog.All || (entity2.ActionsToLog & ActionLog.Read) == ActionLog.Read)
                {
                    OMF.Common.ActivityLog.ActivityLog activityLog = ActivityLogManager.CreateActivityLog(entity2);
                    activityLog.Action = 1;
                    ActivityLogManager.Save(activityLog);
                }
            }
            return entity1;
        }

        protected virtual TEntity FindEntity(params object[] keyValues)
        {
            return this.ViewRepository.Find(keyValues);
        }

        public void LoadReference<TProperty>(
          TEntity entity,
          Expression<Func<TEntity, TProperty>> navigationProperty,
          bool forceToDetach = false)
          where TProperty : class, IEntity
        {
            if (!this.UseForAnonymousUser)
            {
                SecurityManager.ThrowIfUserContextNull();
                if (!this.OperationAccess.CanView)
                    throw new OperationAccessException(this.ViewKey);
            }
            this.LoadReferenceEntity<TProperty>(entity, navigationProperty, forceToDetach);
        }

        protected virtual void LoadReferenceEntity<TProperty>(
          TEntity entity,
          Expression<Func<TEntity, TProperty>> navigationProperty,
          bool forceToDetach)
          where TProperty : class, IEntity
        {
            this.ViewRepository.LoadReference<TProperty>(entity, navigationProperty, forceToDetach);
        }

        public TProperty LoadReference<TProperty>(
          Expression<Func<TEntity, TProperty>> navigationProperty,
          params object[] keyValues)
          where TProperty : class, IEntity
        {
            TEntity entity = new TEntity();
            entity.SetID(keyValues);
            this.LoadReference<TProperty>(entity, navigationProperty, true);
            PropertyInfo member = navigationProperty.GetMember() as PropertyInfo;
            if (member == (PropertyInfo)null)
                return default(TProperty);
            return member.GetValue((object)entity) as TProperty;
        }

        public void LoadCollection<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          bool forceToDetach = false)
          where TElement : class, IEntity
        {
            this.LoadCollection<TElement>(entity, navigationProperty, (Expression<Func<TElement, bool>>)null, forceToDetach);
        }

        public void LoadCollection<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          Expression<Func<TElement, bool>> predicate,
          bool forceToDetach = false)
          where TElement : class, IEntity
        {
            if (!this.UseForAnonymousUser)
            {
                SecurityManager.ThrowIfUserContextNull();
                if (!this.OperationAccess.CanView)
                    throw new OperationAccessException(this.ViewKey);
            }
            this.LoadCollectionEntity<TElement>(entity, navigationProperty, predicate, forceToDetach);
        }

        protected virtual void LoadCollectionEntity<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          Expression<Func<TElement, bool>> predicate,
          bool forceToDetach)
          where TElement : class, IEntity
        {
            this.ViewRepository.LoadCollection<TElement>(entity, navigationProperty, predicate, forceToDetach);
        }

        public ICollection<TElement> LoadCollection<TElement>(
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          params object[] keyValues)
          where TElement : class, IEntity
        {
            return this.LoadCollection<TElement>(navigationProperty, (Expression<Func<TElement, bool>>)null, keyValues);
        }

        public ICollection<TElement> LoadCollection<TElement>(
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          Expression<Func<TElement, bool>> predicate,
          params object[] keyValues)
          where TElement : class, IEntity
        {
            TEntity entity = new TEntity();
            entity.SetID(keyValues);
            this.LoadCollection<TElement>(entity, navigationProperty, predicate, true);
            PropertyInfo member = navigationProperty.GetMember() as PropertyInfo;
            if (member == (PropertyInfo)null)
                return (ICollection<TElement>)new List<TElement>();
            return member.GetValue((object)entity) as ICollection<TElement> ?? (ICollection<TElement>)new List<TElement>();
        }

        public async Task<TEntity> FindAsync(params object[] keyValues)
        {
            TEntity async = await this.FindAsync(CancellationToken.None, keyValues);
            return async;
        }

        public async Task<TEntity> FindAsync(
          CancellationToken cancellationToken,
          params object[] keyValues)
        {
            if (!this.UseForAnonymousUser)
            {
                SecurityManager.ThrowIfUserContextNull();
                OperationAccess operationAccessAsync = await this.CreateOperationAccessAsync();
                if (!this.OperationAccess.CanView)
                    throw new OperationAccessException(this.ViewKey);
            }
            TEntity entity1 = await this.FindEntityAsync(cancellationToken, keyValues);
            TEntity entity2 = entity1;
            entity1 = default(TEntity);
            if ((object)entity2 is IActivityLoggable)
            {
                IActivityLoggable entityLog = (IActivityLoggable)(object)entity2;
                if (entityLog.ActionsToLog == ActionLog.All || (entityLog.ActionsToLog & ActionLog.Read) == ActionLog.Read)
                {
                    OMF.Common.ActivityLog.ActivityLog actLog = ActivityLogManager.CreateActivityLog(entityLog);
                    actLog.Action = 1;
                    ActivityLogManager.Save(actLog);
                    actLog = (OMF.Common.ActivityLog.ActivityLog)null;
                }
                entityLog = (IActivityLoggable)null;
            }
            return entity2;
        }

        protected virtual async Task<TEntity> FindEntityAsync(
          CancellationToken cancellationToken,
          params object[] keyValues)
        {
            TEntity entity1 = await this.ViewRepository.FindAsync(cancellationToken, keyValues);
            TEntity entity2 = entity1;
            entity1 = default(TEntity);
            return entity2;
        }

        public async Task LoadReferenceAsync<TProperty>(
          TEntity entity,
          Expression<Func<TEntity, TProperty>> navigationProperty,
          bool forceToDetach = false)
          where TProperty : class, IEntity
        {
            if (!this.UseForAnonymousUser)
            {
                SecurityManager.ThrowIfUserContextNull();
                OperationAccess operationAccessAsync = await this.CreateOperationAccessAsync();
                if (!this.OperationAccess.CanView)
                    throw new OperationAccessException(this.ViewKey);
            }
            await this.LoadReferenceEntityAsync<TProperty>(entity, navigationProperty, forceToDetach);
        }

        protected virtual async Task LoadReferenceEntityAsync<TProperty>(
          TEntity entity,
          Expression<Func<TEntity, TProperty>> navigationProperty,
          bool forceToDetach)
          where TProperty : class, IEntity
        {
            await this.ViewRepository.LoadReferenceAsync<TProperty>(entity, navigationProperty, forceToDetach);
        }

        public async Task<TProperty> LoadReferenceAsync<TProperty>(
          Expression<Func<TEntity, TProperty>> navigationProperty,
          params object[] keyValues)
          where TProperty : class, IEntity
        {
            TEntity entity = new TEntity();
            entity.SetID(keyValues);
            await this.LoadReferenceAsync<TProperty>(entity, navigationProperty, true);
            PropertyInfo propInfo = navigationProperty.GetMember() as PropertyInfo;
            if (propInfo == (PropertyInfo)null)
                return default(TProperty);
            TProperty referenceValue = propInfo.GetValue((object)entity) as TProperty;
            return referenceValue;
        }

        public async Task LoadCollectionAsync<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          bool forceToDetach = false)
          where TElement : class, IEntity
        {
            await this.LoadCollectionAsync<TElement>(entity, navigationProperty, (Expression<Func<TElement, bool>>)null, forceToDetach);
        }

        public async Task LoadCollectionAsync<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          Expression<Func<TElement, bool>> predicate,
          bool forceToDetach = false)
          where TElement : class, IEntity
        {
            if (!this.UseForAnonymousUser)
            {
                SecurityManager.ThrowIfUserContextNull();
                OperationAccess operationAccessAsync = await this.CreateOperationAccessAsync();
                if (!this.OperationAccess.CanView)
                    throw new OperationAccessException(this.ViewKey);
            }
            await this.LoadCollectionEntityAsync<TElement>(entity, navigationProperty, predicate, forceToDetach);
        }

        protected virtual async Task LoadCollectionEntityAsync<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          Expression<Func<TElement, bool>> predicate,
          bool forceToDetach)
          where TElement : class, IEntity
        {
            await this.ViewRepository.LoadCollectionAsync<TElement>(entity, navigationProperty, predicate, forceToDetach);
        }

        public async Task<ICollection<TElement>> LoadCollectionAsync<TElement>(
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          params object[] keyValues)
          where TElement : class, IEntity
        {
            ICollection<TElement> elements = await this.LoadCollectionAsync<TElement>(navigationProperty, (Expression<Func<TElement, bool>>)null, keyValues);
            return elements;
        }

        public async Task<ICollection<TElement>> LoadCollectionAsync<TElement>(
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          Expression<Func<TElement, bool>> predicate,
          params object[] keyValues)
          where TElement : class, IEntity
        {
            TEntity entity = new TEntity();
            entity.SetID(keyValues);
            await this.LoadCollectionAsync<TElement>(entity, navigationProperty, predicate, true);
            PropertyInfo propInfo = navigationProperty.GetMember() as PropertyInfo;
            if (propInfo == (PropertyInfo)null)
                return (ICollection<TElement>)new List<TElement>();
            ICollection<TElement> collection = propInfo.GetValue((object)entity) as ICollection<TElement>;
            return collection ?? (ICollection<TElement>)new List<TElement>();
        }

        public string ViewKey
        {
            get
            {
                return this.oprKeys.ViewKey;
            }
        }

        public string ExportKey
        {
            get
            {
                return this.oprKeys.ExportKey;
            }
        }

        public string PrintKey
        {
            get
            {
                return this.oprKeys.PrintKey;
            }
        }

        public OperationAccess OperationAccess
        {
            get
            {
                if (this.oprAccess == null)
                    this.oprAccess = this.CreateOperationAccess();
                return this.oprAccess;
            }
            set
            {
                this.oprAccess = value;
            }
        }

        public virtual OperationAccess CreateOperationAccess()
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
                operationAccess.CanImport = false;
                operationAccess.CanView = this.HasAccess(this.ViewKey);
                operationAccess.CanInsert = false;
                operationAccess.CanDelete = false;
                operationAccess.CanUpdate = false;
                operationAccess.CanExport = this.HasAccess(this.ExportKey);
                operationAccess.CanPrint = this.HasAccess(this.PrintKey);
                SessionManager.Add(this.OprAccessSessionKey, (object)operationAccess);
            }
            return operationAccess;
        }

        public virtual async Task<OperationAccess> CreateOperationAccessAsync()
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
                oprAccess.CanImport = false;
                OperationAccess operationAccess1 = oprAccess;
                bool flag1 = await this.HasAccessAsync(this.ViewKey);
                operationAccess1.CanView = flag1;
                operationAccess1 = (OperationAccess)null;
                oprAccess.CanInsert = false;
                oprAccess.CanDelete = false;
                oprAccess.CanUpdate = false;
                OperationAccess operationAccess2 = oprAccess;
                bool flag2 = await this.HasAccessAsync(this.ExportKey);
                operationAccess2.CanExport = flag2;
                operationAccess2 = (OperationAccess)null;
                OperationAccess operationAccess3 = oprAccess;
                bool flag3 = await this.HasAccessAsync(this.PrintKey);
                operationAccess3.CanPrint = flag3;
                operationAccess3 = (OperationAccess)null;
                SessionManager.Add(this.OprAccessSessionKey, (object)oprAccess);
            }
            return oprAccess;
        }

        public virtual string OprAccessSessionKey
        {
            get
            {
                return "OprAccess_" + this.GetType().FullName;
            }
        }

        protected bool HasAccess(string securityKey)
        {
            return SecurityManager.HasAccess(securityKey);
        }

        protected async Task<bool> HasAccessAsync(string securityKey)
        {
            bool result = await SecurityManager.HasAccessAsync(securityKey);
            return result;
        }

        public void Dispose()
        {
            if (!this.businessOwnsUnitOfWork)
                return;
            this.unitOfWork.Dispose();
        }
    }
}
