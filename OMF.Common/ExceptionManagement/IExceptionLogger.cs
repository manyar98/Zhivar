using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMF.Common.ExceptionManagement
{
    public interface IExceptionLogger
    {
        void LogException(ExceptionData exData);
    }
}
