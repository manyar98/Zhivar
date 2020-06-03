using System;
using System.Collections.Generic;
using OMF.Business;
using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.Query;
using OMF.EntityFramework.UnitOfWork;
using OMF.Security.Model;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCValidation = OMF.Common.Validation;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Accounting;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.Business.Accounting
{
    public partial class TransactionRule : BusinessRuleBase<Transaction>
    {
        public TransactionRule()
            : base()
        {

        }

        public TransactionRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public TransactionRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public async Task<List<TransactionVM>> GetAllByOrganIdAsync(int organId)
        {
            var finanYear = await this.unitOfWork.RepositoryAsync<FinanYear>().Queryable().Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync2();
            var documentQuery = this.unitOfWork.RepositoryAsync<Document>().Queryable().Where(x => x.OrganId == organId && x.FinanYearId == finanYear.ID).AsQueryable();
            var transactionQuery = this.Queryable();
          //  var accountQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId ).AsQueryable();

            var joinQuery = from document in documentQuery
                            join transaction in transactionQuery
                            on document.ID equals transaction.DocumentId
                            select new TransactionVM
                                 {
                                     Date = transaction.Date,
                                     Account = new AccountVM
                                     {
                                         Coding = transaction.Account.Coding,
                                         ComplteCoding = transaction.Account.ComplteCoding,
                                         ID = transaction.Account.ID,
                                         Level = transaction.Account.Level,
                                         Name = transaction.Account.Name,
                                         OrganId = transaction.Account.OrganId,
                                         ParentId = transaction.Account.ParentId
                                     },
                                     AccountId = transaction.AccountId,
                                     ID = transaction.ID,
                                     Amount = transaction.Amount,
                                     Cheque = transaction.Cheque,
                                     ChequeId = transaction.ChequeId,
                                     ContactId = transaction.ContactId,
                                     CostId = transaction.CostId,
                                     Credit = transaction.Credit,
                                     Debit = transaction.Debit,
                                     Description = transaction.Description,
                                     DisplayDate = transaction.DisplayDate,
                                     DocumentId = transaction.DocumentId,
                                     InvoiceId = transaction.InvoiceId,
                                     IsCredit = transaction.IsCredit,
                                     IsDebit = transaction.IsDebit,
                                     PaymentMethod = transaction.PaymentMethod,
                                     PaymentMethodString = transaction.PaymentMethodString,
                                     Reference = transaction.Reference,
                                     RefTrans = transaction.RefTrans,
                                     Remaining = transaction.Remaining,
                                     RemainingType = transaction.RemainingType,
                                     RowNumber = transaction.RowNumber,
                                     Stock = transaction.Stock,
                                     //Transactions = transaction.Transactions,
                                     TransactionTypeString = transaction.TransactionTypeString,
                                     Type = transaction.Type,
                                     UnitPrice = transaction.UnitPrice
                                 };

            return await joinQuery.ToListAsync2();
            //return _documents.AsQueryable().Where(x => x.OrganId == organId).Select(x => x.Transactions).ToList();
        }
        public async Task<List<TransactionVM>> GetAllByInvoiceIdAsync(int invoiceId, bool isDebit, bool isCredit)
        {
            var transactionsQuery = this.unitOfWork.RepositoryAsync<Transaction>().Queryable().Where(x => x.InvoiceId == invoiceId && x.IsCredit == isCredit && x.IsDebit == isDebit);//.Include(x => x.Account);
            var accountQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable();

            var joinQuery = from transactions in transactionsQuery
                            join account in accountQuery
                            on transactions.AccountId equals account.ID
                            select new TransactionVM()
                            {
                                //AccDocument = new DocumentVM()
                                //{
                                //    Credit = transactions.AccDocument.Credit,
                                //    DateTime = transactions.AccDocument.DateTime,
                                //    Debit = transactions.AccDocument.Debit,
                                //    Description = transactions.AccDocument.Description,
                                //    DisplayDate = transactions.AccDocument.DisplayDate,
                                //    Id = transactions.AccDocument.ID,
                                //    IsFirsDocument = transactions.AccDocument.IsFirsDocument,
                                //    IsManual = transactions.AccDocument.IsManual,
                                //    Number = transactions.AccDocument.Number,
                                //    Number2 = transactions.AccDocument.Number2,
                                //    Status = transactions.AccDocument.Status,
                                //    StatusString = transactions.AccDocument.StatusString,

                                //},
                                Account = new AccountVM()
                                {
                                    Balance = 0,
                                    BalanceType = 0,
                                    Code = transactions.Account.Coding,
                                    Coding = transactions.Account.Coding,
                                    ID = transactions.Account.ID,
                                    //Id = transactions.Account.ID,
                                    Level = transactions.Account.Level,
                                    LevelString = transactions.Account.Level.ToString(),
                                    Name = transactions.Account.Name,
                                    ParentId = transactions.Account.ParentId,


                                },
                                Amount = transactions.Amount,
                                Cheque = transactions.Cheque,
                                //Contact = transactions.co
                                Credit = transactions.Credit,
                                Debit = transactions.Debit,
                                Description = transactions.Description,
                                DetailAccount = new DetailAccount()
                                {
                                    Id = account.ID,
                                    Code = account.Coding,
                                    Name = account.Name
                                },
                                IsCredit = transactions.IsCredit,
                                IsDebit = transactions.IsDebit,
                                PaymentMethod = transactions.PaymentMethod,
                                PaymentMethodString = transactions.PaymentMethodString,
                                RefTrans = transactions.RefTrans,
                                Stock = transactions.Stock,
                                Type = transactions.Type,
                                UnitPrice = transactions.UnitPrice,
                                TransactionTypeString = transactions.TransactionTypeString,
                                RowNumber = transactions.RowNumber,
                                Id = transactions.ID,
                                Reference = transactions.Reference,
                                Remaining = transactions.Remaining,
                                RemainingType = transactions.RemainingType,

                            };

            return await joinQuery.ToListAsync2();
        }

        public async Task<List<TransactionVM>> GetAllByCostIdAsync(int costId)
        {
            var documentQuery = this.unitOfWork.RepositoryAsync<Document>().Queryable();
            var transactionQuery = this.unitOfWork.RepositoryAsync<Transaction>().Queryable().Where(x => x.CostId == costId);
            var accountQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable();

            var joinQuery = from transaction in transactionQuery
                            join document in documentQuery
                            on transaction.DocumentId equals document.ID
                            join account in accountQuery
                            on transaction.AccountId equals account.ID
                            select new TransactionVM
                            {
                                AccDocument = new DocumentVM
                                {
                                    Credit = document.Credit,
                                    DateTime = document.DateTime,
                                    Debit = document.Debit,
                                    Description = document.Description,
                                    DisplayDate = document.DisplayDate,
                                    Id = document.ID,
                                    IsFirsDocument = document.IsFirsDocument,
                                    IsManual = document.IsManual,
                                    Number = document.Number,
                                    Number2 = document.Number2,
                                    Status = document.Status,
                                    StatusString = document.StatusString,

                                },
                                Account = new AccountVM
                                {
                                    Balance = 0,
                                    BalanceType = 0,
                                    Code = account.Coding,
                                    Coding = account.Coding,
                                    ID = account.ID,
                                    //Id = account.ID,
                                    Level = account.Level,
                                    //LevelString = account.Level.GetPersianTitle(),
                                    Name = account.Name,
                                    ParentId = account.ParentId,


                                },
                                Amount = transaction.Amount,
                                Cheque = transaction.Cheque,
                                //Contact = transactions.co
                                Credit = transaction.Credit,
                                Debit = transaction.Debit,
                                Description = transaction.Description,
                                DetailAccount = new DetailAccount
                                {
                                    Id = account.ID,
                                    Code = account.Coding,
                                    Name = account.Name
                                },
                                IsCredit = transaction.IsCredit,
                                IsDebit = transaction.IsDebit,
                                PaymentMethod = transaction.PaymentMethod,
                                PaymentMethodString = transaction.PaymentMethodString,
                                RefTrans = transaction.RefTrans,
                                Stock = transaction.Stock,
                                Type = transaction.Type,
                                UnitPrice = transaction.UnitPrice,
                                TransactionTypeString = transaction.TransactionTypeString,
                                RowNumber = transaction.RowNumber,
                                Id = transaction.ID,
                                Reference = transaction.Reference,
                                Remaining = transaction.Remaining,
                                RemainingType = transaction.RemainingType,

                            };





            return await joinQuery.ToListAsync2();
        }
   

        public async Task<BalanceModelVM> GetBalanceAccountAsync(int accountId)
        {
            var transactions = this.Queryable().Where(x => x.AccountId == accountId);

            BalanceModelVM balanceModelVM = new BalanceModelVM()
            {
                Balance = 0,
                BalanceType = 0,
                Credit = 0,
                Debit = 0
            };

            if (await transactions.AnyAsync2())
            {
                decimal debit = transactions.Sum(x => x.Debit);
                decimal credit = transactions.Sum(x => x.Credit);
                decimal balance = credit - debit;
                int balanceType = 0;
                if (balance >= 0)
                    balanceType = 1;
                else
                    balanceType = -1;

                balanceModelVM = new BalanceModelVM()
                {
                    Balance = balance,
                    Credit = credit,
                    Debit = debit,
                    BalanceType = balanceType

                };


            }
            return balanceModelVM;
        }
    }
}