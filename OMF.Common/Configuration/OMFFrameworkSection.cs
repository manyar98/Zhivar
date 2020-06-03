using System;
using System.Configuration;
using System.Diagnostics;
using OMF.Common.Extensions;
using static OMF.Common.Enums;
using OMF.Common.Log;
using OMF.Common.ExceptionManagement;
using OMF.Common.ExceptionManagement.Exceptions;

namespace OMF.Common.Configuration
{
    internal class OMFFrameworkSection : ConfigurationSection
    {
        private string certificateDefaultName = "BPJServicesCertificate";
        private ConfigurationSourceType configSourceType = ConfigurationSourceType.ConfigFile;
        private AppLanguage appLanguage = AppLanguage.Farsi;
        private static OMFFrameworkSection current;

        [ConfigurationProperty("applicationID")]
        public int? ApplicationID
        {
            get
            {
                return this["applicationID"].ConvertTo<int?>();
            }
            set
            {
                this["applicationID"] = (object)value;
            }
        }

        [ConfigurationProperty("isOrganizational")]
        public bool IsOrganizational
        {
            get
            {
                return Convert.ToBoolean(this["isOrganizational"]);
            }
            set
            {
                this["isOrganizational"] = (object)value;
            }
        }

        [ConfigurationProperty("p1Enabled")]
        public bool P1Enabled
        {
            get
            {
                return Convert.ToBoolean(this["p1Enabled"]);
            }
            set
            {
                this["p1Enabled"] = (object)value;
            }
        }

        [ConfigurationProperty("customIdentityEnabled")]
        public bool CustomIdentityEnabled
        {
            get
            {
                return Convert.ToBoolean(this["customIdentityEnabled"]);
            }
            set
            {
                this["customIdentityEnabled"] = (object)value;
            }
        }

        [ConfigurationProperty("businessAssemblyName")]
        public string BusinessAssemblyName
        {
            get
            {
                return this["businessAssemblyName"].ToString();
            }
            set
            {
                this["businessAssemblyName"] = (object)value;
            }
        }

        [ConfigurationProperty("serviceCertificateName")]
        public string ServiceCertificateName
        {
            get
            {
                return string.IsNullOrWhiteSpace(this["serviceCertificateName"].ToString()) ? this.certificateDefaultName : this["serviceCertificateName"].ToString();
            }
            set
            {
                this["serviceCertificateName"] = (object)value;
            }
        }

        [ConfigurationProperty("sourceType")]
        public ConfigurationSourceType ConfigSourceType
        {
            get
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(this["sourceType"].ToString()))
                        this.configSourceType = (ConfigurationSourceType)Enum.Parse(typeof(ConfigurationSourceType), this["sourceType"].ToString(), true);
                }
                catch
                {
                }
                return this.configSourceType;
            }
            set
            {
                this["sourceType"] = (object)value;
            }
        }

        [ConfigurationProperty("appLanguage")]
        public AppLanguage ApplicationLanguage
        {
            get
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(this["appLanguage"].ToString()))
                        this.appLanguage = (AppLanguage)Enum.Parse(typeof(AppLanguage), this["appLanguage"].ToString(), true);
                }
                catch
                {
                }
                return this.appLanguage;
            }
        }

        [ConfigurationProperty("BcsSettings")]
        public BCSSettings BcsSettings
        {
            get
            {
                return (BCSSettings)this[nameof(BcsSettings)];
            }
        }

        [ConfigurationProperty("MailSettings")]
        public MailSettings MailSettings
        {
            get
            {
                return (MailSettings)this[nameof(MailSettings)];
            }
        }

        [ConfigurationProperty("ConnectionStringSettings")]
        public ConnectionStringSettingCollection ConnectionStringSettings
        {
            get
            {
                return (ConnectionStringSettingCollection)this[nameof(ConnectionStringSettings)];
            }
        }

        [ConfigurationProperty("AssemblySettings")]
        public AssemblySettingCollection AssemblySettings
        {
            get
            {
                return (AssemblySettingCollection)this[nameof(AssemblySettings)];
            }
        }

        [ConfigurationProperty("ExceptionSettings")]
        public ExceptionSettings ExceptionSettings
        {
            get
            {
                return (ExceptionSettings)this[nameof(ExceptionSettings)];
            }
        }

        [ConfigurationProperty("ReportSettings")]
        public ReportSettings ReportSettings
        {
            get
            {
                return (ReportSettings)this[nameof(ReportSettings)];
            }
        }

        [ConfigurationProperty("SecuritySettings")]
        public SecuritySettings SecurtiySettings
        {
            get
            {
                return (SecuritySettings)this["SecuritySettings"];
            }
        }

        [ConfigurationProperty("OTPCodeSettings")]
        public OTPCodeSettings OTPCodeSettings
        {
            get
            {
                return (OTPCodeSettings)this[nameof(OTPCodeSettings)];
            }
        }

        [ConfigurationProperty("ActivityLogSettings")]
        public ActivityLogSettings ActivityLogSettings
        {
            get
            {
                return (ActivityLogSettings)this[nameof(ActivityLogSettings)];
            }
        }

        public static OMFFrameworkSection Current
        {
            get
            {
                return Configuration.OMFFrameworkSection.current;
            }
        }

        static OMFFrameworkSection()
        {
            try
            {
                OMFFrameworkSection.current = ConfigurationManager.GetSection("OMF.Framework") as OMFFrameworkSection;
            }
            catch (Exception ex)
            {
                EventLogHelper.Write(ExceptionManager.GetExceptionMessageWithDebugInfo((Exception)new OMFException("BPJ.Framework configuration exception", ex)), EventLogEntryType.Error);
            }
        }

        private OMFFrameworkSection()
        {
        }
    }
}
