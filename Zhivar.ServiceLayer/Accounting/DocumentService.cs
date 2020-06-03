using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using Zhivar.ServiceLayer.Contracts.Accounting;
using Zhivar.ViewModel.Accunting;
using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.ServiceLayer.Accounting
{
    public class DocumentService : IDocument
    {
        IUnitOfWork _uow;
        readonly IDbSet<Document> _documents;
        readonly IDbSet<Transaction> _transactions;
        readonly IDbSet<DomainClasses.Accounting.Account> _accounts;
        readonly IDbSet<Shareholder> _shareholders;
        readonly IDbSet<Contact> _contacts;
        readonly IDbSet<Item> _items;
        readonly IDbSet<FinanYear> _finanYears;
        readonly IDbSet<Cheque> _cheques;
        readonly IDbSet<ChequeBank> _chequeBanks;
        readonly IDbSet<PayRecevie> _payRecevies;
        readonly IDbSet<DetailPayRecevie> _detailPayRecevie;
        readonly IDbSet<Bank> _banks;

        public DocumentService(IUnitOfWork uow)
        {
            _uow = uow;
            _documents = _uow.Set<Document>();
            _transactions = _uow.Set<Transaction>();
            _accounts = _uow.Set<DomainClasses.Accounting.Account>();
            _shareholders = _uow.Set<Shareholder>();
            _contacts = _uow.Set<Contact>();
            _items = _uow.Set<Item>();
            _finanYears = _uow.Set<FinanYear>();
            _cheques = _uow.Set<Cheque>();
            _chequeBanks = _uow.Set<ChequeBank>();
            _payRecevies = _uow.Set<PayRecevie>();
            _detailPayRecevie = _uow.Set<DetailPayRecevie>();
            _banks = _uow.Set<Bank>();
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var document = await GetByIdAsync(id);
                Delete(document);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Document document)
        {
            try
            {
                //_documents.Attach(document);
                //_documents.Remove(document);

                //var local = _uow.Set<Document>()
                //   .Local
                //   .FirstOrDefault(f => f.ID == document.ID);
                //if (local != null)
                //{
                //    _uow.Entry(local).State = EntityState.Detached;
                //}

                _documents.Attach(document);

                

                foreach (var transaction in document.Transactions.ToList() ?? new List<Transaction>())
                {
                    var localTransaction = _uow.Set<Transaction>()
                  .Local
                  .FirstOrDefault(f => f.ID == transaction.ID);
                    if (localTransaction != null)
                    {
                        _uow.Entry(localTransaction).State = EntityState.Detached;
                    }

                    //_documents.Attach(document);

                    _uow.Entry(transaction).State = EntityState.Deleted;
                }

                _uow.Entry(document).State = EntityState.Deleted;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IList<Document>> GetAllByOrganIdAsync(int organId)
        {
            var finanYear = await _finanYears.Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync();
            return await _documents.AsQueryable().Where(x => x.OrganId == organId && x.FinanYearId == finanYear.ID ).Include(x => x.Transactions).OrderByDescending(x => x.ID).ToListAsync();
        }
        
        public async Task<bool> Insert(Document document, int organId)
        {
            try
            {
                document.OrganId = organId;
                var finanYear = await _finanYears.Where(x => x.OrganId == organId &&  x.Closed == false).SingleOrDefaultAsync();
            
                document.FinanYearId = finanYear.ID;
                _documents.Add(document);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<bool> Update(Document document)
        {
            try
            {
                if (document.FinanYearId <= 0)
                {
                    var finanYear = await _finanYears.Where(x => x.OrganId == document.OrganId && x.Closed == false).SingleOrDefaultAsync();
                    document.FinanYearId = finanYear.ID;
                }
                foreach (var transaction in document.Transactions.ToList() ?? new List<Transaction>())
                {
                    if (transaction.ID > 0)
                    {

                        var local = _uow.Set<Transaction>()
                             .Local
                             .FirstOrDefault(f => f.ID == transaction.ID);
                        if (local != null)
                        {
                            _uow.Entry(local).State = EntityState.Detached;
                        }

                        _uow.Entry(transaction).State = EntityState.Modified;
                    }
                    else
                    {

                        _uow.Entry(transaction).State = EntityState.Added;
                    }

                    if (transaction.Cheque != null)
                    {
                        if(transaction.Cheque.Contact != null)
                        {
                            var local = _uow.Set<Contact>()
                            .Local
                            .FirstOrDefault(f => f.ID == transaction.Cheque.Contact.ID);
                            if (local != null)
                            {
                                _uow.Entry(local).State = EntityState.Unchanged;
                            }

                            //_uow.Entry(transaction.Cheque.Contact).State = EntityState.;
                        }

                        _uow.Entry(transaction.Cheque).State = EntityState.Modified;
                    }
                }

                _uow.Entry(document).State = EntityState.Modified;

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        public async Task<Document> GetByIdAsync(int id)
        {
      
            return await _documents.AsQueryable().Where(x => x.ID == id).Include(x => x.Transactions).Include(x => x.Transactions.Select(y => y.Account)).FirstOrDefaultAsync();
        }

        public async Task<DocumentVM> GetDocumentByIdAsync(int id)
        {
            var documentsQuery = _documents.AsQueryable().Where(x => x.ID ==id);
            var transactionsQuery = _transactions.AsQueryable().Where(x => x.DocumentId == id);
            var accountQuery = _accounts.AsQueryable();

            var joinQuery = from transaction in transactionsQuery
                            join account in accountQuery
                            on transaction.AccountId equals account.ID
                            join parent in accountQuery
                            on account.ParentId equals parent.ID
                            select new TransactionVM
                            {
                                //AccDocument = transaction.AccDocument,
                                Amount = transaction.Amount,
                                ContactId = transaction.ContactId,
                                Credit = transaction.Credit,
                                Date = transaction.Date,
                                AccountId = transaction.AccountId,
                                ChequeId = transaction.ChequeId,
                                Debit = transaction.Debit,
                                Description = transaction.Description,
                                DisplayDate = transaction.DisplayDate,
                                DocumentId = transaction.DocumentId,
                                Id = transaction.ID,
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
                                Type = transaction.Type,
                                TransactionTypeString = transaction.TransactionTypeString,
                                UnitPrice = transaction.UnitPrice,
                                AccountName = parent.Name+" / "+ account.Name,
                                AccountComplteCoding = account.ComplteCoding

                            };

            var document = await (from document1 in documentsQuery
                           select new DocumentVM
                           {
                               Credit = document1.Credit,
                               DateTime = document1.DateTime,
                               Debit = document1.Debit,
                               Description = document1.Description,
                               DisplayDate = document1.DisplayDate,
                               Id = document1.ID,
                               IsFirsDocument = document1.IsFirsDocument,
                               IsManual = document1.IsManual,
                               Number = document1.Number,
                               Number2 = document1.Number2,
                               Status = document1.Status,
                               StatusString = document1.StatusString,
                             
                           }).SingleOrDefaultAsync();
            document.Transactions = new List<TransactionVM>();

            document.Transactions = await joinQuery.ToListAsync();

            return document;
        }
        public async Task<int> createNumberDocumentAsync(int organId)
        {
            var count = await _documents.Where(x=> x.OrganId ==  organId).CountAsync();

            return count++;
        }

        public async Task<decimal> CreateDocumentOpeningBalanceCash(Document document, List<TransactionVM> transactions, string docDate, int organId)
        {

            transactions = transactions.Where(x => x.DetailAccount != null).ToList();

            foreach (var item in transactions ?? new List<TransactionVM>())
            {
                Transaction transaction = new Transaction()
                { 
                    //AccDocument = document,
                    AccountId = item.DetailAccount.Id,
                    Amount = item.Amount,
                    Date = DateTime.Now,
                    Debit = item.Amount,
                    Credit = 0,
                    Description = "سند افتتاحیه",
                    DocumentId = document.ID,
                    ID = item.Id,
                    IsDebit = true,
                    IsCredit = false,
                    DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                    Stock = item.Stock,
                    UnitPrice = item.UnitPrice


                };

                //document.Transactions.Add(cashTransaction);


                if (transaction.ID <= 0)
                {

                    _uow.Entry<Transaction>(transaction).State = EntityState.Added;
                    await _uow.SaveAllChangesAsync();

                }
                else
                {
                    var relTransactions = await _transactions.Where(x => x.RefTrans == transaction.ID).ToListAsync();

                    foreach (var relTransaction in relTransactions)
                    {
                        _uow.Entry<Transaction>(relTransaction).State = EntityState.Deleted;
                        await _uow.SaveAllChangesAsync();
                    }

                    if (transaction.Account != null)
                    {
                        var localAccount = _uow.Set<DomainClasses.Accounting.Account>()
                          .Local
                          .FirstOrDefault(f => f.ID == transaction.Account.ID);
                        if (localAccount != null)
                        {
                            _uow.Entry(localAccount).State = EntityState.Detached;
                        }
                    }

                    var local = _uow.Set<Transaction>()
                            .Local
                            .FirstOrDefault(f => f.ID == transaction.ID);
                    if (local != null)
                    {
                        _uow.Entry(local).State = EntityState.Detached;
                    }

                    _uow.Entry(transaction).State = EntityState.Modified;

                }


                var parentAccount = await _accounts.Where(x => x.OrganId == organId && x.ComplteCoding == "3101" && x.Level == DomainClasses.ZhivarEnums.AccountType.Moen).SingleOrDefaultAsync();
                var shareholdersQuery = _shareholders.Where(x => x.OrganId == organId && x.IsActive == true);
                var contactQuery = _contacts.AsQueryable();
                var accountsQuery = _accounts.Where(x => x.OrganId == organId && x.ParentId == parentAccount.ID).AsQueryable();

                var joinQuery = from shareholder in shareholdersQuery
                                join contact in contactQuery
                                on shareholder.ContactId equals contact.ID
                                join account in accountsQuery
                                on contact.Code equals account.Coding
                                select new
                                {
                                    AccounttId = account.ID,
                                    Amount = (transaction.Amount * shareholder.SharePercent)/100
                                };

                foreach (var shareholder in joinQuery)
                {
                    var transaction2 = new Transaction()
                    {
                        //AccDocument = document,
                        AccountId = shareholder.AccounttId,
                        Amount = shareholder.Amount,
                        Date = DateTime.Now,
                        Debit = 0,
                        Credit = shareholder.Amount,
                        Description = "سند افتتاحیه",
                        DocumentId = document.ID,
                        IsDebit = false,
                        IsCredit = true,
                        DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                        RefTrans = transaction.ID

                    };

                    _uow.Entry<Transaction>(transaction2).State = EntityState.Added;
                }
            }

            //if (document.ID > 0)
            //    await Update(document);
            //else
            //    await Insert(document, organId);

            return transactions.Sum(x => x.Amount);
        }

        public async Task<decimal> CreateDocumentOpeningBalanceReceivables(Document document, List<TransactionVM> transactions, string docDate, int organId)
        {
            transactions = transactions.Where(x => x.DetailAccount != null).ToList();

            foreach (var item in transactions ?? new List<TransactionVM>())
            {
                var findAccount = await _accounts.Where(x => x.ID == item.DetailAccount.Id).SingleOrDefaultAsync();
                Transaction transaction = new Transaction()
                {
                    //AccDocument = document,
                    AccountId = item.DetailAccount.Id,
                    Account = findAccount,
                    Amount = item.Amount,
                    Date = DateTime.Now,
                    Debit = item.Amount,
                    Credit = 0,
                    Description = "سند افتتاحیه",
                    DocumentId = document.ID,
                    ID = item.Id,
                    IsDebit = true,
                    IsCredit = false,
                    DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                    Stock = item.Stock,
                    UnitPrice = item.UnitPrice,
                    //ContactId = item.Cheque.ContactId,
                    
                    Cheque = new Cheque()
                    {
                        ID = item.Cheque.ID,
                        Amount = item.Amount,
                        BankBranch = item.Cheque.BankBranch,
                        BankName = item.Cheque.BankName,
                        ChequeNumber = item.Cheque.ChequeNumber,
                        Contact = item.Cheque.Contact,
                        ContactId = item.Cheque.ContactId,
                        Date = Utilities.PersianDateUtils.ToDateTime(item.Cheque.DisplayDate),
                        DepositBank = item.Cheque.DepositBank,
                        DisplayDate = item.Cheque.DisplayDate,
                        OrganId = organId,
                        Status = item.Cheque.Status,
                        ReceiptDate = Utilities.PersianDateUtils.ToDateTime(item.Cheque.DisplayDate),
                        Type = item.Cheque.Type,
                        
                    }
                    

                };
                transaction.Cheque.ContactId = transaction.Cheque.Contact.ID;

                if (transaction.Cheque != null)
                    transaction.ChequeId = transaction.Cheque.ID;
                
                

                //document.Transactions.Add(transaction);


                if (transaction.ID <= 0)
                {

                    _uow.Entry<Transaction>(transaction).State = EntityState.Added;
                    await _uow.SaveAllChangesAsync();

                }
                else
                {
                    var relTransactions = await _transactions.Where(x => x.RefTrans == transaction.ID).ToListAsync();

                    foreach (var relTransaction in relTransactions)
                    {
                        _uow.Entry<Transaction>(relTransaction).State = EntityState.Deleted;
                        await _uow.SaveAllChangesAsync();
                    }

                    if (transaction.Account != null)
                    {
                        var localAccount = _uow.Set<DomainClasses.Accounting.Account>()
                          .Local
                          .FirstOrDefault(f => f.ID == transaction.Account.ID);
                        if (localAccount != null)
                        {
                            _uow.Entry(localAccount).State = EntityState.Detached;
                        }
                    }

                    var local = _uow.Set<Transaction>()
                            .Local
                            .FirstOrDefault(f => f.ID == transaction.ID);
                    if (local != null)
                    {
                        _uow.Entry(local).State = EntityState.Detached;
                    }

                    _uow.Entry(transaction).State = EntityState.Modified;

                }


                var parentAccount = await _accounts.Where(x => x.OrganId == organId && x.ComplteCoding == "3101" && x.Level == DomainClasses.ZhivarEnums.AccountType.Moen).SingleOrDefaultAsync();
                var shareholdersQuery = _shareholders.Where(x => x.OrganId == organId && x.IsActive == true);
                var contactQuery = _contacts.AsQueryable();
                var accountsQuery = _accounts.Where(x => x.OrganId == organId && x.ParentId == parentAccount.ID).AsQueryable();

                var joinQuery = from shareholder in shareholdersQuery
                                join contact in contactQuery
                                on shareholder.ContactId equals contact.ID
                                join account in accountsQuery
                                on contact.Code equals account.Coding
                                select new
                                {
                                    AccounttId = account.ID,
                                    Amount = (transaction.Amount * shareholder.SharePercent) / 100
                                };

                foreach (var shareholder in joinQuery)
                {
                    var transaction2 = new Transaction()
                    {
                        //AccDocument = document,
                        AccountId = shareholder.AccounttId,
                        Amount = shareholder.Amount,
                        Date = DateTime.Now,
                        Debit = 0,
                        Credit = shareholder.Amount,
                        Description = "سند افتتاحیه",
                        DocumentId = document.ID,
                        IsDebit = false,
                        IsCredit = true,
                        DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                        RefTrans = transaction.ID

                    };
                    _uow.Entry<Transaction>(transaction2).State = EntityState.Added;
                }
            }

            //if (document.ID > 0)
            //    await Update(document);
            //else
            //    await Insert(document, organId);

            return transactions.Sum(x => x.Amount);
        }

        public async Task<DocumentVM> GetFirstDocumentVM(int organId)
        {
            //var documentQuery =_documents.Where(x => x.OrganId == organId && x.IsFirsDocument == true).Include(x => x.Transactions).Include(x => x.Transactions.Select(x2 => x2.Account));

            var documentsQuery = _documents.AsQueryable().Where(x => x.OrganId == organId && x.IsFirsDocument == true);
            var transactionsQuery = _transactions.AsQueryable();
            var accountQuery = _accounts.AsQueryable();

            var joinQuery = from transaction in transactionsQuery
                            join account in accountQuery
                            on transaction.AccountId equals account.ID
                            join parent in accountQuery
                            on account.ParentId equals parent.ID
                            select new TransactionVM
                            {
                                //AccDocument = transaction.AccDocument,
                                Amount = transaction.Amount,
                                ContactId = transaction.ContactId,
                                Credit = transaction.Credit,
                                Date = transaction.Date,
                                AccountId = transaction.AccountId,
                                ChequeId = transaction.ChequeId,
                                Debit = transaction.Debit,
                                Description = transaction.Description,
                                DisplayDate = transaction.DisplayDate,
                                DocumentId = transaction.DocumentId,
                                Id = transaction.ID,
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
                                Type = transaction.Type,
                                TransactionTypeString = transaction.TransactionTypeString,
                                UnitPrice = transaction.UnitPrice,
                                AccountName = parent.Name + " / " + account.Name,
                                AccountComplteCoding = account.ComplteCoding
                            };

            var document = await (from document1 in documentsQuery
                                  select new DocumentVM
                                  {
                                      Credit = document1.Credit,
                                      DateTime = document1.DateTime,
                                      Debit = document1.Debit,
                                      Description = document1.Description,
                                      DisplayDate = document1.DisplayDate,
                                      Id = document1.ID,
                                      IsFirsDocument = document1.IsFirsDocument,
                                      IsManual = document1.IsManual,
                                      Number = document1.Number,
                                      Number2 = document1.Number2,
                                      Status = document1.Status,
                                      StatusString = document1.StatusString,

                                  }).SingleOrDefaultAsync();
            document.Transactions = new List<TransactionVM>();

            document.Transactions = await joinQuery.ToListAsync();

            return document;

           // return await documentQuery.SingleOrDefaultAsync();
        }
        public async Task<Document> GetFirstDocument(int organId)
        {
            var documentQuery =_documents.Where(x => x.OrganId == organId && x.IsFirsDocument == true).Include(x => x.Transactions).Include(x => x.Transactions.Select(x2 => x2.Account));
            return await documentQuery.SingleOrDefaultAsync();
        }
        public async Task<OpeningBalanceStat> OpeningBalanceStatAsync(Document document, int organId)
        {
            var result = new OpeningBalanceStat();

            result.docDate = document.DisplayDate;

            var accuntQuery = _accounts.AsQueryable();
            var transactionQuery = _transactions.Where(x => x.DocumentId == document.ID);

            var cashParentQuery = _accounts.Where(x => x.ComplteCoding == "1101" && x.OrganId == organId).AsQueryable();
        

            var cashJoinQuery = from cashParent in cashParentQuery
                                join account in accuntQuery
                                on cashParent.ID equals account.ParentId
                                join transaction in transactionQuery
                                on account.ID equals transaction.AccountId
                                select transaction;

            if (await cashJoinQuery.AnyAsync())
                result.cash = await cashJoinQuery.SumAsync(x => x.Amount);

            var bankParentQuery = _accounts.Where(x => x.ComplteCoding == "1103" && x.OrganId == organId).AsQueryable();
   
            
            var bankJoinQuery = from bankParent in bankParentQuery
                                join account in accuntQuery
                                on bankParent.ID equals account.ParentId
                                join transaction in transactionQuery
                                on account.ID equals transaction.AccountId
                                select transaction;

            if(await bankJoinQuery.AnyAsync())
                result.bank = await bankJoinQuery.SumAsync(x => x.Amount);

            var itemParentQuery = _accounts.Where(x => x.ComplteCoding == "1108" && x.OrganId == organId).AsQueryable();


            var itemJoinQuery = from itemParent in itemParentQuery
                                join account in accuntQuery
                                on itemParent.ID equals account.ParentId
                                join transaction in transactionQuery
                                on account.ID equals transaction.AccountId
                                select transaction;

            if (await itemJoinQuery.AnyAsync())
                result.inventory = await itemJoinQuery.SumAsync(x => x.Amount);


            var debtorParentQuery = _accounts.Where(x => x.ComplteCoding == "1104" && x.OrganId == organId).AsQueryable();


            var debtorsJoinQuery = from debtorParent in debtorParentQuery
                                   join account in accuntQuery
                                on debtorParent.ID equals account.ParentId
                                join transaction in transactionQuery
                                on account.ID equals transaction.AccountId
                                select transaction;

            if (await debtorsJoinQuery.AnyAsync())
                result.debtors = await debtorsJoinQuery.SumAsync(x => x.Amount);

            var receivableParentQuery = _accounts.Where(x => x.ComplteCoding == "1105" && x.OrganId == organId).AsQueryable();


            var receivableJoinQuery = from receivableParent in receivableParentQuery
                                   join account in accuntQuery
                                on receivableParent.ID equals account.ParentId
                                   join transaction in transactionQuery
                                   on account.ID equals transaction.AccountId
                                   select transaction;

            if (await receivableJoinQuery.AnyAsync())
                result.receivables = await receivableJoinQuery.SumAsync(x => x.Amount);


            var inProgressParentQuery = _accounts.Where(x => x.ComplteCoding == "1106" && x.OrganId == organId).AsQueryable();


            var inProgressJoinQuery = from inProgressParent in inProgressParentQuery
                                      join account in accuntQuery
                                   on inProgressParent.ID equals account.ParentId
                                      join transaction in transactionQuery
                                      on account.ID equals transaction.AccountId
                                      select transaction;

            if (await inProgressJoinQuery.AnyAsync())
                result.inProgress = await inProgressJoinQuery.SumAsync(x => x.Amount);

            var creditorParentQuery = _accounts.Where(x => x.ComplteCoding == "2101" && x.OrganId == organId).AsQueryable();


            var creditorJoinQuery = from creditorParent in creditorParentQuery
                                      join account in accuntQuery
                                   on creditorParent.ID equals account.ParentId
                                      join transaction in transactionQuery
                                      on account.ID equals transaction.AccountId
                                      select transaction;

            if (await creditorJoinQuery.AnyAsync())
                result.creditors = await creditorJoinQuery.SumAsync(x => x.Amount);

            var payablesParentQuery = _accounts.Where(x => x.ComplteCoding == "2102" && x.OrganId == organId).AsQueryable();


            var payablesJoinQuery = from payablesParent in payablesParentQuery
                                    join account in accuntQuery
                                 on payablesParent.ID equals account.ParentId
                                    join transaction in transactionQuery
                                    on account.ID equals transaction.AccountId
                                    select transaction;

            if (await payablesJoinQuery.AnyAsync())
                result.payables = await payablesJoinQuery.SumAsync(x => x.Amount);


            List<string> ids = new List<string>() { "1102", "1107", "1109", "1110", "1111" };
            var parentAccountsQuery = _accounts.AsQueryable().Where(x => ids.Contains(x.ComplteCoding)).ToList();

            var accountsQuery = _accounts.AsQueryable().Where(x => ids.Contains(x.ComplteCoding)).ToList();
            var allAccountQuery = _accounts.AsQueryable();


            List<int> childcashIds = (from account1 in accountsQuery
                                      join allAccount in allAccountQuery
                                      on account1.ID equals allAccount.ParentId
                                      select allAccount.ID).ToList();



            List<int> childcashIds2 = (from parentAccounts in parentAccountsQuery
                                       select parentAccounts.ID).ToList();

            var selected = await transactionQuery.Where(a => childcashIds.Contains(a.AccountId) || childcashIds2.Contains(a.AccountId)).ToListAsync();

            if (selected.Any())
                result.otherAssets = selected.Sum(x => x.Amount);

            List<string> otherLiabilitiesMoenIds = new List<string>() { "2103", "2104", "2105", "2106", "2201" };
            var otherLiabilitiesMoenQuery = _accounts.AsQueryable().Where(x => otherLiabilitiesMoenIds.Contains(x.ComplteCoding)).ToList();

            var otherLiabilitiesMoenIdsAccountsQuery = _accounts.AsQueryable().Where(x => otherLiabilitiesMoenIds.Contains(x.ComplteCoding)).ToList();
            


            List<int> childcashIds3 = (from otherLiabilitiesMoenIdsAccounts in otherLiabilitiesMoenIdsAccountsQuery
                                      join allAccount in allAccountQuery
                                      on otherLiabilitiesMoenIdsAccounts.ID equals allAccount.ParentId
                                      select allAccount.ID).ToList();



            List<int> childcashIds4 = (from otherLiabilitiesMoen in otherLiabilitiesMoenQuery
                                       select otherLiabilitiesMoen.ID).ToList();

            var selected2 = await transactionQuery.Where(a => childcashIds3.Contains(a.AccountId) || childcashIds4.Contains(a.AccountId)).ToListAsync();

            if (selected2.Any())
                result.otherLiabilities = selected2.Sum(x => x.Amount);


            var withdrawalsParentQuery = _accounts.Where(x => x.ComplteCoding == "3101" && x.OrganId == organId).AsQueryable();


            var withdrawalsJoinQuery = from withdrawalsParent in withdrawalsParentQuery
                                       join account in accuntQuery
                                   on withdrawalsParent.ID equals account.ParentId
                                      join transaction in transactionQuery
                                      on account.ID equals transaction.AccountId
                                      select transaction;

            if (await withdrawalsJoinQuery.AnyAsync())
            {
                var debitList = await withdrawalsJoinQuery.Where( x => x.IsDebit == true).ToListAsync();
                var creditList = await withdrawalsJoinQuery.Where(x => x.IsCredit == true).ToListAsync();

                var debit =  debitList.Sum(x => x.Amount);
                var credit = creditList.Sum(x => x.Amount);

                result.withdrawals = credit - debit;
            }
               

            return result;
        }

        public async Task<decimal> CreateDocumentOpeningBalanceItem(Document document, List<ItemInfo> items, string docDate, int organId)
        {
            List<int> Ids = items.Select(x => x.Id).ToList();
            var itemsQuery = _items.AsQueryable().Where(a => Ids.Contains(a.ID));
            var parenAccountQuery = _accounts.Where(x => x.OrganId == organId && x.ComplteCoding == "1108").AsQueryable();
            var itemAccountQuery = _accounts.Where(x => x.OrganId == organId).AsQueryable();
           

            var oldTransactionsQuery = _transactions.Where( x => x.DocumentId == document.ID).AsQueryable();

            var joinOldList = await( from parenAccount in parenAccountQuery
                           join itemAccount in itemAccountQuery
                           on parenAccount.ID equals itemAccount.ParentId
                           join oldTransaction in oldTransactionsQuery
                           on itemAccount.ID equals oldTransaction.AccountId
                           select new
                           {
                               TransactionId = oldTransaction.ID,
                               itemAccount.Coding,
                               oldTransaction.Stock
                           }
                           ).ToListAsync();

            foreach (var old in joinOldList)
            {
             
                var relTransactions = await _transactions.Where(x => x.RefTrans == old.TransactionId).ToListAsync();

                foreach (var item in relTransactions)
                {
                    _uow.Entry<Transaction>(item).State = EntityState.Deleted;
                    //await _uow.SaveAllChangesAsync();
                }

                var oldTransaction = _transactions.Find(old.TransactionId);
                _uow.Entry<Transaction>(oldTransaction).State = EntityState.Deleted;

                var p = await _items.Where(x => x.Code == old.Coding && x.OrganIdItem == organId).SingleOrDefaultAsync();
                p.Stock -= old.Stock;
                _uow.Entry<Item>(p).State = EntityState.Modified;
            }

            var joinList = await (from parenAccount in parenAccountQuery
                            join itemAccount in itemAccountQuery
                            on parenAccount.ID equals itemAccount.ParentId
                            join item in itemsQuery
                            on itemAccount.Coding equals item.Code
                       //     join inputItem in inputItemQuery
                         //   on item.ID equals inputItem.Id
                            select new
                            {
                                AccountId = itemAccount.ID,
                                ID = item.ID
                              //  Amount = inputItem.totalAmount,
                              //  Stock = inputItem.stock,
                              //  UnitPrice = inputItem.totalAmount / inputItem.stock
                            }).ToListAsync();



            foreach (var item in joinList)
            {
                var temp = items.Where(x => x.Id == item.ID).SingleOrDefault();

                Transaction cashTransaction = new Transaction()
                {
                    //AccDocument = document,
                    AccountId = item.AccountId,
                    Amount = temp.totalAmount,
                    Date = DateTime.Now,
                    Debit = temp.totalAmount,
                    Credit = 0,
                    Description = "سند افتتاحیه",
                    DocumentId = document.ID,
                    //ID = transaction.Id,
                    IsDebit = true,
                    IsCredit = false,
                    DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                    Stock = temp.stock,
                    UnitPrice = temp.totalAmount/temp.stock// temp.UnitPrice


                };

                document.Transactions.Add(cashTransaction);
                if (document.ID > 0)
                    await Update(document);
                else
                    await Insert(document, organId);
                await _uow.SaveAllChangesAsync();


                var p = await _items.Where(x => x.ID == item.ID).SingleOrDefaultAsync();
                p.Stock += temp.stock;
                _uow.Entry<Item>(p).State = EntityState.Modified;


                var parentAccount = await _accounts.Where(x => x.OrganId == organId && x.ComplteCoding == "3101" && x.Level == DomainClasses.ZhivarEnums.AccountType.Moen).SingleOrDefaultAsync();
                var shareholdersQuery = _shareholders.Where(x => x.OrganId == organId && x.IsActive == true);
                var contactQuery = _contacts.AsQueryable();
                var accountsQuery = _accounts.Where(x => x.OrganId == organId && x.ParentId == parentAccount.ID).AsQueryable();

                var joinQuery = from shareholder in shareholdersQuery
                                join contact in contactQuery
                                on shareholder.ContactId equals contact.ID
                                join account in accountsQuery
                                on contact.Code equals account.Coding
                                select new
                                {
                                    AccounttId = account.ID,
                                    Amount = (temp.totalAmount * shareholder.SharePercent) / 100
                                };

                foreach (var shareholder in joinQuery)
                {
                    document.Transactions.Add(new Transaction()
                    {
                        //AccDocument = document,
                        AccountId = shareholder.AccounttId,
                        Amount = shareholder.Amount,
                        Date = DateTime.Now,
                        Debit = 0,
                        Credit = shareholder.Amount,
                        Description = "سند افتتاحیه",
                        DocumentId = document.ID,
                        IsDebit = false,
                        IsCredit = true,
                        DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                        RefTrans = cashTransaction.ID

                    });
                }
            }

            if (document.ID > 0)
                await Update(document);
            else
                await Insert(document, organId);

            return items.Sum(x => x.totalAmount);
        }

        public async Task<decimal> CreateDocumentOpeningBalanceAssets(Document document,List<TransactionVM> transactions, string docDate, int organId)
        {

            transactions = transactions.Where(x => x.DetailAccount != null || x.Account != null).ToList();

            Transaction transaction = new Transaction();

            foreach (var item in transactions ?? new List<TransactionVM>())
            {
                transaction = new Transaction();
                //transaction.AccDocument = document;

                if(item.DetailAccount != null)
                    transaction.AccountId = item.DetailAccount.Id;
                else
                {
                    transaction.Account = new DomainClasses.Accounting.Account()
                    {
                        Coding = item.Account.Coding,
                        ID = (int)item.Account.ID,
                        Level = item.Account.Level,
                        OrganId = item.Account.OrganId,
                        ComplteCoding = item.Account.ComplteCoding,
                        Name = item.Account.Name,
                        ParentId = item.Account.ParentId
                    };
       
                    transaction.AccountId = Convert.ToInt32(item.Account.ID);
                }
                    

                transaction.Amount = item.Amount;
                transaction.Date = DateTime.Now;
                transaction.Debit = item.Amount;
                transaction.Credit = 0;
                transaction.Description = "سند افتتاحیه";
                transaction.DocumentId = document.ID;
                transaction.ID = item.Id;
                transaction.IsDebit = true;
                transaction.IsCredit = false;
                transaction.DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now);
                transaction.Stock = transaction.Stock;
                transaction.UnitPrice = transaction.UnitPrice;

                //document.Transactions.Add(transaction);

                if (transaction.ID <= 0)
                {

                    _uow.Entry<Transaction>(transaction).State = EntityState.Added;
                    await _uow.SaveAllChangesAsync();

                }
                else
                {
                    var relTransactions = await _transactions.Where(x => x.RefTrans == transaction.ID).ToListAsync();

                    foreach (var relTransaction in relTransactions)
                    {
                        _uow.Entry<Transaction>(relTransaction).State = EntityState.Deleted;
                        await _uow.SaveAllChangesAsync();
                    }

                    if (transaction.Account != null)
                    {
                        var localAccount = _uow.Set<DomainClasses.Accounting.Account>()
                          .Local
                          .FirstOrDefault(f => f.ID == transaction.Account.ID);
                        if (localAccount != null)
                        {
                            _uow.Entry(localAccount).State = EntityState.Detached;
                        }
                    }

                    var local = _uow.Set<Transaction>()
                            .Local
                            .FirstOrDefault(f => f.ID == transaction.ID);
                    if (local != null)
                    {
                        _uow.Entry(local).State = EntityState.Detached;
                    }

                    _uow.Entry(transaction).State = EntityState.Modified;

                }

                var parentAccount = await _accounts.Where(x => x.OrganId == organId && x.ComplteCoding == "3101" && x.Level == DomainClasses.ZhivarEnums.AccountType.Moen).SingleOrDefaultAsync();
                var shareholdersQuery = _shareholders.Where(x => x.OrganId == organId && x.IsActive == true);
                var contactQuery = _contacts.AsQueryable();
                var accountsQuery = _accounts.Where(x => x.OrganId == organId && x.ParentId == parentAccount.ID).AsQueryable();

                var joinQuery = from shareholder in shareholdersQuery
                                join contact in contactQuery
                                on shareholder.ContactId equals contact.ID
                                join account in accountsQuery
                                on contact.Code equals account.Coding
                                select new
                                {
                                    AccounttId = account.ID,
                                    Amount = (transaction.Amount * shareholder.SharePercent) / 100
                                };

                foreach (var shareholder in joinQuery)
                {
                    var transaction2 = new  Transaction()
                    {
                        //AccDocument = document,
                        AccountId = shareholder.AccounttId,
                        Amount = shareholder.Amount,
                        Date = DateTime.Now,
                        Debit = 0,
                        Credit = shareholder.Amount,
                        Description = "سند افتتاحیه",
                        DocumentId = document.ID,
                        IsDebit = false,
                        IsCredit = true,
                        DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                        RefTrans = transaction.ID

                    };

                    _uow.Entry<Transaction>(transaction2).State = EntityState.Added;

                }
            }

            //if (document.ID > 0)
            //    await Update(document);
            //else
            //    await Insert(document, organId);

            return transactions.Sum(x => x.Amount);
        }

        public async Task<decimal> CreateDocumentOpeningBalanceCreditor(Document document, List<TransactionVM> transactions, string docDate, int organId)
        {
            transactions = transactions.Where(x => x.DetailAccount != null).ToList();

            foreach (var item in transactions ?? new List<TransactionVM>())
            {
                Transaction transaction = new Transaction()
                {
                    //AccDocument = document,
                    AccountId = item.DetailAccount.Id,
                    Amount = item.Amount,
                    Date = DateTime.Now,
                    Debit = 0,
                    Credit = item.Amount,
                    Description = "سند افتتاحیه",
                    DocumentId = document.ID,
                    ID = item.Id,
                    IsDebit = false,
                    IsCredit = true,
                    DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                    Stock = item.Stock,
                    UnitPrice = item.UnitPrice


                };

                //document.Transactions.Add(transaction);


                if (transaction.ID <= 0)
                {

                    _uow.Entry<Transaction>(transaction).State = EntityState.Added;
                    await _uow.SaveAllChangesAsync();

                }
                else
                {
                    var relTransactions = await _transactions.Where(x => x.RefTrans == transaction.ID).ToListAsync();

                    foreach (var relTransaction in relTransactions)
                    {
                        _uow.Entry<Transaction>(relTransaction).State = EntityState.Deleted;
                        await _uow.SaveAllChangesAsync();
                    }

                    if (transaction.Account != null)
                    {
                        var localAccount = _uow.Set<DomainClasses.Accounting.Account>()
                          .Local
                          .FirstOrDefault(f => f.ID == transaction.Account.ID);
                        if (localAccount != null)
                        {
                            _uow.Entry(localAccount).State = EntityState.Detached;
                        }
                    }

                    var local = _uow.Set<Transaction>()
                            .Local
                            .FirstOrDefault(f => f.ID == transaction.ID);
                    if (local != null)
                    {
                        _uow.Entry(local).State = EntityState.Detached;
                    }

                    _uow.Entry(transaction).State = EntityState.Modified;

                }


                var parentAccount = await _accounts.Where(x => x.OrganId == organId && x.ComplteCoding == "3101" && x.Level == DomainClasses.ZhivarEnums.AccountType.Moen).SingleOrDefaultAsync();
                var shareholdersQuery = _shareholders.Where(x => x.OrganId == organId && x.IsActive == true);
                var contactQuery = _contacts.AsQueryable();
                var accountsQuery = _accounts.Where(x => x.OrganId == organId && x.ParentId == parentAccount.ID).AsQueryable();

                var joinQuery = from shareholder in shareholdersQuery
                                join contact in contactQuery
                                on shareholder.ContactId equals contact.ID
                                join account in accountsQuery
                                on contact.Code equals account.Coding
                                select new
                                {
                                    AccounttId = account.ID,
                                    Amount = (transaction.Amount * shareholder.SharePercent) / 100
                                };

                foreach (var shareholder in joinQuery)
                {
                    var transaction2 = new Transaction()
                    {
                        //AccDocument = document,
                        AccountId = shareholder.AccounttId,
                        Amount = shareholder.Amount,
                        Date = DateTime.Now,
                        Debit = shareholder.Amount,
                        Credit = 0,
                        Description = "سند افتتاحیه",
                        DocumentId = document.ID,
                        IsDebit = true,
                        IsCredit = false,
                        DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                        RefTrans = transaction.ID

                    };

                    _uow.Entry<Transaction>(transaction2).State = EntityState.Added;
                }
            }

            //if (document.ID > 0)
            //    await Update(document);
            //else
            //    await Insert(document, organId);

            return transactions.Sum(x => x.Amount);
        
    }

        public async Task<decimal> CreateDocumentOpeningBalancePayables(Document document, List<TransactionVM> transactions, string docDate, int organId)
        {
            transactions = transactions.Where(x => x.DetailAccount != null).ToList();
            var finanYear = await _finanYears.Where(x => x.OrganId == organId && x.Closed == false).SingleOrDefaultAsync();

            foreach (var item in transactions ?? new List<TransactionVM>())
            {

                Transaction transaction = new Transaction()
                {
                    //AccDocument = document,
                    AccountId = item.DetailAccount.Id,
                    Amount = item.Amount,
                    Date = DateTime.Now,
                    Debit = 0,
                    Credit = item.Amount,
                    Description = "سند افتتاحیه",
                    DocumentId = document.ID,
                    ID = item.Id,
                    IsDebit = false,
                    IsCredit = true,
                    DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                    Stock = item.Stock,
                    UnitPrice = item.UnitPrice,
                    ChequeId = item.Cheque.ID,
                    Cheque = new Cheque()
                    {
                        ID = item.Cheque.ID,
                        Amount = item.Amount,
                        BankBranch = item.Cheque.BankBranch,
                        BankName = item.Cheque.BankName,
                        ChequeNumber = item.Cheque.ChequeNumber,
                        Contact = item.Cheque.Contact,
                        ContactId = item.Cheque.Contact.ID,
                        Date = Utilities.PersianDateUtils.ToDateTime(item.Cheque.DisplayDate),
                        DepositBank = item.Cheque.DepositBank,
                        DisplayDate = item.Cheque.DisplayDate,
                        OrganId = organId,
                        Status = item.Cheque.Status,
                        ReceiptDate = Utilities.PersianDateUtils.ToDateTime(item.Cheque.DisplayDate),
                        Type = DomainClasses.ZhivarEnums.ChequeType.Pardakhtani,

                    }


                };

                //document.Transactions.Add(transaction);


                if (transaction.ID <= 0)
                {
                    var bank = _banks.Where(x => x.OrganId == organId && x.Code == item.DetailAccount.Code).SingleOrDefaultAsync();
                    var chequeBank = new ChequeBank()
                    {
                        BankId = bank.Id,
                        ChequeId = item.Cheque.ID,
                        OrganId = organId

                    };
                    _uow.Entry<ChequeBank>(chequeBank).State = EntityState.Added;
                    _uow.Entry<Transaction>(transaction).State = EntityState.Added;
                    await _uow.SaveAllChangesAsync();

                }
                else
                {
                    var relTransactions = await _transactions.Where(x => x.RefTrans == transaction.ID).ToListAsync();

                    foreach (var relTransaction in relTransactions)
                    {
                        _uow.Entry<Transaction>(relTransaction).State = EntityState.Deleted;
                        await _uow.SaveAllChangesAsync();
                    }

                    if (transaction.Account != null)
                    {
                        var localAccount = _uow.Set<DomainClasses.Accounting.Account>()
                          .Local
                          .FirstOrDefault(f => f.ID == transaction.Account.ID);
                        if (localAccount != null)
                        {
                            _uow.Entry(localAccount).State = EntityState.Detached;
                        }
                    }

                    var local = _uow.Set<Transaction>()
                            .Local
                            .FirstOrDefault(f => f.ID == transaction.ID);
                    if (local != null)
                    {
                        _uow.Entry(local).State = EntityState.Detached;
                    }

                    _uow.Entry(transaction).State = EntityState.Modified;

                    var bank = await _banks.Where(x => x.OrganId == organId && x.Code == item.DetailAccount.Code).SingleOrDefaultAsync();
                    var chequeBank = await _chequeBanks.Where(x => x.OrganId == organId && x.ChequeId == item.Cheque.ID).SingleOrDefaultAsync();

                    chequeBank.BankId = bank.ID;

                    _uow.Entry<ChequeBank>(chequeBank).State = EntityState.Modified;
                }


                var parentAccount = await _accounts.Where(x => x.OrganId == organId && x.ComplteCoding == "3101" && x.Level == DomainClasses.ZhivarEnums.AccountType.Moen).SingleOrDefaultAsync();
                var shareholdersQuery = _shareholders.Where(x => x.OrganId == organId && x.IsActive == true);
                var contactQuery = _contacts.AsQueryable();
                var accountsQuery = _accounts.Where(x => x.OrganId == organId && x.ParentId == parentAccount.ID).AsQueryable();

                var joinQuery = from shareholder in shareholdersQuery
                                join contact in contactQuery
                                on shareholder.ContactId equals contact.ID
                                join account in accountsQuery
                                on contact.Code equals account.Coding
                                select new
                                {
                                    AccounttId = account.ID,
                                    Amount = (transaction.Amount * shareholder.SharePercent) / 100
                                };

                foreach (var shareholder in joinQuery)
                {
                    var transaction2 = new Transaction()
                    {
                        //AccDocument = document,
                        AccountId = shareholder.AccounttId,
                        Amount = shareholder.Amount,
                        Date = DateTime.Now,
                        Debit = shareholder.Amount,
                        Credit = 0,
                        Description = "سند افتتاحیه",
                        DocumentId = document.ID,
                        IsDebit = true,
                        IsCredit = false,
                        DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                        RefTrans = transaction.ID,
                        

                    };
                    _uow.Entry<Transaction>(transaction2).State = EntityState.Added;
                }
            }

            //if (document.ID > 0)
            //    await Update(document);
            //else
            //    await Insert(document, organId);

            return transactions.Sum(x => x.Amount);
        }

        public async Task<decimal> CreateDocumentOpeningBalanceOtherLiabilities(Document document, List<TransactionVM> transactions, string docDate, int organId)
        {
            transactions = transactions.Where(x => x.DetailAccount != null || x.Account != null).ToList();
            var finanYear = await _finanYears.Where(x => x.OrganId == organId && x.Closed == false).SingleOrDefaultAsync();

            foreach (var item in transactions ?? new List<TransactionVM>())
            {

                Transaction transaction = new Transaction();

                //transaction.AccDocument = document;

                if (item.DetailAccount != null)
                {
                    transaction.Account = new DomainClasses.Accounting.Account()
                    {
                        Coding = item.DetailAccount.Code,
                        //ComplteCoding = item.DetailAccount.
                        ID = item.DetailAccount.Id,
                        Name = item.DetailAccount.Name,
                        OrganId = organId,

                    };
                    transaction.AccountId = item.DetailAccount.Id;
                }
                else
                {
                    transaction.AccountId = (int)item.Account.ID;
                }
                transaction.Amount = item.Amount;
                transaction.Date = DateTime.Now;
                transaction.Debit = 0;
                transaction.Credit = item.Amount;
                transaction.Description = "سند افتتاحیه";
                transaction.DocumentId = document.ID;
                transaction.ID = item.Id;
                transaction.IsDebit = false;
                transaction.IsCredit = true;
                transaction.DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now);
                transaction.Stock = item.Stock;
                transaction.UnitPrice = item.UnitPrice;

                // document.Transactions.Add(transaction);


                if (transaction.ID <= 0)
                {

                    _uow.Entry<Transaction>(transaction).State = EntityState.Added;
                    await _uow.SaveAllChangesAsync();

                }
                else
                {
                    var relTransactions = await _transactions.Where(x => x.RefTrans == transaction.ID).ToListAsync();

                    foreach (var relTransaction in relTransactions)
                    {
                        _uow.Entry<Transaction>(relTransaction).State = EntityState.Deleted;
                        await _uow.SaveAllChangesAsync();
                    }

                    if (transaction.Account != null)
                    {
                        var localAccount = _uow.Set<DomainClasses.Accounting.Account>()
                          .Local
                          .FirstOrDefault(f => f.ID == transaction.Account.ID);
                        if (localAccount != null)
                        {
                            _uow.Entry(localAccount).State = EntityState.Detached;
                        }
                    }

                    var local = _uow.Set<Transaction>()
                            .Local
                            .FirstOrDefault(f => f.ID == transaction.ID);
                    if (local != null)
                    {
                        _uow.Entry(local).State = EntityState.Detached;
                    }

                    _uow.Entry(transaction).State = EntityState.Modified;

                }


                var parentAccount = await _accounts.Where(x => x.OrganId == organId && x.ComplteCoding == "3101" && x.Level == DomainClasses.ZhivarEnums.AccountType.Moen).SingleOrDefaultAsync();
                var shareholdersQuery = _shareholders.Where(x => x.OrganId == organId && x.IsActive == true);
                var contactQuery = _contacts.AsQueryable();
                var accountsQuery = _accounts.Where(x => x.OrganId == organId && x.ParentId == parentAccount.ID).AsQueryable();

                var joinQuery = from shareholder in shareholdersQuery
                                join contact in contactQuery
                                on shareholder.ContactId equals contact.ID
                                join account in accountsQuery
                                on contact.Code equals account.Coding
                                select new
                                {
                                    AccounttId = account.ID,
                                    Amount = (transaction.Amount * shareholder.SharePercent) / 100
                                };

                foreach (var shareholder in joinQuery)
                {
                    var transaction2 = new Transaction()
                    {
                        //AccDocument = document,
                        AccountId = shareholder.AccounttId,
                        Amount = shareholder.Amount,
                        Date = DateTime.Now,
                        Debit = shareholder.Amount,
                        Credit = 0,
                        Description = "سند افتتاحیه",
                        DocumentId = document.ID,
                        IsDebit = true,
                        IsCredit = false,
                        DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                        RefTrans = transaction.ID,


                    };

                    _uow.Entry<Transaction>(transaction2).State = EntityState.Added;
                }
            }

            //if (document.ID > 0)
            //    await Update(document);
            //else
            //    await Insert(document, organId);

            return transactions.Sum(x => x.Amount);
        }

        public Task<IList<Document>> GetByChequeIdAsync(int organId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveDocument(DocumentVM documentVM,int organId)
        {

            var transactions = documentVM.Transactions.Where(x => x.DetailAccount != null || x.Account != null).ToList();
            var finanYear = await _finanYears.Where(x => x.OrganId == organId && x.Closed == false).SingleOrDefaultAsync();

            var document = new Document() {
                
                Credit = documentVM.Credit,
                DateTime = Utilities.PersianDateUtils.ToDateTime(documentVM.DisplayDate),
                Debit = documentVM.Debit,
                Description = documentVM.Description,
                DisplayDate = documentVM.DisplayDate,
                FinanYear = finanYear,
                FinanYearId = finanYear.ID,
                IsFirsDocument = false,
                IsManual = documentVM.IsManual,
                Number = await createNumberDocumentAsync(organId),//documentVM.Number,
                Number2 = await createNumberDocumentAsync(organId), // documentVM.Number2,
                OrganId = organId,
                Status =  documentVM.Status,
                StatusString = documentVM.Status.ToString(),
                
                
            };
            document.Transactions = new List<Transaction>();
            var transaction = new Transaction();

            foreach (var item in transactions ?? new List<TransactionVM>())
            {
                transaction = new Transaction();
               // transaction.AccDocument = document;
                transaction.DocumentId = document.ID;
                if (item.DetailAccount != null)
                {
                    transaction.Account = new DomainClasses.Accounting.Account()
                    {
                        Coding = item.DetailAccount.Code,
                        //ComplteCoding = item.DetailAccount.
                        ID = item.DetailAccount.Id,
                        Name = item.DetailAccount.Name,
                        OrganId = organId,
                        
                    };
                    transaction.AccountId = item.DetailAccount.Id;
                }
                else
                {
                    transaction.AccountId = (int)item.Account.ID;
                }
                transaction.DisplayDate = document.DisplayDate;
                transaction.Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate);
                transaction.InvoiceId = item.InvoiceId;
                transaction.IsCredit = item.IsCredit;
                transaction.IsDebit = item.IsDebit;
                transaction.PaymentMethod = item.PaymentMethod;
                transaction.PaymentMethodString = item.PaymentMethodString;
                transaction.Reference = item.Reference;
                transaction.RefTrans = item.RefTrans;
                transaction.Remaining = item.Remaining;
                transaction.RemainingType = item.RemainingType;
                transaction.RowNumber = item.RowNumber;
                transaction.Stock = item.Stock;
                transaction.TransactionTypeString = item.TransactionTypeString;
                transaction.Type = item.Type;
                transaction.UnitPrice = item.UnitPrice;
                transaction.ID = item.Id;
                transaction.Amount = item.Amount;
                transaction.Cheque = item.Cheque;
                transaction.ChequeId = item.ChequeId;
                transaction.ContactId = item.ContactId;
                transaction.Credit = item.Credit;
                transaction.Debit = item.Debit;
                transaction.Description = item.Description;
                
                document.Transactions.Add(transaction);
            }

            var credit = document.Transactions.Sum( x => x.Credit);
            var debit = document.Transactions.Sum(x => x.Debit);

            document.Credit = credit;
            document.Debit = debit;

            if (document.ID > 0)
                await Update(document);
            else
                await Insert(document, organId);

            return true;

        }



        public async Task<List<Document>> GetChequeRelatedDocuments(int chequeId)
        {

            var chequeQuery = _cheques.Where(x => x.ID == chequeId);
            var documentQuery = _documents;
            var payReceviesQuery = _payRecevies;
            var detailPayRecevieQuery = _detailPayRecevie.Where(x => x.Type == ZhivarEnums.DetailPayReceiveType.Cheque);

            var joinQuery = from payRecevies in payReceviesQuery
                            join detailPayRecevie in detailPayRecevieQuery
                            on payRecevies.ID equals detailPayRecevie.PayRecevieId
                            join cheque in chequeQuery
                            on detailPayRecevie.ChequeId equals cheque.ID
                            join document in documentQuery
                            on payRecevies.DocumentId equals document.ID
                            select document;

            return await joinQuery.OrderByDescending(x => x.ID).ToListAsync();

        }

        public async Task<List<Document>> GetDocumentsByDocumentIDs(List<int> lstDocId)
        {
            return await _documents.AsQueryable().Where(x => lstDocId.Contains(x.ID)).Include(x => x.Transactions).Include(x => x.Transactions.Select(y => y.Account)).OrderByDescending(x => x.ID).ToListAsync();
        }
    }
}
