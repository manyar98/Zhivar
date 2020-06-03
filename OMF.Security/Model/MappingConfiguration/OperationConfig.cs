using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model.Validators;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using static OMF.Common.Enums;

namespace OMF.Security.Model.MappingConfiguration
{
    public class OperationConfig : ActivityLoggableEntityTypeConfig<Operation>
    {
        public OperationConfig()
        {
            //this.ToTable("TBL_OPERATION", ConfigurationController.SecurityDbSchema);
            //if (ConfigurationController.CustomIdentityEnabled)
                this.ToTable("TBL_OPERATION");
           // if (ConfigurationController.CustomIdentityEnabled)
//this.Property(opr => opr.ID)).HasDatabaseGeneratedOption(new DatabaseGeneratedOption?(DatabaseGeneratedOption.None));
            this.Property(opr => opr.ApplicationId).HasColumnName("APPID");
            this.Property(opr => opr.Code).HasColumnName("CODE");
            this.Property(opr => opr.IsActive).HasColumnName("ISACTIVE");
            this.Property(opr => opr.IsDeleted).HasColumnName("ISDELETED");
            this.Property(opr => opr.IsSystem).HasColumnName("ISSYSTEM");
            this.Property(opr => opr.Name).HasColumnName("NAME");
            this.Property(opr => opr.OperationType).HasColumnName("OPERATIONTYPE");
            this.Property(opr => opr.OrderNo).HasColumnName("ORDERNO");
            this.Property(opr => opr.ParentId).HasColumnName("PARENTID");
            this.Property(opr => opr.Tag1).HasColumnName("TAG1").HasMaxLength(100);
            this.Property(opr => opr.Tag2).HasColumnName("TAG2").HasMaxLength(100);
            this.Property(opr => opr.Tag3).HasColumnName("TAG3").HasMaxLength(100);
            this.Property(opr => opr.Tag4).HasColumnName("TAG4").HasMaxLength(100);
            this.Property(opr => opr.Tag5).HasColumnName("TAG5").HasMaxLength(100);
            this.Property(opr => opr.TagInt1).HasColumnName("TAGINT1");
            this.Property(opr => opr.TagInt2).HasColumnName("TAGINT2");
            this.Property(opr => opr.TagInt3).HasColumnName("TAGINT3");
            // this.MapEntityValidator((EntityValidator<Operation>)new OperationValidator());
            this.MapDeleteKey("Security-Operation-Delete");
            this.MapUpdateKey("Security-Operation-Update");
            this.MapInsertKey("Security-Operation-Insert");
            this.MapViewKey("Security-Operation-View");
        }
    }
}
