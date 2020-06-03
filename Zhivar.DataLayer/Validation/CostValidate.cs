using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.DataLayer.Validation
{
    public class CostValidate : AbstractValidator<Cost>
    {
        public CostValidate()
        {
            RuleFor(cost => cost.ContactId).NotNull().NotEqual(0).WithMessage("طرف حساب صورت هزینه باید مشخص شود");
            RuleFor(cost => cost.DisplayDate).NotNull().WithMessage("تاریخ صورت هزینه باید مشخص شود");
            RuleFor(cost => cost.Sum).NotNull().WithMessage("مبلغ صورت هزینه باید مشخص شود");
            RuleFor(cost => cost.CostItems).NotNull().WithMessage("هیچ آیتمی در صورت هزینه ثبت نشده است");
            RuleFor(cost => cost.CostItems).Must((entity, costItems) =>
            {

                var countInvoiceItems = costItems.Count();

                if (countInvoiceItems > 0)
                    return true;
                else
                    return false;

            }).WithMessage("هیچ آیتمی در صورت هزینه ثبت نشده است.");
        }
    }
}
