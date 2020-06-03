using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using OMF.EntityFramework.Ef6;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Contract;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.DataLayer.Validation
{
    public class ContractValidate : AbstractValidator<Contract>
    {
        public ContractValidate()
        {
            RuleFor(contract => contract.ContactId).NotNull().NotEqual(0).WithMessage("طرف حساب قرارداد باید مشخص شود");
            RuleFor(contract => contract.ContractTitle).NotNull().NotEmpty().WithMessage("عنوان قرارداد باید مشخص شود");
            RuleFor(contract => contract.DisplayDate).NotNull().WithMessage("تاریخ قرارداد باید مشخص شود");
           //RuleFor(contract => contract.DateTime).NotNull().GreaterThanOrEqualTo(DateTime.Now.AddDays(-1)).WithMessage("تاریخ قرارداد باید بزرگتر یا مساوی تاریخ روز باشد.");
            RuleFor(contract => contract.Contract_Sazes).NotNull().WithMessage("هیچ رسانه در قرارداد ثبت نشده است");
            RuleFor(contract => contract.Contract_Sazes).Must((entity, contract_Sazes) =>
            {

                var countInvoiceItems = contract_Sazes.Count();

                if (countInvoiceItems > 0)
                    return true;
                else
                    return false;

            }).WithMessage("هیچ رسانه در قرارداد ثبت نشده است.");

            RuleFor(contract => contract.Contract_Sazes).Must((entity, contract_Sazes) =>
            {

                var countInvoiceItems = contract_Sazes.Count();

                if (countInvoiceItems > 0)
                {

                    foreach (var contract_Saze in contract_Sazes)
                    {
                        if (contract_Saze.SazeId == null || contract_Saze.SazeId == 0)
                            return false;

                    }
                    return true;
                }

                else
                    return false;

            }).WithMessage("در لیست رسانه ها رسانه را وارد نمایید..");

            RuleFor(contract => contract.Contract_Sazes).Must((entity, contract_Sazes) =>
            {

                var countInvoiceItems = contract_Sazes.Count();

                if (countInvoiceItems > 0)
                {

                    foreach (var contract_Saze in contract_Sazes)
                    {
                        if (contract_Saze.TarikhShorou == null || contract_Saze.TarikhShorou == DateTime.MinValue)
                            return false;

                    }
                    return true;
                }
                    
                else
                    return false;

            }).WithMessage("در لیست رسانه ها تاریخ شروع رسانه را وارد نمایید..");

            RuleFor(contract => contract.Contract_Sazes).Must((entity, contract_Sazes) =>
            {

                var countInvoiceItems = contract_Sazes.Count();

                if (countInvoiceItems > 0)
                {

                    foreach (var contract_Saze in contract_Sazes)
                    {
                        if (contract_Saze.Quantity == null || contract_Saze.Quantity == 0)
                            return false;

                    }
                    return true;
                }

                else
                    return false;

            }).WithMessage("در لیست رسانه ها مدت اجاره رسانه را وارد نمایید..");

            RuleFor(contract => contract.Contract_Sazes).Must((entity, contract_Sazes) =>
            {

                var countInvoiceItems = contract_Sazes.Count();

                if (countInvoiceItems > 0)
                {

                    foreach (var contract_Saze in contract_Sazes)
                    {
                        if (contract_Saze.UnitPrice == null || contract_Saze.UnitPrice == 0)
                            return false;

                    }
                    return true;
                }

                else
                    return false;

            }).WithMessage("در لیست رسانه ها فی اجاره رسانه را وارد نمایید..");

            RuleFor(contract => contract.Contract_Sazes).Must((entity, contract_Sazes) =>
            {

                var countInvoiceItems = contract_Sazes.Count();

                if (countInvoiceItems > 0)
                {

                    foreach (var contract_Saze in contract_Sazes)
                    {
                        if (contract_Saze.NoeEjareId == null || contract_Saze.NoeEjareId == 0)
                            return false;

                    }
                    return true;
                }

                else
                    return false;

            }).WithMessage("در لیست رسانه ها نوع اجاره رسانه را وارد نمایید..");

            //RuleFor(contract => contract.Contract_Sazes).Must((entity, contract_Sazes) =>
            //{

            //    if (entity.ContractType == DomainClasses.ZhivarEnums.ContractType.RentFrom)
            //        return true;

            //    var countInvoiceItems = contract_Sazes.Count();

            //    if (countInvoiceItems > 0)
            //    {

            //        foreach (var contract_Saze in contract_Sazes)
            //        {
            //            if (contract_Saze.TarikhShorou.Date < DateTime.Now.Date)
            //                return false;

            //        }
            //        return true;
            //    }

            //    else
            //        return true;

            //}).WithMessage("در لیست رسانه ها تاریخ شروع اکران رسانه باید بزرگتر یا مساوی تاریخ روز باشد");

            RuleFor(contract => contract.Contract_Sazes).Must((entity, contract_Sazes) =>
            {
                if (entity.ContractType == DomainClasses.ZhivarEnums.ContractType.RentFrom)
                    return true;
                var countContractSazesItems = contract_Sazes.Count();

                if (countContractSazesItems > 0)
                {
                    using (var uow = new UnitOfWork())
                    {
                        var contractsIds = uow.RepositoryAsync<Contract>().Queryable().Where(x => (x.ContractType == ContractType.PreContract || x.ContractType == ContractType.RentTo) && x.OrganId == entity.OrganId).Select(x => x.ID).ToList();
                        var reservationsIds = uow.RepositoryAsync<Reservation>().Queryable().Where(x => x.OrganID == entity.OrganId).Select(x => x.ID).ToList();

                        foreach (var contract_Saze in contract_Sazes)
                        {
                            var allSazes = uow.RepositoryAsync<Contract_Saze>().Queryable().Where(x => contractsIds.Contains(x.ContractID) && x.SazeId == contract_Saze.SazeId &&
                                            ((x.TarikhShorou >= contract_Saze.TarikhShorou && x.TarikhShorou <= contract_Saze.TarikhPayan) ||
                                                         (x.TarikhShorou <= contract_Saze.TarikhShorou && x.TarikhPayan >= contract_Saze.TarikhShorou) ||
                                                         (x.TarikhShorou <= contract_Saze.TarikhShorou && x.TarikhPayan >= contract_Saze.TarikhShorou) ||
                                                         (x.TarikhShorou >= contract_Saze.TarikhShorou && x.TarikhPayan <= contract_Saze.TarikhPayan)));

                            var contractStopDetails = uow.RepositoryAsync<ContractStopDetails>().Queryable().Where(x => x.SazeID == contract_Saze.SazeId && 
                                      ((x.StartDate <= contract_Saze.TarikhShorou.Date && x.StartDate >= contract_Saze.TarikhPayan) ||
                                       (x.StartDate >= contract_Saze.TarikhShorou.Date && x.EndDate <= contract_Saze.TarikhShorou.Date) ||
                                       (x.StartDate >= contract_Saze.TarikhShorou.Date && x.EndDate <= contract_Saze.TarikhShorou.Date) ||
                                       (x.StartDate <= contract_Saze.TarikhShorou.Date && x.EndDate >= contract_Saze.TarikhPayan)));

                            if (allSazes.Any() && !contractStopDetails.Any())
                                return false;

                            if (allSazes.Any())
                            {
                                return false;
                            }
                            else
                            {
                               

                                var reservation_Details = uow.RepositoryAsync<Reservation_Detail>().Queryable().Where(x => reservationsIds.Contains(x.ReservationID) &&  x.SazeID == contract_Saze.SazeId &&
                                    ((x.StartDate >= contract_Saze.TarikhShorou && x.StartDate <= contract_Saze.TarikhPayan) ||
                                     (x.StartDate <= contract_Saze.TarikhShorou && x.EndDate >= contract_Saze.TarikhShorou) ||
                                     (x.StartDate <= contract_Saze.TarikhShorou && x.EndDate >= contract_Saze.TarikhShorou) ||
                                     (x.StartDate >= contract_Saze.TarikhShorou && x.EndDate <= contract_Saze.TarikhPayan)));

                                if (reservation_Details.Any())
                                    return false;
                                
                              //  else
                                   // return true;
                            }

                        }
                        return true;
                    }
                    
                }

                else
                    return true;

            }).WithMessage("در لیست رسانه ها رسانه ای در لیست رسانه در اجاره و یا رزرو شخص دیگری می باشد");


            RuleFor(contract => contract.Contract_Sazes).Must((entity, contract_Sazes) =>
            {
                if (entity.ContractType == DomainClasses.ZhivarEnums.ContractType.RentFrom)
                    return true;
                var countContractSazesItems = contract_Sazes.Count();

                if (countContractSazesItems > 0)
                {
                    using (var uow = new UnitOfWork())
                    {
                        foreach (var contract_Saze in contract_Sazes)
                        {
                     
                            var contractsRentFromIds = uow.RepositoryAsync<DomainClasses.Contract.Contract>().Queryable().Where(x => (x.ContractType == ContractType.RentFrom) && x.OrganId == entity.OrganId).Select(x => x.ID).ToList();

                            var allSazesRentFrom = uow.RepositoryAsync<Contract_Saze>().Queryable().Where(x => contractsRentFromIds.Contains(x.ContractID) && x.SazeId == contract_Saze.SazeId &&
                                                        ((x.TarikhShorou >= contract_Saze.TarikhShorou.Date && x.TarikhShorou <= contract_Saze.TarikhPayan.Date) ||
                                                         (x.TarikhShorou <= contract_Saze.TarikhShorou.Date && x.TarikhPayan >= contract_Saze.TarikhShorou.Date) ||
                                                         (x.TarikhShorou <= contract_Saze.TarikhShorou.Date && x.TarikhPayan >= contract_Saze.TarikhShorou.Date) ||
                                                         (x.TarikhShorou >= contract_Saze.TarikhShorou.Date && x.TarikhPayan <= contract_Saze.TarikhPayan.Date)));

                            if (!allSazesRentFrom.Any())
                                return false;
                        }

                        return true;
                    }
                }
                else
                    return true;

            }).WithMessage("در لیست رسانه ها رسانه ای است که در اجاره شرکت نمی باشد");
        }
    }
}
