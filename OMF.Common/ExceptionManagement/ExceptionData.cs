using OMF.Common.ActivityLog;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Security;
using System;

namespace OMF.Common.ExceptionManagement
{
    [Serializable]
    public class ExceptionData
    {
        private string detailMessage = "";
        private Exception exception;
        private string machineName;
        private string domainName;
        private string machineIP;
        private string osVersion;
        private string osLogonName;
        private object userID;
        private string userName;
        private DateTime date;
        private string code;
        private int appId;

        public Exception Exception
        {
            get
            {
                return this.exception;
            }
            set
            {
                this.exception = value;
            }
        }

        public string DetailMessage
        {
            get
            {
                if (string.IsNullOrEmpty(this.detailMessage))
                    return ExceptionManager.GetExceptionMessageWithDebugInfo(this.exception);
                return this.detailMessage;
            }
            set
            {
                this.detailMessage = value;
            }
        }

        public string ExceptionMessage
        {
            get
            {
                return this.Exception.Message;
            }
        }

        public string MachineName
        {
            get
            {
                return this.machineName;
            }
            set
            {
                this.machineName = value;
            }
        }

        public string DomainName
        {
            get
            {
                return this.domainName;
            }
            set
            {
                this.domainName = value;
            }
        }

        public string MachineIP
        {
            get
            {
                return this.machineIP;
            }
            set
            {
                this.machineIP = value;
            }
        }

        public string OSVersion
        {
            get
            {
                return this.osVersion;
            }
            set
            {
                this.osVersion = value;
            }
        }

        public string OSLogonName
        {
            get
            {
                return this.osLogonName;
            }
            set
            {
                this.osLogonName = value;
            }
        }

        public object UserID
        {
            get
            {
                return this.userID;
            }
            set
            {
                this.userID = value;
            }
        }

        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                this.userName = value;
            }
        }

        public DateTime Date
        {
            get
            {
                return this.date;
            }
            set
            {
                this.date = value;
            }
        }

        public string Code
        {
            get
            {
                return this.code;
            }
            set
            {
                this.code = value;
            }
        }

        public int ApplicationID
        {
            get
            {
                return this.appId;
            }
            set
            {
                this.appId = value;
            }
        }

        public ExceptionData()
        {
        }

        public ExceptionData(Exception ex)
        {
            this.exception = ex;
            this.SetUserData();
        }

        protected void SetUserData()
        {
            try
            {
                this.detailMessage = ExceptionManager.GetExceptionMessageWithDebugInfo(this.exception);
                this.machineName = ActivityLogManager.ClientIpCatcher();
                this.domainName = !this.Exception.Data.Contains((object)"SourceName") ? Environment.UserDomainName : this.Exception.Data[(object)"SourceName"].ToString();
                this.machineIP = ActivityLogManager.ClientIpCatcher();
                this.osVersion = Environment.OSVersion.ToString();
                this.osLogonName = Environment.UserName;
                if (SecurityManager.CurrentUserContext != null)
                {
                    this.userID = (object)SecurityManager.CurrentUserContext.UserId;
                    this.userName = SecurityManager.CurrentUserContext.UserName;
                }
                else
                {
                    this.userID = (object)-1;
                    this.userName = "Anonymous";
                }
                int? applicationId = ConfigurationController.ApplicationID;
                int num;
                if (!applicationId.HasValue)
                {
                    num = -1;
                }
                else
                {
                    applicationId = ConfigurationController.ApplicationID;
                    num = applicationId.Value;
                }
                this.appId = num;
                this.date = DateTime.Now;
                this.code = Environment.MachineName + this.date.ToString("-yyyyMMddHHmmssffff");
            }
            catch (Exception ex)
            {
                throw new ExpManagementException(ex);
            }
        }
    }
}
