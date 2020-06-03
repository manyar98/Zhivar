using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace OMF.Common
{
    public interface IActivityLoggable : IEntity, IViewEntity, IObjectState, ICloneable
    {
        ActionLog ActionsToLog { get; }
    }
}
