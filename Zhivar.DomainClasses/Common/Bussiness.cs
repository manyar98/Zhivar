using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Common
{
    public class Bussiness: LoggableEntity,IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;

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
