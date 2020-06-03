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
using Zhivar.DomainClasses.Common;
using Zhivar.DataLayer.Validation;
using OMF.Common.Security;
using Zhivar.Business.Common;
using Zhivar.Business.Accounting;
using OMF.Enterprise.MVC;
using OMF.Business;
using Zhivar.Business.BaseInfo;

namespace Zhivar.Web.Controllers.Accounting
{
    [RoutePrefix("api/PayRecevie")]
    public partial class PayRecevieController : NewApiControllerBaseAsync<PayRecevie, PayRecevieVM>
    {
        public PayRecevieRule Rule => this.BusinessRule as PayRecevieRule;

        protected override IBusinessRuleBaseAsync<PayRecevie> CreateBusinessRule()
        {
            return new PayRecevieRule();
        }

        [Route("GetLoadTransfer")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetLoadTransfer()
        {
            LoadTransfer loadTransfer = new LoadTransfer()
            {
                Amount = 0,
                Date = null,
                Description = "",
                DocumentId = 0,
                DocumentNumber = 0,
                From = null,
                FromDetailAccountId = 0,
                FromDetailAccountName = null,
                FromReference = "",
                To = null,
                ToDetailAccountId = 0,
                ToDetailAccountName = null,
                ToReference = "",
            };

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = loadTransfer });
        }

        [HttpPost]
        [Route("GetReceiveAndPayList")]
        public  async Task<HttpResponseMessage> GetReceiveAndPayList([FromBody] bool isReceive)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var payRecevieQuery = await Rule.GetAllByOrganIdAsync(organId, isReceive);

            if(isReceive)
                payRecevieQuery = payRecevieQuery.Where(x => x.IsRecevie == isReceive).OrderByDescending(x => x.ID).ToList(); // && x.IsCredit == true).OrderByDescending(x => x.ID).ToList();
            else
                payRecevieQuery = payRecevieQuery.Where(x => x.IsRecevie == isReceive).OrderByDescending(x => x.ID).ToList();// && x.IsDebit==true ).OrderByDescending(x => x.ID).ToList();


            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = payRecevieQuery });
        }


 
        public async Task<HttpResponseMessage> SaveReceiveAndPay([FromBody] PayRecevie payRecevie)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                FinanYearRule finanYearRule = new FinanYearRule();
                var finanYerar = await finanYearRule.GetCurrentFinanYear(organId);

                var documentNumber = 0;
                int payRecevieNumber = 0;

                if (payRecevie.ID > 0)
                {
                    PayRecevie temp = new PayRecevie();
                    temp = payRecevie;
                    PayRecevieRule payRecevieRule = new PayRecevieRule();
                    payRecevieRule.Delete(payRecevie.ID);

                    documentNumber = payRecevie.Document.Number;
                    payRecevieNumber = payRecevie.Number;

                    await payRecevieRule.SaveChangesAsync();
                }

                payRecevie.ID = -1;

                if (payRecevie.Contact != null)
                {
                    payRecevie.ContactId = payRecevie.Contact.ID;
                    payRecevie.Contact.ObjectState = OMF.Common.Enums.ObjectState.Unchanged;
                }

                    
                if (payRecevie.Invoice != null)
                {
                    payRecevie.InvoiceId = payRecevie.Invoice.ID;
                    payRecevie.Invoice.ObjectState = OMF.Common.Enums.ObjectState.Unchanged;
                }
                   
                if (payRecevie.Cost != null)
                {
                    payRecevie.CostId = payRecevie.Cost.ID;
                    payRecevie.Cost.ObjectState = OMF.Common.Enums.ObjectState.Unchanged;
                }
                    

                payRecevie.OrganId = organId;

                payRecevie.Type = payRecevie.Type;
                payRecevie.Date = PersianDateUtils.ToDateTime(payRecevie.DisplayDate);
                payRecevie.DisplayDate = payRecevie.DisplayDate;
                // payRecevie.Date = DateTime.Now;
                //payRecevie.DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now);
                decimal amount = 0;
                foreach (var item in payRecevie.Items)
                {
                    amount += item.Amount;

                    if (item.Cash != null)
                    {
                        item.CashId = item.Cash.ID;

                    }
                    else if (item.Bank != null)
                    {
                        item.BankId = item.Bank.ID;
                    }
                    else if (item.Cheque != null && item.Type == ZhivarEnums.DetailPayReceiveType.Cheque)
                    {
                        Cheque cheque = new Cheque()
                        {
                            Amount = item.Amount,
                            BankBranch = item.Cheque.BankBranch,
                            BankName = item.Cheque.BankName,
                            ChequeNumber = item.Cheque.ChequeNumber,
                            Contact = payRecevie.Contact,
                            ContactId = payRecevie.Contact.ID,
                            Date = PersianDateUtils.ToDateTime(item.Cheque.DisplayDate),
                            OrganId = organId,
                            Status = ZhivarEnums.ChequeStatus.Normal,
                            DisplayDate = item.Cheque.DisplayDate,

                        };

                        if (payRecevie.IsReceive)
                            cheque.Type = ZhivarEnums.ChequeType.Dareaftani;
                        else
                            cheque.Type = ZhivarEnums.ChequeType.Pardakhtani;

                        if (item.Cheque.ReceiptDate != null)
                            cheque.ReceiptDate = item.Cheque.ReceiptDate;


                        item.Cheque = cheque;
                        item.ChequeId = item.Cheque.ID;
                        item.Cheque.ObjectState = OMF.Common.Enums.ObjectState.Added;

                        if (!payRecevie.IsReceive)
                        {
                            var temp = item.ChequeBank.ID;

                            item.ChequeBank = new ChequeBank()
                            {
                                BankId = temp,
                                ChequeId = item.Cheque.ID,
                                OrganId = organId

                            };

                            item.ChequeBank.ObjectState = OMF.Common.Enums.ObjectState.Added;
                            item.ChequeBankId = item.ChequeBank.ID;
                        }

                        item.ObjectState = OMF.Common.Enums.ObjectState.Added;
                    }

                    if (item.Type == ZhivarEnums.DetailPayReceiveType.KharjCheque)
                    {
                        ChequeRule chequeRule = new ChequeRule();
                        var cheque = await chequeRule.FindAsync(item.Cheque.ID);

                        cheque.Status = ZhivarEnums.ChequeStatus.Sold;
                        chequeRule.Update(cheque);
                        await chequeRule.SaveChangesAsync();
                        item.Cheque.ContactId = item.Cheque.Contact.ID;
                    }

                    item.ObjectState = OMF.Common.Enums.ObjectState.Added;
                }

                payRecevie.Amount = amount;

                if (payRecevie.Invoice != null)
                {
                    //if (payRecevie.Invoice.Contact != null)
                    payRecevie.Invoice.ContactId = payRecevie.Invoice.ContactId;

                    //payRecevie.Invoice.FinanYear = finanYerar;
                    //payRecevie.Invoice.FinanYearId = finanYerar.ID;
                    var invoice = payRecevie.Invoice;
                    invoice.Rest -= amount;
                    invoice.Paid += amount;
                    if (invoice.Rest <= 0)
                        invoice.Status = ZhivarEnums.NoeInsertFactor.Received;
                    this.BusinessRule.UnitOfWork.RepositoryAsync<Invoice>().Update(invoice);
                }

                if (payRecevie.Cost != null)
                {
                    //if (payRecevie.Cost.Contact != null)
                    payRecevie.Cost.ContactId = payRecevie.Cost.ContactId;


                    var cost = payRecevie.Cost;
                    cost.Rest -= amount;
                    cost.Paid += amount;
                    if (cost.Rest <= 0)
                        cost.Status = ZhivarEnums.CostStatus.Paid;


                    this.BusinessRule.UnitOfWork.RepositoryAsync<Cost>().Update(cost);
                }

                payRecevie.Document = await createDocument(payRecevie, organId, documentNumber);

                if (payRecevieNumber > 0)
                    payRecevie.Number = payRecevieNumber;
                else
                {
                    var payRecevies = await Rule.GetAllByOrganIdAsync(organId);
                    var lastPayRecevies = payRecevies.OrderByDescending(x => x.ID).FirstOrDefault();

                    if (lastPayRecevies != null)
                        payRecevie.Number = lastPayRecevies.Number + 1;
                    else
                        payRecevie.Number = 1;
                }

                foreach (var transaction in payRecevie.Document.Transactions)
                {
                    transaction.ObjectState = OMF.Common.Enums.ObjectState.Added;
                    //if (transaction.AccDocument != null)
                    //    transaction.AccDocument.ObjectState = OMF.Common.Enums.ObjectState.Unchanged;

                    if (transaction.Account != null)
                        transaction.Account.ObjectState = OMF.Common.Enums.ObjectState.Unchanged;

                   // transaction.Transactions = null;

                }
                payRecevie.Document.ObjectState = OMF.Common.Enums.ObjectState.Added;

                payRecevie.ObjectState = OMF.Common.Enums.ObjectState.Added;
                this.BusinessRule.UnitOfWork.RepositoryAsync<PayRecevie>().InsertOrUpdateGraph(payRecevie);
              
                await this.BusinessRule.UnitOfWork.SaveChangesAsync();

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = payRecevie });
            }
            catch (Exception ex)
            {

                throw;
            }

            



            
        }

        [Route("LoadReceiveAndPay")]
        [HttpPost]
        public async Task<HttpResponseMessage> LoadReceiveAndPay([FromBody] int id)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            FinanYearRule finanYearRule = new FinanYearRule();
            var finanYerar = await finanYearRule.GetCurrentFinanYear(organId);

            var payRecevie = await Rule.FindAsync(id);

            ReceiveAndPayVM receiveAndPayVM = new ReceiveAndPayVM();


          
            //var account = await accountRule.FindAsync((int)payRecevie.AccountId);
            //receiveAndPayVM.Account = account;
            receiveAndPayVM.Amount = payRecevie.Amount;
            receiveAndPayVM.Contact = payRecevie.Contact;
            receiveAndPayVM.Description = payRecevie.Description;
            //receiveAndPayVM.DetailAccount = payRecevie.de
            receiveAndPayVM.DisplayDate = payRecevie.DisplayDate;
            receiveAndPayVM.Id = payRecevie.ID;
            receiveAndPayVM.Invoice = payRecevie.Invoice;
            receiveAndPayVM.Items = payRecevie.Items.ToList();
            receiveAndPayVM.Number = payRecevie.Document.Number;
            receiveAndPayVM.Number2 = payRecevie.Document.Number2;
            //receiveAndPayVM.PayItems = payRecevie.pa
            receiveAndPayVM.Type = payRecevie.Type;
            if (payRecevie.Contact != null)
                payRecevie.ContactId = payRecevie.Contact.ID;
            if (payRecevie.Invoice != null)
                payRecevie.InvoiceId = payRecevie.Invoice.ID;


            decimal amount = 0;
            foreach (var item in payRecevie.Items)
            {
                amount += item.Amount;

                if (item.CashId > 0)
                {
                    CashRule cashRule = new CashRule();
                    item.Cash = await cashRule.FindAsync((int)item.CashId);

                }
                else if (item.Bank != null)
                {
                    item.BankId = item.Bank.ID;
                }
                else if (item.Cheque != null)
                {
                    Cheque cheque = new Cheque()
                    {
                        Amount = item.Amount,
                        BankBranch = item.Cheque.BankBranch,
                        BankName = item.Cheque.BankName,
                        ChequeNumber = item.Cheque.ChequeNumber,
                        Contact = payRecevie.Contact,
                        ContactId = payRecevie.Contact.ID,
                        Date = DateTime.Now,// item.Cheque.Date,
                        OrganId = organId,
                        Status = ZhivarEnums.ChequeStatus.Normal,
                        DisplayDate = item.Cheque.DisplayDate,
                        Type = ZhivarEnums.ChequeType.Pardakhtani,
                        ReceiptDate = DateTime.Now,

                    };
                    item.Cheque = cheque;
                    item.ChequeId = item.Cheque.ID;

                }
            }

            payRecevie.Amount = amount;

            if (payRecevie.Contact != null)
            {
                var account1104 = await CalcAccountByCodeTafziliAsync(organId, "1104" + payRecevie.Contact.Code);
                var account1105 = await CalcAccountByCodeTafziliAsync(organId, "1105" + payRecevie.Contact.Code);
                var account2101 = await CalcAccountByCodeTafziliAsync(organId, "2101" + payRecevie.Contact.Code);


                payRecevie.Contact.Balance = account1104.sumTotal + account1105.sumTotal + account2101.sumTotal - amount;
                payRecevie.Contact.Credits = account1104.sumCredit + account1105.sumCredit + account2101.sumCredit - amount;
                payRecevie.Contact.Liability = account1104.sumDebit + account1105.sumDebit + account2101.sumDebit;
            }


            //if (payRecevie.Invoice != null)
            //{
            //    if (payRecevie.Invoice.Contact != null)
            //        payRecevie.Invoice.ContactId = payRecevie.Invoice.Contact.ID;

            //    //payRecevie.Invoice.FinanYear = finanYerar;
            //    //payRecevie.Invoice.FinanYearId = finanYerar.ID;
            //    var invoice = payRecevie.Invoice;
            //    invoice.Rest -= amount;
            //    invoice.Paid += amount;
            //    invoiceRule.Update(invoice);
            //}


            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = payRecevie });
        }

        [Route("DeleteReceiveAndPay")]
        [HttpPost]
        public async Task<HttpResponseMessage> DeleteReceiveAndPay([FromBody] int id)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            FinanYearRule finanYearRule = new FinanYearRule();
            var finanYerar = await finanYearRule.GetCurrentFinanYear(organId);

            var res = await Rule.FindAsync(id);

            PayRecevieDeleteValidate validator = new PayRecevieDeleteValidate();
            FluentValidation.Results.ValidationResult results = validator.Validate(res);

            string failurs = "";

            if (!results.IsValid)
            {
                foreach (var error in results.Errors)
                {
                    failurs += "<br/>" + error.ErrorMessage;

                }
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failurs });
            }

        

            await Rule.DeleteAsync(id);
            await Rule.SaveChangesAsync();

            //var res = await payRecevieRule.GetAllByOrganIdAsync(organId);
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = res });
        }
        
    
        private async Task<Document> createDocument(PayRecevie payRecevie, int organId, int numberDocument)
        {
            FinanYearRule finanYearRule = new FinanYearRule();
            var finanYears = await finanYearRule.GetAllByOrganIdAsync(organId);
            var finanYear = finanYears.Where(x => x.Closed == false).SingleOrDefault();

            Document document = new Document();
            document.Credit = payRecevie.Amount;
            document.DateTime = PersianDateUtils.ToDateTime(payRecevie.DisplayDate);
            document.Debit = payRecevie.Amount;
            document.Description = payRecevie.Description;
            document.DisplayDate = payRecevie.DisplayDate;
            document.IsManual = false;
            document.Status = ZhivarEnums.DocumentStatus.TaeedShode;
            document.ObjectState = OMF.Common.Enums.ObjectState.Added;
            if(numberDocument != 0)
            {
                document.Number = numberDocument;
                document.Number2 = numberDocument;
            }
            else
            {
                DocumentRule documentRule = new DocumentRule();
                document.Number = await documentRule.createNumberDocumentAsync(organId);
                document.Number2 = await documentRule.createNumberDocumentAsync(organId);
            }

            document.Status = ZhivarEnums.DocumentStatus.TaeedShode;
            document.StatusString = "تایید شده";
            document.OrganId = organId;
            document.FinanYear = finanYear;
            document.FinanYearId = finanYear.ID;
            List<Transaction> transactions = new List<Transaction>();

            if(payRecevie.IsReceive)
            {
                document.Type = ZhivarEnums.NoeDoc.Recive;
                transactions = await RegisterTransactionRecevie(document, payRecevie, organId);
            }   
            else
            {
                document.Type = ZhivarEnums.NoeDoc.Pay;
                transactions = await RegisterTransactionPay(document, payRecevie, organId);
            }
                

            document.Transactions = transactions;

            return document;
        }


        private async Task<List<Transaction>> RegisterTransactionPay(Document document, PayRecevie payRecevie, int organId)
        {
            List<Transaction> transactions = new List<Transaction>();

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            foreach (var item in payRecevie.Items)
            {
                switch (payRecevie.Type)
                {
                    case ZhivarEnums.PayRecevieType.AzShakhs:
                        {
                            var accountDareaftani = accounts.Where(x => x.ComplteCoding == "2101" + payRecevie.Contact.Code).SingleOrDefault();

                            transactions.Add(new Transaction()
                            {
                                //AccDocument = document,
                                Account = accountDareaftani,
                                AccountId = accountDareaftani.ID,
                                Amount = item.Amount,
                                //Contact = payRecevieVM.ContactVM,
                                ContactId = payRecevie.Contact.ID,
                                Credit = 0,
                                Debit = item.Amount,
                                IsCredit = false,
                                IsDebit = true,
                                DocumentId = document.ID,
                                Description = document.Description,
                                Date = PersianDateUtils.ToDateTime(payRecevie.DisplayDate),
                                DisplayDate = payRecevie.DisplayDate,
        
                            });
                            break;
                        }
                    case ZhivarEnums.PayRecevieType.Hazine:
                    case ZhivarEnums.PayRecevieType.Sir:
                        {
                            var accountDaramd = accounts.Where(x => x.ComplteCoding == payRecevie.Account.ComplteCoding).SingleOrDefault();

                            transactions.Add(new Transaction()
                            {
                                //AccDocument = document,
                                Account = accountDaramd,
                                AccountId = accountDaramd.ID,
                                Amount = item.Amount,
                                //Contact = payRecevieVM.ContactVM,
                                //ContactId = payRecevieVM.ContactVM.ID,
                                Credit = 0,
                                Debit = item.Amount,
                                IsCredit = false,
                                IsDebit = true,
                                DocumentId = document.ID,
                                Description = document.Description,
                                Date = PersianDateUtils.ToDateTime(payRecevie.DisplayDate),
                                DisplayDate = payRecevie.DisplayDate

                            });
                            break;
                        }
                }

                if (item.Bank != null)
                {
                    var bankAccount = accounts.Where(x => x.ComplteCoding == "1103" + item.Bank.Code).SingleOrDefault();

                    transactions.Add(new Transaction()
                    {
                        //AccDocument = document,
                        Account = bankAccount,
                        AccountId = bankAccount.ID,
                        Amount = item.Amount,
                        Credit = item.Amount,
                        Debit = 0,
                   
                        DocumentId = document.ID,
                        IsCredit = true,
                        IsDebit = false,
                        Date = PersianDateUtils.ToDateTime(payRecevie.DisplayDate),
                        DisplayDate = payRecevie.DisplayDate,
                        Description = document.Description,

                    });
                }

                if (item.Cash != null)
                {
                    var cashAccount = accounts.Where(x => x.ComplteCoding == "1101" + item.Cash.Code).SingleOrDefault();

                    transactions.Add(new Transaction()
                    {
                        //AccDocument = document,
                        Account = cashAccount,
                        AccountId = cashAccount.ID,
                        Amount = item.Amount,
                        Credit = item.Amount,
                        Debit = 0,
                        Description = document.Description,
                        DocumentId = document.ID,
                        IsCredit = true,
                        IsDebit = false,
                        Date = PersianDateUtils.ToDateTime(payRecevie.DisplayDate),
                        DisplayDate = payRecevie.DisplayDate

                    });


                }

                if (item.Cheque != null)
                {
                    var asnadDreaftaniAccount = new DomainClasses.Accounting.Account();

                    switch (payRecevie.Type)
                    {
                        
                        case ZhivarEnums.PayRecevieType.AzShakhs:
                            {
                                asnadDreaftaniAccount = accounts.Where(x => x.ComplteCoding == "1105" + payRecevie.Contact.Code).SingleOrDefault();

                            }
                            break;
                        case ZhivarEnums.PayRecevieType.Daramd:
                        case ZhivarEnums.PayRecevieType.Hazine:
                        case ZhivarEnums.PayRecevieType.Sir:
                            {
                                asnadDreaftaniAccount = accounts.Where(x => x.ComplteCoding == "1105" + item.Cheque.Contact.Code).SingleOrDefault();
                            }
                            break;
                        default:
                            break;
                    }

                    transactions.Add(new Transaction()
                    {
                        //AccDocument = document,
                        Account = asnadDreaftaniAccount,
                        AccountId = asnadDreaftaniAccount.ID,
                        Amount = item.Amount,
                        Credit = item.Amount,
                        Debit = 0,
                        Description = document.Description,
                        DocumentId = document.ID,
                        IsCredit = true,
                        IsDebit = false,
                        Date = PersianDateUtils.ToDateTime(payRecevie.DisplayDate),
                        DisplayDate = payRecevie.DisplayDate

                    });
                }




            }

            return transactions;
        }

        private async Task<List<Transaction>> RegisterTransactionRecevie(Document document, PayRecevie payRecevie, int organId)
        {
            List<Transaction> transactions = new List<Transaction>();

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            foreach (var item in payRecevie.Items)
            {
                var transaction = new Transaction();
                switch (payRecevie.Type)
                {
                    case ZhivarEnums.PayRecevieType.AzShakhs:
                        {
                            var accountDareaftani = accounts.Where(x => x.ComplteCoding == "1104" + payRecevie.Contact.Code).SingleOrDefault();

                            transaction = new Transaction();

                            //transaction.AccDocument = document;
                            //transaction.Account = accountDareaftani;
                            transaction.AccountId = accountDareaftani.ID;
                            transaction.Amount = item.Amount;
                            //Contact = payRecevieVM.ContactVM,
                            transaction.ContactId = payRecevie.Contact.ID;
                            transaction.Credit = item.Amount;
                            transaction.Debit = 0;
                            transaction.IsCredit = true;
                            transaction.IsDebit = false;
                            transaction.DocumentId = document.ID;
                            transaction.Date = PersianDateUtils.ToDateTime(payRecevie.DisplayDate);
                            transaction.DisplayDate = payRecevie.DisplayDate;
                            transaction.Description = document.Description;
                            if (payRecevie.Invoice != null)
                            {
                                transaction.InvoiceId = payRecevie.Invoice.ID;
                            }



                            transactions.Add(transaction);
                            break;
                        }
                    case ZhivarEnums.PayRecevieType.Daramd:
                    case ZhivarEnums.PayRecevieType.Sir:
                        {
                            var accountDaramd = accounts.Where(x => x.ComplteCoding == payRecevie.Account.ComplteCoding).SingleOrDefault();

                            transactions.Add(new Transaction()
                            {
                                //AccDocument = document,
                                //Account = accountDaramd,
                                AccountId = accountDaramd.ID,
                                Amount = item.Amount,
                                Credit = item.Amount,
                                Debit = 0,
                                IsCredit = true,
                                IsDebit = false,
                                DocumentId = document.ID,
                                Date = PersianDateUtils.ToDateTime(payRecevie.DisplayDate),
                                DisplayDate = payRecevie.DisplayDate,
                                Description = document.Description,

                            });
                            break;
                        }
                }

                if (item.Bank != null)
                {
                    var bankAccount = accounts.Where(x => x.ComplteCoding == "1103" + item.Bank.Code).SingleOrDefault();

                    transaction = new Transaction();


                    //transaction.AccDocument = document;
                    //transaction.Account = bankAccount;
                    transaction.AccountId = bankAccount.ID;
                    transaction.Amount = item.Amount;
                    transaction.Credit = 0;
                    transaction.Debit = item.Amount;
                    transaction.Description = document.Description;
                    transaction.DocumentId = document.ID;
                    transaction.IsCredit = false;
                    transaction.IsDebit = true;
                    transaction.Date = PersianDateUtils.ToDateTime(payRecevie.DisplayDate);
                    transaction.DisplayDate = payRecevie.DisplayDate;
            

                    if (payRecevie.Invoice != null)
                        transaction.InvoiceId = payRecevie.Invoice.ID;

                    transactions.Add(transaction);
                }
                if (item.Cash != null)
                {
                    var cashAccount = accounts.Where(x => x.ComplteCoding == "1101" + item.Cash.Code).SingleOrDefault();

                    transaction = new Transaction();

                    //transaction.AccDocument = document;
                    //transaction.Account = cashAccount;
                    transaction.AccountId = cashAccount.ID;
                    transaction.Amount = item.Amount;
                    transaction.Credit = 0;
                    transaction.Debit = item.Amount;
                    transaction.Description = document.Description;
                    transaction.DocumentId = document.ID;
                    transaction.IsCredit = false;
                    transaction.IsDebit = true;
                    transaction.Date = PersianDateUtils.ToDateTime(payRecevie.DisplayDate);
                    transaction.DisplayDate = payRecevie.DisplayDate;
                    if (payRecevie.Invoice != null)
                        transaction.InvoiceId = payRecevie.Invoice.ID;

                    transactions.Add(transaction);


                }

                if (item.Cheque != null)
                {
                    var asnadDreaftaniAccount = accounts.Where(x => x.ComplteCoding == "1105" + payRecevie.Contact.Code).SingleOrDefault();

                    transaction = new Transaction();


                   // transaction.AccDocument = document;
                    //transaction.Account = asnadDreaftaniAccount;
                    transaction.AccountId = asnadDreaftaniAccount.ID;
                    transaction.Amount = item.Amount;
                    transaction.Credit = 0;
                    transaction.Debit = item.Amount;
                    transaction.Description = document.Description;
                    transaction.DocumentId = document.ID;
                    transaction.IsCredit = false;
                    transaction.IsDebit = true;
                    transaction.Date = PersianDateUtils.ToDateTime(payRecevie.DisplayDate);
                    transaction.DisplayDate = payRecevie.DisplayDate;


                    if (payRecevie.Invoice != null)
                        transaction.InvoiceId = payRecevie.Invoice.ID;


                    transactions.Add(transaction);


                }
            }
                return transactions;
            
        }

        private async Task<AccountTafzili> CalcAccountByCodeTafziliAsync(int organId, string code)
        {
            var organId2 = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            decimal amount = 0;
            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId2);

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
    }

  
}

