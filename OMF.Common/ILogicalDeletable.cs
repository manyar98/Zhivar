using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMF.Common
{
    public interface ILogicalDeletable
    {
        bool IsDeleted { get; set; }
    }
}
