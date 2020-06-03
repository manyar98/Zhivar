using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Common
{
    public class Contact: LoggableEntityName, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        public decimal Balance { get; set; }
        public string City { get; set; }
        public decimal Credits { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NationalCode { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public int OrganId { get; set; }
        public string EconomicCode { get; set; }
        public string Fax { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsShareHolder { get; set; }
        public bool IsVendor { get; set; }
        public decimal Liability { get; set; }
        public string Note { get; set; }
        public int Rating { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal SharePercent { get; set; }
        public string Website { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender? Jensiat { get; set; }
        public string Color { get; set; }
        public string Company { get; set; }
        
        public ZhivarEnums.ContactType ContactType { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public byte[] Blob { get; set; }
    }
}
