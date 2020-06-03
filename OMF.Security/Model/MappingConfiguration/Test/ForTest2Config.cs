using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model.Test;
using OMF.Security.Model.Validators;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq.Expressions;
using static OMF.Common.Enums;

namespace OMF.Security.Model.MappingConfiguration.Test
{
    public class ForTest2Config : ActivityLoggableEntityTypeConfig<ForTest2>
    {
        public ForTest2Config()
        {
            this.ToTable("ForTest2s");
            //this.Property(x => x.Id).HasColumnName("MyTableKey");
            //this.Property(x => x.RowVersion).HasColumnType("Timestamp");
        }
        //    public class ForTest2Config : EntityTypeConfiguration<ForTest2>//, ActivityLoggableEntityTypeConfig<ForTest2>
        //{
        //    public ForTest2Config()
        //    {
        //        //this.ToTable("TBL_OPERATION", ConfigurationController.SecurityDbSchema);

        //        this.ToTable("ForTest2");
        //       // this.Property<int>((Expression<Func<ForTest2, int>>)(e => e.ID)).IsRequired();
        //      //  this.HasKey<int>((Expression<Func<ForTest2, int>>)(e => e.ID));
        //      //  this.Property((Expression<Func<ForTest2, string>>)(opr => opr.UserName)).HasColumnName("UserName");

        //        // this.MapEntityValidator((EntityValidator<Operation>)new OperationValidator());
        //        //this.MapDeleteKey("Security-Operation-Delete");
        //        //this.MapUpdateKey("Security-Operation-Update");
        //        //this.MapInsertKey("Security-Operation-Insert");
        //        //this.MapViewKey("Security-Operation-View");
        //    }
        //}
    }
}
