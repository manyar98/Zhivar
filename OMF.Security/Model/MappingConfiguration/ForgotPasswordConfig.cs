using OMF.Common.Configuration;
using OMF.EntityFramework.Ef6;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace OMF.Security.Model.MappingConfiguration
{
    public class ForgotPasswordConfig : BaseEntityTypeConfig<ForgotPasswordEntity>
    {
        public ForgotPasswordConfig()
        {
            this.ToTable("TBL_FORGOTPASSWORD");
            if (ConfigurationController.CustomIdentityEnabled)
                this.Property<int>((Expression<Func<ForgotPasswordEntity, int>>)(forgotPass => forgotPass.ID)).HasDatabaseGeneratedOption(new DatabaseGeneratedOption?(DatabaseGeneratedOption.None));
            this.Property((Expression<Func<ForgotPasswordEntity, string>>)(forgotPass => forgotPass.Code)).HasColumnName("CODE").HasMaxLength(new int?(50)).IsRequired();
            this.Property<int>((Expression<Func<ForgotPasswordEntity, int>>)(forgotPass => forgotPass.UserId)).HasColumnName("USERID").IsRequired();
            this.Property((Expression<Func<ForgotPasswordEntity, string>>)(forgotPass => forgotPass.UserName)).HasColumnName("USERNAME").HasMaxLength(new int?(50)).IsRequired();
            this.Property((Expression<Func<ForgotPasswordEntity, string>>)(forgotPass => forgotPass.UserEmail)).HasColumnName("USEREMAIL").HasMaxLength(new int?(50));
            this.Property((Expression<Func<ForgotPasswordEntity, string>>)(forgotPass => forgotPass.UserMobile)).HasColumnName("USERMOBILE").HasMaxLength(new int?(10));
            this.Property((Expression<Func<ForgotPasswordEntity, DateTime>>)(forgotPass => forgotPass.InsertDateTime)).HasColumnName("INSERT_DATETIME").IsRequired();
            this.Property((Expression<Func<ForgotPasswordEntity, DateTime>>)(forgotPass => forgotPass.UpdateDateTime)).HasColumnName("UPDATE_DATETIME");
            this.Property<bool>((Expression<Func<ForgotPasswordEntity, bool>>)(forgotPass => forgotPass.IsActive)).HasColumnName("ISACTIVE").IsRequired();
            this.Property((Expression<Func<ForgotPasswordEntity, string>>)(forgotPass => forgotPass.ClientIP)).HasColumnName("CLIENT_IP").IsRequired();
        }
    }
}
