using System;
using System.Runtime.Serialization;
using OMF.Common.Extensions;
using static OMF.Common.Enums;

namespace OMF.Common.Security
{
    [DataContract]
    [Serializable]
    public class UserContext
    {
        [DataMember]
        public Gender? Gender { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string NationalCode { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Tel { get; set; }

        [DataMember]
        public string Token { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string MobileNo { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public int? ApplicationID { get; set; }

        [DataMember]
        public int AuthenticationType { get; set; }

        [DataMember]
        public int? OrganizationId { get; set; }

        [DataMember]
        public bool? NeedOTP { get; set; }

        [DataMember]
        public string OTPCode { get; set; }

        [DataMember]
        public int? OTPTryNo { get; set; }

        [DataMember]
        public DateTime? LastOTPDate { get; set; }

        [DataMember]
        public string Tag1 { get; set; }

        [DataMember]
        public string Tag2 { get; set; }

        [DataMember]
        public string Tag3 { get; set; }

        [DataMember]
        public string Tag4 { get; set; }

        [DataMember]
        public string Tag5 { get; set; }

        [DataMember]
        public string Tag6 { get; set; }

        [DataMember]
        public string Tag7 { get; set; }

        [DataMember]
        public string Tag8 { get; set; }

        [DataMember]
        public string Tag9 { get; set; }

        [DataMember]
        public string Tag10 { get; set; }

        [DataMember]
        public int? TagInt1 { get; set; }

        [DataMember]
        public int? TagInt2 { get; set; }

        [DataMember]
        public int? TagInt3 { get; set; }

        [DataMember]
        public int? TagInt4 { get; set; }

        [DataMember]
        public int? TagInt5 { get; set; }

        [DataMember]
        public DateTime LastLoginDateTime { get; set; }

        [DataMember]
        public string ClientIP { get; set; }

        [DataMember]
        public RoleDataCollection Roles { get; set; }

        public string FullName
        {
            get
            {
                if (!this.Gender.HasValue)
                    return string.Format("{0} {1}", (object)this.FirstName, (object)this.LastName);
                Gender? gender1 = this.Gender;
                Gender gender2 = Enums.Gender.Male;
                if (gender1.GetValueOrDefault() == gender2 && gender1.HasValue)
                    return string.Format("{0}ی {1} {2}", (object)((Enum)(ValueType)this.Gender).GetPersianTitle(), (object)this.FirstName, (object)this.LastName);
                return string.Format("{0} {1} {2}", (object)((Enum)(ValueType)this.Gender).GetPersianTitle(), (object)this.FirstName, (object)this.LastName);
            }
        }

        public override string ToString()
        {
            return string.Format("User Name : {0}\n FirstName : {1}\n Last Name : {2}", (object)this.UserName, (object)this.FirstName, (object)this.LastName);
        }

        public bool IsDeveloperUser( UserContext userContext)
        {
            if (userContext == null)
                return false;

            return userContext.AuthenticationType == 1;// (int)ZhivarEnums.ZhivarUserType.Developers;
        }

        public bool IsOperatorPersonnelUser( )
        {
            if (this == null)
                return false;
            return (this.AuthenticationType == 3 || this.AuthenticationType == 9);// (int)ZhivarEnums.ZhivarUserType.MarkazDarmaniUser;
        }
  
      
    }
}
