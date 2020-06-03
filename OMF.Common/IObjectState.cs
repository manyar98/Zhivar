using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMF.Common
{
    public interface IObjectState : IViewEntity
    {
        Enums.ObjectState ObjectState { get; set; }
    }
}