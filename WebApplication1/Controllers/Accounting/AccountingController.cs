using AutoMapper;
using OMF.Business;
using OMF.Common;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.Enterprise.MVC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Zhivar.Business.Accounting;
using Zhivar.Business.Common;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Account;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.Common;
using Zhivar.DomainClasses.Contract;
using Zhivar.ServiceLayer.Contracts.Accounting;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.ViewModel.Accunting;
using Zhivar.ViewModel.Common;
using Zhivar.ViewModel.Contract;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.Web.Controllers.Accounting
{

    [RoutePrefix("api/Accounting")]
    public class AccountingController : NewApiControllerBaseAsync<Zhivar.DomainClasses.Accounting.Account, AccountVM>
    {
        public AccountRule Rule => this.BusinessRule as AccountRule;

        protected override IBusinessRuleBaseAsync<DomainClasses.Accounting.Account> CreateBusinessRule()
        {
            return new AccountRule();
        }


        [Route("GetAll")]
        public virtual async Task<HttpResponseMessage> GetAll()
        {
            AccountRule accountRule = new AccountRule();
            var list = await accountRule.Queryable().ToListAsync();

            List<AccountVM> accountVMs = new List<AccountVM>();

            foreach (var item in list)
            {
                accountVMs.Add(new AccountVM()
                {
                    Coding = item.Coding,
                    Name = item.Name,
                    ID = item.ID,
                    ParentId = item.ParentId,
                    Level = item.Level

                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = accountVMs });
        }


        [Route("GetOtherAccountsByGroup4OpeningBalance")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetOtherAccountsByGroup4OpeningBalance([FromBody]string groupCode)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            if (groupCode == "1")
            {
                List<string> ids = new List<string>() { "1102", "1107", "1109", "1110", "1111" };
                accounts = accounts.Where(x => ids.Contains(x.ComplteCoding)).ToList();
            }
            else
            {
                List<string> ids = new List<string>() { "2103", "2104", "2105", "2106", "2201" };
                accounts = accounts.Where(x => ids.Contains(x.ComplteCoding)).ToList();
            }


            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = accounts });
          
        }

        [Route("GetIncomeAccounts")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetIncomeAccounts()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);
     

            AccountRule accountRule = new AccountRule();
            var list = await accountRule.GetAllByOrganIdAsync(organId);
            List<int> childIds = list.Where(x => x.ComplteCoding == "71" || x.ComplteCoding == "72").Select(x => x.ID).ToList();
            list = list.Where(x => childIds.Contains(x.ParentId)).ToList();

            List<AccountVM> accountVMs = new List<AccountVM>();

            foreach (var item in list)
            {
                accountVMs.Add(new AccountVM()
                {
                    Coding = item.Coding,
                    ComplteCoding = item.ComplteCoding,
                    Name = item.Name,
                    ID = item.ID,
                    ParentId = item.ParentId,
                    Level = item.Level

                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = accountVMs });
        }

        [Route("GetExpenseAccounts")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetExpenseAccounts()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var list = await accountRule.GetAllByOrganIdAsync(organId);
            var parents = list.Where(x => x.ComplteCoding == "81" || x.ComplteCoding == "82" || x.ComplteCoding == "83").ToList();
            list = list.Where(x => x.ParentId == parents[0].ID || x.ParentId == parents[1].ID || x.ParentId == parents[2].ID).ToList();

            List<AccountVM> accountVMs = new List<AccountVM>();

            foreach (var item in list)
            {
                accountVMs.Add(new AccountVM()
                {
                    ComplteCoding = item.ComplteCoding,
                    Coding = item.Coding,
                    Name = item.Name,
                    ID = item.ID,
                    ParentId = item.ParentId,
                    Level = item.Level

                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = accountVMs });
        }


        [Route("GetAccountsLevel3")]
        public virtual async Task<HttpResponseMessage> GetAccountsLevel3()
        {
            AccountRule accountRule = new AccountRule();
            var list = await accountRule.Queryable().ToListAsync();
            list = list.Where(x => x.Level == AccountType.Moen).ToList();

            List<AccountVM> accountVMs = new List<AccountVM>();

            foreach (var item in list)
            {
                accountVMs.Add(new AccountVM()
                {
                    Coding = item.Coding,
                    Name = item.Name,
                    ID = item.ID,
                    ParentId = item.ParentId,
                    Level = item.Level

                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = accountVMs });
        }

        [Route("GetAccountsByLevel")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetAccountsByLevel([FromBody] int level)
        {
            AccountRule accountRule = new AccountRule();
            var list = await accountRule.Queryable().ToListAsync();
            list = list.Where(x => x.Level == (AccountType)level).ToList();

            List<AccountVM> accountVMs = new List<AccountVM>();

            foreach (var item in list)
            {
                accountVMs.Add(new AccountVM()
                {
                    Coding = item.Coding,
                    ComplteCoding = item.ComplteCoding,
                    Name = item.Name,
                    ID = item.ID,
                    ParentId = item.ParentId,
                    Level = item.Level

                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = accountVMs });
        }

        //[Route("GetOtherAccountsByGroup4OpeningBalance")]
        //[HttpPost]
        //public virtual async Task<HttpResponseMessage> GetOtherAccountsByGroup4OpeningBalance([FromBody]string groupCode)
        //{
        //    var userId = SecurityManager.CurrentUserContext.UserId;
        //    var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

        //    var accounts = await accountRule.GetAllByOrganIdAsync(organId);

        //    if (groupCode == "1")
        //    {
        //        List<string> ids = new List<string>() { "1102", "1107", "1109", "1110", "1111" };
        //        accounts = accounts.Where(x => ids.Contains(x.ComplteCoding)).ToList();
        //    }
        //    else
        //    {
        //        List<string> ids = new List<string>() { "2103", "2104", "2105", "2106", "2201" };
        //        accounts = accounts.Where(x => ids.Contains(x.ComplteCoding)).ToList();
        //    }



        //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = accounts });
        //}

        [Route("GetFinanAccounts")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetFinanAccounts()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            accounts = accounts.Where(x => x.Level != AccountType.Tafzeli).ToList();
            var responseFinanAccounts = new ResponseFinanAccounts();

            responseFinanAccounts.systemAccountTypes = new[] {  "حساب های دریافتنی","حساب های پرداختنی",
                        "صندوق","بانک","تنخواه گردان","اسناد دریافتنی","اسناد پرداختنی","اسناد در جریان وصول",
                        "موجودی کالا","فروش","خرید","برگشت از فروش","برگشت از خرید","مالیات بر ارزش افزوده",
                        "مالیات بر ارزش افزوده فروش","مالیات بر ارزش افزوده خرید","مالیات بر درآمد","فروش خدمات",
                        "هزینه خدمات خریداری شده","سرمایه اولیه","افزایش یا کاهش سرمایه","برداشت","سهم سود و زیان",
                        "تخفیفات نقدی خرید","تخفیفات نقدی فروش","هزینه ضایعات کالا","کنترل ضایعات کالا","حقوق","تراز افتتاحیه",
                        "تراز اختتامیه","خلاصه سود و زیان","سود انباشته"
                    };

            responseFinanAccounts.finanAccounts = new List<FinanAccount>();

            var finanAccounts = new FinanAccount();

            foreach (var account in accounts)
            {
                var parentAccount = await accountRule.FindAsync(account.ParentId);

                finanAccounts = new FinanAccount();

                finanAccounts.Balance = 0;
                finanAccounts.BalanceType = 0;
                finanAccounts.Code = account.Coding;
                finanAccounts.Coding = account.ComplteCoding;
                finanAccounts.credit = 0;
                finanAccounts.debit = 0;
                finanAccounts.Id = account.ID;
                finanAccounts.Level = account.Level;
                finanAccounts.LevelString = account.Level.GetPersianTitle();
                finanAccounts.Name = account.Name.Replace("(گروه)", "").Replace("(کل)", "").Replace("(معین)", "");

                if (parentAccount != null)
                    finanAccounts.ParentCoding = parentAccount.ComplteCoding;
                finanAccounts.SystemAccountName = account.Name;
                responseFinanAccounts.finanAccounts.Add(finanAccounts);
            }



            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = responseFinanAccounts });
        }


        [Route("GetFinanAccountsBalance")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetFinanAccountsBalance()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            accounts = accounts.Where(x => x.Level == AccountType.Moen).ToList();
            var responseFinanAccounts = new ResponseFinanAccounts();

            responseFinanAccounts.systemAccountTypes = new[] {  "حساب های دریافتنی","حساب های پرداختنی",
                        "صندوق","بانک","تنخواه گردان","اسناد دریافتنی","اسناد پرداختنی","اسناد در جریان وصول",
                        "موجودی کالا","فروش","خرید","برگشت از فروش","برگشت از خرید","مالیات بر ارزش افزوده",
                        "مالیات بر ارزش افزوده فروش","مالیات بر ارزش افزوده خرید","مالیات بر درآمد","فروش خدمات",
                        "هزینه خدمات خریداری شده","سرمایه اولیه","افزایش یا کاهش سرمایه","برداشت","سهم سود و زیان",
                        "تخفیفات نقدی خرید","تخفیفات نقدی فروش","هزینه ضایعات کالا","کنترل ضایعات کالا","حقوق","تراز افتتاحیه",
                        "تراز اختتامیه","خلاصه سود و زیان","سود انباشته"
                    };

            responseFinanAccounts.finanAccounts = new List<FinanAccount>();

            var finanAccounts = new FinanAccount();

            foreach (var account in accounts)
            {
                var parentAccount = await accountRule.FindAsync(account.ParentId);
                var parentParentAccount = await accountRule.FindAsync(parentAccount.ParentId);

                ResponseAccountBalance res = new ResponseAccountBalance();
                if (parentParentAccount.ComplteCoding == "1" || parentParentAccount.ComplteCoding == "5" || parentParentAccount.ComplteCoding == "8")
                    res = await CalcAccountByCodeAsync(organId, account.ComplteCoding, BalanceType.Debit);
                else
                    res = await CalcAccountByCodeAsync(organId, account.ComplteCoding, BalanceType.Credit);


                finanAccounts = new FinanAccount();

                finanAccounts.Balance = res.Balance;
                finanAccounts.BalanceType = res.Type;
                finanAccounts.Code = account.Coding;
                finanAccounts.Coding = account.ComplteCoding;
                finanAccounts.credit = res.SumCredit;
                finanAccounts.debit = res.SumDebit;
                finanAccounts.Id = account.ID;
                finanAccounts.Level = account.Level;
                finanAccounts.LevelString = "معین";
                finanAccounts.Name = account.Name.Replace("(گروه)", "").Replace("(کل)", "").Replace("(معین)", "");

                if (parentAccount != null)
                    finanAccounts.ParentCoding = parentAccount.ComplteCoding;
                finanAccounts.SystemAccountName = account.Name;
                responseFinanAccounts.finanAccounts.Add(finanAccounts);
            }



            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = responseFinanAccounts });
        }

        [Route("GetAccountDetailAccounts")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetAccountDetailAccounts([FromBody] DomainClasses.Accounting.Account item)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            //accounts = accounts.Where(x => x.Level != AccountType.Tafzeli).ToList();


            accounts = accounts.Where(x => x.ParentId == item.ID).ToList();



            var responseAccountDetailAccounts = new List<ResponseAccountDetailAccount>();

            var responseAccountDetailAccount = new ResponseAccountDetailAccount();

            foreach (var account in accounts)
            {
                var parentAccount = await accountRule.FindAsync(account.ParentId);

                responseAccountDetailAccount = new ResponseAccountDetailAccount();

                responseAccountDetailAccount.Balance = 0;
                responseAccountDetailAccount.BalanceType = 0;
                responseAccountDetailAccount.Code = account.Coding;
                responseAccountDetailAccount.credit = 0;
                responseAccountDetailAccount.debit = 0;
                responseAccountDetailAccount.Id = account.ID;
                responseAccountDetailAccount.Name = account.Name.Replace("(گروه)", "").Replace("(کل)", "").Replace("(معین)", "");
                responseAccountDetailAccount.Node = new Node()
                {
                    Name = account.Name,
                    Id = account.ID,

                };


                responseAccountDetailAccounts.Add(responseAccountDetailAccount);
            }



            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = responseAccountDetailAccounts });
        }


        [Route("GetAccountsToExplore")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetAccountsToExplore([FromBody] RequestAccountsToExplore requestAccountsToExplore)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            if (requestAccountsToExplore.account == null)
                accounts = accounts.Where(x => x.Level == AccountType.Kol).ToList();

            var res = new List<ResponseAccountToExplore>();
            var finanAccount = new ResponseAccountToExplore();

            foreach (var account in accounts)
            {
                //var parentAccount = await accountRule.FindAsync(account.ParentId);
                finanAccount = new ResponseAccountToExplore();

                finanAccount.Coding = account.ComplteCoding;
                finanAccount.Credit = 0;
                finanAccount.SumCredit = 0;
                finanAccount.SumDebit = 0;
                finanAccount.Name = account.Name.Replace("(گروه)", "").Replace("(کل)", "").Replace("(معین)", "");



                res.Add(finanAccount);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = res });
        }

        private async Task<ResponseAccountBalance> CalcAccountByCodeAsync(int organId, string code, BalanceType balanceType)
        {
            var organId2 = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var responseAccountBalance = new ResponseAccountBalance();
            decimal amount = 0;

            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            var operatinIncomeAccount = accounts.Where(x => x.ComplteCoding == code).SingleOrDefault();

            var accountsMoienQuery = accounts.AsQueryable().Where(x => x.ParentId == operatinIncomeAccount.ID);

            var allAccountQuery = accounts.AsQueryable();

            List<int> childIds = (from account in accountsMoienQuery
                                  select account.ID).ToList();

            List<int> childChildIds = (from account in allAccountQuery
                                       join accountsMoien in accountsMoienQuery
                                       on account.ParentId equals accountsMoien.ID
                                       select account.ID).ToList();


            var selected = transactions.Where(a => a.AccountId == operatinIncomeAccount.ID || childIds.Contains(a.AccountId) || childChildIds.Contains(a.AccountId)).ToList();

            responseAccountBalance.Type = 2;
            if (selected.Any())
            {
                var credit = selected.Sum(x => x.Credit);
                var debit = selected.Sum(x => x.Debit);

                if (balanceType == BalanceType.Debit)
                {
                    amount = debit - credit;

                    if (amount > 0)
                    {
                        responseAccountBalance.Type = 0;
                    }
                    else
                    {
                        responseAccountBalance.Type = 1;
                    }
                }
                else if (balanceType == BalanceType.Credit)
                {
                    amount = credit - debit;
                    if (amount < 0)
                    {
                        responseAccountBalance.Type = 0;
                    }
                    else
                    {
                        responseAccountBalance.Type = 1;
                    }
                }

                responseAccountBalance.Balance = Math.Abs(amount);


            }

            return responseAccountBalance;
        }
    }

     

    public class FinanAccount
    {
        public decimal Balance { get; set; }
        public int BalanceType { get; set; }
        public string Code { get; set; }
        public string Coding { get; set; }
        public string GroupCode { get; set; }
        public int Id { get; set; }
        public AccountType Level { get; set; }
        public string LevelString { get; set; }
        public string Name { get; set; }
        public string ParentCoding { get; set; }
        public systemAccountTypes SystemAccount { get; set; }
        public string SystemAccountName { get; set; }
        public decimal credit { get; set; }
        public decimal debit { get; set; }
    }

    public class ResponseFinanAccounts
    {
        public List<FinanAccount> finanAccounts { get; set; }
        public string[] systemAccountTypes { get; set; }
    }

    public class ResponseAccountDetailAccount
    {
        public DomainClasses.Accounting.Account Accounts { get; set; }
        public decimal Balance { get; set; }
        public int BalanceType { get; set; }
        public string Code { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public Node Node { get; set; }
        public string RelatedAccounts { get; set; }
        public decimal credit { get; set; }
        public decimal debit{ get; set; }
}

    public class RequestAccountsToExplore
    {
        public DomainClasses.Accounting.Account account { get; set; }
        public string start { get; set; }
        public string end { get; set; }
    }

    public class ResponseAccountToExplore
    {
        public string Coding { get; set; }
        public string Name { get; set; }
        public decimal SumDebit { get; set; }
        public decimal SumCredit { get; set; }
        public decimal dataField { get; set; }
        public decimal Credit { get; set; }
				
    }

    public class ResponseAccountBalance
    {
        
        public decimal Balance { get; set; }
        public decimal SumDebit { get; set; }
        public decimal SumCredit { get; set; }
        public int Type { get; set; }
     
    }
}