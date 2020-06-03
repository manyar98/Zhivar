//using AutoMapper;
using OMF.Common;
using OMF.Common.ActivityLog;
using OMF.Common.Cache;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.Common;
using OMF.EntityFramework.DataContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace OMF.EntityFramework.Ef6
{
    public class DataContext : DbContext, IDataContextAsync, IDataContext, IDisposable
    {
        private readonly Guid _instanceId;
        private bool _disposed;

        public DataContext()
          : base(ConnectionManager.CreateConnection("OMF.App.ConstructionString"), true)
        {
            this._instanceId = Guid.NewGuid();
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DataContext(string connectionName)
          : base(ConnectionManager.CreateConnection(connectionName), true)
        {
            this._instanceId = Guid.NewGuid();
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DataContext(DbConnection existingConnection)
          : base(existingConnection, true)
        {
            this._instanceId = Guid.NewGuid();
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DataContext(DbConnection existingConnection, bool contextOwnsConnection)
          : base(existingConnection, contextOwnsConnection)
        {
            this._instanceId = Guid.NewGuid();
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public Guid InstanceId
        {
            get
            {
                return this._instanceId;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
       
            //Mapper.Initialize((Action<IMapperConfigurationExpression>)(config =>
            //{
            //    config.AllowNullCollections = true;// new bool?(true);
            //    config.AllowNullDestinationValues = true;// new bool?(true);
            //    config.CreateMissingTypeMaps = true;// new bool?(true);
            //}));
        }

        public override int SaveChanges()
        {
            try
            {
                int num = 0;
                this.SyncObjectsStatePreCommit();
                if (this.ChangeTracker.HasChanges())
                {
                    List<object> objectList = new List<object>();
                    foreach (DbEntityEntry entry in this.ChangeTracker.Entries())
                    {
                        if (entry.State != System.Data.Entity.EntityState.Detached && entry.State != System.Data.Entity.EntityState.Unchanged)
                        {
                            if (ConfigurationController.CustomIdentityEnabled && entry.State == System.Data.Entity.EntityState.Added && entry.Entity is IEntity && entry.Entity is ICustomIdentity)
                                (entry.Entity as IEntity).SetID((object)this.Database.SqlQuery<int>(string.Format("select {0}.NEXTVAL from dual", (object)(entry.Entity as ICustomIdentity).IdentityGeneratorSequenceName)).FirstOrDefault<int>());
                            if (entry.Entity is IActivityLoggable)
                            {
                                IActivityLoggable entity = (IActivityLoggable)entry.Entity;
                                if (entity.ActionsToLog == ActionLog.All)
                                    objectList.Add(entry.Entity);
                                else if (entry.State == System.Data.Entity.EntityState.Added && (entity.ActionsToLog & ActionLog.Insert) == ActionLog.Insert)
                                    objectList.Add(entry.Entity);
                                else if (entry.State == System.Data.Entity.EntityState.Modified && (entity.ActionsToLog & ActionLog.Update) == ActionLog.Update)
                                    objectList.Add(entry.Entity);
                                else if (entry.State == System.Data.Entity.EntityState.Deleted && (entity.ActionsToLog & ActionLog.Delete) == ActionLog.Delete)
                                    objectList.Add(entry.Entity);
                            }
                            if (entry.Entity is ILoggableEntityID)
                            {
                                if (entry.State == System.Data.Entity.EntityState.Modified)
                                {
                                    ILoggableEntityID loggableEntityId = entry.GetDatabaseValues().ToObject() as ILoggableEntityID;
                                    if (loggableEntityId != null)
                                    {
                                        ILoggableEntityID entity = (ILoggableEntityID)entry.Entity;
                                        entity.LogData = new EntityIDLogData();
                                        entity.LogData.InsertUserID = loggableEntityId.LogData.InsertUserID;
                                        entity.LogData.InsertDateTime = loggableEntityId.LogData.InsertDateTime;
                                        entity.LogData.UpdateUserID = new int?(SecurityManager.CurrentUserContext == null ? -1 : SecurityManager.CurrentUserContext.UserId);
                                        entity.LogData.UpdateDateTime = new DateTime?(DateTime.Now);
                                    }
                                }
                                else if (entry.State == System.Data.Entity.EntityState.Added)
                                {
                                    ILoggableEntityID entity = (ILoggableEntityID)entry.Entity;
                                    entity.LogData = new EntityIDLogData();
                                    entity.LogData.InsertUserID = new int?(SecurityManager.CurrentUserContext == null ? -1 : SecurityManager.CurrentUserContext.UserId);
                                    entity.LogData.InsertDateTime = new DateTime?(DateTime.Now);
                                    entity.LogData.UpdateUserID = new int?();
                                    entity.LogData.UpdateDateTime = new DateTime?();
                                }
                            }
                            else if (entry.Entity is ILoggableEntityName)
                            {
                                if (entry.State == System.Data.Entity.EntityState.Modified)
                                {
                                    ILoggableEntityName loggableEntityName = entry.GetDatabaseValues().ToObject() as ILoggableEntityName;
                                    if (loggableEntityName != null)
                                    {
                                        ILoggableEntityName entity = (ILoggableEntityName)entry.Entity;
                                        entity.LogData = new EntityNameLogData();
                                        entity.LogData.InsertUserName = loggableEntityName.LogData.InsertUserName;
                                        entity.LogData.InsertDateTime = loggableEntityName.LogData.InsertDateTime;
                                        entity.LogData.UpdateUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                        entity.LogData.UpdateDateTime = new DateTime?(DateTime.Now);
                                    }
                                }
                                else if (entry.State == System.Data.Entity.EntityState.Added)
                                {
                                    ILoggableEntityName entity = (ILoggableEntityName)entry.Entity;
                                    entity.LogData = new EntityNameLogData();
                                    entity.LogData.InsertUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                    entity.LogData.InsertDateTime = DateTime.Now;
                                    entity.LogData.UpdateUserName = (string)null;
                                    entity.LogData.UpdateDateTime = new DateTime?();
                                }
                            }
                            else if (entry.Entity is ILoggableEntityNameAndID)
                            {
                                if (entry.State == System.Data.Entity.EntityState.Modified)
                                {
                                    ILoggableEntityNameAndID loggableEntityNameAndId = entry.GetDatabaseValues().ToObject() as ILoggableEntityNameAndID;
                                    if (loggableEntityNameAndId != null)
                                    {
                                        ILoggableEntityNameAndID entity = (ILoggableEntityNameAndID)entry.Entity;
                                        entity.LogData = new EntityNameAndIDLogData();
                                        entity.LogData.InsertUserId = loggableEntityNameAndId.LogData.InsertUserId;
                                        entity.LogData.InsertUserName = loggableEntityNameAndId.LogData.InsertUserName;
                                        entity.LogData.InsertDateTime = loggableEntityNameAndId.LogData.InsertDateTime;
                                        entity.LogData.UpdateUserId = new int?(SecurityManager.CurrentUserContext == null ? -1 : SecurityManager.CurrentUserContext.UserId);
                                        entity.LogData.UpdateUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                        entity.LogData.UpdateDateTime = new DateTime?(DateTime.Now);
                                    }
                                }
                                else if (entry.State == System.Data.Entity.EntityState.Added)
                                {
                                    ILoggableEntityNameAndID entity = (ILoggableEntityNameAndID)entry.Entity;
                                    entity.LogData = new EntityNameAndIDLogData();
                                    entity.LogData.InsertUserId = SecurityManager.CurrentUserContext == null ? -1 : SecurityManager.CurrentUserContext.UserId;
                                    entity.LogData.InsertUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                    entity.LogData.InsertDateTime = DateTime.Now;
                                    entity.LogData.UpdateUserId = new int?();
                                    entity.LogData.UpdateUserName = (string)null;
                                    entity.LogData.UpdateDateTime = new DateTime?();
                                }
                            }
                            else if (entry.Entity is ILoggableMCIEntity)
                            {
                                if (entry.State == System.Data.Entity.EntityState.Modified)
                                {
                                    ILoggableMCIEntity loggableMciEntity = entry.GetDatabaseValues().ToObject() as ILoggableMCIEntity;
                                    if (loggableMciEntity != null)
                                    {
                                        ILoggableMCIEntity entity = (ILoggableMCIEntity)entry.Entity;
                                        entity.LogData = new MCIEntityLogData();
                                        entity.LogData.InsertUserName = loggableMciEntity.LogData.InsertUserName;
                                        entity.LogData.InsertDate = loggableMciEntity.LogData.InsertDate;
                                        entity.LogData.InsertTime = loggableMciEntity.LogData.InsertTime;
                                        entity.LogData.UpdateUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                        entity.LogData.UpdateDate = DateTime.Now.ToPersianDateTime().ToDateString();
                                        entity.LogData.UpdateTime = DateTime.Now.ToPersianDateTime().ToTimeString();
                                    }
                                }
                                else if (entry.State == System.Data.Entity.EntityState.Added)
                                {
                                    ILoggableMCIEntity entity = (ILoggableMCIEntity)entry.Entity;
                                    entity.LogData = new MCIEntityLogData();
                                    entity.LogData.InsertUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                    entity.LogData.InsertDate = DateTime.Now.ToPersianDateTime().ToDateString();
                                    entity.LogData.InsertTime = DateTime.Now.ToPersianDateTime().ToTimeString();
                                    entity.LogData.UpdateUserName = (string)null;
                                    entity.LogData.UpdateDate = (string)null;
                                    entity.LogData.UpdateTime = (string)null;
                                }
                            }
                            else if (entry.Entity is ILoggableMCIEntity2)
                            {
                                if (entry.State == System.Data.Entity.EntityState.Modified)
                                {
                                    ILoggableMCIEntity2 loggableMciEntity2 = entry.GetDatabaseValues().ToObject() as ILoggableMCIEntity2;
                                    if (loggableMciEntity2 != null)
                                    {
                                        ILoggableMCIEntity2 entity = (ILoggableMCIEntity2)entry.Entity;
                                        entity.LogData = new MCIEntityLogData2();
                                        entity.LogData.InsertUserName = loggableMciEntity2.LogData.InsertUserName;
                                        entity.LogData.InsertDate = loggableMciEntity2.LogData.InsertDate;
                                        entity.LogData.InsertTime = loggableMciEntity2.LogData.InsertTime;
                                        entity.LogData.UpdateUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                        entity.LogData.UpdateDate = DateTime.Now.ToPersianDateTime().ToDateString();
                                        entity.LogData.UpdateTime = DateTime.Now.ToPersianDateTime().ToTimeString();
                                    }
                                }
                                else if (entry.State == System.Data.Entity.EntityState.Added)
                                {
                                    ILoggableMCIEntity2 entity = (ILoggableMCIEntity2)entry.Entity;
                                    entity.LogData = new MCIEntityLogData2();
                                    entity.LogData.InsertUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                    entity.LogData.InsertDate = DateTime.Now.ToPersianDateTime().ToDateString();
                                    entity.LogData.InsertTime = DateTime.Now.ToPersianDateTime().ToTimeString();
                                    entity.LogData.UpdateUserName = (string)null;
                                    entity.LogData.UpdateDate = (string)null;
                                    entity.LogData.UpdateTime = (string)null;
                                }
                            }
                            else if (entry.Entity is ILoggableMCIEntityWithIP)
                            {
                                if (entry.State == System.Data.Entity.EntityState.Modified)
                                {
                                    ILoggableMCIEntityWithIP loggableMciEntityWithIp = entry.GetDatabaseValues().ToObject() as ILoggableMCIEntityWithIP;
                                    if (loggableMciEntityWithIp != null)
                                    {
                                        ILoggableMCIEntityWithIP entity = (ILoggableMCIEntityWithIP)entry.Entity;
                                        entity.LogData = new MCIEntityWithIPLogData();
                                        entity.LogData.InsertUserName = loggableMciEntityWithIp.LogData.InsertUserName;
                                        entity.LogData.InsertDate = loggableMciEntityWithIp.LogData.InsertDate;
                                        entity.LogData.InsertTime = loggableMciEntityWithIp.LogData.InsertTime;
                                        entity.LogData.InsertUserIP = loggableMciEntityWithIp.LogData.InsertUserIP;
                                        entity.LogData.UpdateUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                        entity.LogData.UpdateDate = DateTime.Now.ToPersianDateTime().ToDateString();
                                        entity.LogData.UpdateTime = DateTime.Now.ToPersianDateTime().ToTimeString();
                                        entity.LogData.UpdateUserIP = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.ClientIP;
                                    }
                                }
                                else if (entry.State == System.Data.Entity.EntityState.Added)
                                {
                                    ILoggableMCIEntityWithIP entity = (ILoggableMCIEntityWithIP)entry.Entity;
                                    entity.LogData = new MCIEntityWithIPLogData();
                                    entity.LogData.InsertUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                    entity.LogData.InsertDate = DateTime.Now.ToPersianDateTime().ToDateString();
                                    entity.LogData.InsertTime = DateTime.Now.ToPersianDateTime().ToTimeString();
                                    entity.LogData.InsertUserIP = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.ClientIP;
                                    entity.LogData.UpdateUserName = (string)null;
                                    entity.LogData.UpdateDate = (string)null;
                                    entity.LogData.UpdateTime = (string)null;
                                    entity.LogData.UpdateUserIP = (string)null;
                                }
                            }
                            if (entry.Entity is ICacheable)
                                CacheManager.Remove(entry.Entity.GetType().FullName);
                            if (entry.Entity is IConcurrencySupportable && (entry.State == System.Data.Entity.EntityState.Modified || entry.State == System.Data.Entity.EntityState.Added))
                                entry.Property("RowVersion").CurrentValue = (object)Guid.NewGuid().ToString().Replace("-", "");
                        }
                    }
                    num = base.SaveChanges();
                    List<OMF.Common.ActivityLog.ActivityLog> activityLogList = new List<OMF.Common.ActivityLog.ActivityLog>();
                    foreach (object obj in objectList)
                    {
                        OMF.Common.ActivityLog.ActivityLog activityLog = ActivityLogManager.CreateActivityLog(obj as IActivityLoggable);
                        activityLogList.Add(activityLog);
                    }
                    this.SyncObjectsStatePostCommit();
                    ActivityLogManager.Save((IEnumerable<OMF.Common.ActivityLog.ActivityLog>)activityLogList);
                }
                return num;
            }
            catch (DbEntityValidationException ex)
            {
                throw new DataAccessException(string.Join(Constants.NewLine, ex.EntityValidationErrors.SelectMany<DbEntityValidationResult, string>((Func<DbEntityValidationResult, IEnumerable<string>>)(failure => failure.ValidationErrors.Select<DbValidationError, string>((Func<DbValidationError, string>)(error => string.Format("{0}: {1}", (object)error.PropertyName, (object)error.ErrorMessage)))))), (Exception)ex);
            }
        }

        public override async Task<int> SaveChangesAsync()
        {
            int num = await this.SaveChangesAsync(CancellationToken.None);
            return num;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            int num;
            try
            {
                int changes = 0;
                this.SyncObjectsStatePreCommit();
                if (this.ChangeTracker.HasChanges())
                {
                    List<object> logList = new List<object>();
                    List<IWorkflowStarterEntity> list = new List<IWorkflowStarterEntity>();
                    foreach (DbEntityEntry entry in this.ChangeTracker.Entries())
                    {
                        DbEntityEntry dbEntry = entry;
                        if (dbEntry.State != System.Data.Entity.EntityState.Detached && dbEntry.State != System.Data.Entity.EntityState.Unchanged)
                        {
                            if (ConfigurationController.CustomIdentityEnabled && dbEntry.State == System.Data.Entity.EntityState.Added && dbEntry.Entity is IEntity && dbEntry.Entity is ICustomIdentity)
                            {
                                IEntity entity = dbEntry.Entity as IEntity;
                                string sequenceName = (dbEntry.Entity as ICustomIdentity).IdentityGeneratorSequenceName;
                                int identity = this.Database.SqlQuery<int>(string.Format("select {0}.NEXTVAL from dual", (object)sequenceName)).FirstOrDefault<int>();
                                entity.SetID((object)identity);
                                entity = (IEntity)null;
                                sequenceName = (string)null;
                            }
                            if (dbEntry.Entity is IActivityLoggable)
                            {
                                IActivityLoggable entityForLog = (IActivityLoggable)dbEntry.Entity;
                                if (entityForLog.ActionsToLog == ActionLog.All)
                                    logList.Add(dbEntry.Entity);
                                else if (dbEntry.State == System.Data.Entity.EntityState.Added && (entityForLog.ActionsToLog & ActionLog.Insert) == ActionLog.Insert)
                                    logList.Add(dbEntry.Entity);
                                else if (dbEntry.State == System.Data.Entity.EntityState.Modified && (entityForLog.ActionsToLog & ActionLog.Update) == ActionLog.Update)
                                    logList.Add(dbEntry.Entity);
                                else if (dbEntry.State == System.Data.Entity.EntityState.Deleted && (entityForLog.ActionsToLog & ActionLog.Delete) == ActionLog.Delete)
                                    logList.Add(dbEntry.Entity);
                                entityForLog = (IActivityLoggable)null;
                            }
                            if (dbEntry.Entity is ILoggableEntityID)
                            {
                                if (dbEntry.State == System.Data.Entity.EntityState.Modified)
                                {
                                    DbPropertyValues dbValues = await dbEntry.GetDatabaseValuesAsync(cancellationToken);
                                    ILoggableEntityID dbEntity = dbValues.ToObject() as ILoggableEntityID;
                                    if (dbEntity != null)
                                    {
                                        ILoggableEntityID loggableEntity = (ILoggableEntityID)dbEntry.Entity;
                                        loggableEntity.LogData = new EntityIDLogData();
                                        loggableEntity.LogData.InsertUserID = dbEntity.LogData.InsertUserID;
                                        loggableEntity.LogData.InsertDateTime = dbEntity.LogData.InsertDateTime;
                                        loggableEntity.LogData.UpdateUserID = new int?(SecurityManager.CurrentUserContext == null ? -1 : SecurityManager.CurrentUserContext.UserId);
                                        loggableEntity.LogData.UpdateDateTime = new DateTime?(DateTime.Now);
                                        loggableEntity = (ILoggableEntityID)null;
                                    }
                                    dbValues = (DbPropertyValues)null;
                                    dbEntity = (ILoggableEntityID)null;
                                }
                                else if (dbEntry.State == System.Data.Entity.EntityState.Added)
                                {
                                    ILoggableEntityID loggableEntity = (ILoggableEntityID)dbEntry.Entity;
                                    loggableEntity.LogData = new EntityIDLogData();
                                    loggableEntity.LogData.InsertUserID = new int?(SecurityManager.CurrentUserContext == null ? -1 : SecurityManager.CurrentUserContext.UserId);
                                    loggableEntity.LogData.InsertDateTime = new DateTime?(DateTime.Now);
                                    loggableEntity.LogData.UpdateUserID = new int?();
                                    loggableEntity.LogData.UpdateDateTime = new DateTime?();
                                    loggableEntity = (ILoggableEntityID)null;
                                }
                            }
                            else if (dbEntry.Entity is ILoggableEntityName)
                            {
                                if (dbEntry.State == System.Data.Entity.EntityState.Modified)
                                {
                                    DbPropertyValues dbValues = await dbEntry.GetDatabaseValuesAsync();
                                    ILoggableEntityName dbEntity = dbValues.ToObject() as ILoggableEntityName;
                                    if (dbEntity != null)
                                    {
                                        ILoggableEntityName loggableEntity = (ILoggableEntityName)dbEntry.Entity;
                                        loggableEntity.LogData = new EntityNameLogData();
                                        loggableEntity.LogData.InsertUserName = dbEntity.LogData.InsertUserName;
                                        loggableEntity.LogData.InsertDateTime = dbEntity.LogData.InsertDateTime;
                                        loggableEntity.LogData.UpdateUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                        loggableEntity.LogData.UpdateDateTime = new DateTime?(DateTime.Now);
                                        loggableEntity = (ILoggableEntityName)null;
                                    }
                                    dbValues = (DbPropertyValues)null;
                                    dbEntity = (ILoggableEntityName)null;
                                }
                                else if (dbEntry.State == System.Data.Entity.EntityState.Added)
                                {
                                    ILoggableEntityName loggableEntity = (ILoggableEntityName)dbEntry.Entity;
                                    loggableEntity.LogData = new EntityNameLogData();
                                    loggableEntity.LogData.InsertUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                    loggableEntity.LogData.InsertDateTime = DateTime.Now;
                                    loggableEntity.LogData.UpdateUserName = (string)null;
                                    loggableEntity.LogData.UpdateDateTime = new DateTime?();
                                    loggableEntity = (ILoggableEntityName)null;
                                }
                            }
                            else if (dbEntry.Entity is ILoggableEntityNameAndID)
                            {
                                if (dbEntry.State == System.Data.Entity.EntityState.Modified)
                                {
                                    DbPropertyValues dbValues = await dbEntry.GetDatabaseValuesAsync();
                                    ILoggableEntityNameAndID dbEntity = dbValues.ToObject() as ILoggableEntityNameAndID;
                                    if (dbEntity != null)
                                    {
                                        ILoggableEntityNameAndID loggableEntity = (ILoggableEntityNameAndID)dbEntry.Entity;
                                        loggableEntity.LogData = new EntityNameAndIDLogData();
                                        loggableEntity.LogData.InsertUserId = dbEntity.LogData.InsertUserId;
                                        loggableEntity.LogData.InsertUserName = dbEntity.LogData.InsertUserName;
                                        loggableEntity.LogData.InsertDateTime = dbEntity.LogData.InsertDateTime;
                                        loggableEntity.LogData.UpdateUserId = new int?(SecurityManager.CurrentUserContext == null ? -1 : SecurityManager.CurrentUserContext.UserId);
                                        loggableEntity.LogData.UpdateUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                        loggableEntity.LogData.UpdateDateTime = new DateTime?(DateTime.Now);
                                        loggableEntity = (ILoggableEntityNameAndID)null;
                                    }
                                    dbValues = (DbPropertyValues)null;
                                    dbEntity = (ILoggableEntityNameAndID)null;
                                }
                                else if (dbEntry.State == System.Data.Entity.EntityState.Added)
                                {
                                    ILoggableEntityNameAndID loggableEntity = (ILoggableEntityNameAndID)dbEntry.Entity;
                                    loggableEntity.LogData = new EntityNameAndIDLogData();
                                    loggableEntity.LogData.InsertUserId = SecurityManager.CurrentUserContext == null ? -1 : SecurityManager.CurrentUserContext.UserId;
                                    loggableEntity.LogData.InsertUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                    loggableEntity.LogData.InsertDateTime = DateTime.Now;
                                    loggableEntity.LogData.UpdateUserId = new int?();
                                    loggableEntity.LogData.UpdateUserName = (string)null;
                                    loggableEntity.LogData.UpdateDateTime = new DateTime?();
                                    loggableEntity = (ILoggableEntityNameAndID)null;
                                }
                            }
                            else if (dbEntry.Entity is ILoggableMCIEntity)
                            {
                                if (dbEntry.State == System.Data.Entity.EntityState.Modified)
                                {
                                    DbPropertyValues dbValues = await dbEntry.GetDatabaseValuesAsync();
                                    ILoggableMCIEntity dbEntity = dbValues.ToObject() as ILoggableMCIEntity;
                                    if (dbEntity != null)
                                    {
                                        ILoggableMCIEntity loggableEntity = (ILoggableMCIEntity)dbEntry.Entity;
                                        loggableEntity.LogData = new MCIEntityLogData();
                                        loggableEntity.LogData.InsertUserName = dbEntity.LogData.InsertUserName;
                                        loggableEntity.LogData.InsertDate = dbEntity.LogData.InsertDate;
                                        loggableEntity.LogData.InsertTime = dbEntity.LogData.InsertTime;
                                        loggableEntity.LogData.UpdateUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                        loggableEntity.LogData.UpdateDate = DateTime.Now.ToPersianDateTime().ToDateString();
                                        loggableEntity.LogData.UpdateTime = DateTime.Now.ToPersianDateTime().ToTimeString();
                                        loggableEntity = (ILoggableMCIEntity)null;
                                    }
                                    dbValues = (DbPropertyValues)null;
                                    dbEntity = (ILoggableMCIEntity)null;
                                }
                                else if (dbEntry.State == System.Data.Entity.EntityState.Added)
                                {
                                    ILoggableMCIEntity loggableEntity = (ILoggableMCIEntity)dbEntry.Entity;
                                    loggableEntity.LogData = new MCIEntityLogData();
                                    loggableEntity.LogData.InsertUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                    loggableEntity.LogData.InsertDate = DateTime.Now.ToPersianDateTime().ToDateString();
                                    loggableEntity.LogData.InsertTime = DateTime.Now.ToPersianDateTime().ToTimeString();
                                    loggableEntity.LogData.UpdateUserName = (string)null;
                                    loggableEntity.LogData.UpdateDate = (string)null;
                                    loggableEntity.LogData.UpdateTime = (string)null;
                                    loggableEntity = (ILoggableMCIEntity)null;
                                }
                            }
                            else if (dbEntry.Entity is ILoggableMCIEntity2)
                            {
                                if (dbEntry.State == System.Data.Entity.EntityState.Modified)
                                {
                                    DbPropertyValues dbValues = await dbEntry.GetDatabaseValuesAsync();
                                    ILoggableMCIEntity2 dbEntity = dbValues.ToObject() as ILoggableMCIEntity2;
                                    if (dbEntity != null)
                                    {
                                        ILoggableMCIEntity2 loggableEntity = (ILoggableMCIEntity2)dbEntry.Entity;
                                        loggableEntity.LogData = new MCIEntityLogData2();
                                        loggableEntity.LogData.InsertUserName = dbEntity.LogData.InsertUserName;
                                        loggableEntity.LogData.InsertDate = dbEntity.LogData.InsertDate;
                                        loggableEntity.LogData.InsertTime = dbEntity.LogData.InsertTime;
                                        loggableEntity.LogData.UpdateUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                        loggableEntity.LogData.UpdateDate = DateTime.Now.ToPersianDateTime().ToDateString();
                                        loggableEntity.LogData.UpdateTime = DateTime.Now.ToPersianDateTime().ToTimeString();
                                        loggableEntity = (ILoggableMCIEntity2)null;
                                    }
                                    dbValues = (DbPropertyValues)null;
                                    dbEntity = (ILoggableMCIEntity2)null;
                                }
                                else if (dbEntry.State == System.Data.Entity.EntityState.Added)
                                {
                                    ILoggableMCIEntity2 loggableEntity = (ILoggableMCIEntity2)dbEntry.Entity;
                                    loggableEntity.LogData = new MCIEntityLogData2();
                                    loggableEntity.LogData.InsertUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                    loggableEntity.LogData.InsertDate = DateTime.Now.ToPersianDateTime().ToDateString();
                                    loggableEntity.LogData.InsertTime = DateTime.Now.ToPersianDateTime().ToTimeString();
                                    loggableEntity.LogData.UpdateUserName = (string)null;
                                    loggableEntity.LogData.UpdateDate = (string)null;
                                    loggableEntity.LogData.UpdateTime = (string)null;
                                    loggableEntity = (ILoggableMCIEntity2)null;
                                }
                            }
                            else if (dbEntry.Entity is ILoggableMCIEntityWithIP)
                            {
                                if (dbEntry.State == System.Data.Entity.EntityState.Modified)
                                {
                                    DbPropertyValues dbValues = await dbEntry.GetDatabaseValuesAsync();
                                    ILoggableMCIEntityWithIP dbEntity = dbValues.ToObject() as ILoggableMCIEntityWithIP;
                                    if (dbEntity != null)
                                    {
                                        ILoggableMCIEntityWithIP loggableEntity = (ILoggableMCIEntityWithIP)dbEntry.Entity;
                                        loggableEntity.LogData = new MCIEntityWithIPLogData();
                                        loggableEntity.LogData.InsertUserName = dbEntity.LogData.InsertUserName;
                                        loggableEntity.LogData.InsertDate = dbEntity.LogData.InsertDate;
                                        loggableEntity.LogData.InsertTime = dbEntity.LogData.InsertTime;
                                        loggableEntity.LogData.InsertUserIP = dbEntity.LogData.InsertUserIP;
                                        loggableEntity.LogData.UpdateUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                        loggableEntity.LogData.UpdateDate = DateTime.Now.ToPersianDateTime().ToDateString();
                                        loggableEntity.LogData.UpdateTime = DateTime.Now.ToPersianDateTime().ToTimeString();
                                        loggableEntity.LogData.UpdateUserIP = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.ClientIP;
                                        loggableEntity = (ILoggableMCIEntityWithIP)null;
                                    }
                                    dbValues = (DbPropertyValues)null;
                                    dbEntity = (ILoggableMCIEntityWithIP)null;
                                }
                                else if (dbEntry.State == System.Data.Entity.EntityState.Added)
                                {
                                    ILoggableMCIEntityWithIP loggableEntity = (ILoggableMCIEntityWithIP)dbEntry.Entity;
                                    loggableEntity.LogData = new MCIEntityWithIPLogData();
                                    loggableEntity.LogData.InsertUserName = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.UserName;
                                    loggableEntity.LogData.InsertDate = DateTime.Now.ToPersianDateTime().ToDateString();
                                    loggableEntity.LogData.InsertTime = DateTime.Now.ToPersianDateTime().ToTimeString();
                                    loggableEntity.LogData.InsertUserIP = SecurityManager.CurrentUserContext == null ? "-1" : SecurityManager.CurrentUserContext.ClientIP;
                                    loggableEntity.LogData.UpdateUserName = (string)null;
                                    loggableEntity.LogData.UpdateDate = (string)null;
                                    loggableEntity.LogData.UpdateTime = (string)null;
                                    loggableEntity.LogData.UpdateUserIP = (string)null;
                                    loggableEntity = (ILoggableMCIEntityWithIP)null;
                                }
                            }
                            if (dbEntry.Entity is ICacheable)
                                CacheManager.Remove(dbEntry.Entity.GetType().FullName);
                            if (dbEntry.Entity is IConcurrencySupportable && (dbEntry.State == System.Data.Entity.EntityState.Modified || dbEntry.State == System.Data.Entity.EntityState.Added))
                                dbEntry.Property("RowVersion").CurrentValue = (object)Guid.NewGuid().ToString().Replace("-", "");
                            dbEntry = (DbEntityEntry)null;
                        }
                    }
                    changes = await base.SaveChangesAsync(cancellationToken);
                    List<OMF.Common.ActivityLog.ActivityLog> activityLogList = new List<OMF.Common.ActivityLog.ActivityLog>();
                    foreach (object obj in logList)
                    {
                        object entity = obj;
                        OMF.Common.ActivityLog.ActivityLog activityLog = ActivityLogManager.CreateActivityLog(entity as IActivityLoggable);
                        activityLogList.Add(activityLog);
                        activityLog = (OMF.Common.ActivityLog.ActivityLog)null;
                        entity = (object)null;
                    }
                    this.SyncObjectsStatePostCommit();
                    await ActivityLogManager.SaveAsync((IEnumerable<OMF.Common.ActivityLog.ActivityLog>)activityLogList);
                    logList = (List<object>)null;
                    list = (List<IWorkflowStarterEntity>)null;
                    activityLogList = (List<OMF.Common.ActivityLog.ActivityLog>)null;
                }
                num = changes;
            }
            catch (DbEntityValidationException ex)
            {
                string jointFailures = string.Join(Constants.NewLine, ex.EntityValidationErrors.SelectMany<DbEntityValidationResult, string>((Func<DbEntityValidationResult, IEnumerable<string>>)(failure => failure.ValidationErrors.Select<DbValidationError, string>((Func<DbValidationError, string>)(error => string.Format("{0}: {1}", (object)error.PropertyName, (object)error.ErrorMessage))))));
                throw new DataAccessException(jointFailures, (Exception)ex);
            }
            return num;
        }

        public void SyncObjectState<TEntity>(TEntity entity) where TEntity : class, IObjectState
        {
            this.Entry<TEntity>(entity).State = StateHelper.ConvertState(entity.ObjectState);
        }

        private void SyncObjectsStatePreCommit()
        {
            foreach (DbEntityEntry entry in this.ChangeTracker.Entries())
                entry.State = StateHelper.ConvertState(((IObjectState)entry.Entity).ObjectState);
        }

        public void SyncObjectsStatePostCommit()
        {
            foreach (DbEntityEntry entry in this.ChangeTracker.Entries())
                ((IObjectState)entry.Entity).ObjectState = StateHelper.ConvertState(entry.State);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing && (this.Database.Connection != null && this.Database.Connection.State == ConnectionState.Open))
                    this.Database.Connection.Close();
                this._disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
