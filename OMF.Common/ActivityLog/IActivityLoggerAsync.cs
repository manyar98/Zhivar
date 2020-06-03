using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMF.Common.ActivityLog
{
    public interface IActivityLoggerAsync : IActivityLogger
    {
        Task SaveAsync(ActivityLog exData);

        Task SaveAsync(IEnumerable<ActivityLog> exData);
    }
}
