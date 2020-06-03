using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Zhivar.DataLayer.Context;
using Zhivar.ServiceLayer.Contracts.Accunting;
using System.Net;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ViewModel.Accunting;
using System.Net.Http;
using Newtonsoft.Json;
using Zhivar.Utilities;
using Zhivar.ServiceLayer.Contracts.Common;
using System.Web.Http;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.ServiceLayer.Contracts.Accounting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using OMF.Common.Security;
using OMF.Enterprise.MVC;
using Zhivar.Business.Accounting;
using OMF.Business;
using Zhivar.Business.Common;
using Zhivar.Business.BaseInfo;

namespace Zhivar.Web.Controllers.Accounting
{

    public partial class FinanYearController : NewApiControllerBaseAsync<FinanYear, FinanYearVM>
    {


        public FinanYearRule Rule => this.BusinessRule as FinanYearRule;

        protected override IBusinessRuleBaseAsync<FinanYear> CreateBusinessRule()
        {
            return new FinanYearRule();
        }

        [HttpPost]
        [Route("GetFirstFinanYear")]
        public virtual async Task<HttpResponseMessage> GetFirstFinanYear()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            FinanYearRule finanYearRule = new FinanYearRule();
            var finanQuery = await finanYearRule.GetAllByOrganIdAsync(organId);
            var finanYear = finanQuery.Where(x => x.FirstYear == true).SingleOrDefault();


            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = finanYear });
        }
        [HttpPost]
        [Route("SaveFinanYear")]
        public virtual async Task<HttpResponseMessage> SaveFinanYear([FromBody] RequestFinanYear requestFinanYear)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            FinanYearRule finanYearRule = new FinanYearRule();
            var finanQuery = await finanYearRule.GetAllByOrganIdAsync(organId);
            var finanYear = finanQuery.Where(x => x.FirstYear == true).SingleOrDefault();


            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = requestFinanYear.finanYear });
        }

        [HttpPost]
        [Route("GetRequiredDataToClosingFinanYear")]
        public async Task<HttpResponseMessage> GetRequiredDataToClosingFinanYear()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var incomeAccount = accounts.Where(x => x.ComplteCoding == "71" || x.ComplteCoding == "72").Select(x => x.ID).ToList();
            var costAccount = accounts.Where(x => x.ComplteCoding == "81" || x.ComplteCoding == "82" || x.ComplteCoding == "83").Select(x => x.ID).ToList();

            decimal incomeAmount = 0;
            decimal costAmount = 0;

            var accountsMoienQuery = accounts.AsQueryable().Where(x => incomeAccount.Contains( x.ParentId ));

            var allAccountQuery = accounts.AsQueryable();

            List<int> childIds = (from account in accountsMoienQuery
                                  select account.ID).ToList();

            List<int> childChildIds = (from account in allAccountQuery
                                       join accountsMoien in accountsMoienQuery
                                       on account.ParentId equals accountsMoien.ID
                                       select account.ID).ToList();


            var selected = transactions.Where(a => incomeAccount.Contains(a.AccountId)  || childIds.Contains(a.AccountId) || childChildIds.Contains(a.AccountId)).ToList();

            if (selected.Any())
            {
                var credit = selected.Sum(x => x.Credit);
                var debit = selected.Sum(x => x.Debit);
                incomeAmount = credit - debit;
            }


            var accountsMoienCostQuery = accounts.AsQueryable().Where(x => costAccount.Contains(x.ParentId));


            List<int> childIdsCost = (from account in accountsMoienCostQuery
                                      select account.ID).ToList();

            List<int> childChildIdsCost = (from account in allAccountQuery
                                       join accountsMoien in accountsMoienCostQuery
                                       on account.ParentId equals accountsMoien.ID
                                       select account.ID).ToList();


            var selectedCost = transactions.Where(a => costAccount.Contains(a.AccountId) || childIdsCost.Contains(a.AccountId) || childChildIdsCost.Contains(a.AccountId)).ToList();

            if (selectedCost.Any())
            {
                var credit = selectedCost.Sum(x => x.Credit);
                var debit = selectedCost.Sum(x => x.Debit);
                costAmount = credit - debit;
            }

            FinanYearRule finanYearRule = new FinanYearRule();
            var finanQuery = await finanYearRule.GetAllByOrganIdAsync(organId);
            var finanYear = finanQuery.Where(x => x.FirstYear == true).SingleOrDefault();

            ShareholderRule shareholderRule = new ShareholderRule();
            var shareHolders = await shareholderRule.GetAllByOrganIdAsync(organId);

            var startDate = PersianDateUtils.ToDateTime(finanYear.DisplayStartDate);
            var endDate = PersianDateUtils.ToDateTime(finanYear.DisplayEndDate);

            double diff2 = (endDate - startDate).TotalDays;

            var startDateNew = endDate.AddDays(1);
            var endDateNew = endDate.AddDays(diff2);

            var displayEndDateNew = PersianDateUtils.ToPersianDateTime(endDateNew);
            var displayStartDateNew = PersianDateUtils.ToPersianDateTime(startDateNew);

            var startYear = Convert.ToInt32(displayStartDateNew.Substring(0, 4));
            var startMonth = Convert.ToInt32(displayStartDateNew.Substring(5, 2));
            var startDay = Convert.ToInt32(displayStartDateNew.Substring(8, 2));


            var endYear = Convert.ToInt32(displayEndDateNew.Substring(0, 4));
            var endMonth = Convert.ToInt32(displayEndDateNew.Substring(5, 2));
            var endDay = Convert.ToInt32(displayEndDateNew.Substring(8, 2));

            var isMoreThanYear = false;

            if (diff2 > 365)
                isMoreThanYear = true;

            var resualt = new DataToClosingFinanYear();

            resualt.closingDate = finanYear.DisplayEndDate;
            resualt.netIncome = incomeAmount - costAmount;

            resualt.newFinanYear = new FinanYearVM()
            {
                Closed = false,
                DisplayEndDate = displayEndDateNew,
                DisplayStartDate = displayStartDateNew,
                EndDate = endDateNew,
                EndDateDay = endDay,
                EndDateMonth = endMonth,
                EndDateYear = endYear,
                FirstYear = false,
                Id = 0,
                IsMoreThanOneYear = isMoreThanYear,
                Name = endYear+" سال مال منتهی به",
                Note = "",
                StartDate = startDate,
                StartDateDay = startDay,
                StartDateMonth = startMonth,
                StartDateYear = startYear,
            };
            resualt.shareholders = new List<ShareholderVM>();

            foreach (var shareHolder in shareHolders)
            {


                resualt.shareholders.Add(new ShareholderVM()
                {

                    Address = shareHolder.Address,
                    City = shareHolder.City,
                    Code = shareHolder.Code,
                    ContactEmail = "",
                    ContactType = shareHolder.ContactType,
                    Credits = shareHolder.Credits,
                    //DetailAccount = new DetailAccount()
                    //{
                    //    Accounts = null,
                    //    Balance = 0,
                    //    BalanceType = 0,
                    //    Code = "000001",
                    //    Id = 1,
                    //    Name = "مونا ابراهیمی",
                    //    Node = new Node()
                    //    {
                    //        FamilyTree = "اشخاص",
                    //        Id = 1,
                    //        Name = "اشخاص",
                    //        Parent = null,
                    //        Parents = ",1,",
                    //        SystemAccount = 1
                    //    },
                    //    RelatedAccounts = ",6,22,7,32,33,34,35,",
                    //    credit = 0,
                    //    debit = 0,
                    //},
                    Email = shareHolder.Email,
                    Fax = shareHolder.Fax,
                    FirstName = shareHolder.FirstName,
                    Id = (int)shareHolder.ID,
                    IsCustomer = shareHolder.IsCustomer,
                    IsEmployee = shareHolder.IsEmployee,
                    IsShareHolder = shareHolder.IsShareHolder,
                    IsVendor = shareHolder.IsVendor,
                    LastName = shareHolder.LastName,
                    Liability = shareHolder.Liability,
                    Mobile = shareHolder.Mobile,
                    Name = shareHolder.Name,
                    NationalCode = shareHolder.NationalCode,
                    Note = shareHolder.Note,
                    People = shareHolder.People,
                    Phone = shareHolder.Phone,
                    PostalCode = shareHolder.PostalCode,
                    Rating = shareHolder.Rating,
                    RegistrationDate = shareHolder.RegistrationDate,
                    RegistrationNumber = shareHolder.RegistrationNumber,
                    SharePercent = shareHolder.SharePercent,
                    State = shareHolder.State,
                    Website = shareHolder.Website,
                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });
        }
    }

    public class RequestFinanYear
    {
        public FinanYear finanYear { get; set; }
        public int calendar { get; set; }
    }
}