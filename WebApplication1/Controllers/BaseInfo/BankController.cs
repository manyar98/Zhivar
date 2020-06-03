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
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ServiceLayer.Contracts.Accounting;
using OMF.Common.Security;
using OMF.Enterprise.MVC;
using OMF.Business;
using Zhivar.Business.BaseInfo;
using Zhivar.Business.Accounting;
using Zhivar.Business.Common;

namespace Zhivar.Web.Controllers.Accounting
{
    [RoutePrefix("api/Bank")]
    public partial class BankController : NewApiControllerBaseAsync<Bank, BankVM>
    {
        public BankRule Rule => this.BusinessRule as BankRule;

        protected override IBusinessRuleBaseAsync<Bank> CreateBusinessRule()
        {
            return new BankRule();
        }

        [Route("GetAllByOrganId")]
        [HttpPost]
        public  async Task<HttpResponseMessage> GetAllByOrganId()
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                BankRule bankRule = new BankRule();
                var list = await bankRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

                AccountRule accountRule = new AccountRule();
                var accounts = await accountRule.GetAllByOrganIdAsync(organId);

                BankVM bankVM = new BankVM();
                List<BankVM> bankVMs = new List<BankVM>();

                foreach (var item in list)
                {

                    var bankAccount = accounts.Where(x => x.ComplteCoding == "1103" + item.Code).SingleOrDefault();
                    TransactionRule transactionRule = new TransactionRule();
                    BalanceModelVM transaction = await transactionRule.GetBalanceAccountAsync(bankAccount.ID);

                    bankVMs.Add(new BankVM()
                    {
                        AccountNumber = item.AccountNumber,
                        Balance = item.Balance,
                        Branch = item.Branch,
                        Code = item.Code,
                        FullName = item.FullName,
                        ID = item.ID,
                        Name = item.Name,
                        DetailAccount = new DetailAccount()
                        {
                            Accounts = new List<AccountVM>() {
                        new AccountVM()
                        {
                            Balance = transaction.Balance,
                            BalanceType = transaction.BalanceType,
                            Code = bankAccount.Coding,
                            Coding = bankAccount.Coding,
                            credit = transaction.Credit,
                            debit = transaction.Debit,
                            ID = bankAccount.ID,
                            ParentId = bankAccount.ParentId,
                            Name = bankAccount.Name,

                        }
                    },
                            Code = bankAccount.Coding,
                            Id = bankAccount.ID,
                            Balance = transaction.Balance,
                            credit = transaction.Credit,
                            debit = transaction.Debit,
                            BalanceType = transaction.BalanceType,
                            Name = bankAccount.Name,

                        }
                    });

                }

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = bankVMs });
            }
            catch (Exception ex)
            {

                throw;
            }
      
        }

        [Route("GetAllByOrganIdAndType")]
        [HttpPost]
        public  async Task<HttpResponseMessage> GetAllByOrganIdAndType([FromBody] string type)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            BankRule bankRule = new BankRule();
            var list = await bankRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            BankVM bankVM = new BankVM();
            List<BankVM> bankVMs = new List<BankVM>();

            foreach (var item in list)
            {
                var bankAccount = accounts.Where(x => x.ComplteCoding == "1103" + item.Code).SingleOrDefault();

                if (type == "inProgress")
                {
                    bankAccount = accounts.Where(x => x.ComplteCoding == "1106" + item.Code).SingleOrDefault();
                }
                else if (type == "payables")
                {
                    bankAccount = accounts.Where(x => x.ComplteCoding == "2102" + item.Code).SingleOrDefault();
                }

                TransactionRule transactionRule = new TransactionRule();
                BalanceModelVM transaction = await transactionRule.GetBalanceAccountAsync(bankAccount.ID);
                
                bankVMs.Add(new BankVM()
                {
                    AccountNumber = item.AccountNumber,
                    Balance = item.Balance,
                    Branch = item.Branch,
                    Code = item.Code,
                    FullName = item.FullName,
                    ID = item.ID,
                    Name = item.Name,
                    DetailAccount = new DetailAccount()
                    {
                        Accounts = new List<AccountVM>() {
                        new AccountVM()
                        {
                            Balance = transaction.Balance,
                            BalanceType = transaction.BalanceType,
                            Code = bankAccount.Coding,
                            Coding = bankAccount.Coding,
                            credit = transaction.Credit,
                            debit = transaction.Debit,
                            ID = bankAccount.ID,
                            ParentId = bankAccount.ParentId,
                            Name = bankAccount.Name,

                        }
                    },
                        Code = bankAccount.Coding,
                        Id = bankAccount.ID,
                        Balance = transaction.Balance,
                        credit = transaction.Credit,
                        debit = transaction.Debit,
                        BalanceType = transaction.BalanceType,
                        Name = bankAccount.Name,

                    }
                });

            }

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = bankVMs });
        }

        [Route("GetNewBankObject")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetNewBankObject()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            //   var list = await _sandoghService.GetAllByOrganIdAsync(Convert.ToInt32(organId));

            var Accounts = new List<AccountVM>();
            Accounts.Add(new AccountVM()
            {
                Balance = 0,
                BalanceType = 2,
                Code = "03",
                Coding = "1103",
                GroupCode = "1",
                ID = 5,
                Level = ZhivarEnums.AccountType.Moen,
                LevelString = "معین",
                Name = "بانک ها",
                ParentCoding = "11",
                SystemAccount = 8,
                SystemAccountName = "بانک",
                credit = 0,
                debit = 0,


            });
            Accounts.Add(new AccountVM()
            {
                Balance = 0,
                BalanceType = 2,
                Code = "02",
                Coding = "2102",
                GroupCode = "2",
                ID = 23,
                Level = ZhivarEnums.AccountType.Moen,
                LevelString = "معین",
                Name = "اسناد پرداختنی",
                ParentCoding = "21",
                SystemAccount = 12,
                SystemAccountName = "اسناد پرداختنی",
                credit = 0,
                debit = 0,
            });

            Accounts.Add(new AccountVM()
            {
                Balance = 0,
                BalanceType = 2,
                Code = "06",
                Coding = "1106",
                GroupCode = "1",
                ID = 8,
                Level = ZhivarEnums.AccountType.Moen,
                LevelString = "معین",
                Name = "چک های در جریان وصول",
                ParentCoding = "11",
                SystemAccount = 10,
                SystemAccountName = "اسناد در جریان وصول",
                credit = 0,
                debit = 0,
            });

            BankVM bankVM = new BankVM()
            {
                AccountNumber = "",
                Balance = 0,
                Branch = "",
                Code = await CreateBankCode(organId),
                DetailAccount = new DetailAccount()
                {
                    Accounts = Accounts,
                    Balance = 0,
                    BalanceType = 0,
                    Code = "0002",
                    Id = 0,
                    Name = "",
                    Node = new Node {
                        FamilyTree = "حسابهای بانکی",
                        Id = 4,
                        Name = "حسابهای بانکی",
                        Parent = null,
                        Parents = ",4,",
                        SystemAccount = 4
                    },
                    RelatedAccounts = ",5,23,8,",
                    credit = 0,
                    debit = 0,

                },
                FullName = "",
                //Id = 0,
                Name = ""

            };

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = bankVM });
        }

        
        [Route("GetBankById")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetBankById([FromBody] int id)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);


            var bank = await Rule.FindAsync(id);

            BankVM bankVM = new BankVM()
                {
                    AccountNumber = bank.AccountNumber,
                    Balance = bank.Balance,
                    Branch = bank.Branch,
                    Code = bank.Code,
                    FullName = bank.FullName,
                    ID = bank.ID,
                    Name = bank.Name,
                    DetailAccount = new DetailAccount()
                    {
                        Code = bank.Code,
                        Id = bank.ID,
                    }
                };

            

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = bankVM });
        }
        [Route("Add")]
        [HttpPost]
        public  async Task<HttpResponseMessage> Add(BankVM bankVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.ValidationError, data = "" });
            }

            var bank = new Bank();
            Mapper.Map(bankVM, bank);

            bank.OrganId = organId;

            if (bankVM.ID.HasValue)
            {
                Rule.Update(bank);
            }
            else
            {
                Rule.Insert(bank);

                AccountRule accountRule = new AccountRule();
                var accounts = await accountRule.GetAllByOrganIdAsync(organId);

                var accountBank = accounts.Where(x => x.ComplteCoding == "1103").SingleOrDefault();

                DomainClasses.Accounting.Account tempAccountBank = new DomainClasses.Accounting.Account();
                tempAccountBank.Coding = bank.Code;
                tempAccountBank.ComplteCoding = "1103" + bank.Code;
                tempAccountBank.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountBank.Name = bank.Name;
                tempAccountBank.OrganId = organId;
                tempAccountBank.ParentId = accountBank.ID;

                this.BusinessRule.UnitOfWork.Repository<DomainClasses.Accounting.Account>().Insert(tempAccountBank);



                var accountAsnadPardakhtani = accounts.Where(x => x.ComplteCoding == "2102").SingleOrDefault();

                DomainClasses.Accounting.Account tempAccountAsnadPardakhtani = new DomainClasses.Accounting.Account();
                tempAccountAsnadPardakhtani.Coding = bank.Code;
                tempAccountAsnadPardakhtani.ComplteCoding = "2102" + bank.Code;
                tempAccountAsnadPardakhtani.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountAsnadPardakhtani.Name = bank.Name;
                tempAccountAsnadPardakhtani.OrganId = organId;
                tempAccountAsnadPardakhtani.ParentId = accountAsnadPardakhtani.ID;

               // accountRule.Insert(tempAccountAsnadPardakhtani);
                this.BusinessRule.UnitOfWork.Repository<DomainClasses.Accounting.Account>().Insert(tempAccountAsnadPardakhtani);


                var accountChequeDarJareanVosol = accounts.Where(x => x.ComplteCoding == "1106").SingleOrDefault();

                DomainClasses.Accounting.Account tempAccountChequeDarJareanVosol = new DomainClasses.Accounting.Account();
                tempAccountChequeDarJareanVosol.Coding = bank.Code;
                tempAccountChequeDarJareanVosol.ComplteCoding = "1106" + bank.Code;
                tempAccountChequeDarJareanVosol.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountChequeDarJareanVosol.Name = bank.Name;
                tempAccountChequeDarJareanVosol.OrganId = organId;
                tempAccountChequeDarJareanVosol.ParentId = accountChequeDarJareanVosol.ID;

                //accountRule.Insert(tempAccountChequeDarJareanVosol);
                this.BusinessRule.UnitOfWork.Repository<DomainClasses.Accounting.Account>().Insert(tempAccountChequeDarJareanVosol);
            }

            await this.BusinessRule.UnitOfWork.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = bank });

        }

        //[Route("Post")]
        [HttpPost]
        public async Task<HttpResponseMessage> Post(BankVM bankVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = "" });


            var bank = new Bank();

            Mapper.Map(bankVM, bank);

            bank.OrganId = organId;

            Rule.Insert(bank);

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = bank });
        }

        [HttpPut]
        //[Route("Update/{id =int}")]
        public  async Task<HttpResponseMessage> Update(int id, Bank bank)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var item = await Rule.FindAsync(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.NotFound, data = "" });



            if (!ModelState.IsValid || id != item.ID)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = "" });

            bank.OrganId = organId;

            Rule.Update(bank);

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = bank });

        }

        [Route("Delete")]
        [HttpPost]
        public  async Task<HttpResponseMessage> Delete([FromBody]int id)
        {

            var item = Rule.Find(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.NotFound, data = "" });

            await Rule.DeleteAsync(id);

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = "" });

        }

        private async Task<string> CreateBankCode(int organId)
        {
            var count = 0;
            var banklst = await Rule.GetAllByOrganIdAsync(organId);

            count = banklst.Count();
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