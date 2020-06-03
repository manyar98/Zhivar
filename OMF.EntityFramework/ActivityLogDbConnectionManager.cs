using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.EntityFramework.Ef6;
using System;
using System.Data.Common;

namespace OMF.EntityFramework
{
    public class ActivityLogDbConnectionManager
    {
        public static DbConnection CreateConnection()
        {
            string connectionName = "OMF.App.ConstructionString";// string.IsNullOrWhiteSpace(ConfigurationController.ActivityLogDbConnectionName) ? "OMF.App.ConstructionString" : ConfigurationController.ActivityLogDbConnectionName;
            DbConnection dbConnection = (DbConnection)null;
            try
            {
                return ConnectionManager.CreateConnection(connectionName);
            }
            catch (Exception ex)
            {
                if (dbConnection != null)
                    dbConnection.Dispose();
                throw new DataAccessException("Create connection to ActivityLog database failed!", ex);
            }
        }
    }
}
