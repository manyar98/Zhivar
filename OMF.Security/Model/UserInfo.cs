using OMF.Common;
using OMF.Common.ActivityLog;
using OMF.Common.Security;
using System;
using System.Collections.Generic;
using static OMF.Common.Enums;

namespace OMF.Security.Model
{
    public class UserInfo : LoggableEntity, IAggregateRoot, ILogicalDeletable, IActivatable, IActivityLoggable, IEntity, IViewEntity, IObjectState, ICloneable//, ICustomIdentity
    {

        public ActionLog ActionsToLog
        {
            get
            {
                return ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
            }
        }
   
        public Gender? Gender { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string NationalCode { get; set; }

        public string Email { get; set; }

        public string Tel { get; set; }

        public string UserName { get; set; }

        public string PlainPassword { get; set; }

        public string Password { get; set; }

        public string MobileNo { get; set; }

        public int? ApplicationId { get; set; }

        public int? OrganizationId { get; set; }

        public bool? NeedOTP { get; set; }

        public string OTPCode { get; set; }

        public int? OTPTryNo { get; set; }

        public DateTime? LastOTPDate { get; set; }

        public int LoginTryTime { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public int AuthenticationType { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public string Tag1 { get; set; }

        public string Tag2 { get; set; }

        public string Tag3 { get; set; }

        public string Tag4 { get; set; }

        public string Tag5 { get; set; }

        public string Tag6 { get; set; }

        public string Tag7 { get; set; }

        public string Tag8 { get; set; }

        public string Tag9 { get; set; }

        public string Tag10 { get; set; }

        public int? TagInt1 { get; set; }

        public int? TagInt2 { get; set; }

        public int? TagInt3 { get; set; }

        public int? TagInt4 { get; set; }

        public int? TagInt5 { get; set; }


        public List<UserOperation> UserOperations { get; set; }

        public List<UserRole> UserRoles { get; set; }

        //public string IdentityGeneratorSequenceName
        //{
        //    get
        //    {
        //        return "tbl_user_info_seq";
        //    }
        //}

        public UserContext ConvertToUserContext()
        {
            UserContext userContext = new UserContext();

            userContext.UserId = this.ID;
            userContext.Gender = this.Gender;
            userContext.UserName = this.UserName;
            userContext.FirstName = this.FirstName;
            userContext.Tel = this.Tel;
            userContext.Email = this.Email;
            userContext.LastName = this.LastName;
            userContext.NationalCode = this.NationalCode;
            userContext.Password = this.Password;
            userContext.MobileNo = this.MobileNo;
            userContext.ApplicationID = this.ApplicationId;
            userContext.AuthenticationType = this.AuthenticationType;
            userContext.OrganizationId = this.OrganizationId;
            userContext.NeedOTP = this.NeedOTP;
            userContext.OTPCode = this.OTPCode;
            userContext.OTPTryNo = this.OTPTryNo;
            userContext.LastOTPDate = this.LastOTPDate;
            userContext.Tag1 = this.Tag1;
            userContext.Tag2 = this.Tag2;
            userContext.Tag3 = this.Tag3;
            userContext.Tag4 = this.Tag4;
            userContext.Tag5 = this.Tag5;
            userContext.Tag6 = this.Tag6;
            userContext.Tag7 = this.Tag7;
            userContext.Tag8 = this.Tag8;
            userContext.Tag9 = this.Tag9;
            userContext.Tag10 = this.Tag10;
            userContext.TagInt1 = this.TagInt1;
            userContext.TagInt2 = this.TagInt2;
            userContext.TagInt3 = this.TagInt3;
            userContext.TagInt4 = this.TagInt4;
            userContext.TagInt5 = this.TagInt5;
            userContext.LastLoginDateTime = DateTime.Now;
            userContext.Roles = SecurityManager.GetUserRoles(this.ID);
            userContext.ClientIP = ActivityLogManager.ClientIpCatcher == null ? "" : ActivityLogManager.ClientIpCatcher();


            return userContext;
            
        }
    }
}
