using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.Common;
using Zhivar.ViewModel.Common;

namespace Zhivar.ViewModel.Accunting
{
    public class InvoiceVM: IValidatableObject
    {
        public int ID { get; set; }

        public int OrganId { get; set; }
        public ContactVM Contact { get; set; }
        public int ContactId { get; set; }
        public string ContactTitle { get; set; }
        public DateTime DateTime { get; set; }
        public string DisplayDueDate { get; set; }
        public DateTime DueDate { get; set; }
        public List<InvoiceItemVM> InvoiceItems { get; set; }
        public string InvoiceStatusString { get; set; }
        public ZhivarEnums.NoeFactor InvoiceType { get; set; }
        public string InvoiceTypeString { get; set; }
        public bool IsDraft { get; set; }
        public bool IsPurchase { get; set; }
        public bool IsPurchaseReturn { get; set; }
        public bool IsSale { get; set; }
        public bool IsSaleReturn { get; set; }
        public bool IsWaste { get; set; }
        public string Note { get; set; }
        public string Number { get; set; }
        public decimal Paid { get; set; }
        public decimal Payable { get; set; }
        public decimal Profit { get; set; }
        public string Reference { get; set; }
        public decimal Rest { get; set; }
        public bool Returned { get; set; }
        public bool Sent { get; set; }
        public ZhivarEnums.NoeInsertFactor Status { get; set; }
        public decimal Sum { get; set; }
        public string Tag { get; set; }

        public string invoiceDueDate { get; set; }
        public string DisplayDate { get; set; }
        public string Refrence { get; set; }
        public bool? IsContract { get; set; }

        public int? DocumentID { get; set; }
        //public void CreateMappings(IConfiguration configuration)
        //{
        //    configuration.CreateMap<InvoiceVM, Invoice>().ForMember(x => x.Contact,opt => opt.MapFrom(src => src.Contact)) ;
        //    //.ForMember(x => x.DueDate,
        //    //        opt => opt.MapFrom(src => Utilities.PersianDateUtils.ToDateTime(src.DisplayDueDate)));


        //    configuration.CreateMap<Invoice, InvoiceVM>().ForMember(x => x.Contact, opt => opt.MapFrom(src => src.Contact));

        //            //   .ForMember(x => x.DisplayDueDate,
        //            //opt => opt.MapFrom(src => Utilities.PersianDateUtils.ToPersianDate(src.DueDate))).
        //            //ForMember(x =>x.DetailAccount.); ;

        //}

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (InvoiceType == ZhivarEnums.NoeFactor.Sell || InvoiceType == ZhivarEnums.NoeFactor.Buy)
            {
                if (string.IsNullOrEmpty(Number))
                    yield return new ValidationResult("لطفا شماره فاکتور را وارد نمایید");
            }
        }
    }
}