using System.Configuration;
using OMF.Common.Extensions;

namespace OMF.Common.Configuration
{
    internal class BCSSettings : ConfigurationElement
    {
        [ConfigurationProperty("userName")]
        public string UserName
        {
            get
            {
                return this["userName"].ToString();
            }
        }

        [ConfigurationProperty("password")]
        public string Password
        {
            get
            {
                return this["password"].ToString();
            }
        }

        [ConfigurationProperty("isEncrypted")]
        public bool IsEncrypted
        {
            get
            {
                try
                {
                    return this["isEncrypted"].ConvertTo<bool>();
                }
                catch
                {
                    return false;
                }
            }
        }

        [ConfigurationProperty("enabled")]
        public bool Enabled
        {
            get
            {
                try
                {
                    return this["enabled"].ConvertTo<bool>();
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
