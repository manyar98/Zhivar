using System.Collections.Generic;

namespace OMF.Common.ActivityLog
{
    public interface IActivityLogger
    {
        void Save(ActivityLog exData);

        void Save(IEnumerable<ActivityLog> exData);
    }
}
