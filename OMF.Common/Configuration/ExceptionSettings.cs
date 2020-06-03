using System;
using System.Configuration;

namespace OMF.Common.Configuration
{
    internal class ExceptionSettings : ConfigurationElement
    {
        [ConfigurationProperty("errorPageUrl")]
        public string ErrorPageUrl
        {
            get
            {
                return this["errorPageUrl"].ToString();
            }
            set
            {
                this["errorPageUrl"] = (object)value;
            }
        }

        [ConfigurationProperty("withDebugInfo")]
        public bool WithDebugInfo
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this["withDebugInfo"]);
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                this["withDebugInfo"] = (object)value;
            }
        }

        [ConfigurationProperty("exceptionLogPath")]
        public string ExceptionLogPath
        {
            get
            {
                return this["exceptionLogPath"].ToString();
            }
            set
            {
                this["exceptionLogPath"] = (object)value;
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

        [ConfigurationProperty("ExceptionLoggerSettings")]
        public ExceptionLoggerSettings ExceptionLoggerSettings
        {
            get
            {
                return (ExceptionLoggerSettings)this[nameof(ExceptionLoggerSettings)];
            }
        }
    }
}
