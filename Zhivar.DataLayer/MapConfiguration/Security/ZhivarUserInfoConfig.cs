using Zhivar.DomainClasses.Security;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMF.Common;

namespace Zhivar.Access.MapConfiguration.Security
{
    //class ZhivarUserInfoConfig : BaseEntityTypeConfig<ZhivarUserInfo>
    //{
    //    public ZhivarUserInfoConfig()
    //    {
    //        Ignore(u => u.PlainPassword);
    //        Ignore(u => u.ShakhsId);
    //       // Ignore(u => u.MarkazDarmaniId);
    //        //Ignore(u => u.OrganizationId);
    //        Ignore(u => u.OTPCode);
    //        Ignore(u => u.OTPTryNo);
    //        Ignore(u => u.NeedOTP);
    //        Ignore(u => u.LastOTPDate);
    //        Ignore(u => u.DisplayName);

    //        Property(uInfo => uInfo.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
    //        HasMany(uInfo => uInfo.UserOperations).WithRequired().HasForeignKey(oprUser => oprUser.UserId);
    //        HasMany(uInfo => uInfo.UserRoles).WithRequired().HasForeignKey(userRole => userRole.UserId);

    //        this.Map(map =>
    //        {
    //            map.MapInheritedProperties();
    //            map.Property(uInfo => uInfo.ID).HasColumnName("ID");
    //            map.Property(uInfo => uInfo.ApplicationId).HasColumnName("APPID");
    //            map.Property(uInfo => uInfo.AuthenticationType).HasColumnName("AUTHENTICATIONTYPE");
    //            map.Property(uInfo => uInfo.Gender).HasColumnName("GENDER");
    //            map.Property(uInfo => uInfo.NationalCode).HasColumnName("NATIONALCODE");
    //            map.Property(uInfo => uInfo.Email).HasColumnName("EMAIL");
    //            map.Property(uInfo => uInfo.FirstName).HasColumnName("FIRSTNAME");
    //            map.Property(uInfo => uInfo.IsActive).HasColumnName("ISACTIVE");
    //            map.Property(uInfo => uInfo.IsDeleted).HasColumnName("ISDELETED");
    //            map.Property(uInfo => uInfo.LastLoginDate).HasColumnName("LASTLOGINDATE");
    //            map.Property(uInfo => uInfo.LastName).HasColumnName("LASTNAME");
    //            map.Property(uInfo => uInfo.LoginTryTime).HasColumnName("LOGINTRYTIME");
    //            map.Property(uInfo => uInfo.MobileNo).HasColumnName("MOBILENO");
    //            map.Property(uInfo => uInfo.Password).HasColumnName("PASSWORD");
    //            map.Property(uInfo => uInfo.Tel).HasColumnName("TEL");
    //            map.Property(uInfo => uInfo.UserName).HasColumnName("USERNAME");
    //            map.Property(uInfo => uInfo.OrganizationId).HasColumnName("ORGN_ID");

    //            map.Property(uInfo => uInfo.Tag1).HasColumnName("TAG1");
    //            map.Property(uInfo => uInfo.Tag2).HasColumnName("TAG2");
    //            map.Property(uInfo => uInfo.Tag3).HasColumnName("TAG3");
    //            map.Property(uInfo => uInfo.Tag4).HasColumnName("TAG4");
    //            map.Property(uInfo => uInfo.Tag5).HasColumnName("TAG5");
    //            map.Property(uInfo => uInfo.Tag6).HasColumnName("TAG6");
    //            map.Property(uInfo => uInfo.Tag7).HasColumnName("TAG7");
    //            map.Property(uInfo => uInfo.Tag8).HasColumnName("TAG8");
    //            map.Property(uInfo => uInfo.Tag9).HasColumnName("TAG9");
    //            map.Property(uInfo => uInfo.Tag10).HasColumnName("TAG10");

    //            map.Property(uInfo => uInfo.TagInt1).HasColumnName("TAGINT1");
    //            map.Property(uInfo => uInfo.TagInt2).HasColumnName("TAGINT2");
    //            map.Property(uInfo => uInfo.TagInt3).HasColumnName("TAGINT3");
    //            map.Property(uInfo => uInfo.TagInt4).HasColumnName("TAGINT4");
    //            map.Property(uInfo => uInfo.TagInt5).HasColumnName("TAGINT5");

    //            //map.Property(uInfo => uInfo.LogData.InsertDateTime).HasColumnName("INSERT_DATETIME");
    //            //map.Property(uInfo => uInfo.LogData.InsertUserID).HasColumnName("INSERT_USER");
    //            //map.Property(uInfo => uInfo.LogData.UpdateDateTime).HasColumnName("UPDATE_DATETIME");
    //            //map.Property(uInfo => uInfo.LogData.UpdateUserID).HasColumnName("UPDATE_USER");

    //            map.ToTable("TBL_USER_INFO");
    //        });

    //        MapViewKey( OMF.Common.ResourceKeys.Security_UserInfo_View);
    //        MapInsertKey(OMF.Common.ResourceKeys.Security_UserInfo_Insert);
    //        MapUpdateKey(OMF.Common.ResourceKeys.Security_UserInfo_Update);
    //        MapDeleteKey(OMF.Common.ResourceKeys.Security_UserInfo_Delete);

    //        //MapEntityValidator(new ZhivarUserInfoValidator());
    //    }
    //}
}
