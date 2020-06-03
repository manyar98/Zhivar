using OMF.Common;
using System;
using System.Data.Entity.ModelConfiguration;
using System.Linq.Expressions;

namespace OMF.EntityFramework.Ef6
{
    public class LoggableEntityNameTypeConfig : ComplexTypeConfiguration<EntityNameLogData>
    {
        public LoggableEntityNameTypeConfig()
        {
            this.Property(log => log.InsertUserName).HasColumnName("INSERTUSER").IsRequired();
            this.Property(log => log.InsertDateTime).HasColumnName("INSERTDATE").IsRequired();
            this.Property(log => log.UpdateUserName).HasColumnName("UPDATEUSER");
            this.Property(log => log.UpdateDateTime).HasColumnName("UPDATEDATE");
        }
    }
}
