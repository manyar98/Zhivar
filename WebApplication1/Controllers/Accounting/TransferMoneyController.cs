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
using OMF.Common.Security;
using OMF.Business;
using OMF.Enterprise.MVC;
using Zhivar.Business.Accounting;
using Zhivar.Business.Common;

namespace Zhivar.Web.Controllers.Accounting
{
    [RoutePrefix("api/TransferMoney")]
    public partial class TransferMoneyController : NewApiControllerBaseAsync<TransferMoney, TransferMoneyVM>
    {
        public TransferMoneyRule Rule => this.BusinessRule as TransferMoneyRule;

        protected override IBusinessRuleBaseAsync<TransferMoney> CreateBusinessRule()
        {
            return new TransferMoneyRule();
        }
        //[Route("GetAllByOrganId")]
        //public virtual async Task<HttpResponseMessage> GetAllByOrganId()
        //{
        //    var userId = SecurityManager.CurrentUserContext.UserId;
        //    var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));


        //    var list = await transferMoneyRule.GetAllByPersonIdAsync(Convert.ToInt32(organId));

        //    TransferMoneyVM transferMoneyVM = new TransferMoneyVM();
        //    List<TransferMoneyVM> transferMoneyVMs = new List<TransferMoneyVM>();

        //    foreach (var item in list)
        //    {
        //        transferMoneyVMs.Add(new TransferMoneyVM()
        //        {
        //            AccountNumber = item.AccountNumber,
        //            Balance = item.Balance,
        //            Branch = item.Branch,
        //            Code = item.Code,
        //            CodeShobe = item.CodeShobe,
        //            FullName = item.FullName,
        //            ID = item.ID,
        //            Nam = item.Nam,
        //            Name = item.Name,
        //            ShomareHesab = item.ShomareHesab,
        //            ShomareShobe = item.ShomareShobe,
        //            DetailAccount = new DetailAccount()
        //            {
        //                Code = item.Code,
        //                Id = item.ID,
        //            }
        //        });

        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Successful, data = transferMoneyVMs });
        //}

        [Route("GetLoadTransfer")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetLoadTransfer([FromBody] int id)
        {
            LoadTransfer loadTransfer = new LoadTransfer();

            TransferMoneyRule transferMoneyRule = new TransferMoneyRule();

            if (id > 0)
            {
                
               var transfer = await transferMoneyRule.GetByDocIdAsync(id);
               // int fromId, toId = 0;

               // if (transfer.From == "cash")
               // {
               //     var cashFrom = await cashRule.GetByAccountId(Convert.ToInt32(transfer.FromDetailAccountId));
               //     fromId = cashFrom.ID;

               //     if (transfer.To == "cash")
               //     {
               //         var cashTo = await cashRule.GetByAccountId(transfer.ToDetailAccountId);
               //         toId = cashTo.ID;
               //     }
               //     else
               //     {
               //         var bankTo = await bankRule.GetByAccountId(transfer.ToDetailAccountId);
               //         toId = bankTo.ID;
               //     }
               // }
               // else
               // {
               //     var bankFrom = await bankRule.GetByAccountId(Convert.ToInt32(transfer.FromDetailAccountId));
               //     fromId = bankFrom.ID;

               //     if (transfer.To == "cash")
               //     {
               //         var cashTo = await cashRule.GetByAccountId(transfer.ToDetailAccountId);
               //         toId = cashTo.ID;
               //     }
               //     else
               //     {
               //         var bankTo = await bankRule.GetByAccountId(transfer.ToDetailAccountId);
               //         toId = bankTo.ID;
               //     }
               // }
                loadTransfer = new LoadTransfer()
                {
                    Amount = transfer.Amount,
                    Date = transfer.Date,
                    DisplayDate = transfer.DisplayDate,
                    Description = transfer.Description,
                    DocumentId = (int)transfer.DocumentId,
                    DocumentNumber = transfer.DocumentNumber,
                    From = transfer.From,
                    FromDetailAccountId = Convert.ToInt32( transfer.FromDetailAccountId),
                    FromDetailAccountName = transfer.FromDetailAccountName,
                    FromReference = transfer.FromReference,
                    To = transfer.To,
                    ToDetailAccountId = Convert.ToInt32(transfer.ToDetailAccountId),// toId,
                    ToDetailAccountName = transfer.ToDetailAccountName,
                    ToReference = transfer.ToReference,
                };

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = loadTransfer });
            }

            loadTransfer = new LoadTransfer() {
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

        [Route("SaveTransfer")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> SaveTransfer([FromBody]TransferMoneyVM transferMoneyVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            if (!string.IsNullOrEmpty(transferMoneyVM.DisplayDate))
             {
                var str = transferMoneyVM.DisplayDate.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("۷", "7").Replace("۸", "8").Replace("۹", "9");

                transferMoneyVM.Date = PersianDateUtils.ToDateTime(str);
            }
            else
                transferMoneyVM.Date = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.ValidationError, data = "" });
            }

            var transferMoney = new TransferMoney();
            Mapper.Map(transferMoneyVM, transferMoney);

            transferMoney.OrganId = organId;

           

            var fromName = string.Empty;
            var toName = string.Empty;

            AccountRule accountRule = new AccountRule();

            if (transferMoneyVM.From == "bank")
            {
                var bankFrom = await accountRule.FindAsync(Convert.ToInt32(transferMoneyVM.FromDetailAccountId));
                fromName = bankFrom.Name;
            }
            else
            {
                var cashFrom = await accountRule.FindAsync(Convert.ToInt32(transferMoneyVM.FromDetailAccountId));
                fromName = cashFrom.Name;
            }

            if (transferMoneyVM.To == "bank")
            {
                var bankTo = await accountRule.FindAsync(Convert.ToInt32(transferMoneyVM.ToDetailAccountId));
                toName = bankTo.Name;
            }
            else
            {
                var cashTo = await accountRule.FindAsync(Convert.ToInt32(transferMoneyVM.ToDetailAccountId));
                toName = cashTo.Name;
            }

            transferMoney.Document = await createDocument(transferMoney, fromName, toName, organId);

            TransferMoneyRule transferMoneyRule = new TransferMoneyRule();

            if (transferMoney.DocumentId != null && transferMoney.DocumentId > 0)
            {
                var oldTransfer = await transferMoneyRule.GetByDocIdAsync((int)transferMoney.DocumentId);
                //transferMoneyVM.ID = oldTransfer.ID;
                //transferMoney.ID = oldTransfer.ID;

                //transferMoney.Document.ID = oldTransfer.Document.ID;
                //transferMoney.DocumentId = oldTransfer.DocumentId;

                //transferMoney.Document.Transactions[0].ID = oldTransfer.Document.Transactions[0].ID;
                //transferMoney.Document.Transactions[1].ID = oldTransfer.Document.Transactions[1].ID;
                //transferMoney.Document.Transactions[0].DocumentId = oldTransfer.Document.ID;
                //transferMoney.Document.Transactions[1].DocumentId = oldTransfer.Document.ID;

                transferMoneyRule.Delete(oldTransfer.ID);
                await transferMoneyRule.SaveChangesAsync();
            }
            //if (transferMoneyVM.ID.HasValue)
            //{
            //    transferMoneyRule.Update(transferMoney);
            //}
            //else
            //{
            //    transferMoneyRule.Insert(transferMoney);
            //}
            transferMoneyRule.Insert(transferMoney);
            await transferMoneyRule.SaveChangesAsync();

            string result = transferMoneyVM.Amount + ","+fromName +","+toName;
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = result });

        }

        [Route("GetTransferList")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetTransferList()
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                TransferMoneyRule transferMoneyRule = new TransferMoneyRule();

                var resualt = await transferMoneyRule.GetAllByOrganIdAsync(organId);

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        [Route("DeleteTransfer")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> DeleteTransfer([FromBody] int id)
        {
            TransferMoneyRule transferMoneyRule = new TransferMoneyRule();
            var resualt = await Rule.FindAsync(id);
             await Rule.DeleteAsync(id);
            await Rule.SaveChangesAsync();
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });
        }
        

        private async Task<Document> createDocument(TransferMoney transferMoney,string from,string to, int organId)
        {
            DocumentRule documentRule = new DocumentRule();

            FinanYearRule finanYearRule = new FinanYearRule();
            var finanYears = await finanYearRule.GetAllByOrganIdAsync(organId);
            var finanYear = finanYears.Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefault();
            Document document = new Document();
            document.Credit = transferMoney.Amount;
            document.DateTime = DateTime.Now;
            document.Debit = transferMoney.Amount;
            document.Description = " انتقال وجه از "+ from + " به " + to ;
            document.DisplayDate = PersianDateUtils.ToPersianDate(DateTime.Now);
            document.IsManual = false;
            document.Number = await documentRule.createNumberDocumentAsync(organId);
            document.Number2 = await documentRule.createNumberDocumentAsync(organId);
            document.Status = ZhivarEnums.DocumentStatus.TaeedShode;
            document.StatusString = "تایید شده";
            document.OrganId = organId;
            document.FinanYear = finanYear;
            document.FinanYearId = finanYear.ID;
            document.Type = ZhivarEnums.NoeDoc.Transfer;
            List<Transaction> transactions = new List<Transaction>();

            transactions = await RegisterTransaction(document, transferMoney, organId);

            document.Transactions = transactions;

            return document;
        }


        private async Task<List<Transaction>> RegisterTransaction(Document document, TransferMoney transferMoney, int organId)
        {
            List<Transaction> transactions = new List<Transaction>();

  

            if (transferMoney.From == "bank")
            {
                if(transferMoney.To == "bank")
                {
                    transactions = await createTransactionFromBankToBank(document,transferMoney, organId);
                }
                else
                {
                    transactions = await createTransactionFromBankToCash(document, transferMoney, organId);
                }
            }
            else
            {
                if (transferMoney.To == "bank")
                {
                    transactions = await createTransactionFromCashToBank(document, transferMoney, organId);
                }
                else
                {
                    transactions = await createTransactionFromCashToCash(document, transferMoney, organId);
                }

                
            }
      

            return transactions;
        }

        private async Task<List<Transaction>> createTransactionFromCashToCash(Document document, TransferMoney transferMoney, int organId)
        {
            List<Transaction> transactions = new List<Transaction>();

            AccountRule accountRule = new AccountRule();
            var accountFromCash = await accountRule.FindAsync(Convert.ToInt32(transferMoney.FromDetailAccountId));
            var accountToCash = await accountRule.FindAsync(Convert.ToInt32(transferMoney.ToDetailAccountId));


            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                Account = accountFromCash,
                AccountId = accountFromCash.ID,
                Amount = transferMoney.Amount,
                Credit = transferMoney.Amount,
                Debit = 0,
                IsCredit = true,
                IsDebit = false,
                DocumentId = document.ID,
                Date = transferMoney.Date,
                Description = document.Description,// cashTo + " به " + cashFrom + " انتقال وجه از",
                DisplayDate = transferMoney.DisplayDate,
                Reference = transferMoney.FromReference,


            });


            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                Account = accountToCash,
                AccountId = accountToCash.ID,
                Amount = transferMoney.Amount,
                Credit = 0,
                Debit = transferMoney.Amount,
                IsCredit = false,
                IsDebit = true,
                DocumentId = document.ID,
                Date = transferMoney.Date,
                Description = document.Description,// cashTo + " به " + cashFrom + " انتقال وجه از",
                DisplayDate = transferMoney.DisplayDate,
                Reference = transferMoney.FromReference,


            });

            return transactions;
        }

        private async Task<List<Transaction>> createTransactionFromCashToBank(Document document, TransferMoney transferMoney, int organId)
        {
            List<Transaction> transactions = new List<Transaction>();
            AccountRule accountRule = new AccountRule();
            var accountFromCash = await accountRule.FindAsync(Convert.ToInt32(transferMoney.FromDetailAccountId));
            var accountToBank = await accountRule.FindAsync(Convert.ToInt32(transferMoney.ToDetailAccountId));
            
            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                Account = accountFromCash,
                AccountId = accountFromCash.ID,
                Amount = transferMoney.Amount,
                Credit = transferMoney.Amount,
                Debit = 0,
                IsCredit = true,
                IsDebit = false,
                DocumentId = document.ID,
                Date = transferMoney.Date,
                Description = document.Description,// bankTo + " به " + cashFrom + " انتقال وجه از",
                DisplayDate = transferMoney.DisplayDate,
                Reference = transferMoney.FromReference,


            });


            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                Account = accountToBank,
                AccountId = accountToBank.ID,
                Amount = transferMoney.Amount,
                Credit =0 ,
                Debit = transferMoney.Amount,
                IsCredit = false,
                IsDebit = true,
                DocumentId = document.ID,
                Date = transferMoney.Date,
                Description = document.Description,// bankTo + " به " + cashFrom + " انتقال وجه از",
                DisplayDate = transferMoney.DisplayDate,
                Reference = transferMoney.FromReference,


            });

            return transactions;
        }

        private async Task<List<Transaction>> createTransactionFromBankToCash(Document document, TransferMoney transferMoney, int organId)
        {           
            List<Transaction> transactions = new List<Transaction>();

            AccountRule accountRule = new AccountRule();
            var accountFromBank = await accountRule.FindAsync(Convert.ToInt32(transferMoney.FromDetailAccountId));
            var accountToCash = await accountRule.FindAsync(Convert.ToInt32(transferMoney.ToDetailAccountId));

            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                Account = accountFromBank,
                AccountId = accountFromBank.ID,
                Amount = transferMoney.Amount,
                Credit = transferMoney.Amount,
                Debit = 0,
                IsCredit = true,
                IsDebit = false,
                DocumentId = document.ID,
                Date = transferMoney.Date,
                Description = document.Description,// cashTo + " به " + bankFrom + " انتقال وجه از",
                DisplayDate = transferMoney.DisplayDate,
                Reference = transferMoney.FromReference,


            });


            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                Account = accountToCash,
                AccountId = accountToCash.ID,
                Amount = transferMoney.Amount,
                Credit = 0,
                Debit = transferMoney.Amount,
                IsCredit = false,
                IsDebit = true,
                DocumentId = document.ID,
                Date = transferMoney.Date,
                Description = document.Description,// cashTo + " به " + bankFrom + " انتقال وجه از",
                DisplayDate = transferMoney.DisplayDate,
                Reference = transferMoney.FromReference,


            });

            return transactions;
        }

        private async Task<List<Transaction>> createTransactionFromBankToBank(Document document, TransferMoney transferMoney, int organId)
        {
            List<Transaction> transactions = new List<Transaction>();

            AccountRule accountRule = new AccountRule();
            var accountFromBank = await accountRule.FindAsync(Convert.ToInt32(transferMoney.FromDetailAccountId));
            var accountToBank = await accountRule.FindAsync(Convert.ToInt32(transferMoney.ToDetailAccountId));

            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                Account = accountFromBank,
                AccountId = accountFromBank.ID,
                Amount = transferMoney.Amount,
                Credit = transferMoney.Amount,
                Debit = 0,
                IsCredit = true,
                IsDebit = false,
                DocumentId = document.ID,
                Date = transferMoney.Date,
                Description = document.Description,// bankTo+" به "+bankFrom + " انتقال وجه از",
                DisplayDate = transferMoney.DisplayDate,
                Reference = transferMoney.FromReference,
                

            });


            transactions.Add(new Transaction()
            {
                //AccDocument = document,
                Account = accountToBank,
                AccountId = accountToBank.ID,
                Amount = transferMoney.Amount,
                Credit = 0,
                Debit = transferMoney.Amount,
                IsCredit = false,
                IsDebit = true,
                DocumentId = document.ID,
                Date = transferMoney.Date,
                Description = document.Description,// bankTo + " به " + bankFrom + " انتقال وجه از",
                DisplayDate = transferMoney.DisplayDate,
                Reference = transferMoney.FromReference,


            });

            return transactions;
        }
        
    }
}