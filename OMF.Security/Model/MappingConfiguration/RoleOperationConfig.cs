using OMF.Common.Configuration;
using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model.Validators;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace OMF.Security.Model.MappingConfiguration
{
    public class RoleOperationConfig : ActivityLoggableEntityTypeConfig<RoleOperation>
    {
        public RoleOperationConfig()
        {
            this.ToTable("TBL_ROLE_OPERATION");//, ConfigurationController.SecurityDbSchema);
            if (ConfigurationController.CustomIdentityEnabled)
                this.Property<int>((Expression<Func<RoleOperation, int>>)(roleOpr => roleOpr.ID)).HasDatabaseGeneratedOption(new DatabaseGeneratedOption?(DatabaseGeneratedOption.None));
            this.Property<int>((Expression<Func<RoleOperation, int>>)(roleOpr => roleOpr.IsPermision)).HasColumnName("ISPERMISION");
            this.Property<int>((Expression<Func<RoleOperation, int>>)(roleOpr => roleOpr.OperationId)).HasColumnName("OPERATIONID");
            this.Property<int>((Expression<Func<RoleOperation, int>>)(roleOpr => roleOpr.RoleId)).HasColumnName("ROLEID");
            this.MapEntityValidator((EntityValidator<RoleOperation>)new RoleOperationValidator());
            this.MapDeleteKey("Security-RoleOperation-Save");
            this.MapUpdateKey("Security-RoleOperation-Save");
            this.MapInsertKey("Security-RoleOperation-Save");
            this.MapViewKey("Security-RoleOperation-View");
        }
    }
}
