using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMF.Common
{
    public interface IEntity : IViewEntity, IObjectState, ICloneable
    {
        object this[string propName] { get; set; }

        int ID { get; set; }
    }
}
