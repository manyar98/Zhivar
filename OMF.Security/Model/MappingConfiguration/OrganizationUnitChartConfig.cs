using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using static OMF.Common.Enums;

namespace OMF.Security.Model.MappingConfiguration
{
    public class OrganizationUnitChartConfig : BaseEntityTypeConfig<OrganizationUnitChart>
    {
        public OrganizationUnitChartConfig()
        {
            this.ToTable("TBL_ORGANIZATION_UNIT_CHART");//, ConfigurationController.SecurityDbSchema);
            if (ConfigurationController.CustomIdentityEnabled)
                this.Property<int>((Expression<Func<OrganizationUnitChart, int>>)(ouc => ouc.ID)).HasDatabaseGeneratedOption(new DatabaseGeneratedOption?(DatabaseGeneratedOption.None));
            this.Property((Expression<Func<OrganizationUnitChart, string>>)(ouc => ouc.Code)).HasColumnName("CODE").HasMaxLength(new int?(200));
            this.Property((Expression<Func<OrganizationUnitChart, string>>)(ouc => ouc.Title)).HasColumnName("TITLE").HasMaxLength(new int?(100));
            this.Property<bool>((Expression<Func<OrganizationUnitChart, bool>>)(ouc => ouc.IsActive)).HasColumnName("ISACTIVE");
            this.Property<bool>((Expression<Func<OrganizationUnitChart, bool>>)(ouc => ouc.IsDeleted)).HasColumnName("ISDELETED");
            this.Property<UnitChartType>((Expression<Func<OrganizationUnitChart, UnitChartType>>)(ouc => ouc.UnitChartType)).HasColumnName("UNIT_CHART_TYPE");
            this.Property((Expression<Func<OrganizationUnitChart, string>>)(ouc => ouc.Tag1)).HasColumnName("TAG1").HasMaxLength(new int?(100));
            this.Property((Expression<Func<OrganizationUnitChart, string>>)(ouc => ouc.Tag2)).HasColumnName("TAG2").HasMaxLength(new int?(100));
            this.Property<int>((Expression<Func<OrganizationUnitChart, int?>>)(ouc => ouc.IntTag1)).HasColumnName("INTTAG1");
            this.Property<int>((Expression<Func<OrganizationUnitChart, int?>>)(ouc => ouc.IntTag2)).HasColumnName("INTTAG2");
            this.Property<int>((Expression<Func<OrganizationUnitChart, int?>>)(ouc => ouc.ParentId)).HasColumnName("CHRT_ID");
            this.HasMany<OrganizationUnitChart>((Expression<Func<OrganizationUnitChart, ICollection<OrganizationUnitChart>>>)(ouc => ouc.ChildUnitCharts)).WithOptional().HasForeignKey<int?>((Expression<Func<OrganizationUnitChart, int?>>)(ouc => ouc.ParentId));
            this.MapUpdateKey("Security-OrganizationUnitChart-Update");
            this.MapInsertKey("Security-OrganizationUnitChart-Insert");
            this.MapViewKey("Security-OrganizationUnitChart-View");
           // this.MapEntityValidator((EntityValidator<OrganizationUnitChart>)new OrganizationUnitChartValidator());
        }
    }
}
