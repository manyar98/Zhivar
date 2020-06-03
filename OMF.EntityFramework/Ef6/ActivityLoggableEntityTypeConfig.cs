using OMF.Common;
using System;
using System.Linq.Expressions;
using static OMF.Common.Enums;

namespace OMF.EntityFramework.Ef6
{
    public class ActivityLoggableEntityTypeConfig<TEntity> : BaseEntityTypeConfig<TEntity>
      where TEntity : class, IActivityLoggable
    {
        public ActivityLoggableEntityTypeConfig()
        {
            this.Ignore(e => e.ActionsToLog);
        }
    }
}
