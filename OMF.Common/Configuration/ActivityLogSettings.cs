using System;
using System.Configuration;
using OMF.Common.ActivityLog;

namespace OMF.Common.Configuration
{
    internal class ActivityLogSettings : ConfigurationElement
    {
        [ConfigurationProperty("enable")]
        public bool Enable
        {
            get
            {
                return Convert.ToBoolean(this["enable"]);
            }
            set
            {
                this["enable"] = (object)value;
            }
        }

        [ConfigurationProperty("tableSchema")]
        public string TableSchema
        {
            get
            {
                return this["tableSchema"].ToString();
            }
            set
            {
                this["tableSchema"] = (object)value;
            }
        }

        [ConfigurationProperty("dbConnectionName")]
        public string DbConnectionName
        {
            get
            {
                return this["dbConnectionName"].ToString();
            }
            set
            {
                this["dbConnectionName"] = (object)value;
            }
        }

        [ConfigurationProperty("className")]
        public string ClassName
        {
            get
            {
                return this["className"].ToString();
            }
            set
            {
                this["className"] = (object)value;
            }
        }

        [ConfigurationProperty("assemblyName")]
        public string AssemblyName
        {
            get
            {
                return this["assemblyName"].ToString();
            }
            set
            {
                this["assemblyName"] = (object)value;
            }
        }

        internal IActivityLogger GetActivityLogger()
        {
            if (!this.Enable)
                return (IActivityLogger)new NullActivityLogger();
            return (IActivityLogger)ConfigurationController.GetAssembly(this.AssemblyName).CreateInstance(this.ClassName, true);
        }
    }
}
