using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using OMF.EntityFramework.Ef6;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Contract;

namespace Zhivar.DataLayer.Validation
{
    public class ReservationValidate : AbstractValidator<Reservation>
    {
        public ReservationValidate()
        {
            RuleFor(contract => contract.ContactID).NotNull().NotEqual(0).WithMessage("طرف حساب باید مشخص شود");
            RuleFor(contract => contract.DisplayRegisterDate).NotNull().WithMessage("تاریخ رزرو باید مشخص شود");
            RuleFor(contract => contract.RegisterDate).NotNull().GreaterThanOrEqualTo(DateTime.Now.AddDays(-1)).WithMessage("تاریخ رزرو باید بزرگتر یا مساوی تاریخ روز باشد.");
           // RuleFor(contract => contract.RegisterDate).NotNull().GreaterThan(DateTime.Now).WithMessage("تاریخ رزرو باید بزرگتر از تاریخ روز باشد.");
            RuleFor(contract => contract.ReservationDetails).NotNull().WithMessage("هیچ رسانه در رزرو ثبت نشده است");
            RuleFor(contract => contract.ValiditDuration).NotNull().NotEqual(0).WithMessage("مدت اعتبار باید مشخص شود");
            RuleFor(contract => contract.ReservationDetails).Must((entity, ReservationDetails) =>
            {

                var items = ReservationDetails.Count();

                if (items > 0)
                    return true;
                else
                    return false;

            }).WithMessage("هیچ رسانه در رزرو ثبت نشده است.");

            RuleFor(contract => contract.ReservationDetails).Must((entity, ReservationDetails) =>
            {

                var items = ReservationDetails.Count();

                if (items > 0)
                {

                    foreach (var reservationDetail in ReservationDetails)
                    {
                        if (reservationDetail.SazeID == null || reservationDetail.SazeID == 0)
                            return false;

                    }
                    return true;
                }

                else
                    return false;

            }).WithMessage("در لیست رسانه ها رسانه را وارد نمایید..");

            RuleFor(contract => contract.ReservationDetails).Must((entity, reservationDetails) =>
            {

                var countItems = reservationDetails.Count();

                if (countItems > 0)
                {

                    foreach (var reservationDetail in reservationDetails)
                    {
                        if (reservationDetail.StartDate == null || reservationDetail.StartDate == DateTime.MinValue)
                            return false;

                    }
                    return true;
                }

                else
                    return false;

            }).WithMessage("در لیست رسانه ها تاریخ شروع رسانه را وارد نمایید..");

            RuleFor(contract => contract.ReservationDetails).Must((entity, reservationDetails) =>
            {

                var countReservationDetailsItems = reservationDetails.Count();

                if (countReservationDetailsItems > 0)
                {

                    foreach (var reservationDetail in reservationDetails)
                    {
                        if (reservationDetail.Quantity == null || reservationDetail.Quantity == 0)
                            return false;

                    }
                    return true;
                }

                else
                    return false;

            }).WithMessage("در لیست رسانه ها مدت اجاره رسانه را وارد نمایید..");


            RuleFor(contract => contract.ReservationDetails).Must((entity, reservationDetails) =>
            {

                var countReservationItems = reservationDetails.Count();

                if (countReservationItems > 0)
                {

                    foreach (var reservationDetail in reservationDetails)
                    {
                        if (reservationDetail.NoeEjareID == null || reservationDetail.NoeEjareID == 0)
                            return false;

                    }
                    return true;
                }

                else
                    return false;

            }).WithMessage("در لیست رسانه ها نوع اجاره رسانه را وارد نمایید..");

            RuleFor(contract => contract.ReservationDetails).Must((entity, reservationDetails) =>
            {

                var countReservationDetails = reservationDetails.Count();

                if (countReservationDetails > 0)
                {

                    foreach (var reservationDetail in reservationDetails)
                    {
                        if (reservationDetail.StartDate.Date < DateTime.Now.Date)
                            return false;

                    }
                    return true;
                }

                else
                    return true;

            }).WithMessage("در لیست رسانه ها تاریخ شروع اکران رسانه باید بزرگتر از تاریخ روز باشد");

            //RuleFor(contract => contract.ReservationDetails).Must((entity, reservationDetails) =>
            //{
            
            //    var countContractSazesItems = reservationDetails.Count();

            //    if (countContractSazesItems > 0)
            //    {

            //        foreach (var reservationDetail in reservationDetails)
            //        {
            //            using (var uow = new UnitOfWork())
            //            {
            //                var allSazes = uow.RepositoryAsync<Contract_Saze>().Queryable().Where(x => x.SazeId == reservationDetail.SazeID &&
            //                                            ((x.TarikhShorou >= reservationDetail.StartDate && x.TarikhShorou <= reservationDetail.EndDate) ||
            //                                             (x.TarikhShorou <= reservationDetail.StartDate && x.TarikhPayan >= reservationDetail.StartDate) ||
            //                                              x.TarikhShorou <= reservationDetail.StartDate && x.TarikhPayan >= reservationDetail.StartDate) ||
            //                                              x.TarikhShorou >= reservationDetail.StartDate && x.TarikhPayan <= reservationDetail.EndDate);

            //                var contractStopDetails = uow.RepositoryAsync<ContractStopDetails>().Queryable().Where(x => x.SazeID == reservationDetail.SazeID &&
            //                          ((x.StartDate <= reservationDetail.StartDate.Date && x.StartDate >= reservationDetail.EndDate) ||
            //                           (x.StartDate >= reservationDetail.StartDate.Date && x.EndDate <= reservationDetail.StartDate.Date) ||
            //                            x.StartDate >= reservationDetail.StartDate.Date && x.EndDate <= reservationDetail.StartDate.Date) ||
            //                            x.StartDate <= reservationDetail.StartDate.Date && x.EndDate >= reservationDetail.EndDate);

            //                if (contractStopDetails.Any())
            //                    return true;
            //                if (allSazes.Any())
            //                {
            //                    return false;
            //                }
            //                else
            //                {
            //                    var reservation_Details = uow.RepositoryAsync<Reservation_Detail>().Queryable().Where(x => x.SazeID == reservationDetail.SazeID &&
            //                        ((x.StartDate >= reservationDetail.StartDate && x.StartDate <= reservationDetail.EndDate) ||
            //                         (x.StartDate <= reservationDetail.StartDate && x.EndDate >= reservationDetail.StartDate) ||
            //                          x.StartDate <= reservationDetail.StartDate && x.EndDate >= reservationDetail.StartDate) ||
            //                          x.StartDate >= reservationDetail.StartDate && x.EndDate <= reservationDetail.EndDate);

            //                    if (reservation_Details.Any())
            //                    {
            //                        return false;
            //                    }
            //                    else
            //                        return true;
            //                }

            //            }

            //        }
            //        return true;
            //    }

            //    else
            //        return true;

            //}).WithMessage("در لیست رسانه ها رسانه ای در لیست رسانه در اجاره و یا رزرو شخص دیگری می باشد");

        }
    }
}
