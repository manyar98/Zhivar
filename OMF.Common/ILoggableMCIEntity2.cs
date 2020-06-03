using System;

namespace OMF.Common
{
    public interface ILoggableMCIEntity2 : IEntity, IViewEntity, IObjectState, ICloneable
    {
        MCIEntityLogData2 LogData { get; set; }
    }
}
