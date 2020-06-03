using System;
using System.Configuration;

namespace OMF.Common.Configuration
{
    internal class SecuritySettings : ConfigurationElement
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

        [ConfigurationProperty("captchaEnable")]
        public bool CaptchaEnable
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this["captchaEnable"]);
                }
                catch
                {
                    return true;
                }
            }
            set
            {
                this["captchaEnable"] = (object)value;
            }
        }

        [ConfigurationProperty("dbSchema")]
        public string DbSchema
        {
            get
            {
                return this["dbSchema"].ToString();
            }
            set
            {
                this["dbSchema"] = (object)value;
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

        [ConfigurationProperty("useMainAccess")]
        public bool UseMainAccess
        {
            get
            {
                return Convert.ToBoolean(this["useMainAccess"]);
            }
            set
            {
                this["useMainAccess"] = (object)value;
            }
        }

        [ConfigurationProperty("userInfoUiFilterationEnabled")]
        public bool UserInfoUiFilterationEnabled
        {
            get
            {
                return Convert.ToBoolean(this["userInfoUiFilterationEnabled"]);
            }
            set
            {
                this["userInfoUiFilterationEnabled"] = (object)value;
            }
        }

        [ConfigurationProperty("hasViewButtonPerRowInGridView")]
        public bool HasViewButtonPerRowInGridView
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this["hasViewButtonPerRowInGridView"]);
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                this["hasViewButtonPerRowInGridView"] = (object)value;
            }
        }

        [ConfigurationProperty("loginTryNo")]
        public short LoginTryNo
        {
            get
            {
                try
                {
                    short int16 = Convert.ToInt16(this["loginTryNo"]);
                    if (int16 == (short)0)
                        return 3;
                    return int16;
                }
                catch
                {
                    return 3;
                }
            }
        }

        [ConfigurationProperty("strongPass")]
        public bool StrongPass
        {
            get
            {
                return Convert.ToBoolean(this["strongPass"]);
            }
        }

        [ConfigurationProperty("tokenExpireMins")]
        public int TokenExpireMins
        {
            get
            {
                try
                {
                    return Convert.ToInt32(this["tokenExpireMins"]);
                }
                catch
                {
                    return 20;
                }
            }
        }
    }
}
