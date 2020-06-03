using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using OMF.Common.ActivityLog;
using OMF.Common.ExceptionManagement;
using OMF.Common.Mail;
using static OMF.Common.Enums;
using OMF.Common.Cryptography;
using OMF.Common.Security;
using OMF.Common.ExceptionManagement.Exceptions;

namespace OMF.Common.Configuration
{
    public static class ConfigurationController
    {
        private static OMFFrameworkSection CurrentWebConfig = (OMFFrameworkSection)null;

        static ConfigurationController()
        {
            ConfigurationController.CurrentWebConfig = OMFFrameworkSection.Current;
        }

        public static string ErrorPageUrl
        {
            get
            {
                string str = "~\\ErrorInfoPage.aspx";
                return string.IsNullOrEmpty(ConfigurationController.CurrentWebConfig.ExceptionSettings.ErrorPageUrl) ? str : ConfigurationController.CurrentWebConfig.ExceptionSettings.ErrorPageUrl;
            }
        }

        public static List<IExceptionLogger> ExceptionLoggers
        {
            get
            {
                List<IExceptionLogger> exceptionLoggerList = new List<IExceptionLogger>();
                foreach (ExceptionLoggerSetting exceptionLoggerSetting in (ConfigurationElementCollection)ConfigurationController.CurrentWebConfig.ExceptionSettings.ExceptionLoggerSettings)
                {
                    IExceptionLogger exceptionLogger = exceptionLoggerSetting.GetExceptionLogger();
                    exceptionLoggerList.Add(exceptionLogger);
                }
                return exceptionLoggerList;
            }
        }

        public static string BcsUserName
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.BcsSettings.UserName;
            }
        }

        public static string BcsPassword
        {
            get
            {
                if (ConfigurationController.CurrentWebConfig.BcsSettings.IsEncrypted)
                    return CryptoHelper.Decrypt(ConfigurationController.CurrentWebConfig.BcsSettings.Password, "BPJCryptoHelperTest");
                return ConfigurationController.CurrentWebConfig.BcsSettings.Password;
            }
        }

        public static bool BcsEnabled
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.BcsSettings.Enabled;
            }
        }

        public static bool UseGridViewDataForExport
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.ReportSettings.UseGridViewDataForExport;
            }
        }

        public static bool WithDebugInfo
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.ExceptionSettings.WithDebugInfo;
            }
        }

        public static string ExceptionLogPath
        {
            get
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                return string.IsNullOrEmpty(ConfigurationController.CurrentWebConfig.ExceptionSettings.ExceptionLogPath) ? baseDirectory : ConfigurationController.CurrentWebConfig.ExceptionSettings.ExceptionLogPath;
            }
        }

        public static string ExceptionTableSchema
        {
            get
            {
                string str = "dbo";
                return string.IsNullOrEmpty(ConfigurationController.CurrentWebConfig.ExceptionSettings.TableSchema) ? str : ConfigurationController.CurrentWebConfig.ExceptionSettings.TableSchema;
            }
        }

        public static string ExceptionDbConnectionName
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.ExceptionSettings.DbConnectionName;
            }
        }

        public static ConnectionStringSettingElement GetDbConnectionSetting(
          string conStrName)
        {
            return ConfigurationController.CurrentWebConfig.ConnectionStringSettings.GetConnectionStringSetting(conStrName);
        }

        public static ConnectionStringSettingCollection GetDbConnectionSettingCollection()
        {
            return ConfigurationController.CurrentWebConfig.ConnectionStringSettings;
        }

        public static bool EnableSecurityCheck
        {
            get
            {
                if (ConfigurationController.CurrentWebConfig.SecurtiySettings.Enable && SecurityManager.CurrentUserContext != null)
                    return SecurityManager.CurrentUserContext.AuthenticationType != 10;
                return ConfigurationController.CurrentWebConfig.SecurtiySettings.Enable;
            }
        }

        public static string SecurityDbConnectionName
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.SecurtiySettings.DbConnectionName;
            }
        }

        public static string SecurityDbSchema
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.SecurtiySettings.DbSchema;
            }
        }

        public static bool CaptchaEnable
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.SecurtiySettings.CaptchaEnable;
            }
        }

        public static short LoginTryNo
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.SecurtiySettings.LoginTryNo;
            }
        }

        public static int TokenExpireMins
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.SecurtiySettings.TokenExpireMins;
            }
        }

        public static bool StrongPass
        {
            get
            {
                try
                {
                    return ConfigurationController.CurrentWebConfig.SecurtiySettings.StrongPass;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool UserInfoUiFilterationEnabled
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.SecurtiySettings.UserInfoUiFilterationEnabled;
            }
        }

        public static bool HasViewButtonPerRowInGridView
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.SecurtiySettings.HasViewButtonPerRowInGridView;
            }
        }

        public static int? ApplicationID
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.ApplicationID;
            }
        }

        public static bool IsOrganizational
        {
            get
            {
                try
                {
                    return ConfigurationController.CurrentWebConfig.IsOrganizational;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool P1Enabled
        {
            get
            {
                try
                {
                    return ConfigurationController.CurrentWebConfig.P1Enabled;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool CustomIdentityEnabled
        {
            get
            {
                try
                {
                    return ConfigurationController.CurrentWebConfig.CustomIdentityEnabled;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static Assembly GetAssembly(string assemblyName)
        {
            AssemblySettingElement assemblySetting = ConfigurationController.CurrentWebConfig.AssemblySettings[assemblyName];
            if (assemblySetting == null)
                throw new OMFException(string.Format("Couldn't find assemblySetting: '{0}'", (object)assemblyName));
            return assemblySetting.GetAssembly();
        }

        public static string BusinessAssemblyName
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.BusinessAssemblyName;
            }
        }

        public static AppLanguage ApplicationLanguage
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.ApplicationLanguage;
            }
        }

        public static IActivityLogger ActivityLogger
        {
            get
            {
               

                return ConfigurationController.CurrentWebConfig.ActivityLogSettings.GetActivityLogger();
            }
        }

        public static string ActivityLogDbConnectionName
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.ActivityLogSettings.DbConnectionName;
            }
        }

        public static string ActivityLogTableSchema
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.ActivityLogSettings.TableSchema;
            }
        }

        public static bool EnableActivityLog
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.ActivityLogSettings.Enable;
            }
        }

        public static string ForgetPassURL
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings[nameof(ForgetPassURL)];
                }
                catch
                {
                    return (string)null;
                }
            }
        }

        public static bool? OTPCodeEnable
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.OTPCodeSettings.Enable;
            }
        }

        public static int OTPCodeLength
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.OTPCodeSettings.Length;
            }
        }

        public static short OTPTryNo
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.OTPCodeSettings.TryNo;
            }
        }

        public static bool OTPCodeWithCharacter
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.OTPCodeSettings.WithCharacter;
            }
        }

        public static TimeSpan? OTPCodeExpireTime
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.OTPCodeSettings.CodeExpireTime;
            }
        }

        public static MailOptions MailOptions
        {
            get
            {
                return new MailOptions()
                {
                    SecurePassword = ConfigurationController.CurrentWebConfig.MailSettings.SecurePassword,
                    Password = ConfigurationController.CurrentWebConfig.MailSettings.Password,
                    Host = ConfigurationController.CurrentWebConfig.MailSettings.Host,
                    Port = ConfigurationController.CurrentWebConfig.MailSettings.Port,
                    TimeOut = ConfigurationController.CurrentWebConfig.MailSettings.TimeOut,
                    UserName = ConfigurationController.CurrentWebConfig.MailSettings.UserName
                };
            }
        }

        public static string MailFrom
        {
            get
            {
                return ConfigurationController.CurrentWebConfig.MailSettings.MailFrom;
            }
        }

        public static bool DebugMode { get; set; } = false;
    }
}
