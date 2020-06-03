using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.Security;
using OMF.Common.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;

namespace OMF.EntityFramework.Ef6
{
    public class BaseEntityTypeConfig<TEntity> : BaseViewEntityTypeConfig<TEntity>
      where TEntity : class, IEntity
    {
        public BaseEntityTypeConfig()
        {
            this.Property(e => e.ID).IsRequired();
            this.HasKey(e => e.ID);
            if (!ConfigurationController.CustomIdentityEnabled || !((IEnumerable<Type>)typeof(TEntity).GetInterfaces()).Any<Type>((Func<Type, bool>)(t => t.FullName == typeof(ICustomIdentity).FullName)))
                return;
            this.Property<int>((Expression<Func<TEntity, int>>)(e => e.ID)).HasDatabaseGeneratedOption(new DatabaseGeneratedOption?(DatabaseGeneratedOption.None));
        }

        public void MapUpdateKey(string updateKey)
        {
            OperationKeysManager.GetOperationKeys<TEntity>().UpdateKey = updateKey;
        }

        public void MapDeleteKey(string deleteKey)
        {
            OperationKeysManager.GetOperationKeys<TEntity>().DeleteKey = deleteKey;
        }

        public void MapInsertKey(string insertKey)
        {
            OperationKeysManager.GetOperationKeys<TEntity>().InsertKey = insertKey;
        }

        public void MapImportKey(string importKey)
        {
            OperationKeysManager.GetOperationKeys<TEntity>().ImportKey = importKey;
        }

        public void MapEntityValidator(EntityValidator<TEntity> entityValidator)
        {
            ValidationManager.SetEntityValidator<TEntity>(entityValidator);
        }
    }
}
