using OMF.Common.Cryptography;
using System;
using System.Configuration;

namespace OMF.Common.Configuration
{
    public class ConnectionStringSettingElement : ConfigurationElement
    {
        [ConfigurationProperty("name")]
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

        [ConfigurationProperty("providerName")]
        public string ProviderName
        {
            get
            {
                return this["providerName"].ToString();
            }
            set
            {
                this["providerName"] = (object)value;
            }
        }

        [ConfigurationProperty("isEncrypted")]
        public bool IsEncrypted
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this["isEncrypted"]);
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                this["isEncrypted"] = (object)value;
            }
        }

        [ConfigurationProperty("connectionString")]
        public string ConnectionString
        {
            get
            {
                string constr = this["connectionString"].ToString();
                if (this.IsEncrypted)
                    return this.Decrypt(constr);
                return constr;
            }
            set
            {
                this["connectionString"] = (object)value;
            }
        }

        private string Decrypt(string constr)
        {
            return CryptoHelper.Decrypt(constr, "BPJCryptoHelperTest");
        }
    }
}
