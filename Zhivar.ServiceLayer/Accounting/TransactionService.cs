using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.Accounting;
using Zhivar.ServiceLayer.Contracts.Accounting;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ServiceLayer.Accounting
{
    public class TransactionService : ITransaction
    {
        IUnitOfWork _uow;
        readonly IDbSet<Transaction> _transactions;
        readonly IDbSet<Document> _documents;
        readonly IDbSet<DomainClasses.Accounting.Account> _accounts;
        readonly IDbSet<FinanYear> _finanYears;

        public TransactionService(IUnitOfWork uow)
        {
            _uow = uow;
            _transactions = _uow.Set<Transaction>();
            _documents = _uow.Set<Document>();
            _accounts = _uow.Set<DomainClasses.Accounting.Account>();
            _finanYears = _uow.Set<FinanYear>();
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                var transaction = await GetByIdAsync(id);
                Delete(transaction);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Transaction transaction)
        {
            try
            {
                _transactions.Attach(transaction);
                _transactions.Remove(transaction);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        //public IList<Transaction> GetAll()
        //{
        //    return _transactions.ToList();
        //}
        public async Task<List<Transaction>> GetAllByOrganIdAsync(int organId)
        {
            var finanYear = await _finanYears.Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync();
            var documentQuery = _documents.Where(x => x.OrganId == organId && x.FinanYearId == finanYear.ID).AsQueryable();
            var transactionQuery = _transactions.AsQueryable();

            var joinQuery = from document in documentQuery
                            join transaction in transactionQuery
                            on document.ID equals transaction.DocumentId
                            select transaction;

            return await joinQuery.ToListAsync();
            //return _documents.AsQueryable().Where(x => x.OrganId == organId).Select(x => x.Transactions).ToList();
        }
        public async Task<List<TransactionVM>> GetAllByInvoiceIdAsync(int invoiceId, bool isDebit, bool isCredit)
        {
            var transactionsQuery = _transactions.AsQueryable().Where(x => x.InvoiceId == invoiceId && x.IsCredit == isCredit && x.IsDebit == isDebit).Include(x => x.Account);
            var accountQuery = _accounts.AsQueryable();

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
                                Account = new AccountVM() {
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

            return await joinQuery.ToListAsync();
        }

        public async Task<List<TransactionVM>> GetAllByCostIdAsync(int costId)
        {
            var transactionsQuery = _transactions.AsQueryable().Where(x => x.CostId == costId).Include(x => x.Account);
            var accountQuery = _accounts.AsQueryable();

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

            return await joinQuery.ToListAsync();
        }
        public bool Insert(Transaction transaction)
        {
            try
            {
                _transactions.Add(transaction);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Transaction transaction)
        {
            try
            {
                var local = _uow.Set<Transaction>()
                     .Local
                     .FirstOrDefault(f => f.ID == transaction.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = System.Data.Entity.EntityState.Detached;
                }

                _transactions.Attach(transaction);

                _uow.Entry(transaction).State = System.Data.Entity.EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Transaction> GetByIdAsync(int id)
        {
            return await _transactions.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task<BalanceModelVM> GetBalanceAccountAsync(int accountId)
        {
            var transactions = _transactions.Where(x => x.AccountId == accountId);

            BalanceModelVM balanceModelVM = new BalanceModelVM()
            {
                Balance = 0,
                BalanceType = 0,
                Credit = 0,
                Debit = 0
            };

            if (await transactions.AnyAsync())
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
