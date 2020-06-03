using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMF.EntityFramework.DataContext
{
    public interface IDataContext : IDisposable
    {
        int SaveChanges();

        void SyncObjectState<TEntity>(TEntity entity) where TEntity : class, IObjectState;

        void SyncObjectsStatePostCommit();
    }
}
