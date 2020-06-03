using OMF.Common.Configuration;
using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model.Validators;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace OMF.Security.Model.MappingConfiguration
{
    public class UserRoleConfig : ActivityLoggableEntityTypeConfig<UserRole>
    {
        public UserRoleConfig()
        {
            this.ToTable("TBL_USER_ROLE");//, ConfigurationController.SecurityDbSchema);
            //if (ConfigurationController.CustomIdentityEnabled)
            //    this.Property<int>((Expression<Func<UserRole, int>>)(userRole => userRole.ID)).HasDatabaseGeneratedOption(new DatabaseGeneratedOption?(DatabaseGeneratedOption.None));
            this.Property<int>((Expression<Func<UserRole, int>>)(userRole => userRole.RoleId)).HasColumnName("ROLEID");
            this.Property<int>((Expression<Func<UserRole, int>>)(userRole => userRole.UserId)).HasColumnName("USERID");


            //this.MapEntityValidator((EntityValidator<UserRole>)new UserRoleValidator());
            this.MapDeleteKey("Security-UserRole-Save");
            this.MapUpdateKey("Security-UserRole-Save");
            this.MapInsertKey("Security-UserRole-Save");
            this.MapViewKey("Security-UserRole-View");
        }
    }
}
