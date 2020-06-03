using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMF.Common
{
    public interface ILoggableEntityID : IEntity, IViewEntity, IObjectState, ICloneable
    {
        EntityIDLogData LogData { get; set; }
    }
}