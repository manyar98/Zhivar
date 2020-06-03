using OMF.Common;
using OMF.Common.Security;
using System;
using System.Data.Entity.ModelConfiguration;
using System.Linq.Expressions;

namespace OMF.EntityFramework.Ef6
{
    public class BaseViewEntityTypeConfig<TEntity> : EntityTypeConfiguration<TEntity>
      where TEntity : class, IViewEntity
    {
        public void MapViewKey(string viewKey)
        {
            OperationKeysManager.GetOperationKeys<TEntity>().ViewKey = viewKey;
        }

        public void MapExportKey(string exportKey)
        {
            OperationKeysManager.GetOperationKeys<TEntity>().ExportKey = exportKey;
        }

        public void MapPrintKey(string printKey)
        {
            OperationKeysManager.GetOperationKeys<TEntity>().PrintKey = printKey;
        }

        public void SetPersianTitle(string persianTitle)
        {
            EntityConfigManager.SetPersianTitle<TEntity>(persianTitle);
        }

        public void SetPropertyPersianTitle(
          Expression<Func<TEntity, object>> propertySelector,
          string persianTitle)
        {
            EntityConfigManager.SetPropertyPersianTitle<TEntity>(propertySelector, persianTitle);
        }
    }
}
