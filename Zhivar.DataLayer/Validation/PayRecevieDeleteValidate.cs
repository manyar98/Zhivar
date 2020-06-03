using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.DataLayer.Validation
{
    public class PayRecevieDeleteValidate: AbstractValidator<PayRecevie>
    {
        public PayRecevieDeleteValidate()
        {
     
            RuleFor(payRecevie => payRecevie.Items).Must((entity, items) =>
            {

                var countItems = items.Count();

                if (items.Any(x => x.Cheque != null))
                {
                    foreach (var item in items)
                    {
                        if (item.Cheque != null)
                        {
                            if (item.Cheque.Status != DomainClasses.ZhivarEnums.ChequeStatus.Normal)
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    return true;
                }
                


            }).WithMessage("بدلیل اینکه در این دریافت و پرداخت از چکی استفاده شده است که در وضعیت عادی نیست قابل حذف نیست ابتدا وضعیت چک را مشخص کنید.");
        }
    }
}

