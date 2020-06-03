using OMF.Common.Configuration;
using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace OMF.Security.Model.MappingConfiguration
{
    public class RoleBaseConfig : ActivityLoggableEntityTypeConfig<RoleBase>
    {
        public RoleBaseConfig()
        {
            this.ToTable("TBL_ROLE_INFO");//, ConfigurationController.SecurityDbSchema);
            if (ConfigurationController.CustomIdentityEnabled)
                this.Property<int>((Expression<Func<RoleBase, int>>)(role => role.ID)).HasDatabaseGeneratedOption(new DatabaseGeneratedOption?(DatabaseGeneratedOption.None));
            this.Property<int>((Expression<Func<RoleBase, int?>>)(role => role.ApplicationId)).HasColumnName("APPID");
            this.Property((Expression<Func<RoleBase, string>>)(role => role.Code)).HasColumnName("CODE").HasMaxLength(new int?(50));
            this.Property<bool>((Expression<Func<RoleBase, bool>>)(role => role.IsActive)).HasColumnName("ISACTIVE");
            this.Property<bool>((Expression<Func<RoleBase, bool>>)(role => role.IsDeleted)).HasColumnName("ISDELETED");
            this.Property((Expression<Func<RoleBase, string>>)(role => role.Name)).HasColumnName("NAME").HasMaxLength(new int?(50));
            this.Property<int>((Expression<Func<RoleBase, int?>>)(role => role.ParentId)).HasColumnName("PARENTID");
            this.Property<bool>((Expression<Func<RoleBase, bool?>>)(role => role.IsMultiAssignable)).HasColumnName("ISMULTIASSIGNABLE");
            this.HasMany<RoleOperation>((Expression<Func<RoleBase, ICollection<RoleOperation>>>)(role => role.RoleOperations)).WithRequired().HasForeignKey<int>((Expression<Func<RoleOperation, int>>)(roleopr => roleopr.RoleId));
          //  this.MapEntityValidator((EntityValidator<RoleBase>)new RoleBaseValidator());
            this.MapDeleteKey("Security-Role-Delete");
            this.MapUpdateKey("Security-Role-Update");
            this.MapInsertKey("Security-Role-Insert");
            this.MapViewKey("Security-Role-View");
        }
    }
}
