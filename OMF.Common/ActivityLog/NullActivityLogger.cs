using System.Collections.Generic;
using System.Threading.Tasks;

namespace OMF.Common.ActivityLog
{
    public class NullActivityLogger : IActivityLoggerAsync, IActivityLogger
    {
        public void Save(IEnumerable<ActivityLog> exData)
        {
        }

        public void Save(ActivityLog exData)
        {
        }

        public async Task SaveAsync(IEnumerable<ActivityLog> exData)
        {
        }

        public async Task SaveAsync(ActivityLog exData)
        {
        }
    }
}
