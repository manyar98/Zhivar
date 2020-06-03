using OMF.Common;
using OMF.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static OMF.Common.Enums;

namespace Zhivar.ViewModel.Security
{
    public class UserInfoVM
    {
        public int ID { get; set; }
        public int? ApplicationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalCode { get; set; }
        public Gender? Gender { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string MobileNo { get; set; }
        public int LoginTryTime { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public int AuthenticationType { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int? TagInt1 { get; set; }
        public int? TagInt2 { get; set; }
        public int? TagInt3 { get; set; }
        public int? TagInt4 { get; set; }
        public int? TagInt5 { get; set; }
        public IEnumerable<UserOperationVM> UserOperations { get; set; }
        public IEnumerable<UserRoleVM> UserRoles { get; set; }
        public IEnumerable<string> RoleNames { get; set; }
        public string FullName
        {
            get
            {
                if (Gender.HasValue)
                {
                    if (Gender == OMF.Common.Enums.Gender.Male)
                        return $"{Gender.GetPersianTitle()}ی {FirstName} {LastName}";
                    return $"{Gender.GetPersianTitle()} {FirstName} {LastName}";
                }

                return $"{FirstName} {LastName}";
            }
        }
        public string RoleNamesStr
        {
            get
            {
                if (RoleNames != null && RoleNames.Any())
                    return RoleNames.Aggregate("", (rnStr, roleName) => rnStr += string.IsNullOrEmpty(rnStr) ? roleName : "، " + roleName);
                return "";
            }
        }
        public int? ShakhsId { get; set; }
        public int? MarkazDarmaniId { get; set; }
        public string DisplayName { get; set; }
        public int? OrganizationId { get; set; }
        public int? RoleId { get; set; }
        //public int LOBLOBLOB { get; set; }
    }
}