using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses;
using Zhivar.ServiceLayer.Accunting;
using Zhivar.ServiceLayer.Contracts.Accunting;
using System.Net;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ViewModel.Accunting;
using System.Net.Http;
using Newtonsoft.Json;
using Zhivar.Utilities;
using Zhivar.ServiceLayer.Contracts.Common;
using System.Web.Http;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.Accounting;
using Zhivar.ServiceLayer.Contracts.Accounting;
using OMF.Common.Security;
using OMF.Business;
using OMF.Enterprise.MVC;
using Zhivar.Business.BaseInfo;
using Zhivar.Business.Common;
using Zhivar.Business.Accounting;

namespace Zhivar.Web.Controllers.Accunting
{
    [RoutePrefix("api/Cash")]
    public partial class CashController : NewApiControllerBaseAsync<Cash, CashVM>
    {
        public CashRule Rule => this.BusinessRule as CashRule;

        protected override IBusinessRuleBaseAsync<Cash> CreateBusinessRule()
        {
            return new CashRule();
        }

        [Route("GetAllByOrganId")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetAllByOrganId()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var list = await Rule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId); 

            CashVM cashVM = new CashVM();
            List<CashVM> cashVMs = new List<CashVM>();

            foreach (var item in list)
            {
                var cashAccount = accounts.Where(x => x.ComplteCoding == "1101" + item.Code).SingleOrDefault();

                TransactionRule transactionRule = new TransactionRule();
                BalanceModelVM transaction = await transactionRule.GetBalanceAccountAsync(cashAccount.ID);
                cashVM = new CashVM();

                cashVM.Balance = item.Balance;
                cashVM.Code = item.Code;
                cashVM.DetailAccount = new DetailAccount()
                {
                    Accounts = new List<AccountVM>() {
                        new AccountVM()
                        {
                            Balance = transaction.Balance,
                            BalanceType = transaction.BalanceType,
                            Code = cashAccount.Coding,
                            Coding = cashAccount.Coding,
                            credit = transaction.Credit,
                            debit = transaction.Debit,
                            ID = cashAccount.ID,
                            ParentId = cashAccount.ParentId,
                            Name = cashAccount.Name,
                            
                        }
                    },
                    Code = cashAccount.Coding,
                    Id = cashAccount.ID,
                    Balance = transaction.Balance,
                    credit = transaction.Credit,
                    debit = transaction.Debit,
                    BalanceType = transaction.BalanceType,
                    Name = cashAccount.Name,
                    
                };
                cashVM.ID = item.ID;
                cashVM.Name = item.Name;
                cashVM.OrganId = item.OrganId;

                cashVMs.Add(cashVM);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = cashVMs });
        }

        [Route("GetNewCashObject")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetNewCashObject()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var list = await Rule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

            var Accounts = new List<AccountVM>();
            Accounts.Add(new AccountVM()
            {
                Balance = 5000000, BalanceType = 0, Code = "01", Coding = "1101", GroupCode = "1", ID = 3,
                Level = ZhivarEnums.AccountType.Moen, LevelString = "معین", Name = "صندوق ها", ParentCoding = "11", SystemAccount = 7,
                SystemAccountName = "صندوق", credit = 0, debit = 0
            });
            CashVM cashVM = new CashVM() {
                Balance = 0,
                Code = await CreateCashCode(organId),
                DetailAccount = new DetailAccount()
                {
                    Accounts = Accounts,
                    Balance = 0,
                    BalanceType = 0,
                    Code = await CreateCashCode(organId),
                    Id = 0,
                    Name = "",
                    Node = new Node()
                    {
                        FamilyTree = "صندوق ها",
                        Id = 3,
                        Name = "صندوق ها",
                        Parent = null,
                        Parents = ",3,",
                        SystemAccount = 3
                    },
                    RelatedAccounts = ",3,",
                    credit = 0,
                    debit = 0,

                },
                ID = 0,
                Name = "",
                
            };

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = cashVM });
        }

        [Route("GetCashById")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetCashById([FromBody] int id)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var cash = await Rule.FindAsync(id);

            CashVM cashVM = new CashVM()
            {
                Balance = cash.Balance,
                Code = cash.Code,
                DetailAccount = new DetailAccount()
                {
                    Accounts = null,
                    Code = cash.Code,
                    Id = cash.ID,
                },
                ID = cash.ID,
                Name = cash.Name,
                OrganId = cash.OrganId,
            };

            

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = cashVM });
        }

        [Route("Add")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> Add(CashVM cashVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var cash = new Cash();

            Mapper.Map(cashVM, cash);

            cash.OrganId = organId;
            AccountRule accountRule = new AccountRule();

         
            if (cashVM.ID > 0)
            {
                Rule.Update(cash);
            }
            else
            {
                Rule.Insert(cash);

                var accounts = await accountRule.GetAllByOrganIdAsync(organId);

                var accountCash = accounts.Where(x => x.ComplteCoding == "1101").SingleOrDefault();

                DomainClasses.Accounting.Account tempAccountCash = new DomainClasses.Accounting.Account();
                tempAccountCash.Coding = cash.Code;
                tempAccountCash.ComplteCoding = "1101" + cash.Code;
                tempAccountCash.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountCash.Name = cash.Name;
                tempAccountCash.OrganId = organId;
                tempAccountCash.ParentId = accountCash.ID;

                this.BusinessRule.UnitOfWork.Repository<DomainClasses.Accounting.Account>().Insert(tempAccountCash);
                //await accountRule.SaveChangesAsync();
            }

            await this.BusinessRule.UnitOfWork.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful , data = cash });// RedirectToAction(MVC.cash.ActionNames.Index);
        }

 
        [Route("Delete")]
        [HttpPost]
        public async Task<HttpResponseMessage> Delete([FromBody]int id)
        {

            var item = await Rule.FindAsync(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.NotFound, data = "" });


            await Rule.DeleteAsync(id);

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = item });

        }

        private async Task<string> CreateCashCode(int organId)
        {
            var count = 0;
            var cashlst = await Rule.GetAllByOrganIdAsync(organId);

            count = cashlst.Count();
            count++;
            string code = "";

            if (count < 10)
            {
                code = "00000" + count;
            }
            else if (count < 100)
            {
                code = "0000" + count;
            }
            else if (count < 1000)
            {
                code = "000" + count;
            }
            else if (count < 10000)
            {
                code = "00" + count;
            }
            else if (count < 100000)
            {
                code = "0" + count;
            }
            else
            {
                code = count.ToString();
            }

            return code;
        }
    }
}