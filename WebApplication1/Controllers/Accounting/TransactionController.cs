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
using Zhivar.ViewModel.Common;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using OMF.Common.Security;
using OMF.Enterprise.MVC;
using OMF.Business;
using Zhivar.Business.Accounting;
using Zhivar.Business.Common;
using Zhivar.Business.BaseInfo;
using Zhivar.ViewModel.BaseInfo;
using OMF.EntityFramework.Ef6;
using static Zhivar.DomainClasses.ZhivarEnums;
using Zhivar.DomainClasses.Contract;

namespace Zhivar.Web.Controllers.Accounting
{
    [RoutePrefix("api/Transaction")]
    public partial class TransactionController : NewApiControllerBaseAsync<Transaction, TransactionVM>
    {
        public TransactionRule Rule => this.BusinessRule as TransactionRule;

        protected override IBusinessRuleBaseAsync<Transaction> CreateBusinessRule()
        {
            return new TransactionRule();
        }

        [HttpPost]
        [Route("GetCashTransactions")]
        public  async Task<HttpResponseMessage> GetCashTransactions([FromBody] CashTransactionVM cashTransactionVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            CashRule cashRule = new CashRule();
            var cash = await cashRule.FindAsync(cashTransactionVM.Id);
            var cashAccount = accounts.Where(x => x.ComplteCoding == "1101" + cash.Code).SingleOrDefault();

            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            transactions = transactions.Where(x => x.AccountId == cashAccount.ID).ToList();

            var resultTransactionCash = new ResultTransactionCash();
            resultTransactionCash.cash = new Cash()
            {
                Balance = cash.Balance,
                Code = cash.Code,
                ID = cash.ID,
                Name = cash.Name,
                OrganId = cash.OrganId
            };

            resultTransactionCash.ledger = new ledger();
            resultTransactionCash.ledger.Rows = new List<Row>();
            Row row = new Row();

            decimal totalRemain = 0;
            foreach (var transaction in transactions)
            {
                row = new Row();

                row.Code = transaction.Account.Coding;
                row.Credit = transaction.Credit;
                row.Date = PersianDateUtils.ToPersianDate(transaction.Date);
                row.Debit = transaction.Debit;
                row.Description = transaction.Description;
                row.DocId = transaction.DocumentId;
                row.DocNum = transaction.DocumentId;
                row.DtName = transaction.Account.Name;
                row.Reference = transaction.Reference;

                var remain = transaction.Debit - transaction.Credit;
                totalRemain += remain;

                if (totalRemain > 0)
                    row.RemainType = "بد";

                else
                    row.RemainType = "بس";


                row.Remain = totalRemain;

                resultTransactionCash.ledger.Rows.Add(row);

            }
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = resultTransactionCash });
        }

        [HttpPost]
        [Route("GetBankTransactions")]
        public  async Task<HttpResponseMessage> GetBankTransactions([FromBody] CashTransactionVM cashTransactionVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            BankRule bankRule = new BankRule();
            var bank = await bankRule.FindAsync(cashTransactionVM.Id);
            var bankAccount = accounts.Where(x => x.ComplteCoding == "1103" + bank.Code).SingleOrDefault();

            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            transactions = transactions.Where(x => x.AccountId == bankAccount.ID).ToList();


            var resultTransactionCash = new ResultTransactionCash();
            resultTransactionCash.cash = new Cash()
            {
                Balance = bank.Balance,
                Code = bank.Code,
                ID = bank.ID,
                Name = bank.Name,
                OrganId = bank.OrganId
            };

            resultTransactionCash.ledger = new ledger();
            resultTransactionCash.ledger.Rows = new List<Row>();
            Row row = new Row();


            decimal totalRemain = 0;

            foreach (var transaction in transactions)
            {
                DocumentRule documentRule = new DocumentRule();
                var document = await documentRule.FindAsync(transaction.DocumentId);


                row = new Row();

                row.Code = transaction.Account.Coding;
                row.Credit = transaction.Credit;
                row.Date = PersianDateUtils.ToPersianDate(transaction.Date);
                row.Debit = transaction.Debit;
                row.Description = document.Description;
                row.DocId = transaction.DocumentId;
                row.DocNum = transaction.DocumentId;
                row.DtName = transaction.Account.Name;
                row.Reference = transaction.Reference;

                var remain = transaction.Debit - transaction.Credit;
                totalRemain += remain;

                if (totalRemain > 0)
                    row.RemainType = "بد";

                else
                    row.RemainType = "بس";


                row.Remain = totalRemain;

                resultTransactionCash.ledger.Rows.Add(row);

            }
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = resultTransactionCash });
        }

        [HttpPost]
        [Route("GetCapitalStatement")]
        public  async Task<HttpResponseMessage> GetCapitalStatement()
        {
            var organId = Convert.ToInt32( SecurityManager.CurrentUserContext.OrganizationId);
   

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            var capitalStatement = new CapitalStatement();

            capitalStatement.reportDate = PersianDateUtils.ToPersianDate(DateTime.Now);
            capitalStatement.tableCapitals = new List<tableCapital>();

            #region سرمایه اولیه
            var initialInvestment = accounts.Where(x => x.ComplteCoding == "3101").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childInitialInvestmentIds = accounts.Where(x => x.ParentId == initialInvestment.ID).Select(x => x.ID).ToList();

            foreach (var childInitialInvestmentId in childInitialInvestmentIds)
            {
                decimal amounInitialInvestment = 0;
                var initialInvestmentQuery = transactions.Where(x => x.AccountId == childInitialInvestmentId).AsQueryable();

                if (initialInvestmentQuery.Any())
                {
                    var credit = initialInvestmentQuery.Sum(x => x.Credit);
                    var debit = initialInvestmentQuery.Sum(x => x.Debit);

                    var initialInvestment1 = initialInvestmentQuery.FirstOrDefault();
                    amounInitialInvestment = credit - debit;
                    var type = "Credit";
                    if (amounInitialInvestment > 0)
                        type = "Credit";
                    else
                        type = "Debit";

                    capitalStatement.tableCapitals.Add(new tableCapital()
                    {
                        Code = initialInvestment1.Account.Coding,
                        Amount = amounInitialInvestment,
                        Date = PersianDateUtils.ToPersianDate(initialInvestment1.Date),
                        Description = initialInvestment1.Description + " - " + initialInvestment1.Account.Name,
                        Type = type,

                    });
                }
            }



            #endregion
            #region کاهش یا افزایش سرمایه
            var reduceOrIncreaseCapital = accounts.Where(x => x.ComplteCoding == "3102").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childReduceOrIncreaseCapitalIds = accounts.Where(x => x.ParentId == reduceOrIncreaseCapital.ID).Select(x => x.ID).ToList();

            foreach (var childReduceOrIncreaseCapitalId in childReduceOrIncreaseCapitalIds)
            {
                decimal amounReduceOrIncrease = 0;
                var childReduceOrIncreaseQuery = transactions.Where(x => x.AccountId == childReduceOrIncreaseCapitalId).AsQueryable();

                if (childReduceOrIncreaseQuery.Any())
                {
                    var credit = childReduceOrIncreaseQuery.Sum(x => x.Credit);
                    var debit = childReduceOrIncreaseQuery.Sum(x => x.Debit);

                    var childReduceOrIncrease1 = childReduceOrIncreaseQuery.FirstOrDefault();
                    amounReduceOrIncrease = credit - debit;
                    var type = "Credit";
                    if (amounReduceOrIncrease > 0)
                        type = "Credit";
                    else
                        type = "Debit";

                    capitalStatement.tableCapitals.Add(new tableCapital()
                    {
                        Code = childReduceOrIncrease1.Account.Coding,
                        Amount = amounReduceOrIncrease,
                        Date = Utilities.PersianDateUtils.ToPersianDate(childReduceOrIncrease1.Date),
                        Description = childReduceOrIncrease1.Description + " - " + childReduceOrIncrease1.Account.Name,
                        Type = type,

                    });
                }
            }



            #endregion
            #region برداشت ها
            var removal = accounts.Where(x => x.ComplteCoding == "3103").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childRemovalIds = accounts.Where(x => x.ParentId == removal.ID).Select(x => x.ID).ToList();

            foreach (var childRemovalId in childRemovalIds)
            {
                decimal amounRemoval = 0;
                var childRemovalQuery = transactions.Where(x => x.AccountId == childRemovalId).AsQueryable();

                if (childRemovalQuery.Any())
                {
                    var credit = childRemovalQuery.Sum(x => x.Credit);
                    var debit = childRemovalQuery.Sum(x => x.Debit);

                    var childRemoval1 = childRemovalQuery.FirstOrDefault();
                    amounRemoval = credit - debit;
                    var type = "Credit";
                    if (amounRemoval > 0)
                        type = "Credit";
                    else
                        type = "Debit";

                    capitalStatement.tableCapitals.Add(new tableCapital()
                    {
                        Code = childRemoval1.Account.Coding,
                        Amount = amounRemoval,
                        Date = Utilities.PersianDateUtils.ToPersianDate(childRemoval1.Date),
                        Description = childRemoval1.Description + " - " + childRemoval1.Account.Name,
                        Type = type,

                    });
                }
            }

            #endregion
            #region سهم سود و زیان
            var share = accounts.Where(x => x.ComplteCoding == "3104").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childShareIds = accounts.Where(x => x.ParentId == share.ID).Select(x => x.ID).ToList();
            bool hasProfit = false;
            foreach (var childShareId in childShareIds)
            {
                decimal amounShare = 0;
                var childShareQuery = transactions.Where(x => x.AccountId == childShareId).AsQueryable();

                if (childShareQuery.Any())
                {
                    var credit = childShareQuery.Sum(x => x.Credit);
                    var debit = childShareQuery.Sum(x => x.Debit);

                    var childShare1 = childShareQuery.FirstOrDefault();
                    amounShare = credit - debit;
                    var type = "Credit";
                    if (amounShare > 0)
                        type = "Credit";
                    else
                        type = "Debit";
                    hasProfit = true;

                    capitalStatement.tableCapitals.Add(new tableCapital()
                    {
                        Code = childShare1.Account.Coding,
                        Amount = amounShare,
                        Date = Utilities.PersianDateUtils.ToPersianDate(childShare1.Date),
                        Description = childShare1.Description + " - " + childShare1.Account.Name,
                        Type = type,

                    });
                }
            }


            #endregion

            #region 2سهم سود و زیان
            decimal amountSells = await CalcInvoicesByTypeAsync(organId, ZhivarEnums.NoeFactor.Sell);
            decimal amountBuys = await CalcInvoicesByTypeAsync(organId, ZhivarEnums.NoeFactor.Buy);
            decimal amountReturnSells = await CalcInvoicesByTypeAsync(organId, ZhivarEnums.NoeFactor.ReturnSell);
            decimal amountReturnBuys = await CalcInvoicesByTypeAsync(organId, ZhivarEnums.NoeFactor.ReturnBuy);
            decimal amountInventoryEndOfTheCourse = await CalcInventoryEndOfTheCourseAsync(organId);
            decimal amountInventoryOfTheBeginning = await CalcInventoryOfTheBeginningAsync(organId);
            decimal amountOperatingIncome = await CalcAccountByCodeAsync(organId, "71");
            decimal amountNonOperatingIncome = await CalcAccountByCodeAsync(organId, "72");
            decimal amountStaffCosts = await CalcAccountByCodeAsync(organId, "81");
            decimal amountPublicSpending = await CalcAccountByCodeAsync(organId, "82");
            decimal amountDistributionAndSalesCosts = await CalcAccountByCodeAsync(organId, "83");
            var profit = amountSells - amountBuys + amountReturnBuys - amountReturnSells +
                            amountInventoryEndOfTheCourse - amountInventoryOfTheBeginning +
                            amountOperatingIncome + amountNonOperatingIncome - amountStaffCosts +
                            amountPublicSpending - amountDistributionAndSalesCosts;

            ShareholderRule shareholderRule = new ShareholderRule();
            var shareHolders = await shareholderRule.GetAllByOrganIdAsync(organId);

            foreach (var shareHolder in shareHolders)
            {
                decimal amounShare = 0;

                amounShare = (shareHolder.SharePercent * profit) / 100;

                var type = "Credit";
                if (amounShare > 0)
                    type = "Credit";
                else
                    type = "Debit";

                capitalStatement.tableCapitals.Add(new tableCapital()
                {
                    Code = shareHolder.Code,
                    Amount = amounShare,
                    Date = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                    Description = "سهم سود و زیان" + " - " + shareHolder.Name,
                    Type = type,

                });
            }


            #endregion
            #region سود انباشته
            var accumulatedProfit = accounts.Where(x => x.ComplteCoding == "3105").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childAccumulatedProfitIds = accounts.Where(x => x.ParentId == accumulatedProfit.ID).Select(x => x.ID).ToList();

            foreach (var childAccumulatedProfitId in childAccumulatedProfitIds)
            {
                decimal amounAccumulatedProfit = 0;
                var childAccumulatedProfitQuery = transactions.Where(x => x.AccountId == childAccumulatedProfitId).AsQueryable();

                if (childAccumulatedProfitQuery.Any())
                {

                    var credit = childAccumulatedProfitQuery.Sum(x => x.Credit);
                    var debit = childAccumulatedProfitQuery.Sum(x => x.Debit);

                    var childAccumulatedProfit1 = childAccumulatedProfitQuery.FirstOrDefault();
                    amounAccumulatedProfit = credit - debit;
                    var type = "Credit";
                    if (amounAccumulatedProfit > 0)
                        type = "Credit";
                    else
                        type = "Debit";

                    capitalStatement.tableCapitals.Add(new tableCapital()
                    {
                        Code = childAccumulatedProfit1.Account.Coding,
                        Amount = amounAccumulatedProfit,
                        Date = Utilities.PersianDateUtils.ToPersianDate(childAccumulatedProfit1.Date),
                        Description = childAccumulatedProfit1.Description + " - " + childAccumulatedProfit1.Account.Name,
                        Type = type,

                    });
                }
            }


            #endregion

            #region سرمایه نهایی

            shareHolders = await shareholderRule.GetAllByOrganIdAsync(organId);

            foreach (var shareHolder in shareHolders)
            {
                var tableCapitals = capitalStatement.tableCapitals.Where(x => x.Code == shareHolder.Code);
                decimal amount = 0;
                foreach (var tableCapital in tableCapitals)
                {
                    if (tableCapital.Type == "Credit")
                        amount += tableCapital.Amount;
                    else if (tableCapital.Type == "Debit")
                        amount -= tableCapital.Amount;

                }

                capitalStatement.tableCapitals.Add(new tableCapital()
                {
                    Amount = amount,
                    Date = PersianDateUtils.ToPersianDate(DateTime.Now),
                    Description = "سرمایه نهایی" + " - " + shareHolder.Name,
                    Type = "Title",

                });

            }


            #endregion


            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = capitalStatement });
        }

        [HttpPost]
        [Route("GetJournal")]
        public  async Task<HttpResponseMessage> GetJournal([FromBody] CashTransactionVM cashTransactionVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            DocumentRule documentRule = new DocumentRule();
            var documents = await documentRule.GetAllByOrganIdAsync(organId);

            var jornals = new List<Journal>();

            foreach (var document in documents)
            {
                foreach (var transaction in document.Transactions ?? new List<Transaction>())
                {
                    var acc = await accountRule.FindAsync(transaction.Account.ParentId);

                    jornals.Add(new Journal()
                    {
                        Code = transaction.Account.ComplteCoding,
                        Credit = transaction.Credit,
                        Debit = transaction.Debit,
                        Description = transaction.Description,
                        DisplayDate = transaction.DisplayDate,
                        DtName = transaction.Account.Name,
                        Id = document.ID,
                        IsManual = document.IsManual,
                        Name = acc.Name,
                        Number = document.Number,

                    });
                }
            }





            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = jornals });
        }

        [HttpPost]
        [Route("GetLedger")]
        public  async Task<HttpResponseMessage> GetLedger([FromBody] RequestLedger requestLedger)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            DocumentRule documentRule = new DocumentRule();
            var documents = await documentRule.GetAllByOrganIdAsync(organId);

            var ledger = new ledger();
            ledger.Rows = new List<Row>();
            var row = new Row();

            var childIds = new List<int>();

            var id = 0;
            if (requestLedger.detailAccount != null && requestLedger.detailAccount.Id > 0)
                id = requestLedger.detailAccount.Id;
            else
            {
                id = requestLedger.account.ID;
                childIds = accounts.Where(x => x.ParentId == id).Select(x => x.ID).ToList();
            }


            foreach (var document in documents)
            {
                foreach (var transaction in document.Transactions.Where(x => x.AccountId == id || childIds.Contains(x.AccountId)) ?? new List<Transaction>())
                {
                    var acc = await accountRule.FindAsync(transaction.Account.ParentId);
                    row = new Row();

                    row.Code = transaction.Account.Coding;
                    row.Credit = transaction.Credit;
                    row.Date = PersianDateUtils.ToPersianDate(transaction.Date);
                    row.Debit = transaction.Debit;
                    row.Description = transaction.Description;
                    row.DocId = transaction.DocumentId;
                    row.DocNum = transaction.DocumentId;
                    row.DtName = transaction.Account.Name;
                    row.Reference = transaction.Reference;

                    var remain = transaction.Credit - transaction.Debit;

                    if (remain > 0)
                        row.RemainType = "بس";
                    else
                        row.RemainType = "بد";

                    row.Remain = remain;


                    ledger.Rows.Add(row);
                }
            }





            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = ledger });
        }

        [Route("GetJournalInTotalAccounts")]
        [HttpPost]
        public  async Task<HttpResponseMessage> GetJournalInTotalAccounts()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            accounts = accounts.Where(x => x.Level == ZhivarEnums.AccountType.Moen).ToList();

            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            DocumentRule documentRule = new DocumentRule();
            var documents = await documentRule.GetAllByOrganIdAsync(organId);

            var responseJournalInTotalAccounts = new List<ResponseJournalInTotalAccount>();
            var responseJournalInTotalAccountsTemps = new List<ResponseJournalInTotalAccount>();

            var responseJournalInTotalAccount = new ResponseJournalInTotalAccount();
            var counter = 0;
            foreach (var document in documents)
            {
                responseJournalInTotalAccountsTemps = new List<ResponseJournalInTotalAccount>();

                foreach (var transaction in document.Transactions ?? new List<Transaction>())
                {

                    var acc = await accountRule.FindAsync(transaction.Account.ParentId);
                    responseJournalInTotalAccount = new ResponseJournalInTotalAccount();

                    if (transaction.Account.Level == ZhivarEnums.AccountType.Moen)
                        responseJournalInTotalAccount.Code = transaction.Account.ComplteCoding;
                    else
                        responseJournalInTotalAccount.Code = acc.ComplteCoding;

                    responseJournalInTotalAccount.Credit = transaction.Credit;
                    responseJournalInTotalAccount.Debit = transaction.Debit;
                    responseJournalInTotalAccount.DisplayDate = PersianDateUtils.ToPersianDate(transaction.Date);

                    var remain = transaction.Credit - transaction.Debit;

                    if (remain > 0)
                        responseJournalInTotalAccount.IsDebit = false;
                    else
                        responseJournalInTotalAccount.IsDebit = true;

                    if (transaction.Account.Level == ZhivarEnums.AccountType.Moen)
                        responseJournalInTotalAccount.Name = transaction.Account.Name;
                    else
                        responseJournalInTotalAccount.Name = acc.Name;

                    responseJournalInTotalAccount.Number = transaction.ID;
                    responseJournalInTotalAccount.Id = counter;
                    responseJournalInTotalAccount.AccountId = document.Number;

                    if (responseJournalInTotalAccountsTemps.Any(x => x.Code == responseJournalInTotalAccount.Code))
                    {
                        int index = responseJournalInTotalAccountsTemps.FindIndex(x => x.Code == responseJournalInTotalAccount.Code);
                        responseJournalInTotalAccountsTemps[index].Credit += responseJournalInTotalAccount.Credit;
                        responseJournalInTotalAccountsTemps[index].Debit += responseJournalInTotalAccount.Debit;

                        remain = responseJournalInTotalAccountsTemps[index].Credit - responseJournalInTotalAccountsTemps[index].Debit;

                        if (remain > 0)
                            responseJournalInTotalAccountsTemps[index].IsDebit = false;
                        else
                            responseJournalInTotalAccountsTemps[index].IsDebit = true;

                    }
                    else
                    {
                        counter++;
                        responseJournalInTotalAccount.Id = counter;
                        responseJournalInTotalAccountsTemps.Add(responseJournalInTotalAccount);
                    }

                }

                foreach (var responseJournalInTotalAccountsTemp in responseJournalInTotalAccountsTemps)
                {
                    responseJournalInTotalAccounts.Add(responseJournalInTotalAccountsTemp);
                }
            }



            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = responseJournalInTotalAccounts });
        }

        [Route("GetContactsBalance")]
        [HttpPost]
        public  async Task<HttpResponseMessage> GetContactsBalance()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            ContactRule contactRule = new ContactRule();
            var contacts = await contactRule.GetAllByOrganIdAsync(organId);

            var contactVMs = new List<ContactVM>();
            var contactVM = new ContactVM();

            foreach (var contact in contacts)
            {
                contactVM = new ContactVM();

                contactVM.Balance = contact.Balance;
                contactVM.Credits = contact.Credits;
                contactVM.DetailAccount = new DetailAccount
                {
                    //Id = 1,
                    Code = contact.Code,
                    //Name = "مانیار محمدی",
                    //RelatedAccounts = ",6,22,7,32,33,34,35,",
                    // FamilyTree = "اشخاص",

                };
                contactVM.ID = contact.ID;
                contactVM.IsCustomer = contact.IsCustomer;
                contactVM.IsShareHolder = contact.IsShareHolder;
                contactVM.IsVendor = contact.IsVendor;
                contactVM.Liability = contact.Liability;
                contactVM.Mobile = contact.Mobile;
                contactVM.Name = contact.Name;
                contactVM.Phone = contact.Phone;
                contactVM.SharePercent = contact.SharePercent;

                var account1104 = await CalcAccountByCodeTafziliAsync(organId, "1104" + contact.Code);
                //var account1105 = await CalcAccountByCodeTafziliAsync(organId, "1105" + contact.Code);
                var account2101 = await CalcAccountByCodeTafziliAsync(organId, "2101" + contact.Code);


                //contactVM.Balance = account1104.sumTotal + account1105.sumTotal + account2101.sumTotal;
                //contactVM.Credits = account1104.sumCredit + account1105.sumCredit + account2101.sumCredit;
                //contactVM.Liability = account1104.sumDebit + account1105.sumDebit + account2101.sumDebit;

                contactVM.Balance = account1104.sumTotal + account2101.sumTotal;
                contactVM.Credits = account1104.sumCredit + account2101.sumCredit;
                contactVM.Liability = account1104.sumDebit + account2101.sumDebit;
                contactVMs.Add(contactVM);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = contactVMs });
        }
        [Route("GetContactCard")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetContactCard([FromBody] RequestContactCard requestContactCard)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            ContactRule contactRule = new ContactRule();
            var contact = await contactRule.FindAsync(requestContactCard.contactId);

            var contactAccounts = accounts.Where(x => x.ComplteCoding == "1104" + contact.Code ||
                                                      //x.ComplteCoding == "1105" + contact.Code ||
                                                      x.ComplteCoding == "2101" + contact.Code ||
                                                      x.ComplteCoding == "3101" + contact.Code ||
                                                      x.ComplteCoding == "3102" + contact.Code ||
                                                      x.ComplteCoding == "3103" + contact.Code ||
                                                      x.ComplteCoding == "3104" + contact.Code ||
                                                      x.ComplteCoding == "3105" + contact.Code).Select(x => x.ID).ToList();

            // var transactions = await transactionRule.GetAllByOrganIdAsync(organId);


            var responseContactCard = new ResponseContactCard();


            responseContactCard.contact = new ContactVM()
            {
                Address = contact.Address,
                Balance = contact.Balance,
                City = contact.City,
                Code = contact.Code,
                ContactType = contact.ContactType,
                Credits = contact.Credits,
                Email = contact.Email,
                EconomicCode = contact.EconomicCode,
                Fax = contact.Fax,
                FirstName = contact.FirstName,
                ID = contact.ID,
                IsCustomer = contact.IsCustomer,
                IsEmployee = contact.IsEmployee,
                IsShareHolder = contact.IsShareHolder,
                IsVendor = contact.IsVendor,
                LastName = contact.LastName,
                Liability = contact.Liability,
                Mobile = contact.Mobile,
                Name = contact.Name,
                NationalCode = contact.NationalCode,
                Note = contact.Note,
                OrganId = contact.OrganId,
                Phone = contact.Phone,
                PostalCode = contact.PostalCode,
                Rating = contact.Rating,
                RegistrationDate = contact.RegistrationDate,
                RegistrationNumber = contact.RegistrationNumber,
                SharePercent = contact.SharePercent,
                State = contact.State,
                Website = contact.Website,

            };

            InvoiceRule invoiceRule = new InvoiceRule();
            var invoiceItems = await invoiceRule.GetAllByContactIdAsync(organId, requestContactCard.contactId);

            responseContactCard.invoiceItems = invoiceItems;

            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            var lstDocId = transactions.Where(x => contactAccounts.Contains(x.AccountId)).Select(x => x.DocumentId).Distinct().ToList();

            DocumentRule documentRule = new DocumentRule();

            var documents = await documentRule.GetDocumentsByDocumentIDs(lstDocId);
            responseContactCard.ledger = new ledger();
            responseContactCard.ledger.Rows = new List<Row>();
            Row row = new Row();

            decimal totalRemain = 0;
            decimal subRemain = 0;

            foreach (var document in documents)
            {

                row = new Row();
                row.IsMain = true;
                // row.Code = document..Account.Coding;
                row.Credit = document.Transactions.Where(x => contactAccounts.Contains(x.AccountId)).Sum(x => x.Credit);
                row.Date = document.DisplayDate;// PersianDateUtils.ToPersianDate(document.);
                row.Debit = document.Transactions.Where(x => contactAccounts.Contains(x.AccountId)).Sum(x => x.Debit);
                row.Description = document.Description;
                row.DocId = document.ID;
                row.DocNum = document.ID;
                row.Status = document.Type;
                //row.DtName = transaction.Account.Name;
                //row.Reference = transaction.Reference;

                var remain = row.Debit - row.Credit;
                totalRemain += remain;

                if (totalRemain > 0)
                    row.RemainType = "بد";

                else
                    row.RemainType = "بس";


                row.Remain = Math.Abs(totalRemain);
                responseContactCard.ledger.Rows.Add(row);

                

                var invoiceID = document.Transactions.Where(x => contactAccounts.Contains(x.AccountId) && x.InvoiceId != 0).Select(x => x.InvoiceId).Distinct().ToList();
                if (invoiceID.Any())
                {
                   

                    var invoice = await invoiceRule.FindAsync(invoiceID[0]);

                   // responseContactCard.saleApprovedSum += invoice.Sum;
                  //  responseContactCard.saleApprovedCount += 1;

                    switch (invoice.InvoiceType)
                    {
                        case NoeFactor.Sell:
                            responseContactCard.sumInvoiceSell += invoice.Payable;
                            responseContactCard.countInvoiceSell++;
                            break;
                        case NoeFactor.Buy:
                            responseContactCard.sumInvoiceBuy += invoice.Payable;
                            responseContactCard.countInvoiceBuy++;
                            break;
                        case NoeFactor.ReturnSell:
                            break;
                        case NoeFactor.ReturnBuy:
                            break;
                        case NoeFactor.RentTo:
                            responseContactCard.sumInvoiceRentTo += invoice.Payable;
                            responseContactCard.countInvoiceRentTo++;
                            break;
                        case NoeFactor.RentFrom:
                            responseContactCard.sumInvoiceRentFrom += invoice.Payable;
                            responseContactCard.countInvoiceRentFrom++;
                            break;
                        default:
                            break;
                    }
                    

                    foreach (var invoiceItem in invoice.InvoiceItems)
                    {

                        List<Row> rows = new List<Row>();
                        rows = ConvertInvoiceItemToRows(invoiceItem, invoice);


                        foreach (var rowItem in rows ?? new List<Row>())
                        {
                            rowItem.IsMain = false;
                            rowItem.Status = document.Type;
                            rowItem.DocId = document.ID;
                            rowItem.DocNum = document.ID;
             

                            var localRemain = rowItem.Debit - rowItem.Credit;
                            subRemain += localRemain;

                            rowItem.Remain = Math.Abs(subRemain);

                            if (subRemain > 0)
                                rowItem.RemainType = "بد";

                            else
                                rowItem.RemainType = "بس";
                        }

                        responseContactCard.ledger.Rows.AddRange(rows);

                    }
                }

                else if(document.Type == NoeDoc.Pay || document.Type == NoeDoc.Recive)
                {
                    using (var uow = new UnitOfWork())
                    {
                        var payRecive = uow.RepositoryAsync<PayRecevie>().Queryable().Where(x => x.DocumentId == document.ID).FirstOrDefault();

                        if (payRecive != null)
                        {
                            var detailPayRecevies = uow.RepositoryAsync<DetailPayRecevie>().Queryable().Where(x => x.PayRecevieId == payRecive.ID).ToList();

                            //decimal subRemain = 0;

                            foreach (var detailPayRecevie in detailPayRecevies)
                            {

                                List<Row> rows = new List<Row>();
                                rows = ConvertDetailPayRecevieToRows(detailPayRecevie, payRecive);


                                foreach (var rowItem in rows ?? new List<Row>())
                                {
                                    rowItem.IsMain = false;
                                    rowItem.Status = document.Type;
                                    rowItem.DocId = document.ID;
                                    rowItem.DocNum = document.ID;
                                

                                    var localRemain = rowItem.Debit - rowItem.Credit;
                                    subRemain += localRemain;

                                    rowItem.Remain = Math.Abs(subRemain);

                                    if (subRemain > 0)
                                        rowItem.RemainType = "بد";

                                    else
                                        rowItem.RemainType = "بس";
                                }

                                responseContactCard.ledger.Rows.AddRange(rows);

                            }
                        }
                        
                    }
                }

                else if (document.Type == NoeDoc.FirstDoc)
                {
                    using (var uow = new UnitOfWork())
                    {
                        var transactionList = uow.RepositoryAsync<Transaction>().Queryable().Where(x => x.DocumentId == document.ID && contactAccounts.Contains(x.AccountId)).ToList();
                      
                        if (transactionList != null)
                        {
                 
                            foreach (var transaction in transactionList)
                            {

                                //List<Row> rows = new List<Row>();

                                row = new Row();

                                row.Code = string.Empty;
                                row.Credit = 0;
                                row.Date = document.DisplayDate;

                                if (transaction.IsDebit)
                                    row.Debit = transaction.Amount;
                                else
                                    row.Credit = transaction.Amount;

                                row.Description = "سند افتتاحیه";
                                row.DocId = -1;
                                row.DocNum = 0;
                                row.DtName = string.Empty;
                                row.Reference = string.Empty;
                                row.Count = 1;
                                row.Name = string.Empty;
                                row.Reference = string.Empty;
                                row.Remain = 0;
                                row.RemainType = "-";
                                row.Unit = string.Empty;
                                row.UnitPrice = transaction.Amount;
                                row.Status = NoeDoc.FirstDoc;

                                row.IsMain = false;
                                row.Status = document.Type;
                                row.DocId = document.ID;
                                row.DocNum = document.ID;

                                var localRemain = row.Debit - row.Credit;

                                subRemain += localRemain;

                                row.Remain = Math.Abs(subRemain);

                                if (subRemain > 0)
                                    row.RemainType = "بد";
                                else
                                    row.RemainType = "بس";
                                

                                responseContactCard.ledger.Rows.Add(row);

                            }
                        }

                    }
                }
                else if (document.Type == NoeDoc.Cost)
                {
                    using (var uow = new UnitOfWork())
                    {
                        var cost = uow.RepositoryAsync<Cost>().Queryable().Where(x => x.DocumentId == document.ID).FirstOrDefault();

                        if (cost != null)
                        {
                            var costItems = uow.RepositoryAsync<CostItem>().Queryable().Where(x => x.CostId == cost.ID).ToList();

                            //decimal subRemain = 0;

                            foreach (var costItem in costItems)
                            {

                                List<Row> rows = new List<Row>();
                                rows = ConvertCostItemsToRows(costItem, cost);


                                foreach (var rowItem in rows ?? new List<Row>())
                                {
                                    rowItem.IsMain = false;
                                    rowItem.Status = document.Type;
                                    rowItem.DocId = document.ID;
                                    rowItem.DocNum = document.ID;


                                    var localRemain = rowItem.Debit - rowItem.Credit;
                                    subRemain += localRemain;

                                    rowItem.Remain = Math.Abs(subRemain);

                                    if (subRemain > 0)
                                        rowItem.RemainType = "بد";

                                    else
                                        rowItem.RemainType = "بس";
                                }

                                responseContactCard.ledger.Rows.AddRange(rows);

                            }
                        }

                    }
                }
                //else if (document.Type == NoeDoc.Transfer)
                //{
                //    using (var uow = new UnitOfWork())
                //    {
                //        var transferMoney = uow.RepositoryAsync<TransferMoney>().Queryable().Where(x => x.DocumentId == document.ID).FirstOrDefault();

                //        if (transferMoney != null)
                //        {
                //            var detailPayRecevies = uow.RepositoryAsync<DetailPayRecevie>().Queryable().Where(x => x.PayRecevieId == payRecive.ID).ToList();

                //            //decimal subRemain = 0;

                //            foreach (var detailPayRecevie in detailPayRecevies)
                //            {

                //                List<Row> rows = new List<Row>();
                //                rows = ConvertDetailPayRecevieToRows(detailPayRecevie, payRecive);


                //                foreach (var rowItem in rows ?? new List<Row>())
                //                {
                //                    rowItem.IsMain = false;
                //                    rowItem.Status = document.Type;
                //                    rowItem.DocId = document.ID;
                //                    rowItem.DocNum = document.ID;


                //                    var localRemain = rowItem.Debit - rowItem.Credit;
                //                    subRemain += localRemain;

                //                    rowItem.Remain = Math.Abs(subRemain);

                //                    if (subRemain > 0)
                //                        rowItem.RemainType = "بد";

                //                    else
                //                        rowItem.RemainType = "بس";
                //                }

                //                responseContactCard.ledger.Rows.AddRange(rows);

                //            }
                //        }

                //    }
                //}
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = responseContactCard });
        }

        private List<Row> ConvertInvoiceItemToRows(InvoiceItem invoiceItem, Invoice invoice)
        {
            ZhivarEnums.NoeDoc status = NoeDoc.Sell;

            switch (invoice.InvoiceType)
            {
                case NoeFactor.Sell:
                    status = ZhivarEnums.NoeDoc.Sell;
                    break;
                case NoeFactor.Buy:
                    status = ZhivarEnums.NoeDoc.Buy;
                    break;
                case NoeFactor.ReturnSell:
                    status = ZhivarEnums.NoeDoc.ReturnSell;
                    break;
                case NoeFactor.ReturnBuy:
                    status = ZhivarEnums.NoeDoc.ReturnBuy;
                    break;
                case NoeFactor.RentTo:
                    status = ZhivarEnums.NoeDoc.RentTo;
                    break;
                case NoeFactor.RentFrom:
                    status = ZhivarEnums.NoeDoc.RentFrom;
                    break;
                default:
                    break;
            }

            List<Row> rows = new List<Row>();

            Row row = new Row();

            if (invoiceItem.SumInvoiceItem != 0)
            {
                string desc = string.Empty;

                using (var uow = new UnitOfWork())
                {
                    var item = uow.Repository<Item>().Queryable().Where(x => x.ID == invoiceItem.ItemId).SingleOrDefault();

                    if (item != null)
                    {
                        desc = item.Name;
                    }
                }

                row = new Row();

                row.Code = string.Empty;
                row.Credit = 0;
                row.Date = invoice.DisplayDate;
                row.Debit = invoiceItem.UnitPrice * invoiceItem.Quantity;
                row.Description = desc;
                row.DocId = -1;
                row.DocNum = 0;
                row.DtName = string.Empty;
                row.Reference = string.Empty;
                row.Count = invoiceItem.Quantity;
                row.InvoiceId = invoice.ID;
                row.Name = string.Empty;
                row.Reference = string.Empty;
                row.Remain = 0;
                row.RemainType = "-";
                row.Unit = string.Empty;
                row.UnitPrice = invoiceItem.UnitPrice;
                row.Status = status;

                rows.Add(row);
            }

            if (invoiceItem.Tax != 0)
            {
                row = new Row();

                row.Code = string.Empty;
                row.Credit = 0;
                row.Date = invoice.DisplayDate;
                row.Debit = invoiceItem.Tax;
                row.Description = "مالیات";
                row.DocId = -1;
                row.DocNum = 0;
                row.DtName = string.Empty;
                row.Reference = string.Empty;
                row.Count = 1;
                row.InvoiceId = invoice.ID;
                row.Name = string.Empty;
                row.Reference = string.Empty;
                row.Remain = 0;
                row.RemainType = "-";
                row.Unit = string.Empty;
                row.UnitPrice = invoiceItem.Tax;
                row.Status = status;

                rows.Add(row);
            }

            if (invoiceItem.PriceBazareab!=null && invoiceItem.PriceBazareab != 0)
            {
                row = new Row();

                row.Code = string.Empty;
                row.Credit = 0;
                row.Date = invoice.DisplayDate;
                row.Debit = Convert.ToDecimal( invoiceItem.PriceBazareab);
                row.Description = "هزینه بازاریاب";
                row.DocId = -1;
                row.DocNum = 0;
                row.DtName = string.Empty;
                row.Reference = string.Empty;
                row.Count = 1;
                row.InvoiceId = invoice.ID;
                row.Name = string.Empty;
                row.Reference = string.Empty;
                row.Remain = 0;
                row.RemainType = "-";
                row.Unit = string.Empty;
                row.UnitPrice = Convert.ToDecimal(invoiceItem.PriceBazareab);
                row.Status = status;

                rows.Add(row);
            }

            if (invoiceItem.PriceTarah !=  null && invoiceItem.PriceTarah != 0)
            {
                row = new Row();

                row.Code = string.Empty;
                row.Credit = 0;
                row.Date = invoice.DisplayDate;
                row.Debit = Convert.ToDecimal(invoiceItem.PriceTarah);
                row.Description = "هزینه طراحی";
                row.DocId = -1;
                row.DocNum = 0;
                row.DtName = string.Empty;
                row.Reference = string.Empty;
                row.Count = 1;
                row.InvoiceId = invoice.ID;
                row.Name = string.Empty;
                row.Reference = string.Empty;
                row.Remain = 0;
                row.RemainType = "-";
                row.Unit = string.Empty;
                row.UnitPrice = Convert.ToDecimal(invoiceItem.PriceTarah);
                row.Status = status;

                rows.Add(row);
            }

            if (invoiceItem.PriceChap != null && invoiceItem.PriceChap != 0)
            {
                decimal count = 0;
                decimal total = 0;

                using (var uow = new UnitOfWork())
                {
                    
                    var contract = uow.RepositoryAsync<DomainClasses.Contract.Contract>().Queryable().Where(x => x.InvoiceId == invoice.ID).FirstOrDefault();

                    var mapItemSaze = uow.RepositoryAsync<MapItemSaze>().Queryable().Where(x => x.ItemID == invoiceItem.ItemId).FirstOrDefault();

                    if (contract != null && mapItemSaze != null)
                    {
                        var contract_Saze = uow.RepositoryAsync<Contract_Saze>().Queryable().Where(x => x.ContractID == contract.ID && x.SazeId == mapItemSaze.SazeID).FirstOrDefault();

                        if (contract_Saze != null)
                        {
                            var contract_Saze_Chapkhanes = uow.RepositoryAsync<Contract_Saze_Chapkhane>().Queryable().Where(x => x.ContarctSazeID == contract_Saze.ID).ToList();

                            foreach (var contract_Saze_Chapkhane in contract_Saze_Chapkhanes)
                            {
                                count += contract_Saze_Chapkhane.MetrazhMoshtari;
                                total += contract_Saze_Chapkhane.TotalMoshtari;
                            }
                        }
                    }
                        
                   
                }

                row = new Row();

                row.Code = string.Empty;
                row.Credit = 0;
                row.Date = invoice.DisplayDate;
                row.Debit = total;
                row.Description = "هزینه چاپ";
                row.DocId = -1;
                row.DocNum = 0;
                row.DtName = string.Empty;
                row.Reference = string.Empty;
                row.Count = count;
                row.InvoiceId = invoice.ID;
                row.Name = string.Empty;
                row.Reference = string.Empty;
                row.Remain = 0;
                row.RemainType = "-";
                row.Unit = string.Empty;
                row.UnitPrice = total / count;// Convert.ToDecimal(invoiceItem.PriceChap);
                row.Status = status;

                rows.Add(row);
            }

            if (invoiceItem.PriceNasab != null && invoiceItem.PriceNasab != 0)
            {
                row = new Row();

                row.Code = string.Empty;
                row.Credit = 0;
                row.Date = invoice.DisplayDate;
                row.Debit = Convert.ToDecimal(invoiceItem.PriceNasab);
                row.Description = "هزینه نصب";
                row.DocId = -1;
                row.DocNum = 0;
                row.DtName = string.Empty;
                row.Reference = string.Empty;
                row.Count = 1;
                row.InvoiceId = invoice.ID;
                row.Name = string.Empty;
                row.Reference = string.Empty;
                row.Remain = 0;
                row.RemainType = "-";
                row.Unit = string.Empty;
                row.UnitPrice = Convert.ToDecimal(invoiceItem.PriceNasab);
                row.Status = status;

                rows.Add(row);
            }
            if (invoiceItem.Discount != 0)
            {
                row = new Row();

                row.Code = string.Empty;
                row.Credit = 0;
                row.Date = invoice.DisplayDate;
                row.Credit = invoiceItem.Discount;
                row.Description = "تخفیف";
                row.DocId = -1;
                row.DocNum = 0;
                row.DtName = string.Empty;
                row.Reference = string.Empty;
                row.Count = 1;
                row.InvoiceId = invoice.ID;
                row.Name = string.Empty;
                row.Reference = string.Empty;
                row.Remain = 0;
                row.RemainType = "-";
                row.Unit = string.Empty;
                row.UnitPrice = invoiceItem.Discount;
                row.Status = status;

                rows.Add(row);
            }
          

            return rows;
        }
        private List<Row> ConvertDetailPayRecevieToRows(DetailPayRecevie detailPayRecevie, PayRecevie payRecevie)
        {
            ZhivarEnums.NoeDoc status = NoeDoc.Recive;

            if (payRecevie.IsReceive)
            {
                status = NoeDoc.Recive;
            }
            else
            {
                status = NoeDoc.Pay;
            }

            List<Row> rows = new List<Row>();

            Row row = new Row();

            if (detailPayRecevie.BankId != null && detailPayRecevie.BankId != 0)
            {
                string desc = string.Empty;

                using (var uow = new UnitOfWork())
                {
                    var bank = uow.Repository<Bank>().Queryable().Where(x => x.ID == detailPayRecevie.BankId).SingleOrDefault();

                    if (bank != null)
                    {
                        if (status == NoeDoc.Recive)
                        {
                            desc = " واریز به حساب بانک ";
                            desc += bank.Name;
                        }
                        else
                        {
                            desc = "  پرداخت از حساب بانک ";
                            desc += bank.Name;
                        }
                    }
                }

                row = new Row();

                row.Code = string.Empty;
                row.Credit = 0;
                row.Date = payRecevie.DisplayDate;

                if(!payRecevie.IsReceive)
                    row.Debit = detailPayRecevie.Amount;
                else
                    row.Credit = detailPayRecevie.Amount;

                row.Description = desc;
                row.DocId = -1;
                row.DocNum = 0;
                row.DtName = string.Empty;
                row.Reference = string.Empty;
                row.Count = 1;
                row.Name = string.Empty;
                row.Reference = string.Empty;
                row.Remain = 0;
                row.RemainType = "-";
                row.Unit = string.Empty;
                row.UnitPrice = detailPayRecevie.Amount;
                row.Status = status;

                rows.Add(row);
            }

            else if (detailPayRecevie.CashId !=null && detailPayRecevie.CashId != 0)
            {
                string desc = string.Empty;

                using (var uow = new UnitOfWork())
                {
                    var cash = uow.Repository<Cash>().Queryable().Where(x => x.ID == detailPayRecevie.CashId).SingleOrDefault();

                    if (cash != null)
                    {
                        if (status == NoeDoc.Recive)
                        {
                            desc = " دریافت نقدی از صندوق ";
                            desc += cash.Name;
                        }
                        else
                        {
                            desc = "  پرداخت نقدی از صندوق ";
                            desc += cash.Name;
                        }
                    }
                }

                row = new Row();

                row.Code = string.Empty;
                row.Credit = 0;
                row.Date = payRecevie.DisplayDate;

                if (!payRecevie.IsReceive)
                    row.Debit = detailPayRecevie.Amount;
                else
                    row.Credit = detailPayRecevie.Amount;

                row.Description = desc;
                row.DocId = -1;
                row.DocNum = 0;
                row.DtName = string.Empty;
                row.Reference = string.Empty;
                row.Count = 1;
                row.Name = string.Empty;
                row.Reference = string.Empty;
                row.Remain = 0;
                row.RemainType = "-";
                row.Unit = string.Empty;
                row.UnitPrice = detailPayRecevie.Amount;
                row.Status = status;

                rows.Add(row);
            }
            else if (detailPayRecevie.ChequeId != null &&detailPayRecevie.ChequeId != 0)
            {
                string desc = string.Empty;

                using (var uow = new UnitOfWork())
                {
                    var cheque = uow.Repository<Cheque>().Queryable().Where(x => x.ID == detailPayRecevie.ChequeId).SingleOrDefault();

                    if (cheque != null)
                    {
                        if (status == NoeDoc.Recive)
                        {
                            desc = " دریافت چک بانک  ";
                            desc += cheque.BankName;
                        }
                        else
                        {
                            desc = " پرداخت چک بانک  ";
                            desc += cheque.BankName;
                        }
                      
                    }
                }

                row = new Row();

                row.Code = string.Empty;
                row.Credit = 0;
                row.Date = payRecevie.DisplayDate;

                if (!payRecevie.IsReceive)
                    row.Debit = detailPayRecevie.Amount;
                else
                    row.Credit = detailPayRecevie.Amount;

                row.Description = desc;
                row.DocId = -1;
                row.DocNum = 0;
                row.DtName = string.Empty;
                row.Reference = string.Empty;
                row.Count = 1;
                row.Name = string.Empty;
                row.Reference = string.Empty;
                row.Remain = 0;
                row.RemainType = "-";
                row.Unit = string.Empty;
                row.UnitPrice = detailPayRecevie.Amount;
                row.Status = status;

                rows.Add(row);
            }

            else if (detailPayRecevie.ChequeBankId != null && detailPayRecevie.ChequeBankId != 0)
            {
                string desc = string.Empty;

                using (var uow = new UnitOfWork())
                {
                    var chequeBank = uow.Repository<ChequeBank>().Queryable().Where(x => x.ID == detailPayRecevie.ChequeBankId).SingleOrDefault();

                    if (chequeBank != null)
                    {
                        var bank = uow.Repository<Bank>().Queryable().Where(x => x.ID == chequeBank.BankId).SingleOrDefault();

                        desc = bank.Name;

                        if (status == NoeDoc.Recive)
                        {
                            desc = " دریافت چک بانک  ";
                            desc += bank.Name;
                           
                        }
                        else
                        {
                            desc = " پرداخت چک بانک  ";
                            desc += bank.Name;
                        }
                    }
                }

                row = new Row();

                row.Code = string.Empty;
                row.Credit = 0;
                row.Date = payRecevie.DisplayDate;

                if (!payRecevie.IsReceive)
                    row.Debit = detailPayRecevie.Amount;
                else
                    row.Credit = detailPayRecevie.Amount;

                row.Description = desc;
                row.DocId = -1;
                row.DocNum = 0;
                row.DtName = string.Empty;
                row.Reference = string.Empty;
                row.Count = 1;
                row.Name = string.Empty;
                row.Reference = string.Empty;
                row.Remain = 0;
                row.RemainType = "-";
                row.Unit = string.Empty;
                row.UnitPrice = detailPayRecevie.Amount;
                row.Status = status;

                rows.Add(row);
            }
            return rows;
        }

        private List<Row> ConvertCostItemsToRows(CostItem costItem, Cost cost)
        {
         

            List<Row> rows = new List<Row>();

            Row row = new Row();

             
                row = new Row();

                row.Code = string.Empty;
                row.Credit = costItem.Sum;
                row.Date = cost.DisplayDate;
                row.Debit =0;
                row.Description = costItem.Description;
                row.DocId = -1;
                row.DocNum = 0;
                row.DtName = string.Empty;
                row.Reference = string.Empty;
                row.Count = 1;
                row.Name = string.Empty;
                row.Reference = string.Empty;
                row.Remain = 0;
                row.RemainType = "-";
                row.Unit = string.Empty;
                row.UnitPrice = costItem.Sum;
                row.Status = NoeDoc.Cost;

                rows.Add(row);
            

     
            return rows;
        }
        private async Task<decimal> CalcAccountByCodeAsync(int organId, string code)
        {
            //var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

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

            if (selected.Any())
            {
                var credit = selected.Sum(x => x.Credit);
                var debit = selected.Sum(x => x.Debit);
                amount = credit - debit;
            }

            return amount;
        }
        private async Task<AccountTafzili> CalcAccountByCodeTafziliAsync(int organId, string code)
        {
            //var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            decimal amount = 0;
            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var operatinIncomeAccount = accounts.Where(x => x.ComplteCoding == code).SingleOrDefault();

          


            var selected = transactions.Where(a => a.AccountId == operatinIncomeAccount.ID).ToList();

            AccountTafzili accountTafzili = new AccountTafzili();

            if (selected.Any())
            {
                accountTafzili.sumCredit = selected.Sum(x => x.Credit);
                accountTafzili.sumDebit = selected.Sum(x => x.Debit);
                accountTafzili.sumTotal = accountTafzili.sumDebit - accountTafzili.sumCredit;
            }

            return accountTafzili;
        }

        private async Task<decimal> CalcInventoryOfTheBeginningAsync(int organId)
        {
            //var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            DocumentRule documentRule = new DocumentRule();
            var documents = await documentRule.GetAllByOrganIdAsync(organId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            var firstDocument = documents.Where(x => x.IsFirsDocument == true).SingleOrDefault();

            decimal amount = 0;

            if (firstDocument == null)
                return amount;

            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            var accountsQuery = accounts.AsQueryable().Where(x => x.ComplteCoding == "1108");
            var itemsQuery = accounts.AsQueryable();


            List<int> childIds = (from account in accountsQuery
                                  join item in itemsQuery
                                  on account.ID equals item.ParentId
                                  select item.ID).ToList();

            var selected = firstDocument.Transactions.Where(a => childIds.Contains(a.AccountId)).ToList();

            if (selected.Any())
            {

                foreach (var transaction in selected ?? new List<Zhivar.DomainClasses.Accounting.Transaction>())
                {
                    amount += transaction.Stock * transaction.UnitPrice;
                }
            }

            return amount;
        }

        private async Task<decimal> CalcInventoryEndOfTheCourseAsync(int organId)
        {
            ItemGroupRule itemGroupRule = new ItemGroupRule();

            var itemGroups = await itemGroupRule.GetAllByOrganIdAsync(organId);

            decimal amount = 0;

            foreach (var itemGroup in itemGroups)
            {
                foreach (var item in itemGroup.Items.Where(x => x.ItemType == ZhivarEnums.NoeItem.Item) ?? new List<ItemVM>())
                {
                    amount += item.Stock * item.BuyPrice;
                }
            }

            return amount;
        }

        private async Task<decimal> CalcInvoicesByTypeAsync(int organId, ZhivarEnums.NoeFactor invoiceType)
        {
            InvoiceRule invoiceRule = new InvoiceRule();
            var invoices = await invoiceRule.GetAllByOrganIdAsync(organId);
            invoices = invoices.Where(x => x.InvoiceType == invoiceType && (x.Status == ZhivarEnums.NoeInsertFactor.WaitingToReceive || x.Status == ZhivarEnums.NoeInsertFactor.Received)).ToList();

            decimal amount = 0;

            foreach (var invoice in invoices)
            {
                foreach (var invoiceItem in invoice.InvoiceItems ?? new List<InvoiceItemVM>())
                {
                    if (invoiceItem.Item.ItemType == ZhivarEnums.NoeItem.Item)
                    {
                        amount += invoiceItem.Sum;
                    }
                }
            }

            return amount;
        }

    }

    public class ResultTransactionCash
    {
        public Cash cash { get; set; }
        public ledger ledger { get; set; }
    }

    public class Row
    {
        public string Code { get; set; }
        public decimal Credit { get; set; }
        public string Date { get; set; }
        public decimal Debit { get; set; }
        public string Description { get; set; }
        public int DocId { get; set; }
        public int DocNum { get; set; }
        public string DtName { get; set; }
        public int InvoiceId { get; set; }
        public string Name { get; set; }
        public string Reference { get; set; }
        public decimal Remain { get; set; }
        public string RemainType { get; set; }
        public decimal Count { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }

        public NoeDoc Status { get; set; }

        public bool IsMain { get; set; }

    }
    public class ledger
    {
        public List<Row> Rows { get; set; }
    }

    public class CapitalStatement
    {
        public string reportDate { get; set; }
        public List<tableCapital> tableCapitals { get; set; }
    }

    public class tableCapital
    {
        public decimal Amount { get; set; }
        public string Class { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string GroupCode { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
    }

    public class Journal
    {
        public string Code { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public string Description { get; set; }
        public string DisplayDate { get; set; }
        public string DtName { get; set; }
        public int Id { get; set; }
        public bool IsManual { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }

    public class RequestLedger
    {
        public DomainClasses.Accounting.Account account { get; set; }
        public DetailAccount detailAccount { get; set; }
        public string start { get; set; }
        public string end { get; set; }
    }

    public class ResponseJournalInTotalAccount
    {
        public int AccountId { get; set; }
        public string Code { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public string DisplayDate { get; set; }
        public int Id { get; set; }
        public bool IsDebit { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }

    public class RequestContactCard
    {
        public int contactId { get; set; }
        public string start { get; set; }
        public string end { get; set; }
    }

    public class ResponseContactCard
    {
        public decimal billApprovedCount { get; set; }
        public decimal billApprovedSum { get; set; }
        public decimal billOverDueCount { get; set; }
        public decimal billOverDueSum { get; set; }
        public ContactVM contact { get; set; }
        public List<InvoiceItemContactItem> invoiceItems { get; set; }
        public List<ledgerContactCard> Rows { get; set; }
        public ledger ledger { get; set; }
        public decimal saleApprovedCount { get; set; }
        public decimal saleApprovedSum { get; set; }
        public decimal saleOverDueCount { get; set; }
        public decimal saleOverDueSum { get; set; }
        public decimal sumCredit { get; set; }
        public decimal sumDebit { get; set; }


        public decimal sumInvoiceRentTo { get; set; }
        public int countInvoiceRentTo { get; set; }

        public decimal sumInvoiceRentFrom { get; set; }
        public int countInvoiceRentFrom { get; set; }

        public decimal sumInvoiceSell { get; set; }
        public int countInvoiceSell { get; set; }

        public decimal sumInvoiceBuy { get; set; }
        public int countInvoiceBuy { get; set; }
    }
    public class ledgerContactCard
    {
        public decimal saleApprovedCount { get; set; }
        public decimal saleApprovedSum { get; set; }
        public decimal saleOverDueCount { get; set; }
        public decimal saleOverDueSum { get; set; }
        public decimal sumCredit { get; set; }
        public decimal sumDebit { get; set; }
    }

    public class AccountTafzili
    {
        public decimal sumCredit { get; set; }
        public decimal sumDebit { get; set; }
        public decimal sumTotal { get; set; }
    }
}