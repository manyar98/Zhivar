using OMF.Common;
using System;
using System.Data.Entity.ModelConfiguration;
using System.Linq.Expressions;

namespace OMF.EntityFramework.Ef6
{
    public class LoggableEntityNameAndIDTypeConfig : ComplexTypeConfiguration<EntityNameAndIDLogData>
    {
        public LoggableEntityNameAndIDTypeConfig()
        {
            this.Property(log => log.InsertUserId).HasColumnName("INSERT_USERID").IsRequired();
            this.Property(log => log.InsertUserName).HasColumnName("INSERT_USERNAME").IsRequired();
            this.Property(log => log.InsertDateTime).HasColumnName("INSERT_DATE").IsRequired();
            this.Property(log => log.UpdateUserId).HasColumnName("UPDATE_USERID");
            this.Property(log => log.UpdateUserName).HasColumnName("UPDATE_USERNAME");
            this.Property(log => log.UpdateDateTime).HasColumnName("UPDATE_DATE");
        }
    }
}
