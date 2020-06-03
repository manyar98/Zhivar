using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Common;
using Zhivar.ViewModel.Common;

namespace Zhivar.ViewModel.Accunting
{
    public class PayRecevieVM
    {
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        public Contact Contact { get; set; }
        public string Description { get; set; }
        public string DetailAccountName { get; set; }
        public string DisplayDate { get; set; }
        public int ID { get; set; }
        //public Zhivar.DomainClasses.Accounting.Invoice Invoice { get; set; }
        public int Number { get; set; }
        public int NumberDocument { get; set; }
        public int NumberDocument2 { get; set; }
                                
        public ZhivarEnums.PayRecevieType Type2 { get; set; }
        public string Type {
            get {
                switch (Type2)
                {
                    case ZhivarEnums.PayRecevieType.AzShakhs:
                        return "شخص";
                    case ZhivarEnums.PayRecevieType.Daramd:
                        return "درآمد";
                    case ZhivarEnums.PayRecevieType.Hazine:
                        return "هزینه";
                    case ZhivarEnums.PayRecevieType.Sir:
                        return "سایر";
                    default:
                        return "";
                }
            }
        }

        public bool IsCredit { get; set; }
        public bool IsDebit { get; set; }
        public bool IsRecevie { get; set; }
        public InvoiceVM InvoiceVM { get; set; }
        public CostVM CostVM { get; set; }
        public FinanYearVM FinanYearVM { get; set; }
        public DocumentVM DocumentVM { get; set; }
        public ContactVM ContactVM { get; set; }
        public AccountVM AccountVM { get; set; }

    }
}
