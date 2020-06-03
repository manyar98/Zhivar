using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.DataLayer.Validation
{
    public class ItemValidate : AbstractValidator<Item>
    {
        public ItemValidate()
        {

            //RuleFor(item => item.).NotNull().WithMessage("طرف حساب صورت هزینه باید مشخص شود");
            //RuleFor(item => item.DisplayDate).NotNull().WithMessage("تاریخ صورت هزینه باید مشخص شود");
            //RuleFor(item => item.Sum).NotNull().WithMessage("مبلغ صورت هزینه باید مشخص شود");
            RuleFor(item => item.ItemGroupId).NotNull().WithMessage("هیچ گروهی انتخاب نشده است");
            //RuleFor(item => item.ItemGroup).Must((entity, itemItems) =>
            //{

            //    var countInvoiceItems = itemItems.Count();

            RuleFor(cost => cost.ItemGroupId).NotNull().Equal(0).WithMessage("گروه کالا/ خدمات باید مشخص شود");
            //RuleFor(cost => cost.DisplayDate).NotNull().WithMessage("تاریخ صورت هزینه باید مشخص شود");
            //RuleFor(cost => cost.Sum).NotNull().WithMessage("مبلغ صورت هزینه باید مشخص شود");
            //RuleFor(cost => cost.CostItems).NotNull().WithMessage("هیچ آیتمی در صورت هزینه ثبت نشده است");
            //RuleFor(cost => cost.CostItems).Must((entity, costItems) =>
            //{

            //    var countInvoiceItems = costItems.Count();


            //    if (countInvoiceItems > 0)
            //        return true;
            //    else
            //        return false;

            //}).WithMessage("هیچ آیتمی در صورت هزینه ثبت نشده است.");
        }
    }
}
