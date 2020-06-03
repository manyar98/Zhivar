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
using Zhivar.ViewModel.BaseInfo;
using Zhivar.ViewModel.Accounting;
using OMF.Common.Security;
using Zhivar.Business.Accounting;
using Zhivar.Business.BaseInfo;
using OMF.Enterprise.MVC;
using OMF.Business;
using Zhivar.Business.Common;

namespace Zhivar.Web.Controllers.Accounting
{
    [RoutePrefix("api/Cheque")]
    public partial class ChequeController : NewApiControllerBaseAsync<Cheque, ChequeVM>
    {
        public ChequeRule Rule => this.BusinessRule as ChequeRule;

        protected override IBusinessRuleBaseAsync<Cheque> CreateBusinessRule()
        {
            return new ChequeRule();
        }

        [HttpPost]
        [Route("GetChequesAndStats")]
        public virtual async Task<HttpResponseMessage> GetChequesAndStats([FromBody] ZhivarEnums.ChequeType type)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                ChequeRule chequeRule = new ChequeRule();
                var chequeQuery = await chequeRule.GetAllByOrganIdAsync(organId);
                chequeQuery = chequeQuery.Where(x => x.Type == type).OrderByDescending(x => x.ID).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = chequeQuery });
            }
            catch (Exception ex)
            {

                throw;
            }
          
        }


        [HttpPost]
        [Route("ChangeChequeStatus")]
        public virtual async Task<HttpResponseMessage> ChangeChequeStatus([FromBody] ChequesAndStatsVM chequesAndStatsVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            ChequeVM chequeVM = new ChequeVM();

            switch (chequesAndStatsVM.change)
            {
                // وصول چک پرداختی
                case "PaidChequeReceipt":
                    {
                        chequesAndStatsVM.description = " وصول چک پرداختی";
                        var doucument = await CreateDocumentPaidChequeReceipt(chequesAndStatsVM, organId);

                        ChequeRule chequeRule = new ChequeRule();
                        var cheque = await chequeRule.FindAsync(chequesAndStatsVM.chequeId);
                        cheque.Status = ZhivarEnums.ChequeStatus.Passed;
                        chequeRule.Update(cheque);
                        await chequeRule.SaveChangesAsync();

                        DocumentRule documentRule = new DocumentRule();
                        await documentRule.InsertAsync(doucument, organId);
                        await documentRule.SaveChangesAsync();
                       


                        //AccountNumber: ""
                        chequeVM.Amount = cheque.Amount;
                        chequeVM.BankBranch = cheque.BankBranch;
                        //chequeVM.BankDetailAccount: null
                        chequeVM.BankName = cheque.BankName;
                        chequeVM.ChequeNumber = cheque.ChequeNumber;
                        chequeVM.Contact = cheque.Contact;
                        chequeVM.Date = cheque.Date;
                        chequeVM.DepositBank = cheque.DepositBank;
                        //chequeVM.DepositDate = cheque.DepositBank;
                        chequeVM.DisplayDate = cheque.DisplayDate;
                        chequeVM.ID = cheque.ID;
                        //chequeVM.MyCheque = cheque.DepositBank;
                        //chequeVM.Overdue = cheque.Overdue;
                        //chequeVM.Payee = cheque.Payee;
                        //chequeVM.Payer = cheque.Payer;
                        chequeVM.ReceiptDate = cheque.ReceiptDate;
                        //chequeVM.ReturnDate = cheque.ReturnDate;
                        chequeVM.Status = cheque.Status;
                        chequeVM.StatusString = "وصول شده";
                        break;
                    }
                // وصول چک دریافتی
                case "ReceivedChequeReceipt":
                    {
                        chequesAndStatsVM.description = " وصول چک دریافتی";

                        var doucument = await CreateDocumentChangeChequeStatus(chequesAndStatsVM, organId);

                        
                        ChequeRule chequeRule = new ChequeRule();
                        var cheque = await chequeRule.FindAsync(chequesAndStatsVM.chequeId);
                        cheque.Status = ZhivarEnums.ChequeStatus.Passed;
                        chequeRule.Update(cheque);

                        await chequeRule.SaveChangesAsync();

                        DocumentRule documentRule = new DocumentRule();
                        await documentRule.InsertAsync(doucument, organId);

                        await documentRule.SaveChangesAsync();


                        //AccountNumber: ""
                        chequeVM.Amount = cheque.Amount;
                        chequeVM.BankBranch = cheque.BankBranch;
                        //chequeVM.BankDetailAccount: null
                        chequeVM.BankName = cheque.BankName;
                        chequeVM.ChequeNumber = cheque.ChequeNumber;
                        chequeVM.Contact = cheque.Contact;
                        chequeVM.Date = cheque.Date;
                        chequeVM.DepositBank = cheque.DepositBank;
                        //chequeVM.DepositDate = cheque.DepositBank;
                        chequeVM.DisplayDate = cheque.DisplayDate;
                        chequeVM.ID = cheque.ID;
                        //chequeVM.MyCheque = cheque.DepositBank;
                        //chequeVM.Overdue = cheque.Overdue;
                        //chequeVM.Payee = cheque.Payee;
                        //chequeVM.Payer = cheque.Payer;
                        chequeVM.ReceiptDate = cheque.ReceiptDate;
                        //chequeVM.ReturnDate = cheque.ReturnDate;
                        chequeVM.Status = cheque.Status;
                        chequeVM.StatusString = "وصول شده";
                        break;
                    }
                // وصول چک واگذار شده به بانک
                case "DepositChequeReceipt":
                    {
                        chequesAndStatsVM.description = "وصول چک واگذار شده به بانک";
                        var doucument = await CreateDocumentDepositChequeReceipt(chequesAndStatsVM, organId);

                        ChequeRule chequeRule = new ChequeRule();
                        var cheque = await chequeRule.FindAsync(chequesAndStatsVM.chequeId);
                        cheque.Status = ZhivarEnums.ChequeStatus.Passed;
                        chequeRule.Update(cheque);

                        await chequeRule.SaveChangesAsync();

                        DocumentRule documentRule = new DocumentRule();
                        await documentRule.InsertAsync(doucument, organId);

                        await documentRule.SaveChangesAsync();


                        //AccountNumber: ""
                        chequeVM.Amount = cheque.Amount;
                        chequeVM.BankBranch = cheque.BankBranch;
                        //chequeVM.BankDetailAccount: null
                        chequeVM.BankName = cheque.BankName;
                        chequeVM.ChequeNumber = cheque.ChequeNumber;
                        chequeVM.Contact = cheque.Contact;
                        chequeVM.Date = cheque.Date;
                        chequeVM.DepositBank = cheque.DepositBank;
                        //chequeVM.DepositDate = cheque.DepositBank;
                        chequeVM.DisplayDate = cheque.DisplayDate;
                        chequeVM.ID = cheque.ID;
                        //chequeVM.MyCheque = cheque.DepositBank;
                        //chequeVM.Overdue = cheque.Overdue;
                        //chequeVM.Payee = cheque.Payee;
                        //chequeVM.Payer = cheque.Payer;
                        chequeVM.ReceiptDate = cheque.ReceiptDate;
                        //chequeVM.ReturnDate = cheque.ReturnDate;
                        chequeVM.Status = cheque.Status;
                        chequeVM.StatusString = "وصول شده";
                        break;
                    }
                // واگذاری چک به بانک
                case "ReceivedChequeDeposit":
                    {
                        chequesAndStatsVM.description = "واگذاری چک به بانک";

                        var doucument = await CreateDocumentReceivedChequeDeposit(chequesAndStatsVM, organId);
                        ChequeRule chequeRule = new ChequeRule();
                        var cheque = await chequeRule.FindAsync(chequesAndStatsVM.chequeId);
                        cheque.Status = ZhivarEnums.ChequeStatus.InProgress;

                        BankRule bankRule = new BankRule();
                        var banks = await bankRule.GetAllByOrganIdAsync(organId);
                        var bank = banks.Where(x => x.Code == chequesAndStatsVM.detailAccount.Code).SingleOrDefault();
                        cheque.DepositBankId = bank.ID;

                        chequeRule.Update(cheque);
                        await chequeRule.SaveChangesAsync();

                        DocumentRule documentRule = new DocumentRule();
                        await documentRule.InsertAsync(doucument, organId);
                        await documentRule.SaveChangesAsync();
                    


                        //AccountNumber: ""
                        chequeVM.Amount = cheque.Amount;
                        chequeVM.BankBranch = cheque.BankBranch;
                        //chequeVM.BankDetailAccount: null
                        chequeVM.BankName = cheque.BankName;
                        chequeVM.ChequeNumber = cheque.ChequeNumber;
                        chequeVM.Contact = cheque.Contact;
                        chequeVM.Date = cheque.Date;
                        chequeVM.DepositBank = cheque.DepositBank;
                        //chequeVM.DepositDate = cheque.DepositBank;
                        chequeVM.DisplayDate = cheque.DisplayDate;
                        chequeVM.ID = cheque.ID;
                        //chequeVM.MyCheque = cheque.DepositBank;
                        //chequeVM.Overdue = cheque.Overdue;
                        //chequeVM.Payee = cheque.Payee;
                        //chequeVM.Payer = cheque.Payer;
                        chequeVM.ReceiptDate = cheque.ReceiptDate;
                        //chequeVM.ReturnDate = cheque.ReturnDate;
                        chequeVM.Status = cheque.Status;
                        chequeVM.StatusString = "وصول شده";
                        break;
                    }
                // عودت دادن چک پرداختی
                case "PaidChequeReturn":
                    {
                        //chequesAndStatsVM.description = "عودت دادن چک پرداختی";

                        var doucument = await CreateDocumentPaidChequeReturn(chequesAndStatsVM, organId);
                        ChequeRule chequeRule = new ChequeRule();
                        var cheque = await chequeRule.FindAsync(chequesAndStatsVM.chequeId);
                        cheque.Status = ZhivarEnums.ChequeStatus.ChequeReturn;
                        chequeRule.Update(cheque);

                        await chequeRule.SaveChangesAsync();

                        DocumentRule documentRule = new DocumentRule();
                        await documentRule.InsertAsync(doucument, organId);

                        await documentRule.SaveChangesAsync();


                        //AccountNumber: ""
                        chequeVM.Amount = cheque.Amount;
                        chequeVM.BankBranch = cheque.BankBranch;
                        //chequeVM.BankDetailAccount: null
                        chequeVM.BankName = cheque.BankName;
                        chequeVM.ChequeNumber = cheque.ChequeNumber;
                        chequeVM.Contact = cheque.Contact;
                        chequeVM.Date = cheque.Date;
                        chequeVM.DepositBank = cheque.DepositBank;
                        //chequeVM.DepositDate = cheque.DepositBank;
                        chequeVM.DisplayDate = cheque.DisplayDate;
                        chequeVM.ID = cheque.ID;
                        //chequeVM.MyCheque = cheque.DepositBank;
                        //chequeVM.Overdue = cheque.Overdue;
                        //chequeVM.Payee = cheque.Payee;
                        //chequeVM.Payer = cheque.Payer;
                        chequeVM.ReceiptDate = cheque.ReceiptDate;
                        //chequeVM.ReturnDate = cheque.ReturnDate;
                        chequeVM.Status = cheque.Status;
                        chequeVM.StatusString = "وصول شده";
                        break;
                    }
                // عودت دادن چک دریافتی
                case "ReceivedChequeReturn":
                    {
                        chequesAndStatsVM.description = "عودت دادن چک دریافتی";

                        var doucument = await CreateDocumentReceivedChequeReturn(chequesAndStatsVM, organId);
                        ChequeRule chequeRule = new ChequeRule();
                        var cheque = await chequeRule.FindAsync(chequesAndStatsVM.chequeId);
                        cheque.Status = ZhivarEnums.ChequeStatus.ChequeReturn;
                        chequeRule.Update(cheque);

                        await chequeRule.SaveChangesAsync();

                        DocumentRule documentRule = new DocumentRule();
                        await documentRule.InsertAsync(doucument, organId);

                        await documentRule.SaveChangesAsync();


                        //AccountNumber: ""
                        chequeVM.Amount = cheque.Amount;
                        chequeVM.BankBranch = cheque.BankBranch;
                        //chequeVM.BankDetailAccount: null
                        chequeVM.BankName = cheque.BankName;
                        chequeVM.ChequeNumber = cheque.ChequeNumber;
                        chequeVM.Contact = cheque.Contact;
                        chequeVM.Date = cheque.Date;
                        chequeVM.DepositBank = cheque.DepositBank;
                        //chequeVM.DepositDate = cheque.DepositBank;
                        chequeVM.DisplayDate = cheque.DisplayDate;
                        chequeVM.ID = cheque.ID;
                        //chequeVM.MyCheque = cheque.DepositBank;
                        //chequeVM.Overdue = cheque.Overdue;
                        //chequeVM.Payee = cheque.Payee;
                        //chequeVM.Payer = cheque.Payer;
                        chequeVM.ReceiptDate = cheque.ReceiptDate;
                        //chequeVM.ReturnDate = cheque.ReturnDate;
                        chequeVM.Status = cheque.Status;
                        chequeVM.StatusString = "وصول شده";
                        break;
                    }
                // تغییر وضعیت چک پرداختی به پاس نشده
                case "PaidChequeToNotPass":
                    {
                        chequesAndStatsVM.description = "تغییر وضعیت چک پرداختی به پاس نشده";
                        chequesAndStatsVM.date = PersianDateUtils.ToPersianDate(DateTime.Now);

                        var doucument = await CreateDocumentPaidChequeToNotPass(chequesAndStatsVM, organId);

                            ChequeRule chequeRule = new ChequeRule();
                            var cheque = await chequeRule.FindAsync(chequesAndStatsVM.chequeId);
                            cheque.Status = ZhivarEnums.ChequeStatus.Normal;
                            chequeRule.Update(cheque);
                            await chequeRule.SaveChangesAsync();

                            DocumentRule documentRule = new DocumentRule();
                            await documentRule.InsertAsync(doucument, organId);

                            await documentRule.SaveChangesAsync();


                            //AccountNumber: ""
                            chequeVM.Amount = cheque.Amount;
                            chequeVM.BankBranch = cheque.BankBranch;
                            //chequeVM.BankDetailAccount: null
                            chequeVM.BankName = cheque.BankName;
                            chequeVM.ChequeNumber = cheque.ChequeNumber;
                            chequeVM.Contact = cheque.Contact;
                            chequeVM.Date = cheque.Date;
                            chequeVM.DepositBank = cheque.DepositBank;
                            //chequeVM.DepositDate = cheque.DepositBank;
                            chequeVM.DisplayDate = cheque.DisplayDate;
                            chequeVM.ID = cheque.ID;
                            //chequeVM.MyCheque = cheque.DepositBank;
                            //chequeVM.Overdue = cheque.Overdue;
                            //chequeVM.Payee = cheque.Payee;
                            //chequeVM.Payer = cheque.Payer;
                            chequeVM.ReceiptDate = cheque.ReceiptDate;
                            //chequeVM.ReturnDate = cheque.ReturnDate;
                            chequeVM.Status = cheque.Status;
                            chequeVM.StatusString = "وصول شده";
                            break;
                        
                    }
                // تغییر وضعیت چک دریافتی به وصول نشده
                case "ReceivedChequeToNotPass":
                    {
                        chequesAndStatsVM.description = "تغییر وضعیت چک دریافتی به وصول نشده";

                        ChequeRule chequeRule = new ChequeRule();
                        var cheque = await chequeRule.FindAsync(chequesAndStatsVM.chequeId);

                        PayRecevieRule payRecevieRule = new PayRecevieRule();
                        List<Document> doucuments = await payRecevieRule.GetDoucumentIDByChequeIdAsync(chequesAndStatsVM.chequeId);
                        cheque.Status = ZhivarEnums.ChequeStatus.Normal;
                        chequeRule.Update(cheque);

                        Document newDocument = new Document();
                        DocumentRule documentRule = new DocumentRule();

                        foreach (var document in doucuments)
                        {
                            newDocument = new Document() {
                                Credit = document.Credit,
                                Description = chequesAndStatsVM.description,
                                Debit = document.Debit,
                                DateTime = DateTime.Now,
                                DisplayDate = PersianDateUtils.ToPersianDate(DateTime.Now),
                                FinanYear = document.FinanYear,
                                FinanYearId = document.FinanYearId,
                                IsFirsDocument = document.IsFirsDocument,
                                IsManual = document.IsManual,
                                Number = await documentRule.createNumberDocumentAsync(organId),
                                Number2 = await documentRule.createNumberDocumentAsync(organId),
                                OrganId = organId,
                                Status = ZhivarEnums.DocumentStatus.TaeedShode,
                                StatusString = document.StatusString,
                                Type = ZhivarEnums.NoeDoc.Pay
                        };
                            TransactionRule transactionRule = new TransactionRule();
                            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);
                            transactions = transactions.Where(x => x.DocumentId == document.ID).ToList();

                            newDocument.Transactions = new List<Transaction>();
                            Transaction newTransaction = new Transaction();

                            foreach (var transaction in transactions)
                            {
                                //newTransaction.AccDocument = newDocument;
                                //newTransaction.Account = transaction.Account;
                                newTransaction.AccountId = transaction.AccountId;
                                newTransaction.Amount = transaction.Amount;
                                newTransaction.Cheque = transaction.Cheque;
                                newTransaction.ChequeId = transaction.ChequeId;
                                newTransaction.ContactId = transaction.ContactId;
                                newTransaction.Credit = transaction.Debit;
                                newTransaction.Date = DateTime.Now;
                                newTransaction.Debit = transaction.Credit;
                                newTransaction.Description = chequesAndStatsVM.description;
                                newTransaction.DisplayDate = PersianDateUtils.ToPersianDate(DateTime.Now);
                                newTransaction.DocumentId = newDocument.ID;
                                newTransaction.InvoiceId = transaction.InvoiceId;
                                newTransaction.IsCredit = transaction.IsDebit;
                                newTransaction.IsDebit = transaction.IsCredit;
                                newTransaction.PaymentMethod = transaction.PaymentMethod;
                                newTransaction.PaymentMethodString = transaction.PaymentMethodString;
                                newTransaction.Reference = transaction.Reference;
                                newTransaction.RefTrans = transaction.RefTrans;
                                newTransaction.Remaining = transaction.Remaining;
                                newTransaction.RemainingType = transaction.RemainingType;
                                newTransaction.RowNumber = transaction.RowNumber;
                                newTransaction.Stock = transaction.Stock;
                                newTransaction.TransactionTypeString = transaction.TransactionTypeString;
                                newTransaction.Type = transaction.Type;
                                newTransaction.UnitPrice = transaction.UnitPrice;

                                newDocument.Transactions.Add(newTransaction);
                            }
                        }

                        await documentRule.InsertAsync(newDocument, organId);
                        await documentRule.SaveChangesAsync();


                        //AccountNumber: ""
                        chequeVM.Amount = cheque.Amount;
                        chequeVM.BankBranch = cheque.BankBranch;
                        //chequeVM.BankDetailAccount: null
                        chequeVM.BankName = cheque.BankName;
                        chequeVM.ChequeNumber = cheque.ChequeNumber;
                        chequeVM.Contact = cheque.Contact;
                        chequeVM.Date = cheque.Date;
                        chequeVM.DepositBank = cheque.DepositBank;
                        //chequeVM.DepositDate = cheque.DepositBank;
                        chequeVM.DisplayDate = cheque.DisplayDate;
                        chequeVM.ID = cheque.ID;
                        //chequeVM.MyCheque = cheque.DepositBank;
                        //chequeVM.Overdue = cheque.Overdue;
                        //chequeVM.Payee = cheque.Payee;
                        //chequeVM.Payer = cheque.Payer;
                        chequeVM.ReceiptDate = cheque.ReceiptDate;
                        //chequeVM.ReturnDate = cheque.ReturnDate;
                        chequeVM.Status = cheque.Status;
                        chequeVM.StatusString = "وصول شده";
                        break;
                    }
     
      
                default:
                    break;
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = chequeVM });
        }

        [HttpPost]
        [Route("GetChequesToPay")]
        public virtual async Task<HttpResponseMessage> GetChequesToPay()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            ChequeRule chequeRule = new ChequeRule();
            var res = await chequeRule.GetChequesToPay(organId);

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = res });
        }

        private async Task<Document> CreateDocumentPaidChequeToNotPass(ChequesAndStatsVM chequesAndStatsVM, int organId)
        {
            FinanYearRule finanYearRule = new FinanYearRule();
            var finanYears = await finanYearRule.GetAllByOrganIdAsync(organId);
            var finanYear = finanYears.Where(x => x.Closed == false).SingleOrDefault();

            ChequeRule chequeRule = new ChequeRule();
            var cheque = await chequeRule.FindAsync(chequesAndStatsVM.chequeId);
            var chequeBanks = this.BusinessRule.UnitOfWork.RepositoryAsync<ChequeBank>().Queryable().Where(x => x.OrganId == organId);

            var chequeBank = chequeBanks.Where(x => x.ChequeId == cheque.ID).SingleOrDefault();
            BankRule bankRule = new BankRule();
            var bank = await bankRule.FindAsync(chequeBank.BankId);

            DocumentRule documentRule = new DocumentRule();
            Document document = new Document();
            document.Credit = cheque.Amount;
            document.DateTime =  PersianDateUtils.ToDateTime(chequesAndStatsVM.date);
            document.Debit = cheque.Amount;
            document.Description = chequesAndStatsVM.description;
            document.DisplayDate = chequesAndStatsVM.date;
            document.IsManual = false;
            document.Status = ZhivarEnums.DocumentStatus.TaeedShode;
            document.Number = await documentRule.createNumberDocumentAsync(organId);
            document.Number2 = await documentRule.createNumberDocumentAsync(organId);
            document.StatusString = "تایید شده";
            document.OrganId = organId;
            document.FinanYear = finanYear;
            document.FinanYearId = finanYear.ID;
            document.Type = ZhivarEnums.NoeDoc.Recive;

            List<Transaction> transactions = new List<Transaction>();

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            var accountPay = accounts.Where(x => x.ComplteCoding == "2101" + cheque.Contact.Code).SingleOrDefault();

            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                AccountId = accountPay.ID,
                Amount = cheque.Amount,
                Description = chequesAndStatsVM.description,
                Credit = 0,
                Debit = cheque.Amount,
                IsCredit = false,
                IsDebit = true,
                DocumentId = document.ID,
                Date = PersianDateUtils.ToDateTime(chequesAndStatsVM.date),
                DisplayDate = chequesAndStatsVM.date
            });

            var accountRecive = accounts.Where(x => x.ComplteCoding == "2102" + bank.Code).SingleOrDefault();

            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                AccountId = accountRecive.ID,
                Amount = cheque.Amount,
                Credit = cheque.Amount,
                Debit = 0,
                Description = chequesAndStatsVM.description,
                DocumentId = document.ID,
                IsCredit = true,
                IsDebit = false,
                Date = PersianDateUtils.ToDateTime(chequesAndStatsVM.date),
                DisplayDate = chequesAndStatsVM.date

            });


            document.Transactions = transactions;

            return document;
        }

        private async Task<Document> CreateDocumentPaidChequeReturn(ChequesAndStatsVM chequesAndStatsVM, int organId)
        {
            FinanYearRule finanYearRule = new FinanYearRule();
            var finanYears = await finanYearRule.GetAllByOrganIdAsync(organId);
            var finanYear = finanYears.Where(x => x.Closed == false).SingleOrDefault();

            ChequeRule chequeRule = new ChequeRule();
            var cheque = await chequeRule.FindAsync(chequesAndStatsVM.chequeId);
            var chequeBanks = this.BusinessRule.UnitOfWork.RepositoryAsync<ChequeBank>().Queryable().Where(x => x.OrganId == organId);

            var chequeBank = chequeBanks.Where(x => x.ChequeId == cheque.ID).SingleOrDefault();
            BankRule bankRule = new BankRule();
            var bank = await bankRule.FindAsync(chequeBank.BankId);

            DocumentRule documentRule = new DocumentRule();

            Document document = new Document();
            document.Credit = cheque.Amount;
            document.DateTime = PersianDateUtils.ToDateTime(chequesAndStatsVM.date);
            document.Debit = cheque.Amount;
            document.Description = chequesAndStatsVM.description;
            document.DisplayDate = chequesAndStatsVM.date;
            document.IsManual = false;
            document.Status = ZhivarEnums.DocumentStatus.TaeedShode;
            document.Number = await documentRule.createNumberDocumentAsync(organId);
            document.Number2 = await documentRule.createNumberDocumentAsync(organId);
            document.StatusString = "تایید شده";
            document.OrganId = organId;
            document.FinanYear = finanYear;
            document.FinanYearId = finanYear.ID;
            document.Type = ZhivarEnums.NoeDoc.Pay;

            List<Transaction> transactions = new List<Transaction>();

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            var accountPay = accounts.Where(x => x.ComplteCoding == "2102" + bank.Code).SingleOrDefault();

            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                AccountId = accountPay.ID,
                Amount = cheque.Amount,
                Description = chequesAndStatsVM.description,
                Credit = 0,
                Debit = cheque.Amount,
                IsCredit = false,
                IsDebit = true,
                DocumentId = document.ID,
                Date = PersianDateUtils.ToDateTime(chequesAndStatsVM.date),
                DisplayDate = chequesAndStatsVM.date
            });

            var accountRecive = accounts.Where(x => x.ComplteCoding == "2101" + cheque.Contact.Code).SingleOrDefault();

            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                AccountId = accountRecive.ID,
                Amount = cheque.Amount,
                Credit = cheque.Amount,
                Debit = 0,
                Description = chequesAndStatsVM.description,
                DocumentId = document.ID,
                IsCredit = true,
                IsDebit = false,
                Date = PersianDateUtils.ToDateTime(chequesAndStatsVM.date),
                DisplayDate = chequesAndStatsVM.date

            });


            document.Transactions = transactions;

            return document;
        }

        private async Task<Document> CreateDocumentDepositChequeReceipt(ChequesAndStatsVM chequesAndStatsVM, int organId)
        {
            FinanYearRule finanYearRule = new FinanYearRule();
            var finanYears = await finanYearRule.GetAllByOrganIdAsync(organId);
            var finanYear = finanYears.Where(x => x.Closed == false).SingleOrDefault();

            ChequeRule chequeRule = new ChequeRule();
            var cheque = await chequeRule.FindAsync(chequesAndStatsVM.chequeId);

            DocumentRule documentRule = new DocumentRule();

            Document document = new Document();
            document.Credit = cheque.Amount;
            document.DateTime = PersianDateUtils.ToDateTime(chequesAndStatsVM.date);
            document.Debit = cheque.Amount;
            document.Description = chequesAndStatsVM.description;
            document.DisplayDate = chequesAndStatsVM.date;
            document.IsManual = false;
            document.Status = ZhivarEnums.DocumentStatus.TaeedShode;
            document.Number = await documentRule.createNumberDocumentAsync(organId);
            document.Number2 = await documentRule.createNumberDocumentAsync(organId);
            document.StatusString = "تایید شده";
            document.OrganId = organId;
            document.FinanYear = finanYear;
            document.FinanYearId = finanYear.ID;
            document.Type = ZhivarEnums.NoeDoc.Pay;

            List<Transaction> transactions = new List<Transaction>();

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var bank = await this.BusinessRule.UnitOfWork.RepositoryAsync<Bank>().FindAsync((int)cheque.DepositBankId);
            
            var accountPay = accounts.Where(x => x.ComplteCoding == "1103" + bank.Code).SingleOrDefault();

            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                AccountId = accountPay.ID,
                Amount = cheque.Amount,
                Description = chequesAndStatsVM.description,
                Credit = 0,
                Debit = cheque.Amount,
                IsCredit = false,
                IsDebit = true,
                DocumentId = document.ID,
                Date = PersianDateUtils.ToDateTime(chequesAndStatsVM.date),
                DisplayDate = chequesAndStatsVM.date
            });

            var accountBank = accounts.Where(x => x.ComplteCoding == "1106" + bank.Code).SingleOrDefault();

            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                AccountId = accountBank.ID,
                Amount = cheque.Amount,
                Credit = cheque.Amount,
                Debit = 0,
                Description = chequesAndStatsVM.description,
                DocumentId = document.ID,
                IsCredit = true,
                IsDebit = false,
                Date = PersianDateUtils.ToDateTime(chequesAndStatsVM.date),
                DisplayDate = chequesAndStatsVM.date

            });


            document.Transactions = transactions;

            return document;
        }

        private async Task<Document> CreateDocumentPaidChequeReceipt(ChequesAndStatsVM chequesAndStatsVM, int organId)
        {
            FinanYearRule finanYearRule = new FinanYearRule();
            var finanYears = await finanYearRule.GetAllByOrganIdAsync(organId);
            var finanYear = finanYears.Where(x => x.Closed == false).SingleOrDefault();
            ChequeRule chequeRule = new ChequeRule();
            var cheque = await chequeRule.FindAsync(chequesAndStatsVM.chequeId);
            //var chequeBanks = await _chequeBankService.GetAllByOrganIdAsync(organId);
            var chequeBanks = this.BusinessRule.UnitOfWork.RepositoryAsync<ChequeBank>().Queryable().Where(x => x.OrganId == organId);
            var chequeBank = chequeBanks.Where(x => x.ChequeId == cheque.ID).SingleOrDefault();

            BankRule bankRule = new BankRule();
            var bank = await bankRule.FindAsync(chequeBank.BankId);

            DocumentRule documentRule = new DocumentRule();

            Document document = new Document();
            document.Credit = cheque.Amount;
            document.DateTime = PersianDateUtils.ToDateTime(chequesAndStatsVM.date);
            document.Debit = cheque.Amount;
            document.Description = chequesAndStatsVM.description;
            document.DisplayDate = chequesAndStatsVM.date;
            document.IsManual = false;
            document.Status = ZhivarEnums.DocumentStatus.TaeedShode;
            document.Number = await documentRule.createNumberDocumentAsync(organId);
            document.Number2 = await documentRule.createNumberDocumentAsync(organId);
            document.StatusString = "تایید شده";
            document.OrganId = organId;
            document.FinanYear = finanYear;
            document.FinanYearId = finanYear.ID;
            document.Type = ZhivarEnums.NoeDoc.Recive;

            List<Transaction> transactions = new List<Transaction>();

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            var accountPay = accounts.Where(x => x.ComplteCoding == "2102" + bank.Code).SingleOrDefault();

                        transactions.Add(new Transaction()
                        {
                            //AccDocument = document,
                            AccountId = accountPay.ID,
                            Amount = cheque.Amount,
                            Description = chequesAndStatsVM.description,
                            Credit = 0,
                            Debit = cheque.Amount,
                            IsCredit = false,
                            IsDebit = true,
                            DocumentId = document.ID,
                            Date = PersianDateUtils.ToDateTime(chequesAndStatsVM.date),
                            DisplayDate = chequesAndStatsVM.date
                        });
                       
            var accountBank = accounts.Where(x => x.ComplteCoding == "1103" + bank.Code).SingleOrDefault();

            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                AccountId = accountBank.ID,
                Amount = cheque.Amount,
                Credit = cheque.Amount,
                Debit = 0,
                Description = chequesAndStatsVM.description,
                DocumentId = document.ID,
                IsCredit = true,
                IsDebit = false,
                Date = PersianDateUtils.ToDateTime(chequesAndStatsVM.date),
                DisplayDate = chequesAndStatsVM.date

            });


            document.Transactions = transactions;

            return document;
        }

        private async Task<Document> CreateDocumentReceivedChequeReturn(ChequesAndStatsVM chequesAndStatsVM, int organId)
        {
            FinanYearRule finanYearRule = new FinanYearRule();

            var finanYears = await finanYearRule.GetAllByOrganIdAsync(organId);
            var finanYear = finanYears.Where(x => x.Closed == false).SingleOrDefault();
            ChequeRule chequeRule = new ChequeRule();
            var cheque = await chequeRule.FindAsync(chequesAndStatsVM.chequeId);

            DocumentRule documentRule = new DocumentRule();

            Document document = new Document();
            document.Credit = cheque.Amount;
            document.DateTime = PersianDateUtils.ToDateTime(chequesAndStatsVM.date);
            document.Debit = cheque.Amount;
            document.Description = chequesAndStatsVM.description;
            document.DisplayDate = chequesAndStatsVM.date;
            document.IsManual = false;
            document.Status = ZhivarEnums.DocumentStatus.TaeedShode;
            document.Number = await documentRule.createNumberDocumentAsync(organId);
            document.Number2 = await documentRule.createNumberDocumentAsync(organId);
            document.StatusString = "تایید شده";
            document.OrganId = organId;
            document.FinanYear = finanYear;
            document.FinanYearId = finanYear.ID;
            document.Type = ZhivarEnums.NoeDoc.Pay;

            List<Transaction> transactions = new List<Transaction>();

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);



            var accountDareaftani = accounts.Where(x => x.ComplteCoding == "1104" + cheque.Contact.Code).SingleOrDefault();

            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                //Account = accountDareaftani,
                AccountId = accountDareaftani.ID,
                Amount = cheque.Amount,
                //Contact = payRecevieVM.ContactVM,
                //ContactId = payRecevie.Contact.ID,
                Description = chequesAndStatsVM.description,
                Credit = 0,
                Debit = cheque.Amount,
                IsCredit = false,
                IsDebit = true,
                DocumentId = document.ID,
                Date = PersianDateUtils.ToDateTime(chequesAndStatsVM.date),
                DisplayDate = chequesAndStatsVM.date
            });
            var asnadDreaftaniAccount = accounts.Where(x => x.ComplteCoding == "1105" + cheque.Contact.Code).SingleOrDefault();

            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                //Account = asnadDreaftaniAccount,
                AccountId = asnadDreaftaniAccount.ID,
                Amount = cheque.Amount,
                Credit = cheque.Amount,
                Debit = 0,
                Description = chequesAndStatsVM.description,
                DocumentId = document.ID,
                IsCredit = true,
                IsDebit = false,
                Date = PersianDateUtils.ToDateTime(chequesAndStatsVM.date),
                DisplayDate = chequesAndStatsVM.date

            });


            document.Transactions = transactions;

            return document;
        }

        private async Task<Document> CreateDocumentReceivedChequeDeposit(ChequesAndStatsVM chequesAndStatsVM, int organId)
        {
            FinanYearRule finanYearRule = new FinanYearRule();
            var finanYears = await finanYearRule.GetAllByOrganIdAsync(organId);
            var finanYear = finanYears.Where(x => x.Closed == false).SingleOrDefault();
            ChequeRule chequeRule = new ChequeRule();
            var cheque = await chequeRule.FindAsync(chequesAndStatsVM.chequeId);

            DocumentRule documentRule = new DocumentRule();

            Document document = new Document();
            document.Credit = cheque.Amount;
            document.DateTime = PersianDateUtils.ToDateTime(chequesAndStatsVM.date);
            document.Debit = cheque.Amount;
            document.Description = chequesAndStatsVM.description;
            document.DisplayDate = chequesAndStatsVM.date;
            document.IsManual = false;
            document.Status = ZhivarEnums.DocumentStatus.TaeedShode;
            document.Number = await documentRule.createNumberDocumentAsync(organId);
            document.Number2 = await documentRule.createNumberDocumentAsync(organId);
            document.StatusString = "تایید شده";
            document.OrganId = organId;
            document.FinanYear = finanYear;
            document.FinanYearId = finanYear.ID;
            document.Type = ZhivarEnums.NoeDoc.Recive;

            List<Transaction> transactions = new List<Transaction>();

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

     
            var accountBank = accounts.Where(x => x.ComplteCoding == "1106" + chequesAndStatsVM.detailAccount.Code).SingleOrDefault();

            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                AccountId = accountBank.ID,
                Amount = cheque.Amount,
                Description = chequesAndStatsVM.description,
                Credit = 0,
                Debit = cheque.Amount,
                IsCredit = false,
                IsDebit = true,
                DocumentId = document.ID,
                Date = PersianDateUtils.ToDateTime(chequesAndStatsVM.date),
                DisplayDate = chequesAndStatsVM.date
            });
            var asnadDreaftaniAccount = accounts.Where(x => x.ComplteCoding == "1105" + cheque.Contact.Code).SingleOrDefault();

            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                //Account = asnadDreaftaniAccount,
                AccountId = asnadDreaftaniAccount.ID,
                Amount = cheque.Amount,
                Credit = cheque.Amount,
                Debit = 0,
                Description = chequesAndStatsVM.description,
                DocumentId = document.ID,
                IsCredit = true,
                IsDebit = false,
                Date = PersianDateUtils.ToDateTime(chequesAndStatsVM.date),
                DisplayDate = chequesAndStatsVM.date

            });


            document.Transactions = transactions;

            return document;
        
    }

        private async Task<Document> CreateDocumentChangeChequeStatus(ChequesAndStatsVM chequesAndStatsVM, int organId)
        {
            FinanYearRule finanYearRule = new FinanYearRule();

            var finanYears = await finanYearRule.GetAllByOrganIdAsync(organId);
            var finanYear = finanYears.Where(x => x.Closed == false).SingleOrDefault();
            ChequeRule chequeRule = new ChequeRule();
            var cheque = await chequeRule.FindAsync(chequesAndStatsVM.chequeId);

            DocumentRule documentRule = new DocumentRule();

            Document document = new Document();
            document.Credit = cheque.Amount;
            document.DateTime = PersianDateUtils.ToDateTime(chequesAndStatsVM.date);
            document.Debit = cheque.Amount;
            document.Description = chequesAndStatsVM.description;
            document.DisplayDate = chequesAndStatsVM.date;
            document.IsManual = false;
            document.Status = ZhivarEnums.DocumentStatus.TaeedShode;
            document.Number = await documentRule.createNumberDocumentAsync(organId);
            document.Number2 = await documentRule.createNumberDocumentAsync(organId);
            document.StatusString = "تایید شده";
            document.OrganId = organId;
            document.FinanYear = finanYear;
            document.FinanYearId = finanYear.ID;
            document.Type = ZhivarEnums.NoeDoc.Recive;

            List<Transaction> transactions = new List<Transaction>();

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            switch (chequesAndStatsVM.receiveType)
                {
                    case "cash":
                        {
                            var accountCash = accounts.Where(x => x.ComplteCoding == "1101" + chequesAndStatsVM.detailAccount.Code).SingleOrDefault();

                            transactions.Add(new Transaction()
                            {
                                //AccDocument = document,
                                AccountId = accountCash.ID,
                                Amount = cheque.Amount,
                                Description = chequesAndStatsVM.description,
                                Credit = 0,
                                Debit = cheque.Amount,
                                IsCredit = false,
                                IsDebit = true,
                                DocumentId = document.ID,
                                Date = PersianDateUtils.ToDateTime(chequesAndStatsVM.date),
                                DisplayDate = chequesAndStatsVM.date
                            });
                            break;
                        }
                case "bank":
                    {
                        var accountBank = accounts.Where(x => x.ComplteCoding == "1103" + chequesAndStatsVM.detailAccount.Code).SingleOrDefault();

                        transactions.Add(new Transaction()
                        {
                            //AccDocument = document,
                            AccountId = accountBank.ID,
                            Amount = cheque.Amount,
                            Description = chequesAndStatsVM.description,
                            Credit = 0,
                            Debit = cheque.Amount,
                            IsCredit = false,
                            IsDebit = true,
                            DocumentId = document.ID,
                            Date = PersianDateUtils.ToDateTime(chequesAndStatsVM.date),
                            DisplayDate = chequesAndStatsVM.date
                        });
                        break;
                    }
            }

            var asnadDreaftaniAccount = accounts.Where(x => x.ComplteCoding == "1105" + cheque.Contact.Code).SingleOrDefault();

            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                AccountId = asnadDreaftaniAccount.ID,
                Amount = cheque.Amount,
                Credit = cheque.Amount,
                Debit = 0,
                Description = chequesAndStatsVM.description,
                DocumentId = document.ID,
                IsCredit = true,
                IsDebit = false,
                Date = PersianDateUtils.ToDateTime(chequesAndStatsVM.date),
                DisplayDate = chequesAndStatsVM.date

            });
                

            document.Transactions = transactions;

            return document;
        }


     

    }
}