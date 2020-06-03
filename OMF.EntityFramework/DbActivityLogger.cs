using OMF.Common.ActivityLog;
using OMF.EntityFramework.DataContext;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OMF.EntityFramework
{
    public class DbActivityLogger : IActivityLoggerAsync, IActivityLogger
    {
        public async Task SaveAsync(ActivityLog actLog)
        {
            using (Ef6.UnitOfWork uow = new Ef6.UnitOfWork((IDataContextAsync)new ActivityLogDbContext()))
            {
                IRepositoryAsync<OMF.Common.ActivityLog.ActivityLog> rep = uow.RepositoryAsync<OMF.Common.ActivityLog.ActivityLog>();
                rep.InsertOrUpdateGraph(actLog);
                int num = await uow.SaveChangesAsync();
                rep = (IRepositoryAsync<OMF.Common.ActivityLog.ActivityLog>)null;
            }
        }

        public async Task SaveAsync(IEnumerable<OMF.Common.ActivityLog.ActivityLog> actLogs)
        {
            foreach (OMF.Common.ActivityLog.ActivityLog actLog1 in actLogs)
            {
                OMF.Common.ActivityLog.ActivityLog actLog = actLog1;
                await this.SaveAsync(actLog);
                actLog = (OMF.Common.ActivityLog.ActivityLog)null;
            }
        }

        public void Save(OMF.Common.ActivityLog.ActivityLog actLog)
        {
            using (Ef6.UnitOfWork unitOfWork = new Ef6.UnitOfWork((IDataContextAsync)new ActivityLogDbContext()))
            {
                unitOfWork.RepositoryAsync<OMF.Common.ActivityLog.ActivityLog>().InsertOrUpdateGraph(actLog);
                unitOfWork.SaveChanges();
            }
        }

        public void Save(IEnumerable<OMF.Common.ActivityLog.ActivityLog> actLogs)
        {
            foreach (OMF.Common.ActivityLog.ActivityLog actLog in actLogs)
                this.Save(actLog);
        }
    }
}
