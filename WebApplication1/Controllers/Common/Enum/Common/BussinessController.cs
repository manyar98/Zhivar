using AutoMapper;
using OMF.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Common;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.ViewModel.Accunting;
using Zhivar.ViewModel.Common;
using Zhivar.Business.Common;
using Zhivar.Business.Accounting;
using OMF.Enterprise.MVC;
using OMF.Business;

namespace Zhivar.Web.Controllers.Common
{
    public class BussinessController : NewApiControllerBaseAsync<Bussiness, BussinessVM>
    {
        public BussinessSiteRule Rule => this.BusinessRule as BussinessSiteRule;

        protected override IBusinessRuleBaseAsync<Bussiness> CreateBusinessRule()
        {
            return new BussinessSiteRule();
        }
        [Route("GetAllByOrganId")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetAllByOrganId([FromBody] string type)
        {
            var userId = SecurityManager.CurrentUserContext.UserId;
            PersonRule personRule = new PersonRule();
            var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

            var list = await Rule.GetAllByOrganIdAsync(Convert.ToInt32(person.ID));
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Successful, data = list });

        }


        //[Route("AddBussiness")]
        //[HttpPost]
        //public virtual async Task<HttpResponseMessage> AddBussiness([FromBody] BussinessVM bussinessVM)
        //{
        //    var userId = SecurityManager.CurrentUserContext.UserId;
        //    var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Exception, data = bussinessVM });

        //    }

        //    var bussiness = new Bussiness();

        //    Mapper.Map(bussinessVM, bussiness);

        //    bussiness.RegistrationDate = DateTime.Now;

        //    bussiness.OrganId = person.ID;

        //    if (bussinessVM.ID.HasValue && bussiness.ID > 0)
        //    {
        //        _bussinessService.Update(bussiness);
        //    }
        //    else
        //    {
        //        _bussinessService.Insert(bussiness);


        //        var accounts = await accountRule.GetAllByOrganIdAsync(person.ID);

        //        var accountDreaftani = accounts.Where(x => x.ComplteCoding == "1104").SingleOrDefault();

        //        DomainClasses.Accounting.Account tempAccountDreaftani = new DomainClasses.Accounting.Account();
        //        tempAccountDreaftani.Coding = bussiness.Code;
        //        tempAccountDreaftani.ComplteCoding = "1104" + bussiness.Code;
        //        tempAccountDreaftani.Level = Enums.AccountType.Tafzeli;
        //        tempAccountDreaftani.Name = bussiness.Name;
        //        tempAccountDreaftani.OrganId = person.ID;
        //        tempAccountDreaftani.ParentId = accountDreaftani.ID;

        //        accountRule.Insert(tempAccountDreaftani);

        //        var accountPardakhtani = accounts.Where(x => x.ComplteCoding == "2101").SingleOrDefault();

        //        DomainClasses.Accounting.Account tempAccountPardakhtani = new DomainClasses.Accounting.Account();
        //        tempAccountPardakhtani.Coding = bussiness.Code;
        //        tempAccountPardakhtani.ComplteCoding = "2101" + bussiness.Code;
        //        tempAccountPardakhtani.Level = Enums.AccountType.Tafzeli;
        //        tempAccountPardakhtani.Name = bussiness.Name;
        //        tempAccountPardakhtani.OrganId = person.ID;
        //        tempAccountPardakhtani.ParentId = accountPardakhtani.ID;

        //        accountRule.Insert(tempAccountPardakhtani);


        //        var accountAsnadDareaftani = accounts.Where(x => x.ComplteCoding == "1105").SingleOrDefault();

        //        DomainClasses.Accounting.Account tempAccountAsnadDareaftani = new DomainClasses.Accounting.Account();
        //        tempAccountAsnadDareaftani.Coding = bussiness.Code;
        //        tempAccountAsnadDareaftani.ComplteCoding = "1105" + bussiness.Code;
        //        tempAccountAsnadDareaftani.Level = Enums.AccountType.Tafzeli;
        //        tempAccountAsnadDareaftani.Name = bussiness.Name;
        //        tempAccountAsnadDareaftani.OrganId = person.ID;
        //        tempAccountAsnadDareaftani.ParentId = accountAsnadDareaftani.ID;

        //        accountRule.Insert(tempAccountAsnadDareaftani);
        //    }
        //    await _unitOfWork.SaveAllChangesAsync();

        //    bussinessVM.ID = bussiness.ID;
        //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Successful, data = bussinessVM });
        //}

        [Route("GetBussinessById")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetBussinessById([FromBody] int id)
        {
            var userId = SecurityManager.CurrentUserContext.UserId;

            PersonRule personRule = new PersonRule();
            var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

            var item = await Rule.FindAsync(id);

           
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Successful, data = item });

        }

        [Route("GetCurrentUserAndBusinesses")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetCurrentUserAndBusinesses()
        {
            var userId = SecurityManager.CurrentUserContext.UserId;

            PersonRule personRule = new PersonRule();

            var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

            FinanYearRule finanYearRule = new FinanYearRule();

            var finanYears = await finanYearRule.GetAllByOrganIdAsync(person.ID);

            BussinessSiteRule bussinessSiteRule = new BussinessSiteRule();
            var bussinesses = await bussinessSiteRule.GetAllByOrganIdAsync(person.ID);

            var responseCurrentUserAndBusinesses = new ResponseCurrentUserAndBusinesses();

            responseCurrentUserAndBusinesses.appVersion = "3.1";
            responseCurrentUserAndBusinesses.businesses = new List<Bussiness>();
            responseCurrentUserAndBusinesses.businesses = bussinesses.ToList();

            responseCurrentUserAndBusinesses.businessInfo = new BusinessInfo();
            responseCurrentUserAndBusinesses.businessInfo.business = bussinesses.SingleOrDefault();
            responseCurrentUserAndBusinesses.businessInfo.finanYear = finanYears.Where(x => x.Closed == false).SingleOrDefault();
            responseCurrentUserAndBusinesses.businessInfo.finanYears = finanYears.ToList();

            responseCurrentUserAndBusinesses.showCloseFinanYearAlert = false;
            responseCurrentUserAndBusinesses.todayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now);
            responseCurrentUserAndBusinesses.user = new Common.User()
            {
                FirstName = person.Nam,
                Id = person.ID,
                IsConfirmed = true,
                LastName = person.NamKhanvadegi,
                Phone = person.Tel,
                Name = person.Nam +" "+person.NamKhanvadegi,
                
            };

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Successful, data = responseCurrentUserAndBusinesses });
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<HttpResponseMessage> Delete([FromBody]List<Bussiness> bussinesss)
        {

            foreach (var bussiness in bussinesss)
            {
                var item = await Rule.FindAsync(bussiness.ID);

                if (item != null)
                {
                    await Rule.DeleteAsync(item);

                    await Rule.SaveChangesAsync();
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Successful, data = bussinesss });
        }

        [Route("SaveBusiness")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> SaveBusiness([FromBody] Bussiness bussiness)
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Successful, data = bussiness });
        }
        }
    
    public class ResponseCurrentUserAndBusinesses
    {
        public string appVersion { get; set; }
        public BusinessInfo businessInfo { get; set; }
        public List<Bussiness> businesses { get; set; }
        public bool showCloseFinanYearAlert { get; set; }
        public string todayDate { get; set; }
        public User user { get; set; }

    }

    public class BusinessInfo
    {
        public Bussiness business { get; set; }
        public string favReports { get; set; }
        public FinanYear finanYear { get; set; }
        public List<FinanYear> finanYears { get; set; }
        public string[] notifications { get; set; }
        public string permissions { get; set; }
       

    }
    public class User
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public int Id { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsTempRegister { get; set; }
        public string LastLoginDateString { get; set; }
        public string LastLoginTimeString { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string RegistrationDisplayDate { get; set; }
    }
}
