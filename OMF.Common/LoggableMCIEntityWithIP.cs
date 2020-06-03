using System;

namespace OMF.Common
{
    public class LoggableMCIEntityWithIP : Entity, ILoggableMCIEntityWithIP, IEntity, IViewEntity, IObjectState, ICloneable
    {
        public LoggableMCIEntityWithIP()
        {
            this.LogData = new MCIEntityWithIPLogData();
        }

        public virtual MCIEntityWithIPLogData LogData { get; set; }
    }
}
