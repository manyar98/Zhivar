using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;

namespace Zhivar.ViewModel.Common
{
    public class BussinessVM
    {
        public int ID { get; set; }
        public int OrganId { get; set; }
        public string BusinessLine { get; set; }
        public ZhivarEnums.CalendarType CalendarType { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ZhivarEnums.Currency Currency { get; set; }
        public int DocCreditCountry { get; set; }
        public string ExpireDisplayDateCountry { get; set; }
        public bool IsExpire { get; set; }
        public string LegalName { get; set; }
        public string Name { get; set; }
        public int OrganizationType { get; set; }
        public string RegisterationDisplayDate { get; set; }
        public int SetupStep { get; set; }
        public int SubscriptionLevel { get; set; }
        public decimal TaxRate { get; set; }
        public bool setupInProgress { get; set; }
    }
}
