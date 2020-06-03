using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;
using static OMF.Common.Enums;

namespace Zhivar.ViewModel.BaseInfo
{
    public class PersonelVM
    {
        public int ID { get; set; }
        public DateTime AzTarikh { get; set; }
        public DateTime? TaTarikh { get; set; }
        public bool DarHalKhedmat { get; set; }
        public int OrganizationId { get; set; }
        public int RoleID { get; set; }
        public int UserID { get; set; }

        public string OnvanOrganization { get; set; }
        public string OnvanPosition { get; set; }
        public bool NeedToSaveUser { get; set; }

        public int NeedToSign { get; set; }
        public ZhivarEnums.NoeMozd NoeMozd { get; set; }
        public decimal Darsd { get; set; }
        public decimal Salary { get; set; }

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

        public string DisplayAzTarikh
        {
            get
            {
                return PersianDateUtils.ToPersianDate(AzTarikh);
            }
            set { }
        }
    }
}
