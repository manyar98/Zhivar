using OMF.Common.Configuration;
using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model.Validators;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace OMF.Security.Model.MappingConfiguration
{
    public class OrganizationConfig : BaseEntityTypeConfig<Organization>
    {
        public OrganizationConfig()
        {
            this.ToTable("TBL_ORGANIZATION");//, ConfigurationController.SecurityDbSchema);
            if (ConfigurationController.CustomIdentityEnabled)
                this.Property<int>((Expression<Func<Organization, int>>)(org => org.ID)).HasDatabaseGeneratedOption(new DatabaseGeneratedOption?(DatabaseGeneratedOption.None));
            this.Property((Expression<Func<Organization, string>>)(org => org.Code)).HasColumnName("CODE").HasMaxLength(new int?(200));
            this.Property((Expression<Func<Organization, byte[]>>)(org => org.Logo)).HasColumnName("LOGO");
            this.Property((Expression<Func<Organization, string>>)(org => org.Title)).HasColumnName("TITLE").HasMaxLength(new int?(100));
            this.Property<bool>((Expression<Func<Organization, bool>>)(org => org.IsActive)).HasColumnName("ISACTIVE");
            this.Property<bool>((Expression<Func<Organization, bool>>)(org => org.IsDeleted)).HasColumnName("ISDELETED");
            this.Property((Expression<Func<Organization, string>>)(org => org.Address)).HasColumnName("ADDRESS");
            this.Property((Expression<Func<Organization, string>>)(org => org.Alias)).HasColumnName("ALIAS");
            this.Property((Expression<Func<Organization, string>>)(org => org.Email)).HasColumnName("EMAIL");
            this.Property((Expression<Func<Organization, string>>)(org => org.Website)).HasColumnName("WEBSITE");
            this.Property((Expression<Func<Organization, string>>)(org => org.Description)).HasColumnName("DESCRIPTION");
            this.Property((Expression<Func<Organization, string>>)(org => org.Tag1)).HasColumnName("TAG1");
            this.Property((Expression<Func<Organization, string>>)(org => org.Tag2)).HasColumnName("TAG2");
            this.Property((Expression<Func<Organization, string>>)(org => org.Tag3)).HasColumnName("TAG3");
            this.Property((Expression<Func<Organization, string>>)(org => org.Tag4)).HasColumnName("TAG4");
            this.Property((Expression<Func<Organization, string>>)(org => org.Tag5)).HasColumnName("TAG5");
            this.Property<int>((Expression<Func<Organization, int?>>)(org => org.ParentId)).HasColumnName("ORGN_ID");
            this.Property<int>((Expression<Func<Organization, int>>)(org => org.OrganizationUnitChartId)).HasColumnName("CHRT_ID");
            this.Property<int>((Expression<Func<Organization, int>>)(org => org.CityId)).HasColumnName("CITY_ID");
            this.MapUpdateKey("Security-Organization-Update");
            this.MapInsertKey("Security-Organization-Insert");
            this.MapViewKey("Security-Organization-View");
            //this.MapEntityValidator((EntityValidator<Organization>)new OrganizationValidator());
        }
    }
}
