using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Zhivar.DataLayer.Context;
using Zhivar.DataLayer.Validation;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.Common;
using Zhivar.Utilities;
using Zhivar.ViewModel.Accunting;
using Zhivar.ViewModel.BaseInfo;
using Zhivar.ViewModel.Common;
using FluentValidation;
using Zhivar.ViewModel.Accounting;
using Zhivar.DomainClasses.BaseInfo;
using OMF.Common.Security;
using OMF.Enterprise.MVC;
using OMF.Business;
using Zhivar.Business.Accounting;
using Zhivar.Business.Common;
using Zhivar.Business.BaseInfo;
using OMF.EntityFramework.Ef6;
using static Zhivar.DomainClasses.ZhivarEnums;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Workflow;
using OMF.Workflow.Model;
using static OMF.Workflow.Enums;
using static OMF.Common.Enums;
using OMF.Common.Extensions;
using Zhivar.ViewModel.Contract;
using Zhivar.Business.Contract;
using Zhivar.Web.Controllers.Accounting;
using Zhivar.DomainClasses.Contract;
using OMF.Security.Model;
using Zhivar.ViewModel.Security;
using Zhivar.Business.Security;
using System.Globalization;

namespace Zhivar.Web.Controllers.Contract
{
    [RoutePrefix("api/Reservation")]
    public class ReservationController : NewApiControllerBaseAsync<Reservation, ReservationVM>
    {
        public ReservationRule Rule => this.BusinessRule as ReservationRule;

        protected override IBusinessRuleBaseAsync<Reservation> CreateBusinessRule()
        {
            return new ReservationRule();
        }


        [HttpPost]
        [Route("GetReservations")]
        public async Task<HttpResponseMessage> GetReservations()
        {

            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var reservationlist = await Rule.GetAllByOrganIdAsync(organId);

                foreach (var reservation in reservationlist)
                {
                    var diff = DateTime.Now - reservation.RegisterDate;


                    if (diff.TotalDays <= reservation.ValiditDuration)
                        reservation.Color = 1;
                    else if (diff.TotalDays <= reservation.ValiditDuration * 2)
                        reservation.Color = 2;
                    else if (diff.TotalDays <= reservation.ValiditDuration * 3)
                        reservation.Color = 3;
                }
                //reservationQuery = reservationQuery.OrderByDescending(x => x.ID).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = reservationlist });

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost]
        [Route("SaveReservation")]
        public  async Task<HttpResponseMessage> SaveReservation([FromBody] ReservationVM reservationVM)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

       
                reservationVM.ReservationDetails = reservationVM.ReservationDetails.Where(x => x.Saze != null).ToList();

                foreach (var reservationDetailVM in reservationVM.ReservationDetails)
                {
                    if (reservationDetailVM.Saze != null)
                        reservationDetailVM.SazeID = reservationDetailVM.Saze.ID;

                    if (reservationDetailVM.NoeEjare != null)
                        reservationDetailVM.NoeEjareID = reservationDetailVM.NoeEjare.ID;

                    if (reservationDetailVM.StartDisplayDate != null)
                        reservationDetailVM.StartDate = PersianDateUtils.ToDateTime(reservationDetailVM.StartDisplayDate);

                    if (reservationDetailVM.NoeEjareID == 1)
                        reservationDetailVM.EndDate = reservationDetailVM.StartDate.AddDays((double)reservationDetailVM.Quantity -1);
                    else if (reservationDetailVM.NoeEjareID == 2)
                    {
                        PersianCalendar pc = new PersianCalendar();
                        reservationDetailVM.EndDate = pc.AddMonths(reservationDetailVM.StartDate, (int)reservationDetailVM.Quantity);
                  
                    }

                    reservationDetailVM.EndDisplayDate = PersianDateUtils.ToPersianDateTime(reservationDetailVM.EndDate);
                }

                
                if (reservationVM.Contact != null)
                    reservationVM.ContactID = reservationVM.Contact.ID;

                Reservation reservation = new Reservation();
                Mapper.Map(reservationVM, reservation);
                reservation.OrganID = organId;

                reservation.RegisterDate = DateTime.Now;
                reservation.DisplayRegisterDate = PersianDateUtils.ToPersianDateTime(DateTime.Now);

            

                ReservationValidate validator = new ReservationValidate();
                FluentValidation.Results.ValidationResult results = validator.Validate(reservation);

                string failurs = "";

                if (!results.IsValid)
                {
                    foreach (var error in results.Errors)
                    {
                        failurs += "<br/>" + error.ErrorMessage;

                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failurs });
                }

                foreach (var reservationDetailVM in reservationVM.ReservationDetails)
                {
                    using (var uow = new UnitOfWork())
                    {

                        reservationDetailVM.StartDate = PersianDateUtils.ToDateTime(reservationDetailVM.StartDisplayDate);

                        if (reservationDetailVM.NoeEjareID == 1)
                        {
                            reservationDetailVM.EndDate = reservationDetailVM.StartDate.AddDays((double)reservationDetailVM.Quantity);
                            reservationDetailVM.EndDate = reservationDetailVM.EndDate.AddDays(-1);
                        }
                        else if (reservationDetailVM.NoeEjareID == 2)
                        {
                            PersianCalendar pc = new PersianCalendar();
                            reservationDetailVM.EndDate = pc.AddMonths(reservationDetailVM.StartDate, (int)reservationDetailVM.Quantity);
                            reservationDetailVM.EndDate = reservationDetailVM.EndDate.AddDays(-1);

                        }

                        var contractsRentFromIds = await uow.RepositoryAsync<DomainClasses.Contract.Contract>().Queryable().Where(x => (x.ContractType == ContractType.RentFrom) && x.OrganId == organId).Select(x => x.ID).ToListAsync2();

                        var allSazesRentFrom = uow.RepositoryAsync<Contract_Saze>().Queryable().Where(x => contractsRentFromIds.Contains(x.ContractID) && x.SazeId == reservationDetailVM.SazeID &&
                                                    ((x.TarikhShorou >= reservationDetailVM.StartDate.Date && x.TarikhShorou <= reservationDetailVM.EndDate.Date) ||
                                                     (x.TarikhShorou <= reservationDetailVM.StartDate.Date && x.TarikhPayan >= reservationDetailVM.StartDate.Date) ||
                                                     (x.TarikhShorou <= reservationDetailVM.StartDate.Date && x.TarikhPayan >= reservationDetailVM.StartDate.Date) ||
                                                     (x.TarikhShorou >= reservationDetailVM.StartDate.Date && x.TarikhPayan <= reservationDetailVM.EndDate.Date)));

                        if (!await allSazesRentFrom.AnyAsync2())
                        {
                            string str = " این سازه از تاریخ " + reservationDetailVM.StartDisplayDate + " تا تاریخ " + PersianDateUtils.ToPersianDate(reservationDetailVM.EndDate) + " در اجاره شرکت نمی باشد. ";

                            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = str });
                        }

                        var contractsIds = await uow.RepositoryAsync<DomainClasses.Contract.Contract>().Queryable().Where(x => (x.ContractType == ContractType.PreContract || x.ContractType == ContractType.RentTo) && x.OrganId == organId).Select(x => x.ID).ToListAsync2();
                        var allSazes = uow.RepositoryAsync<Contract_Saze>().Queryable().Where(x => contractsIds.Contains(x.ContractID) && x.SazeId == reservationDetailVM.SazeID &&
                                                    ((x.TarikhShorou >= reservationDetailVM.StartDate.Date && x.TarikhShorou <= reservationDetailVM.EndDate.Date) ||
                                                     (x.TarikhShorou <= reservationDetailVM.StartDate.Date && x.TarikhPayan >= reservationDetailVM.StartDate.Date) ||
                                                     (x.TarikhShorou <= reservationDetailVM.StartDate.Date && x.TarikhPayan >= reservationDetailVM.StartDate.Date) ||
                                                     (x.TarikhShorou >= reservationDetailVM.StartDate.Date && x.TarikhPayan <= reservationDetailVM.EndDate.Date)));

                        var contractStopDetails = uow.RepositoryAsync<ContractStopDetails>().Queryable().Where(x => x.SazeID == reservationDetailVM.SazeID &&
                                               x.StartDate <= reservationDetailVM.StartDate.Date && x.EndDate > reservationDetailVM.EndDate.Date);

                        //if (await contractStopDetails.AnyAsync2())
                        //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = "" });

                        if (await allSazes.AnyAsync2())
                        {
                            var p = await allSazes.FirstOrDefaultAsync2();
                            var contract = await uow.RepositoryAsync<DomainClasses.Contract.Contract>().Queryable().Where(x => x.ID == p.ContractID).SingleOrDefaultAsync2();

                            string noeEjareStr = "ماه";
                            if (p.NoeEjareId == 1)
                                noeEjareStr = "روز";
                            string str = " این سازه از تاریخ " + p.DisplayTarikhShorou + " به مدت " + ((int)p.Quantity).ToString() + " " + noeEjareStr + " در اجاره قرارداد شماره " + Convert.ToInt32(contract.Number).ToString() + " با عنوان " + contract.ContractTitle + " می باشد. ";
                            //string str = " می باشد. " + contract.ContactTitle + " با عنوان " + contract.Number + " در اجاره قرارداد شماره " + noeEjareStr + p.Quantity + " به مدت " + p.DisplayTarikhShorou + " این سازه از تاریخ ";
                            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = str });
                        }
                        else
                        {
                            var reservationsIds = uow.RepositoryAsync<Reservation>().Queryable().Where(x => x.OrganID == organId).Select(x => x.ID).ToList();

                            var reservation_Details = uow.RepositoryAsync<Reservation_Detail>().Queryable().Where(x => reservationsIds.Contains(x.ReservationID) && x.SazeID == reservationDetailVM.SazeID &&
                                ((x.StartDate >= reservationDetailVM.StartDate.Date && x.StartDate <= reservationDetailVM.EndDate.Date) ||
                                 (x.StartDate <= reservationDetailVM.StartDate.Date && x.EndDate >= reservationDetailVM.StartDate.Date) ||
                                 (x.StartDate <= reservationDetailVM.StartDate.Date && x.EndDate >= reservationDetailVM.StartDate.Date) ||
                                 (x.StartDate >= reservationDetailVM.StartDate.Date && x.EndDate <= reservationDetailVM.EndDate.Date)));

                            if (await reservation_Details.AnyAsync2())
                            {
                                var reservation_Detail = await reservation_Details.FirstOrDefaultAsync2();
                                var reservationTemp = await uow.RepositoryAsync<Reservation>().Queryable().Where(x => x.ID == reservation_Detail.ReservationID).SingleOrDefaultAsync2();

                                string noeEjareStr = "ماه";
                                if (reservation_Detail.NoeEjareID == 1)
                                    noeEjareStr = "روز";
                                string str = " این سازه از تاریخ " + reservation_Detail.StartDisplayDate + " به مدت " + ((int)reservation_Detail.Quantity).ToString() + " " + noeEjareStr + " رزرو می باشد. ";// + Convert.ToInt32(reservation.).ToString() + " با عنوان " + reservation. + " می باشد. ";
                                                                                                                                                                                                              //string str = " می باشد. " + contract.ContactTitle + " با عنوان " + contract.Number + " در اجاره قرارداد شماره " + noeEjareStr + p.Quantity + " به مدت " + p.DisplayTarikhShorou + " این سازه از تاریخ ";
                                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = str });
                            }
                        
                        }

                    }
                }
                


                if (reservation.ID > 0)
                {
                    foreach (var reservationDetail in reservation.ReservationDetails)
                    {
                        reservationDetail.StartDate = PersianDateUtils.ToDateTime(reservationDetail.StartDisplayDate);
                        reservationDetail.ReservationID = reservation.ID;

                        if (reservationDetail.ID > 0)
                        {
                            reservationDetail.ReservationID = reservation.ID;
                            reservationDetail.ObjectState = ObjectState.Modified;
                        }

                        else
                        {
                            reservationDetail.ReservationID = reservation.ID;
                            reservationDetail.ObjectState = ObjectState.Added;
                        }

                    }

                    reservation.ObjectState = ObjectState.Modified;
                }

                else
                {

                    foreach (var reservationDetail in reservation.ReservationDetails)
                    {
                        reservationDetail.ReservationID = reservation.ID;
                        reservationDetail.StartDate = PersianDateUtils.ToDateTime(reservationDetail.StartDisplayDate);

                        if (reservationDetail.ID > 0)
                        {
                            reservationDetail.ReservationID = reservation.ID;
                            reservationDetail.ObjectState = ObjectState.Modified;
                        }

                        else
                        {
                            reservationDetail.ReservationID = reservation.ID;
                            reservationDetail.ObjectState = ObjectState.Added;
                        }
                    
                    }


                    reservation.ObjectState = ObjectState.Added;
                }


                this.BusinessRule.UnitOfWork.RepositoryAsync<Reservation>().InsertOrUpdateGraph(reservation);

                await this.BusinessRule.UnitOfWork.SaveChangesAsync();


                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = reservation });
            }
            catch (Exception ex)
            {
                var p = ex;
                throw;
            }

        }

        [HttpPost]
        [Route("DeleteReservations")]
        public async Task<HttpResponseMessage> DeleteReservations([FromBody] string strIds)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                string failurs = "";

                string[] values = strIds.Split(',');
                for (int i = 0; i < values.Length-1 ; i++)
                {

                    var id = Convert.ToInt32(values[i].Trim());

                    await Rule.DeleteAsync(id);
                    await this.BusinessRule.UnitOfWork.SaveChangesAsync();
                    // }
                }



                if (!string.IsNullOrEmpty(failurs))
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failurs });
                else
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = "" });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost]
        [Route("DeleteDetailReservations")]
        public async Task<HttpResponseMessage> DeteleDetailReservations([FromBody] string strIds)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                string failurs = "";

                string[] values = strIds.Split(',');
                for (int i = 0; i < values.Length-1; i++)
                {

                    var id = Convert.ToInt32(values[i].Trim());

                    await this.BusinessRule.UnitOfWork.RepositoryAsync<Reservation_Detail>().DeleteAsync(id);
                    await this.BusinessRule.UnitOfWork.SaveChangesAsync();
                    // }
                }



                if (!string.IsNullOrEmpty(failurs))
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failurs });
                else
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = "" });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost]
        [Route("TamdidReservations")]
        public async Task<HttpResponseMessage> TamdidReservations([FromBody] string strIds)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                string failurs = "";

                string[] values = strIds.Split(',');
                for (int i = 0; i < values.Length; i++)
                {

                    var id = Convert.ToInt32(values[i].Trim());

                    var reservation =  await Rule.FindAsync(id);
                    reservation.RegisterDate = DateTime.Now;
                    reservation.DisplayRegisterDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now);

                    Rule.Update(reservation);
                    await this.Rule.SaveChangesAsync();
                   
                }



                if (!string.IsNullOrEmpty(failurs))
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failurs });
                else
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = "" });
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        
        public async Task<HttpResponseMessage> LoadReservationTransObj([FromBody] int id)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var reservation = await Rule.FindAsync(id);
                ReservationTransObj reservationTransObj = new ReservationTransObj();

                reservationTransObj.Reservation = Mapper.Map<ReservationVM>(reservation);
                reservationTransObj.ReservationDetails = reservationTransObj.Reservation.ReservationDetails;
 

                for (int i = 0; i < reservationTransObj.ReservationDetails.Count; i++)
                {
                    ItemRule itemRule = new ItemRule();
                    reservationTransObj.ReservationDetails[i].Saze = Mapper.Map<SazeVM>(await itemRule.FindAsync(reservationTransObj.ReservationDetails[i].SazeID));
                }

                ReservationRule reservationRule = new ReservationRule();

                reservationTransObj.Reservation.Contact = Mapper.Map<ContactVM>(await reservationRule.FindAsync(reservation.ContactID));
                

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = reservationTransObj });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<HttpResponseMessage> loadReservationData([FromBody] loadReservationDataBusi reservationData)
        {
            try
            {


                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var resualt = new ReservationData();

                List<ContactVM> contacts = new List<ContactVM>();
                ContactRule contactRule = new ContactRule();

                var contactsSource = await contactRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                contactsSource = contactsSource.ToList();

                contacts = TranslateHelper.TranslateEntityToEntityVMListContact(contactsSource);

                foreach (var contact in contacts)
                {
                    contact.DetailAccount = new DetailAccount()
                    {
                        Code = contact.Code,
                        Id = (int)contact.ID,
                        Node = new Node()
                        {
                            FamilyTree = "اشخاص",
                            Id = (int)contact.ID,
                            Name = "اشخاص"
                        }
                    };
                }



                resualt.contacts = contacts;
        


                NoeEjareRule noeEjareRule = new NoeEjareRule();
                var noeEjares = await noeEjareRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                resualt.noeEjares = noeEjares;

                GoroheSazeRule goroheSazeRule = new GoroheSazeRule();
                var itemGroups = await goroheSazeRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

                var items = new List<SazeVM>();
                var item = new SazeVM();

                foreach (var itemGroup in itemGroups)
                {
                    foreach (var KalaKhadmat in itemGroup.Items)
                    {
                        item = new SazeVM()
                        {
                            DetailAccount = new DetailAccount()
                            {
                                Code = KalaKhadmat.Code,
                                Id = KalaKhadmat.ID,
                                Node = new Node()
                                {
                                    FamilyTree = itemGroup.Title,
                                    Name = itemGroup.Title,
                                    Id = itemGroup.ID
                                }
                            },
                            Code = KalaKhadmat.Code,
                            OrganId = KalaKhadmat.OrganId,
                            Address = KalaKhadmat.Address,
                            Arz = KalaKhadmat.Arz,
                            GoroheSazeID = KalaKhadmat.GoroheSazeID,
                            ID = KalaKhadmat.ID,
                            Title = KalaKhadmat.Title,
                            Tol = KalaKhadmat.Tol,
                            NoeSazeId = KalaKhadmat.NoeSazeId,
                            NoeEjare = KalaKhadmat.NoeEjare,
                            GoroheName = KalaKhadmat.GoroheName,
                            NoeEjareName = KalaKhadmat.NoeEjareName,
                            NoeSazeName = KalaKhadmat.NoeSazeName,
                            Latitude = KalaKhadmat.Latitude,
                            Longitude = KalaKhadmat.Longitude,
                            NoorDard = KalaKhadmat.NoorDard,
                            NoeEjareID = KalaKhadmat.NoeEjareID,

                        };

                        items.Add(item);
                    }
                }

                resualt.items = items;

                var Reservation_DetailVMs = new List<Reservation_DetailVM>();

                var reservation_DetailVM = new List<Reservation_DetailVM>();
         
                if (reservationData.id == 0)
                {
                    var countRes = 0;
                    if (reservationData.lstSaze != null && reservationData.lstSaze.Count > 0)
                    {
                        reservationData.lstSaze = reservationData.lstSaze.Where(x => x.sazeID != 0).ToList();

                        foreach (var lstSaze in reservationData.lstSaze)
                        {
                            var saze = Mapper.Map<Saze, SazeVM>(this.BusinessRule.UnitOfWork.Repository<Saze>().Find(lstSaze.sazeID));

                            var startDate = DateTime.Now;
                            var displayStartDate = PersianDateUtils.ToPersianDateTime(DateTime.Now);

                            bool minValue = false;
                            bool maxValue = false;

                            if (!string.IsNullOrEmpty(lstSaze.minDate) && !string.IsNullOrWhiteSpace(lstSaze.minDate))
                            {
                                startDate = PersianDateUtils.ToDateTime(lstSaze.minDate);
                                displayStartDate = lstSaze.minDate;
                                minValue = true;
                            }

                            var endDate = DateTime.Now;
                            var displayEndDate = PersianDateUtils.ToPersianDateTime(DateTime.Now);

                            if (!string.IsNullOrEmpty(lstSaze.maxDate) && !string.IsNullOrWhiteSpace(lstSaze.maxDate))
                            {
                                endDate = PersianDateUtils.ToDateTime(lstSaze.maxDate);
                                displayStartDate = lstSaze.maxDate;
                                maxValue = true;
                            }

                            var quantity = 0;

                            if (minValue && maxValue)
                            {
                                var diff = endDate - startDate;
                                var days = diff.TotalDays;

                                quantity = Convert.ToInt32(days);

                            }


                            Reservation_DetailVMs.Add(new Reservation_DetailVM()
                            {
                                ID = 0,
                                Quantity = quantity,
                                Saze = saze,
                                SazeID = saze.ID,
                                NoeEjare = Mapper.Map<NoeEjare, NoeEjareVM>(this.BusinessRule.UnitOfWork.Repository<NoeEjare>().Find(saze.NoeEjareID)),
                                NoeEjareID = saze.NoeEjareID,
                                RowNumber = 0,
                                StartDate = startDate,
                                StartDisplayDate = displayStartDate,
                                //EndDate = endDate,
                                //EndDisplayDate = lstSaze.maxDate,
                                

                            });
                            countRes += 1;
                        }


                        if (countRes < 3)
                        {
                            for (int i = 0; i < 3 - countRes; i++)
                            {
                                Reservation_DetailVMs.Add(new Reservation_DetailVM()
                                {
                                    ID = 0,
                                    Quantity = 0,
                                    Saze = null,
                                    SazeID = 0,
                                    NoeEjare = null,
                                    NoeEjareID = 0,
                                    RowNumber = countRes + i,
                                    //StartDate = startDate,
                                    StartDisplayDate = "",
                                    //EndDate = endDate,
                                    //EndDisplayDate = lstSaze.maxDate,


                                });
                            }

                        }
                    }
                    else
                    {
                        Reservation_DetailVMs.Add(new Reservation_DetailVM()
                        {
                            ID = 0,
                            Quantity = 0,
                            RowNumber = 0

                        });

                        Reservation_DetailVMs.Add(new Reservation_DetailVM()
                        {
                            ID = 0,
                            Quantity = 0,
                            RowNumber = 1

                        });

                        Reservation_DetailVMs.Add(new Reservation_DetailVM()
                        {
                            ID = 0,
                            Quantity = 0,
                            RowNumber = 2

                        });

                        Reservation_DetailVMs.Add(new Reservation_DetailVM()
                        {
                            ID = 0,
                            Quantity = 0,
                            RowNumber = 3

                        });
                    }

              


                    resualt.Reservation = new ReservationVM()
                    {
                        //Contact = Mapper.Map<Contact, ContactVM>(this.BusinessRule.UnitOfWork.Repository<Contact>().Find(reservation.ContactID)),

                        ID = 0,
                        DisplayRegisterDate = Utilities.PersianDateUtils.ToPersianDateTime(DateTime.Now),
                       // ContactID = reservation.ContactID,
                        OrganID = organId,
                        RegisterDate = DateTime.Now,

                        ReservationDetails = Reservation_DetailVMs
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });

                }
                else
                {
                    var reservation = await Rule.FindAsync(reservationData.id);

                    foreach (var reservationDetail in reservation.ReservationDetails ?? new List<Reservation_Detail>())
                    {

                        Reservation_DetailVMs.Add(new Reservation_DetailVM()
                        {
                            ID = reservationDetail.ID,
                            Saze = Mapper.Map<Saze, SazeVM>(this.BusinessRule.UnitOfWork.Repository<Saze>().Find(reservationDetail.SazeID)),
                            SazeID = reservationDetail.SazeID,
                            Quantity = reservationDetail.Quantity,
                            NoeEjare = Mapper.Map<NoeEjare, NoeEjareVM>(this.BusinessRule.UnitOfWork.Repository<NoeEjare>().Find(reservationDetail.NoeEjareID)),
                            NoeEjareID = reservationDetail.NoeEjareID,
                            EndDate = reservationDetail.EndDate,
                            EndDisplayDate = reservationDetail.EndDisplayDate,
                            ReservationID = reservationDetail.ReservationID,
                            StartDate = reservationDetail.StartDate,
                            StartDisplayDate = reservationDetail.StartDisplayDate,
                            UnitPrice = reservationDetail.UnitPrice,
                            PriceBazareab = reservationDetail.PriceBazareab,
                            PriceTarah = reservationDetail.PriceTarah,
                            PriceChap = reservationDetail.PriceChap,
                            PriceNasab = reservationDetail.PriceNasab,
                            RowNumber = reservationDetail.RowNumber
                        });

                    }


                    resualt.Reservation = new ReservationVM()
                    {
                        Contact = Mapper.Map<Contact, ContactVM>(this.BusinessRule.UnitOfWork.Repository<Contact>().Find(reservation.ContactID)),
                     
                        ID = reservation.ID,
                        DisplayRegisterDate = reservation.DisplayRegisterDate,
                        ContactID = reservation.ContactID,
                        OrganID = reservation.OrganID,
                        RegisterDate = reservation.RegisterDate,
                        ValiditDuration = reservation.ValiditDuration,
                        ReservationDetails = Reservation_DetailVMs
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });
                }
               

            }
            catch (Exception ex)
            {

                throw;
            }
        }

    


    }
    public class loadReservationDataBusi
    {
        public int id { get; set; }
        public List<SazeInfo> lstSaze { get; set; }
    }

    public class SazeInfo
    {
        public int sazeID { get; set; }
        public string minDate { get; set; }
        public string maxDate { get; set; }
    }


}
