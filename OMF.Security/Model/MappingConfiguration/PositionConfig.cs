using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model.Validators;
using System;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq.Expressions;

namespace OMF.Security.Model.MappingConfiguration
{
    public class PositionConfig : BaseEntityTypeConfig<Position>
    {
        public PositionConfig()
        {
            this.Map((Action<EntityMappingConfiguration<Position>>)(map => map.Requires("ROLE_TYPE").HasValue("Org")));
            this.Property<int>((Expression<Func<Position, int>>)(pos => pos.OrganizationUnitChartId)).HasColumnName("CHRT_ID");
            this.Property<int>((Expression<Func<Position, int?>>)(pos => pos.OrganizationId)).HasColumnName("ORGANIZATION_ID");
            this.MapEntityValidator((EntityValidator<Position>)new PositionValidator());
            this.MapDeleteKey("Security-Position-Delete");
            this.MapUpdateKey("Security-Position-Update");
            this.MapInsertKey("Security-Position-Insert");
            this.MapViewKey("Security-Position-View");
        }
    }
}
