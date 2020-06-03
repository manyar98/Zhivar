using System;
using System.Configuration;
using System.Reflection;
using OMF.Common.ExceptionManagement.Exceptions;

namespace OMF.Common.Configuration
{
    internal class AssemblySettingElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"].ToString();
            }
            set
            {
                this["name"] = (object)value;
            }
        }

        [ConfigurationProperty("assemblyFullName", IsRequired = true)]
        public string AssemblyFullName
        {
            get
            {
                return this["assemblyFullName"].ToString();
            }
            set
            {
                this["assemblyFullName"] = (object)value;
            }
        }

        public Assembly GetAssembly()
        {
            Assembly assembly = AppDomain.CurrentDomain.Load(this.AssemblyFullName);
            if (assembly == (Assembly)null)
                throw new OMFException(string.Format("Couldn't load file or assembly '{0}' or the assembly not found in excutable path", (object)this.AssemblyFullName));
            return assembly;
        }
    }
}
