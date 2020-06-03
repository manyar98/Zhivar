using System;

namespace OMF.Common
{
    public interface ILoggableMCIEntity : IEntity, IViewEntity, IObjectState, ICloneable
    {
        MCIEntityLogData LogData { get; set; }
    }
}
