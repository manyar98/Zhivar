using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using static OMF.Common.Enums;

namespace OMF.Security.Model.MappingConfiguration
{
    public class UserInfoConfig : ActivityLoggableEntityTypeConfig<UserInfo>
    {
        public UserInfoConfig()
        {
            //this.ToTable("TBL_USER_INFO", ConfigurationController.SecurityDbSchema);
            this.ToTable("TBL_USER_INFO");
            //if (ConfigurationController.IsOrganizational)
                this.Property(uInfo => uInfo.OrganizationId).HasColumnName("ORGN_ID");
            // else
            //      this.Ignore(uInfo => uInfo.OrganizationId));
            bool? otpCodeEnable = true;// ConfigurationController.OTPCodeEnable;
            int num;
            if (otpCodeEnable.HasValue)
            {
                otpCodeEnable = true;// ConfigurationController.OTPCodeEnable;
                num = otpCodeEnable.Value ? 1 : 0;
            }
            else
                num = 0;
            if (num != 0)
            {
                this.Property(uInfo => uInfo.NeedOTP).HasColumnName("NEED_OTP");
                this.Property(uInfo => uInfo.OTPCode).HasColumnName("OTP_CODE");
                this.Property(uInfo => uInfo.OTPTryNo).HasColumnName("OTP_TRYNO");
                this.Property(uInfo => uInfo.LastOTPDate).HasColumnName("LAST_OTP_DATE");
            }
            else
            {
                this.Ignore(uInfo => uInfo.NeedOTP);
                this.Ignore(uInfo => uInfo.OTPCode);
                this.Ignore(uInfo => uInfo.OTPTryNo);
                this.Ignore(uInfo => uInfo.LastOTPDate);
            }
            this.Ignore(uInfo => uInfo.PlainPassword);
           // if (ConfigurationController.CustomIdentityEnabled)
             // this.Property(uInfo => uInfo.ID).HasDatabaseGeneratedOption(new DatabaseGeneratedOption?(DatabaseGeneratedOption.None));
    
            this.Property(uInfo => uInfo.ApplicationId).HasColumnName("APPID");
            this.Property(uInfo => uInfo.AuthenticationType).HasColumnName("AUTHENTICATIONTYPE");
            this.Property(uInfo => uInfo.Email).HasColumnName("EMAIL").HasMaxLength(50);
            this.Property(uInfo => uInfo.Gender).HasColumnName("GENDER");
            this.Property(uInfo => uInfo.FirstName).HasColumnName("FIRSTNAME");
            this.Property(uInfo => uInfo.LastName).HasColumnName("LASTNAME");
            this.Property(uInfo => uInfo.NationalCode).HasColumnName("NATIONALCODE").HasMaxLength(50);
            this.Property(uInfo => uInfo.IsActive).HasColumnName("ISACTIVE");
            this.Property(uInfo => uInfo.IsDeleted).HasColumnName("ISDELETED");
            this.Property(uInfo => uInfo.LastLoginDate).HasColumnName("LASTLOGINDATE");
            this.Property(uInfo => uInfo.LoginTryTime).HasColumnName("LOGINTRYTIME");
            this.Property(uInfo => uInfo.MobileNo).HasColumnName("MOBILENO").HasMaxLength(11);
            this.Property(uInfo => uInfo.Password).HasColumnName("PASSWORD").HasMaxLength(300);
            this.Property(uInfo => uInfo.Tel).HasColumnName("TEL").HasMaxLength(15);
            this.Property(uInfo => uInfo.UserName).HasColumnName("USERNAME").HasMaxLength(50);
            this.HasMany<UserOperation>((Expression<Func<UserInfo, ICollection<UserOperation>>>)(uInfo => uInfo.UserOperations)).WithRequired().HasForeignKey<int>((Expression<Func<UserOperation, int>>)(oprUser => oprUser.UserId));
            this.HasMany<UserRole>((Expression<Func<UserInfo, ICollection<UserRole>>>)(uInfo => uInfo.UserRoles)).WithRequired().HasForeignKey<int>((Expression<Func<UserRole, int>>)(userRole => userRole.UserId));
            this.Property(uInfo => uInfo.Tag1).HasColumnName("TAG1").HasMaxLength(200);
            this.Property(uInfo => uInfo.Tag2).HasColumnName("TAG2").HasMaxLength(200);
            this.Property(uInfo => uInfo.Tag3).HasColumnName("TAG3").HasMaxLength(200);
            this.Property(uInfo => uInfo.Tag4).HasColumnName("TAG4").HasMaxLength(200);
            this.Property(uInfo => uInfo.Tag5).HasColumnName("TAG5").HasMaxLength(200);
            this.Property(uInfo => uInfo.Tag6).HasColumnName("TAG6").HasMaxLength(200);
            this.Property(uInfo => uInfo.Tag7).HasColumnName("TAG7").HasMaxLength(200);
            this.Property(uInfo => uInfo.Tag8).HasColumnName("TAG8").HasMaxLength(200);
            this.Property(uInfo => uInfo.Tag9).HasColumnName("TAG9").HasMaxLength(200);
            this.Property(uInfo => uInfo.Tag10).HasColumnName("TAG10").HasMaxLength(200);
            this.Property(uInfo => uInfo.TagInt1).HasColumnName("TAGINT1");
            this.Property(uInfo => uInfo.TagInt2).HasColumnName("TAGINT2");
            this.Property(uInfo => uInfo.TagInt3).HasColumnName("TAGINT3");
            this.Property(uInfo => uInfo.TagInt4).HasColumnName("TAGINT4");
            this.Property(uInfo => uInfo.TagInt5).HasColumnName("TAGINT5");
            this.Property(log => log.LogData.InsertUserID).HasColumnName("INSERT_USER").IsRequired();
            this.Property(log => log.LogData.InsertDateTime).HasColumnName("INSERT_DATETIME").IsRequired();
            this.Property(log => log.LogData.UpdateUserID).HasColumnName("UPDATE_USER");
            this.Property(log => log.LogData.UpdateDateTime).HasColumnName("UPDATE_DATETIME");

            //    this.MapEntityValidator((EntityValidator<UserInfo>)new UserInfoValidator());
            this.MapDeleteKey("Security-UserInfo-Delete");
            this.MapUpdateKey("Security-UserInfo-Update");
            this.MapInsertKey("Security-UserInfo-Insert");
            this.MapViewKey("Security-UserInfo-View");
        }
    }
}
