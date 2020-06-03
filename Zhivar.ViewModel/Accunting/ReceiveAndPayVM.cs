using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;

namespace Zhivar.ViewModel.Accunting
{
    public class ReceiveAndPayVM
    {
        public Account Account { get; set; }
        public decimal Amount { get; set; }
        public Contact Contact { get; set; }
        public string Description { get; set; }
        public DetailAccount DetailAccount { get; set; }
        public string DisplayDate { get; set; }
        public int Id { get; set; }
        public Invoice Invoice { get; set; }
        public List<DetailPayRecevie> Items { get; set; }
        public int Number { get; set; }
        public int Number2 { get; set; }
        public List<PayItem> PayItems { get; set; }
        public ZhivarEnums.PayRecevieType Type { get; set; }
    }

    public class PayItem { }
    //public class ItemReceiveAndPay
    //{
    //    public decimal Amount { get; set; }
    //    public Bank Bank { get; set; }
    //    public Cash Cash { get; set; }
    //    public Cheque Cheque { get; set; }
    //    public ChequeBank ChequeBank { get; set; }
    //    public int Id { get; set; }
    //    public string Reference { get; set; }
    //    public Enums.PayRecevieType Type { get; set; }
    //}
}
