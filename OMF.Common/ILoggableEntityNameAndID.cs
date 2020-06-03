using System;

namespace OMF.Common
{
    public interface ILoggableEntityNameAndID : IEntity, IViewEntity, IObjectState, ICloneable
    {
        EntityNameAndIDLogData LogData { get; set; }
    }
}
