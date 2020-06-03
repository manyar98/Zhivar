using System;

namespace OMF.Common
{
    public class LoggableEntity : Entity, ILoggableEntityID, IEntity, IViewEntity, IObjectState, ICloneable
    {
        public LoggableEntity()
        {
            this.LogData = new EntityIDLogData();
        }

        public virtual EntityIDLogData LogData { get; set; }
    }
}
