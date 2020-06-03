using AutoMapper;
using OMF.Business;
using OMF.Common.Security;
using OMF.Enterprise.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using Zhivar.Business.Accounting;
using Zhivar.Business.Common;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using Zhivar.ServiceLayer.Contracts.Accounting;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.Web.Controllers.Accounting
{
    [RoutePrefix("api/Document")]
    public class DocumentController : NewApiControllerBaseAsync<Document, DocumentVM>
    {

        public DocumentRule Rule => this.BusinessRule as DocumentRule;

        protected override IBusinessRuleBaseAsync<Document> CreateBusinessRule()
        {
            return new DocumentRule();
        }

        [HttpPost]
        [Route("LoadDocumentData")]
        public async Task<HttpResponseMessage> LoadDocumentData([FromBody] int id)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            accounts = accounts.Where(x => x.Level == ZhivarEnums.AccountType.Moen).ToList();


            var resualt = new DocumentData();

            resualt.accounts = new List<AccountVM>();

            resualt.defaultDescriptions = new List<DefaultDescriptions>();
            resualt.detailAccounts = new List<DetailAccount>();

            foreach (var account in accounts)
            {
                resualt.accounts.Add(new AccountVM()
                {
                    Balance = 0,
                    BalanceType = 0,
                    Code = "",
                    Coding = account.Coding,
                    credit = 0,
                    debit = 0,
                    GroupCode = "",
                    //Id = account.ID,
                    ID = account.ID,
                    Level = account.Level,
                    LevelString = "",
                    Name = account.Name,
                    ParentCoding = account.ParentId.ToString(),
                    SystemAccount = 0,
                    SystemAccountName = ""

                });
            }

            DocumentRule documentRule = new DocumentRule();

            if (id == 0)
            {
                var transactions = new List<TransactionVM>();
                transactions.Add(new TransactionVM()
                {
                    AccDocument = null,
                    Account = null,
                    Amount = 0,
                    Cheque = null,
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = true,
                    IsDebit = false,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",

                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 1,
                    Stock = 0,
                    TransactionTypeString = "بستانکار",
                    Type = 1,
                    UnitPrice = 0,
                });

                resualt.document = new DocumentVM()
                {
                    Credit = 0,
                    DateTime = DateTime.Now,
                    Debit = 0,
                    Description = "",
                    DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                    Id = 0,
                    IsManual = false,
                    Number = 0,
                    Number2 = 0,
                    Status = 0,
                    StatusString = "پیش نویس",
                    Transactions = transactions

                };

            }
            else
            {
                var document = await documentRule.FindAsync(id);

                var transactions = new List<TransactionVM>();
                foreach (var item in document.Transactions)
                {
                    transactions.Add(new TransactionVM()
                    {
                        //AccDocument = item.AccDocument,
                        //Account = item.Account,
                        Amount = item.Amount,
                        Cheque = item.Cheque,
                        //Contact = null,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        Description = item.Description,
                        DetailAccount = null,
                        Id = item.ID,
                        //Invoice = null,
                        IsCredit = item.IsCredit,
                        IsDebit = item.IsDebit,
                        PaymentMethod = item.PaymentMethod,
                        PaymentMethodString = item.PaymentMethodString,

                        Remaining = item.Remaining,
                        RemainingType = item.RemainingType,
                        RowNumber = item.RowNumber,
                        Stock = item.Stock,
                        TransactionTypeString = item.TransactionTypeString,
                        Type = item.Type,
                        UnitPrice = item.UnitPrice,
                        AccountId = item.AccountId,
                        ChequeId = item.ChequeId,
                        Date = item.Date,
                        ContactId = item.ContactId,
                        DisplayDate = item.DisplayDate,
                        Reference = item.Reference,
                        DocumentId = item.DocumentId,
                        InvoiceId = item.InvoiceId,

                    });
                }
                resualt.document = new DocumentVM()
                {
                    Credit = document.Credit,
                    DateTime = document.DateTime,
                    Debit = document.Debit,
                    Description = document.Description,
                    DisplayDate = document.DisplayDate,
                    Id = document.ID,
                    IsManual = document.IsManual,
                    Number = document.Number,
                    Number2 = document.Number2,
                    Status = document.Status,
                    StatusString = document.StatusString,
                    Transactions = transactions,
                    IsFirsDocument = document.IsFirsDocument,


                };
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });
        }

        [HttpPost]
        [Route("GetDocumentsAndStats")]
        public async Task<HttpResponseMessage> GetDocumentsAndStats()
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                DocumentRule documentRule = new DocumentRule();

                var resualt = await documentRule.GetAllByOrganIdAsync(organId);
                return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });
            }
            catch (Exception ex)
            {

                throw;
            }
          
        }

        [HttpPost]
        [Route("GetDocument")]
        public async Task<HttpResponseMessage> GetDocument([FromBody]int id)
        {
            var resualt = await Rule.GetDocumentByIdAsync(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });
        }


        [HttpPost]
        [Route("LoadOpeningBalanceStat")]
        public async Task<HttpResponseMessage> LoadOpeningBalanceStat()
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);


                var document = await Rule.GetFirstDocument(organId);

                var resualt = new OpeningBalanceStat()
                {
                    bank = 0,
                    cash = 0,
                    creditors = 0,
                    debtors = 0,
                    docDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                    inProgress = 0,
                    inventory = 0,
                    otherAssets = 0,
                    otherLiabilities = 0,
                    payables = 0,
                    receivables = 0,
                    share = 0,
                    withdrawals = 0,


                };

                if (document != null)
                {
                    resualt = await this.Rule.OpeningBalanceStatAsync(document, organId);
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });
            }
            catch (Exception ex)
            {

                throw;
            }
          
        }

        [HttpPost]
        [Route("LoadOpeningBalance")]
        public async Task<HttpResponseMessage> LoadOpeningBalance([FromBody]string systemAccount)
        {
            var resualt = new OpeningBalance();

            switch (systemAccount)
            {
                case "cash":
                    {

                        resualt = await CreateTransObjCash();
                        break;
                    }
                case "bank":
                    {
                        resualt = await CreateTransObjBank();
                        break;
                    }
                case "product":
                    {
                        resualt = await CreateTransObjProduct();
                        break;
                    }
                case "debtors":
                    {
                        resualt = await CreateTransObjDebtors();
                        break;
                    }
                case "payables":
                    {
                        resualt = await CreateTransObjPayables();
                        break;
                    }
                case "receivables":
                    {
                        resualt = await CreateTransObjReceivables("1105");
                        break;
                    }
                case "inProgress":
                    {
                        resualt = await CreateTransObjReceivables("1106");
                        break;
                    }
                case "assets":
                    {
                        resualt = await CreateTransObjAssets();
                        break;
                    }
                case "creditors":
                    {
                        resualt = await CreateTransObjCreditors();
                        break;
                    }
                case "liabilities":
                    {
                        resualt = await CreateTransObjLiabilities();
                        break;
                    }
                default:
                    break;
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });
        }

        [HttpPost]
        [Route("GetChequeRelatedDocuments")]
        public async Task<HttpResponseMessage> GetChequeRelatedDocuments([FromBody]int chequeId)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            List<Document> documents = await Rule.GetChequeRelatedDocuments(chequeId);


            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = documents });
        }

        private async Task<OpeningBalance> CreateTransObjAssets()
        {

            var res = new OpeningBalance();

            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            //var account = accounts.Where(x => x.ComplteCoding == "1105").SingleOrDefault();

            var firstDocument = await Rule.GetFirstDocument(organId);
            if (firstDocument == null)
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = null,
                    Amount = 0,
                    Cheque = null,
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                res.transactions = new List<TransactionVM>();

                for (int i = 0; i < 4; i++)
                {
                    res.transactions.Add(new TransactionVM()
                    {
                        AccDocument = null,
                        Account = null,
                        Amount = 0,
                        Cheque = null,
                        Contact = null,
                        Credit = 0,
                        Debit = 0,
                        Description = "",
                        DetailAccount = null,
                        Id = 0,
                        Invoice = null,
                        IsCredit = false,
                        IsDebit = true,
                        PaymentMethod = 0,
                        PaymentMethodString = "نامشخص",
                        Reference = "",
                        Remaining = 0,
                        RemainingType = null,
                        RowNumber = 0,
                        Stock = 0,
                        TransactionTypeString = "بدهکار",
                        Type = 0,
                        UnitPrice = 0,
                    });
                }

            }
            else
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = null,
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                TransactionRule transactionRule = new TransactionRule();
                var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

                List<string> ids = new List<string>() { "1102", "1107", "1109", "1110", "1111" };
                var parentAccountsQuery = accounts.AsQueryable().Where(x => ids.Contains(x.ComplteCoding)).ToList();

                var accountsQuery = accounts.AsQueryable().Where(x => ids.Contains(x.ComplteCoding)).ToList();
                var allAccountQuery = accounts.AsQueryable();


                List<int> childcashIds = (from account1 in accountsQuery
                                          join allAccount in allAccountQuery
                                          on account1.ID equals allAccount.ParentId
                                          select allAccount.ID).ToList();



                List<int> childcashIds2 = (from parentAccounts in parentAccountsQuery
                                           select parentAccounts.ID).ToList();

                var selected = firstDocument.Transactions.Where(a => childcashIds.Contains(a.AccountId) || childcashIds2.Contains(a.AccountId)).ToList();

                if (selected.Any())
                {
                    res.transactions = new List<TransactionVM>();

                    foreach (var transaction in selected ?? new List<DomainClasses.Accounting.Transaction>())
                    {
                        var oldAccount = await accountRule.FindAsync(transaction.AccountId);

                        TransactionVM transactionVM = new TransactionVM();

                        transactionVM.AccDocument = null;
                        if (oldAccount.Level == ZhivarEnums.AccountType.Moen)
                        {
                            transactionVM.Account = new AccountVM()
                            {

                                Balance = 0,
                                BalanceType = 2,
                                Code = oldAccount.Coding,
                                Coding = oldAccount.ComplteCoding,
                                GroupCode = "1",
                                ID = oldAccount.ID,
                                Level = oldAccount.Level,
                                LevelString = "معین",
                                Name = oldAccount.Name,
                                ParentCoding = oldAccount.ParentId.ToString(),
                                SystemAccount = 7,
                                SystemAccountName = oldAccount.Name,
                                credit = 0,
                                debit = 0,
                            };
                        }

                        transactionVM.Amount = transaction.Amount;
                        transactionVM.Cheque = null;
                        transactionVM.Contact = null;
                        transactionVM.Credit = transaction.Credit;
                        transactionVM.Debit = transaction.Debit;
                        transactionVM.Description = transaction.Description;

                        if (oldAccount.Level == ZhivarEnums.AccountType.Tafzeli)
                        {
                            transactionVM.DetailAccount = new DetailAccount()
                            {
                                Accounts = new List<AccountVM>()
                                {
                                    new AccountVM()
                                    {
                                        Balance = transaction.Credit - transaction.Debit,
                                        Coding = oldAccount.Coding,
                                        credit = transaction.Credit,
                                        debit = transaction.Debit,
                                        ID = oldAccount.ID,
                                        Name = oldAccount.Name,
                                        ParentId = oldAccount.ParentId,
                                        Level = ZhivarEnums.AccountType.Tafzeli,
                                        LevelString = "تفضیلی",
                                    }
                                },
                                Balance = transaction.Credit - transaction.Debit,
                                Code = oldAccount.Coding,
                                Name = oldAccount.Name,
                                credit = transaction.Credit,
                                debit = transaction.Debit,
                                Id = oldAccount.ID
                            };
                        }

                        transactionVM.Id = transaction.ID;
                        transactionVM.Invoice = null;
                        transactionVM.IsCredit = false;
                        transactionVM.IsDebit = true;
                        transactionVM.PaymentMethod = 0;
                        transactionVM.PaymentMethodString = "نامشخص";
                        transactionVM.Remaining = 0;
                        transactionVM.RemainingType = null;
                        transactionVM.RowNumber = 0;
                        transactionVM.Stock = 0;
                        transactionVM.TransactionTypeString = "بدهکار";
                        transactionVM.Type = 0;
                        transactionVM.UnitPrice = 0;
                        transactionVM.AccountId = transaction.AccountId;
                        res.transactions.Add(transactionVM);
                    }

                }
                else
                {
                    res.transactions = new List<TransactionVM>();

                    for (int i = 0; i < 4; i++)
                    {
                        res.transactions.Add(new TransactionVM()
                        {
                            AccDocument = null,
                            Account = null,
                            Amount = 0,
                            Cheque = null,
                            Contact = null,
                            Credit = 0,
                            Debit = 0,
                            Description = "",
                            DetailAccount = null,
                            Id = 0,
                            Invoice = null,
                            IsCredit = false,
                            IsDebit = true,
                            PaymentMethod = 0,
                            PaymentMethodString = "نامشخص",
                            Reference = "",
                            Remaining = 0,
                            RemainingType = null,
                            RowNumber = 0,
                            Stock = 0,
                            TransactionTypeString = "بدهکار",
                            Type = 0,
                            UnitPrice = 0,
                        });
                    }
                }


            }

            return res;



        }
        private async Task<OpeningBalance> CreateTransObjLiabilities()
        {

            var res = new OpeningBalance();

            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            //var account = accounts.Where(x => x.ComplteCoding == "1105").SingleOrDefault();

            var firstDocument = await Rule.GetFirstDocument(organId);
            if (firstDocument == null)
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = null,
                    Amount = 0,
                    Cheque = null,
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = true,
                    IsDebit = false,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بستانکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                res.transactions = new List<TransactionVM>();
                for (int i = 0; i < 4; i++)
                {
                    res.transactions.Add(new TransactionVM()
                    {
                        AccDocument = null,
                        Account = null,
                        Amount = 0,
                        Cheque = null,
                        Contact = null,
                        Credit = 0,
                        Debit = 0,
                        Description = "",
                        DetailAccount = null,
                        Id = 0,
                        Invoice = null,
                        IsCredit = true,
                        IsDebit = false,
                        PaymentMethod = 0,
                        PaymentMethodString = "نامشخص",
                        Reference = "",
                        Remaining = 0,
                        RemainingType = null,
                        RowNumber = 0,
                        Stock = 0,
                        TransactionTypeString = "بستانکار",
                        Type = 0,
                        UnitPrice = 0,
                    });
                }

            }
            else
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = null,
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = true,
                    IsDebit = false,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بستانکار",
                    Type = 0,
                    UnitPrice = 0,
                };
                TransactionRule transactionRule = new TransactionRule();
                var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

                List<string> ids = new List<string>() { "2103", "2104", "2105", "2106", "2201" };
                var parentAccountsQuery = accounts.AsQueryable().Where(x => ids.Contains(x.ComplteCoding)).ToList();

                var accountsQuery = accounts.AsQueryable().Where(x => ids.Contains(x.ComplteCoding)).ToList();
                var allAccountQuery = accounts.AsQueryable();


                List<int> childcashIds = (from account1 in accountsQuery
                                          join allAccount in allAccountQuery
                                          on account1.ID equals allAccount.ParentId
                                          select allAccount.ID).ToList();



                List<int> childcashIds2 = (from parentAccounts in parentAccountsQuery
                                           select parentAccounts.ID).ToList();

                var selected = firstDocument.Transactions.Where(a => childcashIds.Contains(a.AccountId) || childcashIds2.Contains(a.AccountId)).ToList();

                if (selected.Any())
                {
                    res.transactions = new List<TransactionVM>();

                    foreach (var transaction in selected ?? new List<DomainClasses.Accounting.Transaction>())
                    {
                        var oldAccount = await accountRule.FindAsync(transaction.AccountId);

                        TransactionVM transactionVM = new TransactionVM();

                        transactionVM.AccDocument = null;
                        if (oldAccount.Level == ZhivarEnums.AccountType.Moen)
                        {
                            transactionVM.Account = new AccountVM
                            {
                                Balance = 0,
                                BalanceType = 2,
                                Code = oldAccount.Coding,
                                Coding = oldAccount.ComplteCoding,
                                GroupCode = "1",
                                ID = oldAccount.ID,
                                Level = oldAccount.Level,
                                LevelString = "معین",
                                Name = oldAccount.Name,
                                ParentCoding = oldAccount.ParentId.ToString(),
                                SystemAccount = 7,
                                SystemAccountName = oldAccount.Name,
                                credit = 0,
                                debit = 0,
                            };
                        }

                        transactionVM.Amount = transaction.Amount;
                        transactionVM.Cheque = null;
                        transactionVM.Contact = null;
                        transactionVM.Credit = transaction.Credit;
                        transactionVM.Debit = transaction.Debit;
                        transactionVM.Description = transaction.Description;

                        if (oldAccount.Level == ZhivarEnums.AccountType.Tafzeli)
                        {
                            transactionVM.DetailAccount = new DetailAccount()
                            {
                                Accounts = new List<AccountVM>()
                                {
                                    new AccountVM()
                                    {
                                        Balance = transaction.Credit - transaction.Debit,
                                        Coding = oldAccount.Coding,
                                        credit = transaction.Credit,
                                        debit = transaction.Debit,
                                        ID = oldAccount.ID,
                                        Name = oldAccount.Name,
                                        ParentId = oldAccount.ParentId,
                                        Level = ZhivarEnums.AccountType.Tafzeli,
                                        LevelString = "تفضیلی",
                                    }
                                },
                                Balance = transaction.Credit - transaction.Debit,
                                Code = oldAccount.Coding,
                                Name = oldAccount.Name,
                                credit = transaction.Credit,
                                debit = transaction.Debit,
                                Id = oldAccount.ID
                            };
                        }

                        transactionVM.Id = transaction.ID;
                        transactionVM.Invoice = null;
                        transactionVM.IsCredit = true;
                        transactionVM.IsDebit = false;
                        transactionVM.PaymentMethod = 0;
                        transactionVM.PaymentMethodString = "نامشخص";
                        transactionVM.Remaining = 0;
                        transactionVM.RemainingType = null;
                        transactionVM.RowNumber = 0;
                        transactionVM.Stock = 0;
                        transactionVM.TransactionTypeString = "بستانکار";
                        transactionVM.Type = 0;
                        transactionVM.UnitPrice = 0;

                        res.transactions.Add(transactionVM);
                    }

                }
                else
                {
                    res.transactions = new List<TransactionVM>();

                    for (int i = 0; i < 4; i++)
                    {
                        res.transactions.Add(new TransactionVM()
                        {
                            AccDocument = null,
                            Account = null,
                            Amount = 0,
                            Cheque = null,
                            Contact = null,
                            Credit = 0,
                            Debit = 0,
                            Description = "",
                            DetailAccount = null,
                            Id = 0,
                            Invoice = null,
                            IsCredit = true,
                            IsDebit = false,
                            PaymentMethod = 0,
                            PaymentMethodString = "نامشخص",
                            Reference = "",
                            Remaining = 0,
                            RemainingType = null,
                            RowNumber = 0,
                            Stock = 0,
                            TransactionTypeString = "بستانکار",
                            Type = 0,
                            UnitPrice = 0,
                        });
                    }
                }


            }

            return res;



        }
        private async Task<OpeningBalance> CreateTransObjReceivables(string complteCoding)
        {

            var res = new OpeningBalance();

            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var account = accounts.Where(x => x.ComplteCoding == complteCoding).SingleOrDefault();

            DocumentRule documentRule = new DocumentRule();
            var firstDocument = await documentRule.GetFirstDocument(organId);
            if (firstDocument == null)
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = new AccountVM()
                    {
                        Balance = 0,
                        BalanceType = 2,
                        Code = account.Coding,
                        Coding = account.ComplteCoding,
                        GroupCode = "1",
                        ID = account.ID,
                        Level = ZhivarEnums.AccountType.Moen,
                        LevelString = "معین",
                        Name = account.Name,
                        ParentCoding = account.ParentId.ToString(),
                        SystemAccount = 7,
                        SystemAccountName = account.Name,
                        credit = 0,
                        debit = 0,
                    },
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                res.transactions = CreateNewCashTransactions(account);

            }
            else
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = new AccountVM()
                    {
                        Balance = 0,
                        BalanceType = 2,
                        Code = account.Coding,
                        Coding = account.ComplteCoding,
                        GroupCode = "1",
                        ID = account.ID,
                        Level = ZhivarEnums.AccountType.Moen,
                        LevelString = "معین",
                        Name = account.Name,
                        ParentCoding = account.ParentId.ToString(),
                        SystemAccount = 7,
                        SystemAccountName = account.Name,
                        credit = 0,
                        debit = 0,
                    },
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                TransactionRule transactionRule = new TransactionRule();
                var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

                var accountsQuery = accounts.AsQueryable().Where(x => x.ComplteCoding == complteCoding);
                var cashsQuery = accounts.AsQueryable();


                List<int> childcashIds = (from account2 in accountsQuery
                                          join cash in cashsQuery
                                          on account2.ID equals cash.ParentId
                                          select cash.ID).ToList();

                var selected = firstDocument.Transactions.Where(a => childcashIds.Contains(a.AccountId)).ToList();

                if (selected.Any())
                {
                    res.transactions = new List<TransactionVM>();

                    foreach (var transaction in selected ?? new List<DomainClasses.Accounting.Transaction>())
                    {

                        var oldAccount = await accountRule.FindAsync(transaction.AccountId);
                        Cheque cheque = null;
                        if (transaction.ChequeId != null)
                        {
                            ChequeRule chequeRule = new ChequeRule();
                            cheque = await chequeRule.FindAsync(transaction.ChequeId);
                        }

                        Contact contact = null;
                        if (transaction.ContactId != null)
                        {
                            ContactRule contactRule = new ContactRule();
                            contact = await contactRule.FindAsync((int)transaction.ContactId);
                        }


                        res.transactions.Add(new TransactionVM()
                        {
                            AccDocument = null,
                            Account = new AccountVM()
                            {
                                Balance = 0,
                                BalanceType = 2,
                                Code = oldAccount.Coding,
                                Coding = oldAccount.ComplteCoding,
                                GroupCode = "1",
                                ID = oldAccount.ID,
                                Level = ZhivarEnums.AccountType.Moen,
                                LevelString = "معین",
                                Name = oldAccount.Name,
                                ParentCoding = oldAccount.ParentId.ToString(),
                                SystemAccount = 7,
                                SystemAccountName = oldAccount.Name,
                                credit = 0,
                                debit = 0,
                            },
                            Amount = transaction.Amount,
                            Cheque = cheque,
                            ChequeId = transaction.ChequeId,
                            Contact = contact,
                            ContactId = transaction.ContactId,
                            Credit = transaction.Credit,
                            Debit = transaction.Debit,
                            Description = transaction.Description,
                            DetailAccount = new DetailAccount()
                            {
                                Accounts = new List<AccountVM>()
                            {
                                new AccountVM()
                                {
                                    Balance = transaction.Credit - transaction.Debit,
                                    Coding = account.Coding,
                                    credit = transaction.Credit,
                                    debit = transaction.Debit,
                                    ID = account.ID,
                                    Name = account.Name,
                                    ParentId = account.ParentId,
                                    Level = ZhivarEnums.AccountType.Tafzeli,
                                    LevelString = "تفضیلی"
                                }
                            },
                                Balance = transaction.Credit - transaction.Debit,
                                Code = oldAccount.Coding,
                                Name = oldAccount.Name,
                                credit = transaction.Credit,
                                debit = transaction.Debit,
                                Id = oldAccount.ID
                            }
                            ,
                            Id = transaction.ID,
                            Invoice = null,
                            IsCredit = false,
                            IsDebit = true,
                            PaymentMethod = 0,
                            PaymentMethodString = "نامشخص",

                            Remaining = 0,
                            RemainingType = null,
                            RowNumber = 0,
                            Stock = 0,
                            TransactionTypeString = "بدهکار",
                            Type = 0,
                            UnitPrice = 0
                        });
                    }

                }
                else
                    res.transactions = CreateNewCashTransactions(account);
            }

            return res;


        }
        private async Task<OpeningBalance> CreateTransObjPayables()
        {

            var res = new OpeningBalance();

            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var account = accounts.Where(x => x.ComplteCoding == "2102").SingleOrDefault();

            DocumentRule documentRule = new DocumentRule();
            var firstDocument = await documentRule.GetFirstDocument(organId);
            if (firstDocument == null)
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = new AccountVM()
                    {
                        Balance = 0,
                        BalanceType = 2,
                        Code = account.Coding,
                        Coding = account.ComplteCoding,
                        GroupCode = "1",
                        ID = account.ID,
                        Level = ZhivarEnums.AccountType.Moen,
                        LevelString = "معین",
                        Name = account.Name,
                        ParentCoding = account.ParentId.ToString(),
                        SystemAccount = 7,
                        SystemAccountName = account.Name,
                        credit = 0,
                        debit = 0,
                    },
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                res.transactions = CreateNewCashTransactions(account);

            }
            else
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = new AccountVM()
                    {
                        Balance = 0,
                        BalanceType = 2,
                        Code = account.Coding,
                        Coding = account.ComplteCoding,
                        GroupCode = "1",
                        ID = account.ID,
                        Level = ZhivarEnums.AccountType.Moen,
                        LevelString = "معین",
                        Name = account.Name,
                        ParentCoding = account.ParentId.ToString(),
                        SystemAccount = 7,
                        SystemAccountName = account.Name,
                        credit = 0,
                        debit = 0,
                    },
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                TransactionRule transactionRule = new TransactionRule();
                var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

                var accountsQuery = accounts.AsQueryable().Where(x => x.ComplteCoding == "2102");
                var cashsQuery = accounts.AsQueryable();


                List<int> childcashIds = (from account2 in accountsQuery
                                          join cash in cashsQuery
                                          on account2.ID equals cash.ParentId
                                          select cash.ID).ToList();

                var selected = firstDocument.Transactions.Where(a => childcashIds.Contains(a.AccountId)).ToList();

                if (selected.Any())
                {
                    res.transactions = new List<TransactionVM>();

                    foreach (var transaction in selected ?? new List<Zhivar.DomainClasses.Accounting.Transaction>())
                    {
                        var oldAccount = await accountRule.FindAsync(transaction.AccountId);

                        ChequeRule chequeRule = new ChequeRule();
                        var cheque = await chequeRule.FindAsync((int)transaction.ChequeId);



                        res.transactions.Add(new TransactionVM()
                        {
                            AccDocument = null,
                            Account = new AccountVM()
                            {
                                Balance = 0,
                                BalanceType = 2,
                                Code = oldAccount.Coding,
                                Coding = oldAccount.ComplteCoding,
                                GroupCode = "1",
                                ID = oldAccount.ID,
                                Level = ZhivarEnums.AccountType.Moen,
                                LevelString = "معین",
                                Name = oldAccount.Name,
                                ParentCoding = oldAccount.ParentId.ToString(),
                                SystemAccount = 7,
                                SystemAccountName = oldAccount.Name,
                                credit = 0,
                                debit = 0,
                            },
                            Amount = transaction.Amount,
                            Cheque = cheque,
                            Contact = null,
                            Credit = transaction.Credit,
                            Debit = transaction.Debit,
                            Description = transaction.Description,
                            DetailAccount = new DetailAccount()
                            {
                                Accounts = new List<AccountVM>()
                            {
                                new AccountVM()
                                {
                                    Balance = transaction.Credit - transaction.Debit,
                                    Coding = account.Coding,
                                    credit = transaction.Credit,
                                    debit = transaction.Debit,
                                    ID = account.ID,
                                    Name = account.Name,
                                    ParentId = account.ParentId,
                                    Level = ZhivarEnums.AccountType.Tafzeli,
                                    LevelString = "تفضیلی"
                                }
                            },
                                Balance = transaction.Credit - transaction.Debit,
                                Code = oldAccount.Coding,
                                Name = oldAccount.Name,
                                credit = transaction.Credit,
                                debit = transaction.Debit,
                                Id = oldAccount.ID
                            }
                            ,
                            Id = transaction.ID,
                            Invoice = null,
                            IsCredit = false,
                            IsDebit = true,
                            PaymentMethod = 0,
                            PaymentMethodString = "نامشخص",

                            Remaining = 0,
                            RemainingType = null,
                            RowNumber = 0,
                            Stock = 0,
                            TransactionTypeString = "بدهکار",
                            Type = 0,
                            UnitPrice = 0
                        });
                    }

                }
                else
                    res.transactions = CreateNewCashTransactions(account);
            }

            return res;


        }
        private async Task<OpeningBalance> CreateTransObjDebtors()
        {

            var res = new OpeningBalance();

            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var account = accounts.Where(x => x.ComplteCoding == "1104").SingleOrDefault();

            var firstDocument = await Rule.GetFirstDocument(organId);
            if (firstDocument == null)
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = new AccountVM()
                    {
                        Balance = 0,
                        BalanceType = 2,
                        Code = account.Coding,
                        Coding = account.ComplteCoding,
                        GroupCode = "1",
                        ID = account.ID,
                        Level = ZhivarEnums.AccountType.Moen,
                        LevelString = "معین",
                        Name = account.Name,
                        ParentCoding = account.ParentId.ToString(),
                        SystemAccount = 7,
                        SystemAccountName = account.Name,
                        credit = 0,
                        debit = 0,
                    },
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                res.transactions = CreateNewCashTransactions(account);

            }
            else
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = new AccountVM()
                    {
                        Balance = 0,
                        BalanceType = 2,
                        Code = account.Coding,
                        Coding = account.ComplteCoding,
                        GroupCode = "1",
                        ID = account.ID,
                        Level = ZhivarEnums.AccountType.Moen,
                        LevelString = "معین",
                        Name = account.Name,
                        ParentCoding = account.ParentId.ToString(),
                        SystemAccount = 7,
                        SystemAccountName = account.Name,
                        credit = 0,
                        debit = 0,
                    },
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                TransactionRule transactionRule = new TransactionRule();
                var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

                var accountsQuery = accounts.AsQueryable().Where(x => x.ComplteCoding == "1104");
                var cashsQuery = accounts.AsQueryable();


                List<int> childcashIds = (from account2 in accountsQuery
                                          join cash in cashsQuery
                                          on account2.ID equals cash.ParentId
                                          select cash.ID).ToList();


                var selected = firstDocument.Transactions.Where(a => childcashIds.Contains(a.AccountId)).ToList();

                if (selected.Any())
                {
                    res.transactions = new List<TransactionVM>();

                    foreach (var transaction in selected ?? new List<Zhivar.DomainClasses.Accounting.Transaction>())
                    {
                        var oldAccount = await accountRule.FindAsync(transaction.AccountId);

                        res.transactions.Add(new TransactionVM()
                        {
                            AccDocument = null,
                            Account = new AccountVM()
                            {
                                Balance = 0,
                                BalanceType = 2,
                                Code = oldAccount.Coding,
                                Coding = oldAccount.ComplteCoding,
                                GroupCode = "1",
                                ID = oldAccount.ID,
                                Level = ZhivarEnums.AccountType.Moen,
                                LevelString = "معین",
                                Name = oldAccount.Name,
                                ParentCoding = oldAccount.ParentId.ToString(),
                                SystemAccount = 7,
                                SystemAccountName = oldAccount.Name,
                                credit = 0,
                                debit = 0,
                            },
                            Amount = transaction.Amount,
                            Cheque = null,
                            Contact = null,
                            Credit = transaction.Credit,
                            Debit = transaction.Debit,
                            Description = transaction.Description,
                            DetailAccount = new DetailAccount()
                            {
                                Accounts = new List<AccountVM>()
                            {
                                new AccountVM()
                                {
                                    Balance = transaction.Credit - transaction.Debit,
                                    Coding = account.Coding,
                                    credit = transaction.Credit,
                                    debit = transaction.Debit,
                                    ID = account.ID,
                                    Name = account.Name,
                                    ParentId = account.ParentId,
                                    Level = ZhivarEnums.AccountType.Tafzeli,
                                    LevelString = "تفضیلی"
                                }
                            },
                                Balance = transaction.Credit - transaction.Debit,
                                Code = oldAccount.Coding,
                                Name = oldAccount.Name,
                                credit = transaction.Credit,
                                debit = transaction.Debit,
                                Id = oldAccount.ID
                            }
                            ,
                            Id = transaction.ID,
                            Invoice = null,
                            IsCredit = false,
                            IsDebit = true,
                            PaymentMethod = 0,
                            PaymentMethodString = "نامشخص",

                            Remaining = 0,
                            RemainingType = null,
                            RowNumber = 0,
                            Stock = 0,
                            TransactionTypeString = "بدهکار",
                            Type = 0,
                            UnitPrice = 0
                        });
                    }

                }
                else
                    res.transactions = CreateNewCashTransactions(account);
            }

            return res;
        }
        private async Task<OpeningBalance> CreateTransObjCreditors()
        {

            var res = new OpeningBalance();

            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var account = accounts.Where(x => x.ComplteCoding == "2101").SingleOrDefault();

            var firstDocument = await Rule.GetFirstDocument(organId);
            if (firstDocument == null)
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = new AccountVM()
                    {
                        Balance = 0,
                        BalanceType = 2,
                        Code = account.Coding,
                        Coding = account.ComplteCoding,
                        GroupCode = "1",
                        ID = account.ID,
                        Level = ZhivarEnums.AccountType.Moen,
                        LevelString = "معین",
                        Name = account.Name,
                        ParentCoding = account.ParentId.ToString(),
                        SystemAccount = 7,
                        SystemAccountName = account.Name,
                        credit = 0,
                        debit = 0,
                    },
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                res.transactions = CreateNewCashTransactions(account);

            }
            else
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = new AccountVM()
                    {
                        Balance = 0,
                        BalanceType = 2,
                        Code = account.Coding,
                        Coding = account.ComplteCoding,
                        GroupCode = "1",
                        ID = account.ID,
                        Level = ZhivarEnums.AccountType.Moen,
                        LevelString = "معین",
                        Name = account.Name,
                        ParentCoding = account.ParentId.ToString(),
                        SystemAccount = 7,
                        SystemAccountName = account.Name,
                        credit = 0,
                        debit = 0,
                    },
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                TransactionRule transactionRule = new TransactionRule();
                var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

                var accountsQuery = accounts.AsQueryable().Where(x => x.ComplteCoding == "2101");
                var cashsQuery = accounts.AsQueryable();


                List<int> childcashIds = (from account2 in accountsQuery
                                          join cash in cashsQuery
                                          on account2.ID equals cash.ParentId
                                          select cash.ID).ToList();

                var selected = firstDocument.Transactions.Where(a => childcashIds.Contains(a.AccountId)).ToList();

                if (selected.Any())
                {
                    res.transactions = new List<TransactionVM>();

                    foreach (var transaction in selected ?? new List<Zhivar.DomainClasses.Accounting.Transaction>())
                    {
                        var oldAccount = await accountRule.FindAsync(transaction.AccountId);

                        res.transactions.Add(new TransactionVM()
                        {
                            AccDocument = null,
                            Account = new AccountVM()
                            {
                                Balance = 0,
                                BalanceType = 2,
                                Code = oldAccount.Coding,
                                Coding = oldAccount.ComplteCoding,
                                GroupCode = "1",
                                ID = oldAccount.ID,
                                Level = ZhivarEnums.AccountType.Moen,
                                LevelString = "معین",
                                Name = oldAccount.Name,
                                ParentCoding = oldAccount.ParentId.ToString(),
                                SystemAccount = 7,
                                SystemAccountName = oldAccount.Name,
                                credit = 0,
                                debit = 0,
                            },
                            Amount = transaction.Amount,
                            Cheque = null,
                            Contact = null,
                            Credit = transaction.Credit,
                            Debit = transaction.Debit,
                            Description = transaction.Description,
                            DetailAccount = new DetailAccount()
                            {
                                Accounts = new List<AccountVM>()
                            {
                                new AccountVM()
                                {
                                    Balance = transaction.Credit - transaction.Debit,
                                    Coding = account.Coding,
                                    credit = transaction.Credit,
                                    debit = transaction.Debit,
                                    ID = account.ID,
                                    OrganId = account.ID,
                                    Name = account.Name,
                                    ParentId = account.ParentId,
                                    Level = ZhivarEnums.AccountType.Tafzeli,
                                    LevelString = "تفضیلی"
                                }
                            },
                                Balance = transaction.Credit - transaction.Debit,
                                Code = oldAccount.Coding,
                                Name = oldAccount.Name,
                                credit = transaction.Credit,
                                debit = transaction.Debit,
                                Id = oldAccount.ID
                            }
                            ,
                            Id = transaction.ID,
                            Invoice = null,
                            IsCredit = false,
                            IsDebit = true,
                            PaymentMethod = 0,
                            PaymentMethodString = "نامشخص",

                            Remaining = 0,
                            RemainingType = null,
                            RowNumber = 0,
                            Stock = 0,
                            TransactionTypeString = "بدهکار",
                            Type = 0,
                            UnitPrice = 0
                        });
                    }

                }
                else
                    res.transactions = CreateNewCashTransactions(account);
            }

            return res;
        }
        private async Task<OpeningBalance> CreateTransObjProduct()
        {

            var res = new OpeningBalance();

            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var account = accounts.Where(x => x.ComplteCoding == "1108").SingleOrDefault();

            var firstDocument = await Rule.GetFirstDocument(organId);
            if (firstDocument == null)
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = new AccountVM()
                    {
                        Balance = 0,
                        BalanceType = 2,
                        Code = account.Coding,
                        Coding = account.ComplteCoding,
                        GroupCode = "1",
                        ID = account.ID,
                        Level = account.Level,
                        LevelString = "معین",
                        Name = account.Name,
                        ParentCoding = account.ParentId.ToString(),
                        SystemAccount = 7,
                        SystemAccountName = account.Name,
                        credit = 0,
                        debit = 0,
                    },
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                res.transactions = CreateNewCashTransactions(account);

            }
            else
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = new AccountVM()
                    {
                        Balance = 0,
                        BalanceType = 2,
                        Code = account.Coding,
                        Coding = account.ComplteCoding,
                        GroupCode = "1",
                        ID = account.ID,
                        Level = account.Level,
                        LevelString = "معین",
                        Name = account.Name,
                        ParentCoding = account.ParentId.ToString(),
                        SystemAccount = 7,
                        SystemAccountName = account.Name,
                        credit = 0,
                        debit = 0,
                    },
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                TransactionRule transactionRule = new TransactionRule();
                var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

                var accountsQuery = accounts.AsQueryable().Where(x => x.ComplteCoding == "1108");
                var cashsQuery = accounts.AsQueryable();


                List<int> childIds = (from account1 in accountsQuery
                                      join cash in cashsQuery
                                      on account1.ID equals cash.ParentId
                                      select cash.ID).ToList();

                var selected = firstDocument.Transactions.Where(a => childIds.Contains(a.AccountId)).ToList();

                if (selected.Any())
                {
                    res.transactions = new List<TransactionVM>();

                    foreach (var transaction in selected ?? new List<Zhivar.DomainClasses.Accounting.Transaction>())
                    {
                        var oldAccount = await accountRule.FindAsync(transaction.AccountId);

                        res.transactions.Add(new TransactionVM()
                        {
                            AccDocument = null,
                            Account = new AccountVM()
                            {
                                Balance = 0,
                                BalanceType = 2,
                                Code = oldAccount.Coding,
                                Coding = oldAccount.ComplteCoding,
                                GroupCode = "1",
                                ID = oldAccount.ID,
                                Level = oldAccount.Level,
                                LevelString = "معین",
                                Name = oldAccount.Name,
                                ParentCoding = oldAccount.ParentId.ToString(),
                                SystemAccount = 7,
                                SystemAccountName = account.Name,
                                credit = 0,
                                debit = 0,
                            },
                            Amount = transaction.Amount,
                            Cheque = null,
                            Contact = null,
                            Credit = transaction.Credit,
                            Debit = transaction.Debit,
                            Description = transaction.Description,
                            DetailAccount = new DetailAccount()
                            {
                                Accounts = new List<AccountVM>()
                            {
                                new AccountVM()
                                {
                                    Balance = transaction.Credit - transaction.Debit,
                                    Coding = account.Coding,
                                    credit = transaction.Credit,
                                    debit = transaction.Debit,
                                    ID = account.ID,
                                    Name = account.Name,
                                    ParentId = account.ParentId,
                                    Level = ZhivarEnums.AccountType.Tafzeli,
                                    LevelString = "تفضیلی"
                                }
                            },
                                Balance = transaction.Credit - transaction.Debit,
                                Code = oldAccount.Coding,
                                Name = oldAccount.Name,
                                credit = transaction.Credit,
                                debit = transaction.Debit,
                                Id = oldAccount.ID
                            }
                            ,
                            Id = transaction.ID,
                            Invoice = null,
                            IsCredit = false,
                            IsDebit = true,
                            PaymentMethod = 0,
                            PaymentMethodString = "نامشخص",

                            Remaining = 0,
                            RemainingType = null,
                            RowNumber = 0,
                            Stock = transaction.Stock,
                            TransactionTypeString = "بدهکار",
                            Type = 0,
                            UnitPrice = transaction.UnitPrice
                        });
                    }

                }
                else
                    res.transactions = CreateNewCashTransactions(account);
            }

            return res;
        }

        private List<TransactionVM> CreateNewProductTransactions(DomainClasses.Accounting.Account account)
        {
            throw new NotImplementedException();
        }



        [HttpPost]
        [Route("SubmitOpeningBalance")]
        public async Task<HttpResponseMessage> SubmitOpeningBalance([FromBody] OpeningBalanceSaveVM openingBalanceSaveVM)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var document = await CreateFirstDocument(openingBalanceSaveVM.docDate, organId);


                switch (openingBalanceSaveVM.systemAccount)
                {
                    case "cash":
                        {

                            decimal sumCash = await Rule.CreateDocumentOpeningBalanceCash(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            await Rule.SaveChangesAsync();

                            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = sumCash });

                        }
                    case "bank":
                        {
                            decimal sumBank = await Rule.CreateDocumentOpeningBalanceCash(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            // decimal sumBank = await Rule.CreateDocumentOpeningBalanceBank(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            await Rule.SaveChangesAsync();
                            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = sumBank });
                        }
                    case "product":
                        {
                            decimal sumProduct = await Rule.CreateDocumentOpeningBalanceCash(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            // decimal sumBank = await Rule.CreateDocumentOpeningBalanceBank(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            await Rule.SaveChangesAsync();
                            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = sumProduct });
                        }
                    case "debtors":
                        {
                            decimal sumDebtors = await Rule.CreateDocumentOpeningBalanceCash(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            // decimal sumBank = await Rule.CreateDocumentOpeningBalanceBank(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            await Rule.SaveChangesAsync();
                            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = sumDebtors });
                        }
                    case "payables":
                        {
                            decimal sumPayables = await Rule.CreateDocumentOpeningBalancePayables(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            // decimal sumBank = await Rule.CreateDocumentOpeningBalanceBank(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            await Rule.SaveChangesAsync();
                            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = sumPayables });
                        }
                    case "receivables":
                    case "inProgress":
                        {
                            decimal sumReceivables = await Rule.CreateDocumentOpeningBalanceReceivables(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            // decimal sumBank = await Rule.CreateDocumentOpeningBalanceBank(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            await Rule.SaveChangesAsync();
                            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = sumReceivables });

                        }
                    case "assets":
                        {
                            decimal sumAssets = await Rule.CreateDocumentOpeningBalanceAssets(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            // decimal sumBank = await Rule.CreateDocumentOpeningBalanceBank(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            await Rule.SaveChangesAsync();
                            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = sumAssets });
                        }
                    case "creditors":
                        {
                            decimal sumCreditor = await Rule.CreateDocumentOpeningBalanceCreditor(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            // decimal sumBank = await Rule.CreateDocumentOpeningBalanceBank(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            await Rule.SaveChangesAsync();
                            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = sumCreditor });
                        }
                    case "liabilities":
                        {
                            decimal sumOtherLiabilities = await Rule.CreateDocumentOpeningBalanceOtherLiabilities(document, openingBalanceSaveVM.transactions, openingBalanceSaveVM.docDate, organId);
                            await Rule.SaveChangesAsync();
                            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = sumOtherLiabilities });
                        }
                    default:
                        break;
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = "" });
            }
            catch (Exception e)
            {

                throw;
            }
        }


        [HttpPost]
        [Route("SubmitOpeningBalanceItems")]
        public async Task<HttpResponseMessage> SubmitOpeningBalanceItems([FromBody] OpeningBalanceSaveVM openingBalanceSaveVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var document = await CreateFirstDocument(openingBalanceSaveVM.docDate, organId);

            decimal sumItem = await Rule.CreateDocumentOpeningBalanceItem(document, openingBalanceSaveVM.items, openingBalanceSaveVM.docDate, organId);

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = sumItem });

        }

        [HttpPost]
        [Route("SaveDocument")]
        public async Task<HttpResponseMessage> SaveDocument([FromBody] DocumentVM documentVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            await Rule.SaveDocument(documentVM, organId);
            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = documentVM });

        }

        [HttpPost]
        [Route("GetOpeningDocument")]
        public async Task<HttpResponseMessage> GetOpeningDocument()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var document = await Rule.GetFirstDocumentVM(organId);

            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = document });
        }

        [HttpPost]
        [Route("DeleteManualDocument")]
        public async Task<HttpResponseMessage> DeleteManualDocument([FromBody] int id)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var document = Rule.FindAsync(id);

            await Rule.DeleteAsync(id);

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = document });
        }


        private async Task<Document> CreateFirstDocument(string docDate, int organId)
        {
            var document = await Rule.GetFirstDocument(organId);

            if (document == null || document.ID  == 0)
            {
                document = new Document();
                document.Credit = 0;
                document.DateTime = Utilities.PersianDateUtils.ToDateTime(docDate);
                document.Debit = 0;
                document.Description = "سند افتتاحیه";
                document.DisplayDate = docDate;
                document.IsFirsDocument = true;
                document.IsManual = false;
                document.Number = await Rule.createNumberDocumentAsync(organId);
                document.Number2 = await Rule.createNumberDocumentAsync(organId);
                document.OrganId = organId;
                document.Status = ZhivarEnums.DocumentStatus.TaeedShode;
                document.StatusString = "تایید شده";
                document.Type = ZhivarEnums.NoeDoc.FirstDoc;

                document.Transactions = new List<DomainClasses.Accounting.Transaction>();

                await Rule.InsertAsync(document, organId);
                await Rule.SaveChangesAsync();
            }

            //if (document.Transactions == null)
            //    document.Transactions = new List<DomainClasses.Accounting.Transaction>();

            return document;

        }

        private async Task<OpeningBalance> CreateTransObjCash()
        {
            var res = new OpeningBalance();

            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var cashAccount = accounts.Where(x => x.ComplteCoding == "1101").SingleOrDefault();

            var firstDocument = await Rule.GetFirstDocument(organId);
            if (firstDocument == null)
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = new AccountVM()
                    {
                        Balance = 0,
                        BalanceType = 2,
                        Code = cashAccount.Coding,
                        Coding = cashAccount.ComplteCoding,
                        GroupCode = "1",
                        ID = cashAccount.ID,
                        Level = ZhivarEnums.AccountType.Moen,
                        LevelString = "معین",
                        Name = cashAccount.Name,
                        ParentCoding = cashAccount.ParentId.ToString(),
                        SystemAccount = 7,
                        SystemAccountName = "صندوق",
                        credit = 0,
                        debit = 0,
                    },
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                res.transactions = CreateNewCashTransactions(cashAccount);

            }
            else
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = new AccountVM()
                    {
                        Balance = 0,
                        BalanceType = 2,
                        Code = cashAccount.Coding,
                        Coding = cashAccount.ComplteCoding,
                        GroupCode = "1",
                        ID = cashAccount.ID,
                        Level = ZhivarEnums.AccountType.Moen,
                        LevelString = "معین",
                        Name = cashAccount.Name,
                        ParentCoding = cashAccount.ParentId.ToString(),
                        SystemAccount = 7,
                        SystemAccountName = "صندوق",
                        credit = 0,
                        debit = 0,
                    },
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                TransactionRule transactionRule = new TransactionRule();
                var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

                var accountsQuery = accounts.AsQueryable().Where(x => x.ComplteCoding == "1101");
                var cashsQuery = accounts.AsQueryable();


                List<int> childcashIds = (from account in accountsQuery
                                          join cash in cashsQuery
                                          on account.ID equals cash.ParentId
                                          select cash.ID).ToList();

                var selected = firstDocument.Transactions.Where(a => childcashIds.Contains(a.AccountId)).ToList();

                if (selected.Any())
                {
                    res.transactions = new List<TransactionVM>();

                    foreach (var transaction in selected ?? new List<Zhivar.DomainClasses.Accounting.Transaction>())
                    {
                        var oldCashAccount = await accountRule.FindAsync(transaction.AccountId);

                        res.transactions.Add(new TransactionVM()
                        {
                            AccDocument = null,
                            Account = new AccountVM()
                            {
                                Balance = 0,
                                BalanceType = 2,
                                Code = oldCashAccount.Coding,
                                Coding = oldCashAccount.ComplteCoding,
                                GroupCode = "1",
                                ID = oldCashAccount.ID,
                                Level = ZhivarEnums.AccountType.Moen,
                                LevelString = "معین",
                                Name = oldCashAccount.Name,
                                ParentCoding = oldCashAccount.ParentId.ToString(),
                                SystemAccount = 7,
                                SystemAccountName = "صندوق",
                                credit = 0,
                                debit = 0,
                            },
                            Amount = transaction.Amount,
                            Cheque = null,
                            Contact = null,
                            Credit = transaction.Credit,
                            Debit = transaction.Debit,
                            Description = transaction.Description,
                            DetailAccount = new DetailAccount()
                            {
                                Accounts = new List<AccountVM>()
                            {
                                new AccountVM()
                                {
                                    Balance = transaction.Credit - transaction.Debit,
                                    Coding = cashAccount.Coding,
                                    credit = transaction.Credit,
                                    debit = transaction.Debit,
                                    ID = cashAccount.ID,
                                    Name = cashAccount.Name,
                                    ParentId = cashAccount.ParentId,
                                    Level = ZhivarEnums.AccountType.Tafzeli,
                                    LevelString = "تفضیلی"
                                }
                            },
                                Balance = transaction.Credit - transaction.Debit,
                                Code = oldCashAccount.Coding,
                                Name = oldCashAccount.Name,
                                credit = transaction.Credit,
                                debit = transaction.Debit,
                                Id = oldCashAccount.ID
                            }
                            ,
                            Id = transaction.ID,
                            Invoice = null,
                            IsCredit = false,
                            IsDebit = true,
                            PaymentMethod = 0,
                            PaymentMethodString = "نامشخص",

                            Remaining = 0,
                            RemainingType = null,
                            RowNumber = 0,
                            Stock = 0,
                            TransactionTypeString = "بدهکار",
                            Type = 0,
                            UnitPrice = 0
                        });
                    }

                }
                else
                    res.transactions = CreateNewCashTransactions(cashAccount);
            }

            return res;
        }

        private List<TransactionVM> CreateNewCashTransactions(DomainClasses.Accounting.Account cashAccount)
        {
            List<TransactionVM> transactions = new List<TransactionVM>();

            transactions.Add(new TransactionVM()
            {
                AccDocument = null,
                Account = new AccountVM()
                {
                    Balance = 0,
                    BalanceType = 2,
                    Code = cashAccount.Coding,
                    Coding = cashAccount.ComplteCoding,
                    GroupCode = "1",
                    ID = cashAccount.ID,
                    Level = ZhivarEnums.AccountType.Moen,
                    LevelString = "معین",
                    Name = cashAccount.Name,
                    ParentCoding = cashAccount.ParentId.ToString(),
                    SystemAccount = 7,
                    SystemAccountName = "صندوق",
                    credit = 0,
                    debit = 0,
                },
                Amount = 0,
                Cheque = null,
                Contact = null,
                Credit = 0,
                Debit = 0,
                Description = "",
                DetailAccount = null,
                Id = 0,
                Invoice = null,
                IsCredit = false,
                IsDebit = true,
                PaymentMethod = 0,
                PaymentMethodString = "نامشخص",
                Remaining = 0,
                RemainingType = null,
                RowNumber = 0,
                Stock = 0,
                TransactionTypeString = "بدهکار",
                Type = 0,
                UnitPrice = 0
            });
            transactions.Add(new TransactionVM()
            {
                AccDocument = null,
                Account = new AccountVM()
                {
                    Balance = 0,
                    BalanceType = 2,
                    Code = cashAccount.Coding,
                    Coding = cashAccount.ComplteCoding,
                    GroupCode = "1",
                    ID = cashAccount.ID,
                    Level = ZhivarEnums.AccountType.Moen,
                    LevelString = "معین",
                    Name = cashAccount.Name,
                    ParentCoding = cashAccount.ParentId.ToString(),
                    SystemAccount = 7,
                    SystemAccountName = "صندوق",
                    credit = 0,
                    debit = 0,
                },
                Amount = 0,
                Cheque = null,
                Contact = null,
                Credit = 0,
                Debit = 0,
                Description = "",
                DetailAccount = null,
                Id = 0,
                Invoice = null,
                IsCredit = false,
                IsDebit = true,
                PaymentMethod = 0,
                PaymentMethodString = "نامشخص",
                //Reference = "",
                Remaining = 0,
                RemainingType = null,
                RowNumber = 0,
                Stock = 0,
                TransactionTypeString = "بدهکار",
                Type = 0,
                UnitPrice = 0
            });

            return transactions;
        }

        private async Task<OpeningBalance> CreateTransObjBank()
        {
            var res = new OpeningBalance();

            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var bankAccount = accounts.Where(x => x.ComplteCoding == "1103").SingleOrDefault();

            var firstDocument = await Rule.GetFirstDocument(organId);
            if (firstDocument == null)
            {
                res = new OpeningBalance();
                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = new AccountVM()
                    {
                        Balance = 0,
                        BalanceType = 2,
                        Code = bankAccount.Coding,
                        Coding = bankAccount.ComplteCoding,
                        GroupCode = "1",
                        ID = bankAccount.ID,
                        Level = ZhivarEnums.AccountType.Moen,
                        LevelString = "معین",
                        Name = bankAccount.Name,
                        ParentCoding = bankAccount.ParentId.ToString(),
                        SystemAccount = 7,
                        SystemAccountName = "بانک",
                        credit = 0,
                        debit = 0,
                    },
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                res.transactions = CreateNewBankTranactions(bankAccount);
            }
            else
            {
                res = new OpeningBalance();

                res.transObj = new TransObj()
                {
                    AccDocument = new DocumentVM(),
                    Account = new AccountVM()
                    {
                        Balance = 0,
                        BalanceType = 2,
                        Code = bankAccount.Coding,
                        Coding = bankAccount.ComplteCoding,
                        GroupCode = "1",
                        ID = bankAccount.ID,
                        Level = ZhivarEnums.AccountType.Moen,
                        LevelString = "معین",
                        Name = bankAccount.Name,
                        ParentCoding = bankAccount.ParentId.ToString(),
                        SystemAccount = 7,
                        SystemAccountName = "بانک",
                        credit = 0,
                        debit = 0,
                    },
                    Amount = 0,
                    Cheque = new Cheque(),
                    Contact = null,
                    Credit = 0,
                    Debit = 0,
                    Description = "",
                    DetailAccount = null,
                    Id = 0,
                    Invoice = null,
                    IsCredit = false,
                    IsDebit = true,
                    PaymentMethod = 0,
                    PaymentMethodString = "نامشخص",
                    Reference = "",
                    Remaining = 0,
                    RemainingType = null,
                    RowNumber = 0,
                    Stock = 0,
                    TransactionTypeString = "بدهکار",
                    Type = 0,
                    UnitPrice = 0,
                };

                res.transactions = new List<TransactionVM>();

                TransactionRule transactionRule = new TransactionRule();
                var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

                var accountsQuery = accounts.AsQueryable().Where(x => x.ComplteCoding == "1103");
                var banksQuery = accounts.AsQueryable();


                List<int> childBankIds = (from account in accountsQuery
                                          join bank in banksQuery
                                          on account.ID equals bank.ParentId
                                          select bank.ID).ToList();

                var selected = firstDocument.Transactions.Where(a => childBankIds.Contains(a.AccountId)).ToList();

                if (selected.Any())
                {
                    foreach (var transaction in selected ?? new List<DomainClasses.Accounting.Transaction>())
                    {
                        var oldbankAccount = await accountRule.FindAsync(transaction.AccountId);

                        res.transactions.Add(new TransactionVM()
                        {
                            AccDocument = null,
                            Account = new AccountVM()
                            {
                                Balance = 0,
                                BalanceType = 2,
                                Code = oldbankAccount.Coding,
                                Coding = oldbankAccount.ComplteCoding,
                                GroupCode = "1",
                                ID = oldbankAccount.ID,
                                Level = ZhivarEnums.AccountType.Moen,
                                LevelString = "معین",
                                Name = oldbankAccount.Name,
                                ParentCoding = oldbankAccount.ParentId.ToString(),
                                SystemAccount = 7,
                                SystemAccountName = "بانک",
                                credit = 0,
                                debit = 0,
                            },
                            Amount = transaction.Amount,
                            Cheque = null,
                            Contact = null,
                            Credit = transaction.Credit,
                            Debit = transaction.Debit,
                            Description = transaction.Description,
                            DetailAccount = new DetailAccount()
                            {
                                Accounts = new List<AccountVM>()
                            {
                                new AccountVM()
                                {
                                    Balance = transaction.Credit - transaction.Debit,
                                    Coding = bankAccount.Coding,
                                    credit = transaction.Credit,
                                    debit = transaction.Debit,
                                    ID = bankAccount.ID,
                                    Name = bankAccount.Name,
                                    ParentId = bankAccount.ParentId,
                                    Level = ZhivarEnums.AccountType.Tafzeli,
                                    LevelString = "تفضیلی"
                                }
                            },
                                Balance = transaction.Credit - transaction.Debit,
                                Code = oldbankAccount.Coding,
                                Name = oldbankAccount.Name,
                                credit = transaction.Credit,
                                debit = transaction.Debit,
                                Id = oldbankAccount.ID
                            }
                            ,
                            Id = transaction.ID,
                            Invoice = null,
                            IsCredit = false,
                            IsDebit = true,
                            PaymentMethod = 0,
                            PaymentMethodString = "نامشخص",
                            //Reference = "",
                            Remaining = 0,
                            RemainingType = null,
                            RowNumber = 0,
                            Stock = 0,
                            TransactionTypeString = "بدهکار",
                            Type = 0,
                            UnitPrice = 0
                        });
                    }
                }
                else
                    res.transactions = CreateNewBankTranactions(bankAccount);

            }

            return res;
        }

        private List<TransactionVM> CreateNewBankTranactions(DomainClasses.Accounting.Account bankAccount)
        {
            List<TransactionVM> transactions = new List<TransactionVM>();
            transactions.Add(new TransactionVM()
            {
                AccDocument = null,
                Account = new AccountVM()
                {
                    Balance = 0,
                    BalanceType = 2,
                    Code = bankAccount.Coding,
                    Coding = bankAccount.ComplteCoding,
                    GroupCode = "1",
                    ID = bankAccount.ID,
                    Level = ZhivarEnums.AccountType.Moen,
                    LevelString = "معین",
                    Name = bankAccount.Name,
                    ParentCoding = bankAccount.ParentId.ToString(),
                    SystemAccount = 7,
                    SystemAccountName = "بانک",
                    credit = 0,
                    debit = 0,
                },
                Amount = 0,
                Cheque = null,
                Contact = null,
                Credit = 0,
                Debit = 0,
                Description = "",
                DetailAccount = null,
                Id = 0,
                Invoice = null,
                IsCredit = false,
                IsDebit = true,
                PaymentMethod = 0,
                PaymentMethodString = "نامشخص",
                //Reference = "",
                Remaining = 0,
                RemainingType = null,
                RowNumber = 0,
                Stock = 0,
                TransactionTypeString = "بدهکار",
                Type = 0,
                UnitPrice = 0
            });
            transactions.Add(new TransactionVM()
            {
                AccDocument = null,
                Account = new AccountVM()
                {
                    Balance = 0,
                    BalanceType = 2,
                    Code = bankAccount.Coding,
                    Coding = bankAccount.ComplteCoding,
                    GroupCode = "1",
                    ID = bankAccount.ID,
                    Level = ZhivarEnums.AccountType.Moen,
                    LevelString = "معین",
                    Name = bankAccount.Name,
                    ParentCoding = bankAccount.ParentId.ToString(),
                    SystemAccount = 7,
                    SystemAccountName = "بانک",
                    credit = 0,
                    debit = 0,
                },
                Amount = 0,
                Cheque = null,
                Contact = null,
                Credit = 0,
                Debit = 0,
                Description = "",
                DetailAccount = null,
                Id = 0,
                Invoice = null,
                IsCredit = false,
                IsDebit = true,
                PaymentMethod = 0,
                PaymentMethodString = "نامشخص",
                //Reference = "",
                Remaining = 0,
                RemainingType = null,
                RowNumber = 0,
                Stock = 0,
                TransactionTypeString = "بدهکار",
                Type = 0,
                UnitPrice = 0
            });

            return transactions;
        }


    }
}
