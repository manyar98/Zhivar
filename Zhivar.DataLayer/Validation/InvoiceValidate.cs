using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.DataLayer.Validation
{
    public class InvoiceValidate : AbstractValidator<Invoice>
    {
        public InvoiceValidate()
        {
            RuleFor(invoice => invoice.ContactId).NotNull().WithMessage("مشتری فاکتور باید مشخص شود");
            RuleFor(invoice => invoice.DisplayDate).NotNull().WithMessage("تاریخ فاکتور باید مشخص شود");
            RuleFor(invoice => invoice.DisplayDueDate).NotNull().WithMessage("تاریخ سررسید فاکتور باید مشخص شود");
            RuleFor(invoice => invoice.InvoiceItems).NotNull().WithMessage("هیچ آیتمی در فاکتور ثبت نشده است");
            RuleFor(invoice => invoice.InvoiceItems).Must((entity, invoiceItems) =>
            {

                    var countInvoiceItems = invoiceItems.Count();

                    if (countInvoiceItems > 0 )
                        return true;
                    else
                        return false;
                
            }).WithMessage("هیچ آیتمی در فاکتور ثبت نشده است.");
        }
    }
}
