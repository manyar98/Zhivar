using OMF.Common.ExceptionManagement.Exceptions;
using OMF.EntityFramework.Ef6;
using System;
using System.Data.Common;

namespace OMF.Workflow
{
    public class WorkflowDbConnectionManager
    {
        public static DbConnection CreateConnection()
        {
            string connectionName = "OMF.App.ConstructionString";
            DbConnection dbConnection = (DbConnection)null;
            try
            {
                return ConnectionManager.CreateConnection(connectionName);
            }
            catch (Exception ex)
            {
                if (dbConnection != null)
                    dbConnection.Dispose();
                throw new DataAccessException("Create workflow database connection failed!", ex);
            }
        }
    }
}
