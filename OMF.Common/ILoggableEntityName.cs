using System;

namespace OMF.Common
{
    public interface ILoggableEntityName : IEntity, IViewEntity, IObjectState, ICloneable
    {
        EntityNameLogData LogData { get; set; }
    }
}
