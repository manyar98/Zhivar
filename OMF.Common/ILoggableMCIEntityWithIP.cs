using System;

namespace OMF.Common
{
    public interface ILoggableMCIEntityWithIP : IEntity, IViewEntity, IObjectState, ICloneable
    {
        MCIEntityWithIPLogData LogData { get; set; }
    }
}
