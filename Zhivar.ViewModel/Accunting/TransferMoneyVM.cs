using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zhivar.DomainClasses.Accounting;

namespace Zhivar.ViewModel.Accunting
{
    public class TransferMoneyVM 
    {
        public int? ID {get;set;}
        public decimal Amount { get; set; }
        public string DisplayDate { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int DocumentId { get; set; }
        public int DocumentNumber { get; set; }
        public string From { get; set; }
        public string FromDetailAccountId { get; set; }
        public string FromDetailAccountName { get; set; }
        public string FromReference{ get; set; }
        public string To{ get; set; }
        public int ToDetailAccountId{ get; set; }
        public string ToDetailAccountName { get; set; }
        public string ToReference{ get; set; }
        public int OrganId { get; set; }
        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<TransferMoneyVM, TransferMoney>();
            configuration.CreateMap<TransferMoney, TransferMoneyVM>();
        }
    }
}
