using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace OMF.EntityFramework.Ef6
{
    public class ConnectionManager
    {
        private static Dictionary<string, ConnectionInfo> connectionInfoes = new Dictionary<string, ConnectionInfo>();

        static ConnectionManager()
        {
            //  foreach (ConnectionStringSettingElement stringSettingElement in ConfigurationController.GetDbConnectionSettingCollection().OfType<ConnectionStringSettingElement>())
            //  {
            ConnectionInfo connectionInfo = new ConnectionInfo()
            {
                Factory = DbProviderFactories.GetFactory("System.Data.SqlClient"),//stringSettingElement.ProviderName),
                    ConnectionString =
"Data Source = 45.159.196.12,1437; Initial Catalog = smbtir_ZhivarDB; Persist Security Info = True; User ID = smbtir_omid; Password = M7a1_4bg"

                //@"Data Source = .\MSSQLSERVER2014; Initial Catalog = smbtir_ZhivarDB; Persist Security Info = True; User ID = smbtir_omid; Password = M7a1_4bg"

                //stringSettingElement.ConnectionString
                //ConnectionString = "Data Source = (local); Initial Catalog = ZhivarDB; Persist Security Info = True;" //stringSettingElement.ConnectionString
            };
            ConnectionManager.connectionInfoes.Add("OMF.App.ConstructionString", connectionInfo);// (stringSettingElement.Name, connectionInfo);
           // }
        }

        public static DbConnection CreateConnection(string connectionName)
        {
            DbConnection dbConnection = (DbConnection)null;
            try
            {
                ConnectionInfo connectionInfoe = ConnectionManager.connectionInfoes[connectionName];
                dbConnection = connectionInfoe.Factory.CreateConnection();
                dbConnection.ConnectionString = connectionInfoe.ConnectionString;
                return dbConnection;
            }
            catch (Exception ex)
            {
                if (dbConnection != null)
                    dbConnection.Dispose();
                throw new DataAccessException("Create connection to database failed!", ex);
            }
        }
    }
}
