using System;

namespace OMF.Common
{
    public class LoggableEntityName : Entity, ILoggableEntityName, IEntity, IViewEntity, IObjectState, ICloneable
    {
        public LoggableEntityName()
        {
            this.LogData = new EntityNameLogData();
        }

        public virtual EntityNameLogData LogData { get; set; }
    }
}
