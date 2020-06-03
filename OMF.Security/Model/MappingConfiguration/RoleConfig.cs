using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model.Validators;
using System;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace OMF.Security.Model.MappingConfiguration
{
    public class RoleConfig : BaseEntityTypeConfig<Role>
    {
        public RoleConfig()
        {
            this.Map((Action<EntityMappingConfiguration<Role>>)(map => map.Requires("ROLE_TYPE").HasValue("Sys")));
           // this.MapEntityValidator((EntityValidator<Role>)new RoleValidator());
            this.MapDeleteKey("Security-Role-Delete");
            this.MapUpdateKey("Security-Role-Update");
            this.MapInsertKey("Security-Role-Insert");
            this.MapViewKey("Security-Role-View");
        }
    }
}
