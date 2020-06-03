using System.Data.Common;

namespace OMF.EntityFramework.Ef6
{
    public class ConnectionInfo
    {
        public DbProviderFactory Factory { get; set; }

        public string ConnectionString { get; set; }
    }
}
