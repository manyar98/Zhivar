using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;

namespace Zhivar.ViewModel.Accounting
{
    public class ChequeVM 
    {
        public int ID { get; set; }
        public int OrganId { get; set; }
        public int ContactId { get; set; }
        public decimal Amount { get; set; }
        public string BankBranch { get; set; }
        public string BankName { get; set; }
        public DateTime Date { get; set; }
        public string DisplayDate { get; set; }
        public string ChequeNumber { get; set; }
        public ZhivarEnums.ChequeStatus Status { get; set; }
        public Bank DepositBank { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public bool Overdue { get; set; }
        public Contact Contact { get; set; }
        public string StatusString { get; set; }
        public int? DepositBankId { get; set; }

        public ZhivarEnums.ChequeType Type { get; set; }

    }
}