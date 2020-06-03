using System;
using System.Configuration;
using OMF.Common.Extensions;

namespace OMF.Common.Configuration
{
    internal class MailSettings : ConfigurationElement
    {
        [ConfigurationProperty("userName")]
        public string UserName
        {
            get
            {
                return this["userName"].ToString();
            }
            set
            {
                this["userName"] = (object)value;
            }
        }

        [ConfigurationProperty("password")]
        public string Password
        {
            get
            {
                return this["password"].ToString();
            }
            set
            {
                this["password"] = (object)value;
            }
        }

        [ConfigurationProperty("securePassword")]
        public string SecurePassword
        {
            get
            {
                return this["securePassword"].ToString();
            }
            set
            {
                this["securePassword"] = (object)value;
            }
        }

        [ConfigurationProperty("host")]
        public string Host
        {
            get
            {
                return this["host"].ToString();
            }
            set
            {
                this["host"] = (object)value;
            }
        }

        [ConfigurationProperty("port")]
        public int Port
        {
            get
            {
                return this["port"].ConvertTo<int>();
            }
            set
            {
                this["port"] = (object)value;
            }
        }

        [ConfigurationProperty("timeOut")]
        public int TimeOut
        {
            get
            {
                try
                {
                    return this["timeOut"].ConvertTo<int>();
                }
                catch (Exception ex)
                {
                    return 100000;
                }
            }
            set
            {
                this["timeOut"] = (object)value;
            }
        }

        [ConfigurationProperty("mailFrom")]
        public string MailFrom
        {
            get
            {
                return this["mailFrom"].ToString();
            }
            set
            {
                this["mailFrom"] = (object)value;
            }
        }
    }
}
