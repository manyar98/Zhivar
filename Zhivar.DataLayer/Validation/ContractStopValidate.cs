using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using OMF.EntityFramework.Ef6;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Contract;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.DataLayer.Validation
{
    public class ContractStopValidate : AbstractValidator<ContractStops>
    {
        public ContractStopValidate()
        {
            RuleFor(contract => contract.DisplayDateRegister).NotNull().WithMessage("تاریخ ثبت توقف باید مشخص شود");
            RuleFor(contract => contract.ContractStopDetails).NotNull().WithMessage("هیچ رسانه برای توقف ثبت نشده است");
            RuleFor(contract => contract.ContractStopDetails).Must((entity, ContractStopDetails) =>
            {

                var items = ContractStopDetails.Count();

                if (items > 0)
                    return true;
                else
                    return false;

            }).WithMessage("هیچ رسانه برای توقف ثبت نشده است.");

            RuleFor(contract => contract.ContractStopDetails).Must((entity, ContractStopDetails) =>
            {

                var items = ContractStopDetails.Count();

                if (items > 0)
                {

                    foreach (var contractStopDetails in ContractStopDetails)
                    {
                        if (contractStopDetails.SazeID == null || contractStopDetails.SazeID == 0)
                            return false;

                    }
                    return true;
                }

                else
                    return false;

            }).WithMessage("در لیست رسانه ها رسانه را وارد نمایید..");

            RuleFor(contract => contract.ContractStopDetails).Must((entity, contractStopDetails) =>
            {

                var countItems = contractStopDetails.Count();

                if (countItems > 0)
                {

                    foreach (var contractStopDetail in contractStopDetails)
                    {
                        if (contractStopDetail.StartDate == null || contractStopDetail.StartDate == DateTime.MinValue)
                            return false;

                    }
                    return true;
                }

                else
                    return false;

            }).WithMessage("در لیست رسانه ها تاریخ شروع توقف رسانه را وارد نمایید..");

            RuleFor(contract => contract.ContractStopDetails).Must((entity, contractStopDetails) =>
            {

                var countContractStopDetailsItems = contractStopDetails.Count();

                if (countContractStopDetailsItems > 0)
                {

                    foreach (var contractStopDetail in contractStopDetails)
                    {
                        if (contractStopDetail.Quantity == null || contractStopDetail.Quantity == 0)
                            return false;

                    }
                    return true;
                }

                else
                    return false;

            }).WithMessage("در لیست رسانه ها مدت توقف رسانه را وارد نمایید..");


            RuleFor(contract => contract.ContractStopDetails).Must((entity, contractStopDetails) =>
            {

                var countContractStopDetailItems = contractStopDetails.Count();

                if (countContractStopDetailItems > 0)
                {

                    foreach (var contractStopDetail in contractStopDetails)
                    {
                        if (contractStopDetail.NoeEjareID == null || contractStopDetail.NoeEjareID == 0)
                            return false;

                    }
                    return true;
                }

                else
                    return false;

            }).WithMessage("در لیست رسانه ها نوع توقف رسانه را وارد نمایید..");

            RuleFor(x =>new {x.Type,x.Ratio }).Must(BeAValidPostcode).WithMessage("ضریب را وارد نمایید");


            RuleFor(contract => contract.ContractStopDetails).Must((entity, contractStopDetails) =>
            {

                var countContractStopDetailItems = contractStopDetails.Count();

                if (countContractStopDetailItems > 0)
                {

                    foreach (var contractStopDetail in contractStopDetails)
                    {
                        using (var uow = new UnitOfWork())
                        {


                            var allSazes = uow.RepositoryAsync<Contract_Saze>().Queryable().Where(x => x.ContractID == entity.ContractID && x.SazeId == contractStopDetail.SazeID &&
                                      x.TarikhShorou <= contractStopDetail.StartDate.Date && x.TarikhPayan >= contractStopDetail.EndDate);

                            if (!contractStopDetails.Any())
                                return false;
                        }
                    }
                    return true;
                }
                else
                    return false;

            }).WithMessage("در لیست رسانه ها نوع توقف رسانه ی در بازه زمانی اکران نیست..");

          
        }

        private bool BeAValidPostcode(dynamic arg)
        {

            if (arg.Type == ContractStopType.RatioAddTime)
            {
                if (arg.Ratio == null)
                    return false;
            }

            return true;
        }

   
    }
}




