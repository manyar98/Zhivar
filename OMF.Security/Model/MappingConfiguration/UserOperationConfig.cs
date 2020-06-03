using OMF.Common.Configuration;
using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model.Validators;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace OMF.Security.Model.MappingConfiguration
{
    public class UserOperationConfig : ActivityLoggableEntityTypeConfig<UserOperation>
    {
        public UserOperationConfig()
        {
            //this.ToTable("TBL_USER_OPERATION", ConfigurationController.SecurityDbSchema);
            this.ToTable("TBL_USER_OPERATION");
           // if (ConfigurationController.CustomIdentityEnabled)
                this.Property<int>((Expression<Func<UserOperation, int>>)(userOpr => userOpr.ID)).HasDatabaseGeneratedOption(new DatabaseGeneratedOption?(DatabaseGeneratedOption.None));
            this.Property<int>((Expression<Func<UserOperation, int>>)(userOpr => userOpr.IsPermision)).HasColumnName("ISPERMISION");
            this.Property<int>((Expression<Func<UserOperation, int>>)(userOpr => userOpr.OperationId)).HasColumnName("OPERATIONID");
            this.Property<int>((Expression<Func<UserOperation, int>>)(userOpr => userOpr.UserId)).HasColumnName("USERID");
            //this.MapEntityValidator((EntityValidator<UserOperation>)new UserOperationValidator());
            this.MapDeleteKey("Security-UserOperation-Save");
            this.MapUpdateKey("Security-UserOperation-Save");
            this.MapInsertKey("Security-UserOperation-Save");
            this.MapViewKey("Security-UserOperation-View");
        }
    }
}
