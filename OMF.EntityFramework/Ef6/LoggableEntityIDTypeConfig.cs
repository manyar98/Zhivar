using OMF.Common;
using System;
using System.Data.Entity.ModelConfiguration;
using System.Linq.Expressions;

namespace OMF.EntityFramework.Ef6
{
    public class LoggableEntityIDTypeConfig : ComplexTypeConfiguration<EntityIDLogData>
    {
        public LoggableEntityIDTypeConfig()
        {
            this.Property(log => log.InsertUserID).HasColumnName("INSERT_USER").IsRequired();
            this.Property(log => log.InsertDateTime).HasColumnName("INSERT_DATETIME").IsRequired();
            this.Property(log => log.UpdateUserID).HasColumnName("UPDATE_USER");
            this.Property(log => log.UpdateDateTime).HasColumnName("UPDATE_DATETIME");
        }
    }
}
