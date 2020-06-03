using OMF.Common.ExceptionManagement;
using System.Configuration;
//using System.Web.Http.ExceptionHandling;

namespace OMF.Common.Configuration
{
    internal class ExceptionLoggerSetting : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"].ToString();
            }
        }

        [ConfigurationProperty("assemblyName", IsRequired = true)]
        public string AssemblyName
        {
            get
            {
                return this["assemblyName"].ToString();
            }
        }

        [ConfigurationProperty("className", IsRequired = true)]
        public string ClassName
        {
            get
            {
                return this["className"].ToString();
            }
        }

        internal IExceptionLogger GetExceptionLogger()
        {
            return (IExceptionLogger)ConfigurationController.GetAssembly(this.AssemblyName).CreateInstance(this.ClassName, true);
        }
    }
}
