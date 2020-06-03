using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;

namespace Zhivar.ViewModel.Accunting
{
    public class ShareholderVM
    {
        public int? ID { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Code { get; set; }
        public string ContactEmail { get; set; }
        public ZhivarEnums.ContactType ContactType { get; set; }
        public decimal Credits { get; set; }
        public DetailAccount DetailAccount { get; set; }
        public string EconomicCode { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string FirstName { get; set; }
        public int Id { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsShareHolder { get; set; }
        public bool IsVendor { get; set; }
        public string LastName { get; set; }
        public decimal Liability { get; set; }
        public string Mobile { get; set; }
        public string Name { get; set; }
        public string NationalCode { get; set; }
        public string Note { get; set; }
        public string People { get; set; }
        public string Phone { get; set; }
        public string PostalCode { get; set; }
        public int Rating { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal SharePercent { get; set; }
        public string State { get; set; }
        public string Website { get; set; }
    }
}
