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
using Zhivar.ViewModel.BaseInfo;
using Zhivar.DomainClasses.Common;
using System.Threading;
using AutoMapper;
using Zhivar.DomainClasses;

namespace Zhivar.Business.Accounting
{
    public partial class DocumentRule : BusinessRuleBase<Document>
    {
        public DocumentRule()
            : base()
        {

        }

        public DocumentRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public DocumentRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }


        public async Task<bool> InsertAsync(Document document, int organId)
        {
            try
            {
 

                foreach (var transaction in document.Transactions ?? new List<Transaction>())
                {
                    transaction.ObjectState = Enums.ObjectState.Added;
                    transaction.DocumentId = document.ID;

                }


                document.OrganId = organId;
                var finanYear = await this.UnitOfWork.Repository<FinanYear>().Queryable().Where(x => x.OrganId == organId && x.Closed == false).SingleOrDefaultAsync2();

                document.FinanYearId = finanYear.ID;

                document.ObjectState = Enums.ObjectState.Added;
                this.InsertOrUpdateGraph(document);


                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        public bool Insert(Document document, int organId)
        {
            try
            {
                foreach (var transaction in document.Transactions ?? new List<Transaction>())
                {
                    transaction.ObjectState = Enums.ObjectState.Added;
                    transaction.DocumentId = document.ID;
                }


                document.OrganId = organId;
                var finanYear = this.UnitOfWork.Repository<FinanYear>().Queryable().Where(x => x.OrganId == organId && x.Closed == false).SingleOrDefault();

                document.FinanYearId = finanYear.ID;

                document.ObjectState = Enums.ObjectState.Added;
                this.InsertOrUpdateGraph(document);
             

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<IList<Document>> GetAllByOrganIdAsync(int organId)
        {
            var finanYear = await this.unitOfWork.RepositoryAsync<FinanYear>().Queryable().Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync2();
            var documentQuery = this.unitOfWork.RepositoryAsync<Document>().Queryable().Where(x => x.OrganId == organId && x.FinanYearId == finanYear.ID);
            var transactionQuery = this.unitOfWork.Repository<Transaction>().Queryable();
            var accountQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable();

             var test = await documentQuery.ToListAsync2();
            var test3 = await transactionQuery.ToListAsync2();
            var test2 =await(from transaction in transactionQuery
                        join account in accountQuery
                        on transaction.AccountId equals account.ID
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
                            AccountId = account.ID,
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
                        }).ToListAsync2();


            var joinQuery = from document in documentQuery
                            join transaction2 in (from transaction in transactionQuery
                                                  join account in accountQuery
                                                  on transaction.AccountId equals account.ID
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
                                                      AccountId = account.ID,
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
                                                    //  Transactions = transaction.Transactions,
                                                      TransactionTypeString = transaction.TransactionTypeString,
                                                      Type = transaction.Type,
                                                      UnitPrice = transaction.UnitPrice
                                                  }) on document.ID equals transaction2.DocumentId into transactionGroup
                            select new DocumentVM
                            {

                                Transactions = transactionGroup.ToList(),
                                Credit = document.Credit,
                                DateTime = document.DateTime,
                                Debit = document.Debit,
                                Description = document.Description,
                                DisplayDate = document.DisplayDate,
                                FinanYear = new FinanYearVM
                                {
                                    Closed = document.FinanYear.Closed,
                                    DisplayEndDate = document.FinanYear.DisplayEndDate,
                                    DisplayStartDate = document.FinanYear.DisplayStartDate,
                                    FirstYear = document.FinanYear.FirstYear,
                                    ID = document.FinanYear.ID,
                                    Name = document.FinanYear.Name,
                                    OrganId = document.FinanYear.OrganId,


                                },
                                FinanYearId = document.FinanYearId,
                                ID = document.ID,
                                IsFirsDocument = document.IsFirsDocument,
                                IsManual = document.IsManual,
                                Number = document.Number,
                                Number2 = document.Number2,
                                OrganId = document.OrganId,
                                Status = document.Status,
                                StatusString = document.StatusString,
                                Type = document.Type
                            };
      
            var resualt = await joinQuery.OrderByDescending(x => x.ID).ToListAsync2();

            return Utilities.TranslateHelper.TranslateEntityVMToEntityListDocument(resualt);
        }

        public IList<Document> GetAllByOrganId(int organId)
        {
            var finanYear = this.unitOfWork.RepositoryAsync<FinanYear>().Queryable().Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefault();
            var documentQuery = this.unitOfWork.RepositoryAsync<Document>().Queryable().Where(x => x.OrganId == organId && x.FinanYearId == finanYear.ID);
            var transactionQuery = this.unitOfWork.Repository<Transaction>().Queryable();
            var accountQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable();

            var test =  documentQuery.ToList();
            var test3 = transactionQuery.ToList();
            var test2 =  (from transaction in transactionQuery
                               join account in accountQuery
                               on transaction.AccountId equals account.ID
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
                                   AccountId = account.ID,
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
                               }).ToList();


            var joinQuery = from document in documentQuery
                            join transaction2 in (from transaction in transactionQuery
                                                  join account in accountQuery
                                                  on transaction.AccountId equals account.ID
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
                                                      AccountId = account.ID,
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
                                                      //  Transactions = transaction.Transactions,
                                                      TransactionTypeString = transaction.TransactionTypeString,
                                                      Type = transaction.Type,
                                                      UnitPrice = transaction.UnitPrice
                                                  }) on document.ID equals transaction2.DocumentId into transactionGroup
                            select new DocumentVM
                            {

                                Transactions = transactionGroup.ToList(),
                                Credit = document.Credit,
                                DateTime = document.DateTime,
                                Debit = document.Debit,
                                Description = document.Description,
                                DisplayDate = document.DisplayDate,
                                FinanYear = new FinanYearVM
                                {
                                    Closed = document.FinanYear.Closed,
                                    DisplayEndDate = document.FinanYear.DisplayEndDate,
                                    DisplayStartDate = document.FinanYear.DisplayStartDate,
                                    FirstYear = document.FinanYear.FirstYear,
                                    ID = document.FinanYear.ID,
                                    Name = document.FinanYear.Name,
                                    OrganId = document.FinanYear.OrganId,


                                },
                                FinanYearId = document.FinanYearId,
                                ID = document.ID,
                                IsFirsDocument = document.IsFirsDocument,
                                IsManual = document.IsManual,
                                Number = document.Number,
                                Number2 = document.Number2,
                                OrganId = document.OrganId,
                                Status = document.Status,
                                StatusString = document.StatusString,
                                Type = document.Type
                            };

            var resualt = joinQuery.OrderByDescending(x => x.ID).ToList();

            return Utilities.TranslateHelper.TranslateEntityVMToEntityListDocument(resualt);
        }
        //public async Task<bool> Update(Document document)
        //{
        //    try
        //    {
        //        if (document.FinanYearId <= 0)
        //        {
        //            var finanYear = await this.unitOfWork.Repository<FinanYear>().Queryable().Where(x => x.OrganId == document.OrganId && x.Closed == false).SingleOrDefaultAsync();
        //            document.FinanYearId = finanYear.ID;
        //        }
        //        foreach (var transaction in document.Transactions.ToList() ?? new List<Transaction>())
        //        {
        //            if (transaction.ID > 0)
        //            {

        //                var local = _uow.Set<Transaction>()
        //                     .Local
        //                     .FirstOrDefault(f => f.ID == transaction.ID);
        //                if (local != null)
        //                {
        //                    _uow.Entry(local).State = EntityState.Detached;
        //                }

        //                _uow.Entry(transaction).State = EntityState.Modified;
        //            }
        //            else
        //            {

        //                _uow.Entry(transaction).State = EntityState.Added;
        //            }

        //            if (transaction.Cheque != null)
        //            {
        //                if (transaction.Cheque.Contact != null)
        //                {
        //                    var local = _uow.Set<Contact>()
        //                    .Local
        //                    .FirstOrDefault(f => f.ID == transaction.Cheque.Contact.ID);
        //                    if (local != null)
        //                    {
        //                        _uow.Entry(local).State = EntityState.Unchanged;
        //                    }

        //                    //_uow.Entry(transaction.Cheque.Contact).State = EntityState.;
        //                }

        //                _uow.Entry(transaction.Cheque).State = EntityState.Modified;
        //            }
        //        }

        //        _uow.Entry(document).State = EntityState.Modified;

        //        return true;

        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //public async Task<Document> GetByIdAsync(int id)
        //{

        //    return await this.unitOfWork.RepositoryAsync<Document>().Queryable().Where(x => x.ID == id).Include(x => x.Transactions).Include(x => x.Transactions.Select(y => y.Account)).FirstOrDefaultAsync();
        //}

        public async Task<DocumentVM> GetDocumentByIdAsync(int id)
        {
            var documentsQuery = this.unitOfWork.RepositoryAsync<Document>().Queryable().Where(x => x.ID == id);
            var transactionsQuery = this.unitOfWork.RepositoryAsync<Transaction>().Queryable().Where(x => x.DocumentId == id);
            var accountQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable();

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

                                  }).SingleOrDefaultAsync2();
            document.Transactions = new List<TransactionVM>();

            document.Transactions = await joinQuery.ToListAsync2();

            return document;
        }
        public async Task<int> createNumberDocumentAsync(int organId)
        {
            var count = await this.unitOfWork.RepositoryAsync<Document>().Queryable().Where(x => x.OrganId == organId).CountAsync2();

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

                TransactionRule transactionRule = new TransactionRule();

                if (transaction.ID <= 0)
                {
               
                    transactionRule.Insert(transaction);
                    //_uow.Entry<Transaction>(transaction).State = EntityState.Added;
                    //await _uow.SaveAllChangesAsync();
                    await transactionRule.SaveChangesAsync();
                }
                else
                {
                    var relTransactions = await this.unitOfWork.Repository<Transaction>().Queryable().Where(x => x.RefTrans == transaction.ID).ToListAsync2();

                    foreach (var relTransaction in relTransactions)
                    {
                        transactionRule.Delete(relTransaction);
                        await transactionRule.SaveChangesAsync();
                    }

                    //if (transaction.Account != null)
                    //{
                    //    var localAccount = _uow.Set<DomainClasses.Accounting.Account>()
                    //      .Local
                    //      .FirstOrDefault(f => f.ID == transaction.Account.ID);
                    //    if (localAccount != null)
                    //    {
                    //        _uow.Entry(localAccount).State = EntityState.Detached;
                    //    }
                    //}

                    //var local = _uow.Set<Transaction>()
                    //        .Local
                    //        .FirstOrDefault(f => f.ID == transaction.ID);
                    //if (local != null)
                    //{
                    //    _uow.Entry(local).State = EntityState.Detached;
                    //}

                    transactionRule.Update(transaction);
                    //_uow.Entry(transaction).State = EntityState.Modified;

                    await transactionRule.SaveChangesAsync();

                }


                var parentAccount = await this.unitOfWork.Repository<Account>().Queryable().Where(x => x.OrganId == organId && x.ComplteCoding == "3101" && x.Level == DomainClasses.ZhivarEnums.AccountType.Moen).SingleOrDefaultAsync2();
                var shareholdersQuery = this.unitOfWork.Repository<Shareholder>().Queryable().Where(x => x.OrganId == organId && x.IsActive == true);
                var contactQuery = this.unitOfWork.Repository<Contact>().Queryable();
                var accountsQuery = this.unitOfWork.Repository<Account>().Queryable().Where(x => x.OrganId == organId && x.ParentId == parentAccount.ID).AsQueryable();

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

                    transactionRule.Insert(transaction2);
                    //_uow.Entry<Transaction>(transaction2).State = EntityState.Added;
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
                var findAccount = await this.unitOfWork.Repository<Account>().Queryable().Where(x => x.ID == item.DetailAccount.Id).SingleOrDefaultAsync2();

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

                //TransactionRule transactionRule = new TransactionRule();

                if (transaction.ID <= 0)
                {
                    this.unitOfWork.Repository<Transaction>().Insert(transaction);
                    await this.unitOfWork.SaveChangesAsync();
                    //_uow.Entry<Transaction>(transaction).State = EntityState.Added;
                    //await _uow.SaveAllChangesAsync();

                }
                else
                {
                    var relTransactions = await this.unitOfWork.Repository<Transaction>().Queryable().Where(x => x.RefTrans == transaction.ID).ToListAsync2();

                    foreach (var relTransaction in relTransactions)
                    {
                        await this.unitOfWork.RepositoryAsync<Transaction>().DeleteAsync(relTransaction);
                        await this.unitOfWork.SaveChangesAsync();
             
                    }

                    //if (transaction.Account != null)
                    //{
                    //    var localAccount = _uow.Set<DomainClasses.Accounting.Account>()
                    //      .Local
                    //      .FirstOrDefault(f => f.ID == transaction.Account.ID);
                    //    if (localAccount != null)
                    //    {
                    //        _uow.Entry(localAccount).State = EntityState.Detached;
                    //    }
                    //}

                    //var local = _uow.Set<Transaction>()
                    //        .Local
                    //        .FirstOrDefault(f => f.ID == transaction.ID);
                    //if (local != null)
                    //{
                    //    _uow.Entry(local).State = EntityState.Detached;
                    //}
                    this.unitOfWork.Repository<Transaction>().Update(transaction);
                    await this.unitOfWork.SaveChangesAsync();
                    //_uow.Entry(transaction).State = EntityState.Modified;

                }


                var parentAccount = await this.unitOfWork.Repository<Account>().Queryable().Where(x => x.OrganId == organId && x.ComplteCoding == "3101" && x.Level == DomainClasses.ZhivarEnums.AccountType.Moen).SingleOrDefaultAsync2();
                var shareholdersQuery = this.unitOfWork.Repository<Shareholder>().Queryable().Where(x => x.OrganId == organId && x.IsActive == true);
                var contactQuery = this.unitOfWork.Repository<Contact>().Queryable().AsQueryable();
                var accountsQuery = this.unitOfWork.Repository<Account>().Queryable().Where(x => x.OrganId == organId && x.ParentId == parentAccount.ID).AsQueryable();

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

                    this.unitOfWork.Repository<Transaction>().Insert(transaction2);
                    await this.unitOfWork.SaveChangesAsync();
                    //_uow.Entry<Transaction>(transaction2).State = EntityState.Added;
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

            var documentsQuery = this.unitOfWork.RepositoryAsync<Document>().Queryable().Where(x => x.OrganId == organId && x.IsFirsDocument == true);
            var transactionsQuery = this.unitOfWork.RepositoryAsync<Transaction>().Queryable();
            var accountQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable();

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

                                  }).SingleOrDefaultAsync2();
            document.Transactions = new List<TransactionVM>();

            document.Transactions = await joinQuery.ToListAsync2();

            return document;

            // return await documentQuery.SingleOrDefaultAsync();
        }
        public async Task<Document> GetFirstDocument(int organId)
        {
            var documentQuery = this.unitOfWork.RepositoryAsync<Document>().Queryable(false,true,new List<System.Linq.Expressions.Expression<Func<Document, object>>>()).Where(x => x.OrganId == organId && x.IsFirsDocument == true);
            var transactionQuery = this.unitOfWork.RepositoryAsync<Transaction>().Queryable();
            var accountQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable();

            var joinQuery = from document in documentQuery
                            join transaction2 in (from transaction in transactionQuery
                                                  join account in accountQuery
                                                  on transaction.AccountId equals account.ID
                                                  select new TransactionVM
                                                  {
                                                      Date = transaction.Date,
                                                      //AccDocument = new DocumentVM
                                                      //{
                                                      //    Credit = transaction.AccDocument.Credit,
                                                      //    DateTime = transaction.AccDocument.DateTime,
                                                      //    Debit = transaction.AccDocument.Credit,
                                                      //    Description = transaction.AccDocument.Description,
                                                      //    DisplayDate = transaction.AccDocument.DisplayDate,
                                                      //    Id = transaction.AccDocument.ID,
                                                      //    IsFirsDocument = transaction.AccDocument.IsFirsDocument,
                                                      //    Number = transaction.AccDocument.Number,
                                                      //    Number2 = transaction.AccDocument.Number2,
                                                      //    Status = transaction.AccDocument.Status,
                                                      //    StatusString = transaction.AccDocument.StatusString,
                                                      //    //Transactions = transaction.AccDocument.Transactions.Select(s => new TransactionVM {
                                                              
                                                      //    //}).ToList(),
                                                      //    IsManual = transaction.AccDocument.IsManual,
                                                      //},
                                                      Account = new AccountVM {
                                                          Coding = transaction.Account.Coding,
                                                          ComplteCoding = transaction.Account.ComplteCoding,
                                                          ID = transaction.Account.ID,
                                                          Level = transaction.Account.Level,
                                                          Name = transaction.Account.Name,
                                                          OrganId = transaction.Account.OrganId,
                                                          ParentId = transaction.Account.ParentId
                                                      },
                                                      AccountId = account.ID,
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
                                                     // Transactions = transaction.Transactions,
                                                      TransactionTypeString = transaction.TransactionTypeString,
                                                      Type = transaction.Type,
                                                      UnitPrice = transaction.UnitPrice
                                                  }) on document.ID equals transaction2.DocumentId into transactionGroup
                            select new DocumentVM
                            {
                                
                                 Transactions = transactionGroup.ToList(),
                                 Credit = document.Credit,
                                 DateTime = document.DateTime,
                                 Debit = document.Debit,
                                 Description = document.Description,
                                 DisplayDate = document.DisplayDate,
                                 FinanYear = new FinanYearVM
                                 {
                                     Closed = document.FinanYear.Closed,
                                     DisplayEndDate = document.FinanYear.DisplayEndDate,
                                     DisplayStartDate = document.FinanYear.DisplayStartDate,
                                     FirstYear = document.FinanYear.FirstYear,
                                     ID = document.FinanYear.ID,
                                     Name = document.FinanYear.Name,
                                     OrganId = document.FinanYear.OrganId,
                                    

                                 }, 
                                 FinanYearId = document.FinanYearId,
                                 ID = document.ID,
                                 IsFirsDocument = document.IsFirsDocument,
                                 IsManual = document.IsManual,
                                 Number = document.Number,
                                 Number2 = document.Number2,
                                 OrganId = document.OrganId,
                                 Status = document.Status,
                                 StatusString = document.StatusString,
                                 Type = document.Type
                            };
            var result = await joinQuery.SingleOrDefaultAsync2();

            return Utilities.TranslateHelper.TranslateEntityVMToEntityDocument(result);
        }
        public async Task<OpeningBalanceStat> OpeningBalanceStatAsync(Document document, int organId)
        {
            var result = new OpeningBalanceStat();

            result.docDate = document.DisplayDate;

            var accuntQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable();
            var transactionQuery = this.unitOfWork.RepositoryAsync<Transaction>().Queryable().Where(x => x.DocumentId == document.ID);

            var cashParentQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.ComplteCoding == "1101" && x.OrganId == organId).AsQueryable();


            var cashJoinQuery = from cashParent in cashParentQuery
                                join account in accuntQuery
                                on cashParent.ID equals account.ParentId
                                join transaction in transactionQuery
                                on account.ID equals transaction.AccountId
                                select transaction;

            if (await cashJoinQuery.AnyAsync2())
                result.cash = cashJoinQuery.Sum(x => x.Amount);

            var bankParentQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.ComplteCoding == "1103" && x.OrganId == organId).AsQueryable();


            var bankJoinQuery = from bankParent in bankParentQuery
                                join account in accuntQuery
                                on bankParent.ID equals account.ParentId
                                join transaction in transactionQuery
                                on account.ID equals transaction.AccountId
                                select transaction;

            if (await bankJoinQuery.AnyAsync2())
                result.bank =  bankJoinQuery.Sum(x => x.Amount);

            var itemParentQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.ComplteCoding == "1108" && x.OrganId == organId).AsQueryable();


            var itemJoinQuery = from itemParent in itemParentQuery
                                join account in accuntQuery
                                on itemParent.ID equals account.ParentId
                                join transaction in transactionQuery
                                on account.ID equals transaction.AccountId
                                select transaction;

            if (await itemJoinQuery.AnyAsync2())
                result.inventory =  itemJoinQuery.Sum(x => x.Amount);


            var debtorParentQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.ComplteCoding == "1104" && x.OrganId == organId).AsQueryable();


            var debtorsJoinQuery = from debtorParent in debtorParentQuery
                                   join account in accuntQuery
                                on debtorParent.ID equals account.ParentId
                                   join transaction in transactionQuery
                                   on account.ID equals transaction.AccountId
                                   select transaction;

            if (await debtorsJoinQuery.AnyAsync2())
                result.debtors = debtorsJoinQuery.Sum(x => x.Amount);

            var receivableParentQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.ComplteCoding == "1105" && x.OrganId == organId).AsQueryable();


            var receivableJoinQuery = from receivableParent in receivableParentQuery
                                      join account in accuntQuery
                                   on receivableParent.ID equals account.ParentId
                                      join transaction in transactionQuery
                                      on account.ID equals transaction.AccountId
                                      select transaction;

            if (await receivableJoinQuery.AnyAsync2())
                result.receivables = receivableJoinQuery.Sum(x => x.Amount);


            var inProgressParentQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.ComplteCoding == "1106" && x.OrganId == organId).AsQueryable();


            var inProgressJoinQuery = from inProgressParent in inProgressParentQuery
                                      join account in accuntQuery
                                   on inProgressParent.ID equals account.ParentId
                                      join transaction in transactionQuery
                                      on account.ID equals transaction.AccountId
                                      select transaction;

            if (await inProgressJoinQuery.AnyAsync2())
                result.inProgress = inProgressJoinQuery.Sum(x => x.Amount);

            var creditorParentQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.ComplteCoding == "2101" && x.OrganId == organId).AsQueryable();


            var creditorJoinQuery = from creditorParent in creditorParentQuery
                                    join account in accuntQuery
                                 on creditorParent.ID equals account.ParentId
                                    join transaction in transactionQuery
                                    on account.ID equals transaction.AccountId
                                    select transaction;

            if (await creditorJoinQuery.AnyAsync2())
                result.creditors = creditorJoinQuery.Sum(x => x.Amount);

            var payablesParentQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.ComplteCoding == "2102" && x.OrganId == organId).AsQueryable();


            var payablesJoinQuery = from payablesParent in payablesParentQuery
                                    join account in accuntQuery
                                 on payablesParent.ID equals account.ParentId
                                    join transaction in transactionQuery
                                    on account.ID equals transaction.AccountId
                                    select transaction;

            if (await payablesJoinQuery.AnyAsync2())
                result.payables = payablesJoinQuery.Sum(x => x.Amount);


            List<string> ids = new List<string>() { "1102", "1107", "1109", "1110", "1111" };
            var parentAccountsQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().AsQueryable().Where(x => ids.Contains(x.ComplteCoding)).ToList();

            var accountsQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().AsQueryable().Where(x => ids.Contains(x.ComplteCoding)).ToList();
            var allAccountQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable();


            List<int> childcashIds = (from account1 in accountsQuery
                                      join allAccount in allAccountQuery
                                      on account1.ID equals allAccount.ParentId
                                      select allAccount.ID).ToList();



            List<int> childcashIds2 = (from parentAccounts in parentAccountsQuery
                                       select parentAccounts.ID).ToList();

            var selected = await transactionQuery.Where(a => childcashIds.Contains(a.AccountId) || childcashIds2.Contains(a.AccountId)).ToListAsync2();

            if (selected.Any())
                result.otherAssets = selected.Sum(x => x.Amount);

            List<string> otherLiabilitiesMoenIds = new List<string>() { "2103", "2104", "2105", "2106", "2201" };
            var otherLiabilitiesMoenQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => otherLiabilitiesMoenIds.Contains(x.ComplteCoding)).ToList();

            var otherLiabilitiesMoenIdsAccountsQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => otherLiabilitiesMoenIds.Contains(x.ComplteCoding)).ToList();



            List<int> childcashIds3 = (from otherLiabilitiesMoenIdsAccounts in otherLiabilitiesMoenIdsAccountsQuery
                                       join allAccount in allAccountQuery
                                       on otherLiabilitiesMoenIdsAccounts.ID equals allAccount.ParentId
                                       select allAccount.ID).ToList();



            List<int> childcashIds4 = (from otherLiabilitiesMoen in otherLiabilitiesMoenQuery
                                       select otherLiabilitiesMoen.ID).ToList();

            var selected2 = await transactionQuery.Where(a => childcashIds3.Contains(a.AccountId) || childcashIds4.Contains(a.AccountId)).ToListAsync2();

            if (selected2.Any())
                result.otherLiabilities = selected2.Sum(x => x.Amount);


            var withdrawalsParentQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.ComplteCoding == "3101" && x.OrganId == organId).AsQueryable();


            var withdrawalsJoinQuery = from withdrawalsParent in withdrawalsParentQuery
                                       join account in accuntQuery
                                   on withdrawalsParent.ID equals account.ParentId
                                       join transaction in transactionQuery
                                       on account.ID equals transaction.AccountId
                                       select transaction;

            if (await withdrawalsJoinQuery.AnyAsync2())
            {
                var debitList = await withdrawalsJoinQuery.Where(x => x.IsDebit == true).ToListAsync2();
                var creditList = await withdrawalsJoinQuery.Where(x => x.IsCredit == true).ToListAsync2();

                var debit = debitList.Sum(x => x.Amount);
                var credit = creditList.Sum(x => x.Amount);

                result.withdrawals = credit - debit;
            }


            return result;
        }

        public async Task<decimal> CreateDocumentOpeningBalanceItem(Document document, List<ItemInfo> items, string docDate, int organId)
        {
            List<int> Ids = items.Select(x => x.Id).ToList();
            var itemsQuery = this.unitOfWork.RepositoryAsync<Item>().Queryable().Where(a => Ids.Contains(a.ID));
            var parenAccountQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId && x.ComplteCoding == "1108").AsQueryable();
            var itemAccountQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId).AsQueryable();


            var oldTransactionsQuery = this.unitOfWork.RepositoryAsync<Transaction>().Queryable().Where(x => x.DocumentId == document.ID).AsQueryable();

            var joinOldList = await (from parenAccount in parenAccountQuery
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
                           ).ToListAsync2();

            foreach (var old in joinOldList)
            {

                var relTransactions = await this.unitOfWork.RepositoryAsync<Transaction>().Queryable().Where(x => x.RefTrans == old.TransactionId).ToListAsync2();

                foreach (var item in relTransactions)
                {
                    await this.unitOfWork.RepositoryAsync<Transaction>().DeleteAsync(item);
                    //await _uow.SaveAllChangesAsync();
                }

                var oldTransaction = this.unitOfWork.RepositoryAsync<Transaction>().Find(old.TransactionId);
                await this.unitOfWork.RepositoryAsync<Transaction>().DeleteAsync(oldTransaction);

                var p = await this.unitOfWork.RepositoryAsync<Item>().Queryable().Where(x => x.Code == old.Coding && x.OrganIdItem == organId).SingleOrDefaultAsync2();
                p.Stock -= old.Stock;
                this.unitOfWork.RepositoryAsync<Item>().Update(p);
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
                                  }).ToListAsync2();



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
                    UnitPrice = temp.totalAmount / temp.stock// temp.UnitPrice


                };

                document.Transactions.Add(cashTransaction);
                if (document.ID > 0)
                    this.Update(document);
                else
                { 

                    await this.InsertAsync(document, organId);
                }
                await this.unitOfWork.SaveChangesAsync();


                var p = await this.unitOfWork.RepositoryAsync<Item>().Queryable().Where(x => x.ID == item.ID).SingleOrDefaultAsync2();
                p.Stock += temp.stock;
                this.unitOfWork.RepositoryAsync<Item>().Update(p);


                var parentAccount = await this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId && x.ComplteCoding == "3101" && x.Level == DomainClasses.ZhivarEnums.AccountType.Moen).SingleOrDefaultAsync2();
                var shareholdersQuery = this.unitOfWork.RepositoryAsync<Shareholder>().Queryable().Where(x => x.OrganId == organId && x.IsActive == true);
                var contactQuery = this.unitOfWork.RepositoryAsync<Contact>().Queryable();
                var accountsQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId && x.ParentId == parentAccount.ID).AsQueryable();

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
                 this.Update(document);
            else
                await InsertAsync(document, organId);

            await unitOfWork.SaveChangesAsync();

            return items.Sum(x => x.totalAmount);
        }

        public async Task<decimal> CreateDocumentOpeningBalanceAssets(Document document, List<TransactionVM> transactions, string docDate, int organId)
        {

            transactions = transactions.Where(x => x.DetailAccount != null || x.Account != null).ToList();

            Transaction transaction = new Transaction();

            foreach (var item in transactions ?? new List<TransactionVM>())
            {
                transaction = new Transaction();
                //transaction.AccDocument = document;

                if (item.DetailAccount != null)
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
                    unitOfWork.RepositoryAsync<Transaction>().Insert(transaction);
                    //_uow.Entry<Transaction>(transaction).State = EntityState.Added;
                    await unitOfWork.SaveChangesAsync();

                }
                else
                {
                    var relTransactions = await unitOfWork.RepositoryAsync<Transaction>().Queryable().Where(x => x.RefTrans == transaction.ID).ToListAsync2();

                    foreach (var relTransaction in relTransactions)
                    {
                        await unitOfWork.RepositoryAsync<Transaction>().DeleteAsync(relTransaction);
                        //_uow.Entry<Transaction>(relTransaction).State = EntityState.Deleted;
                        await unitOfWork.SaveChangesAsync();
                    }

                    //if (transaction.Account != null)
                    //{
                    //    var localAccount = _uow.Set<DomainClasses.Accounting.Account>()
                    //      .Local
                    //      .FirstOrDefault(f => f.ID == transaction.Account.ID);
                    //    if (localAccount != null)
                    //    {
                    //        _uow.Entry(localAccount).State = EntityState.Detached;
                    //    }
                    //}

                    //var local = _uow.Set<Transaction>()
                    //        .Local
                    //        .FirstOrDefault(f => f.ID == transaction.ID);
                    //if (local != null)
                    //{
                    //    _uow.Entry(local).State = EntityState.Detached;
                    //}

                    //_uow.Entry(transaction).State = EntityState.Modified;
                    unitOfWork.RepositoryAsync<Transaction>().Update(transaction);
                    await unitOfWork.SaveChangesAsync();
                }

                var parentAccount = await this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId && x.ComplteCoding == "3101" && x.Level == DomainClasses.ZhivarEnums.AccountType.Moen).SingleOrDefaultAsync2();
                var shareholdersQuery = this.unitOfWork.RepositoryAsync<Shareholder>().Queryable().Where(x => x.OrganId == organId && x.IsActive == true);
                var contactQuery = this.unitOfWork.RepositoryAsync<Contact>().Queryable();
                var accountsQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId && x.ParentId == parentAccount.ID);

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

                    unitOfWork.RepositoryAsync<Transaction>().Insert(transaction2);
                    //_uow.Entry<Transaction>(transaction2).State = EntityState.Added;

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
                    unitOfWork.RepositoryAsync<Transaction>().Insert(transaction);
                   // _uow.Entry<Transaction>(transaction).State = EntityState.Added;
                    await unitOfWork.SaveChangesAsync();

                }
                else
                {
                    var relTransactions = await this.unitOfWork.RepositoryAsync<Transaction>().Queryable().Where(x => x.RefTrans == transaction.ID).ToListAsync2();

                    foreach (var relTransaction in relTransactions)
                    {
                        await unitOfWork.RepositoryAsync<Transaction>().DeleteAsync(relTransaction);
                        //_uow.Entry<Transaction>(relTransaction).State = EntityState.Deleted;
                        await unitOfWork.SaveChangesAsync();
                    }

                    //if (transaction.Account != null)
                    //{
                    //    var localAccount = _uow.Set<DomainClasses.Accounting.Account>()
                    //      .Local
                    //      .FirstOrDefault(f => f.ID == transaction.Account.ID);
                    //    if (localAccount != null)
                    //    {
                    //        _uow.Entry(localAccount).State = EntityState.Detached;
                    //    }
                    //}

                    //var local = _uow.Set<Transaction>()
                    //        .Local
                    //        .FirstOrDefault(f => f.ID == transaction.ID);
                    //if (local != null)
                    //{
                    //    _uow.Entry(local).State = EntityState.Detached;
                    //}

                    unitOfWork.RepositoryAsync<Transaction>().Update(transaction);//.State = EntityState.Modified;

                }


                var parentAccount = await this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId && x.ComplteCoding == "3101" && x.Level == DomainClasses.ZhivarEnums.AccountType.Moen).SingleOrDefaultAsync2();
                var shareholdersQuery = this.unitOfWork.RepositoryAsync<Shareholder>().Queryable().Where(x => x.OrganId == organId && x.IsActive == true);
                var contactQuery = this.unitOfWork.RepositoryAsync<Contact>().Queryable();
                var accountsQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId && x.ParentId == parentAccount.ID).AsQueryable();

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

                    unitOfWork.RepositoryAsync<Transaction>().Insert(transaction2);
                    //_uow.Entry<Transaction>(transaction2).State = EntityState.Added;
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
            var finanYear = await this.unitOfWork.RepositoryAsync<FinanYear>().Queryable().Where(x => x.OrganId == organId && x.Closed == false).SingleOrDefaultAsync2();

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
                    var bank = this.unitOfWork.RepositoryAsync<Bank>().Queryable().Where(x => x.OrganId == organId && x.Code == item.DetailAccount.Code).SingleOrDefaultAsync2();
                    var chequeBank = new ChequeBank()
                    {
                        BankId = bank.Id,
                        ChequeId = item.Cheque.ID,
                        OrganId = organId

                    };
                    unitOfWork.RepositoryAsync<ChequeBank>().Insert(chequeBank);//.State = EntityState.Added;
                    unitOfWork.RepositoryAsync<Transaction>().Insert(transaction);//.State = EntityState.Added;
                    await unitOfWork.SaveChangesAsync();

                }
                else
                {
                    var relTransactions = await this.unitOfWork.RepositoryAsync<Transaction>().Queryable().Where(x => x.RefTrans == transaction.ID).ToListAsync2();

                    foreach (var relTransaction in relTransactions)
                    {
                        this.unitOfWork.RepositoryAsync<Transaction>().Insert(relTransaction);//.State = EntityState.Deleted;
                        await unitOfWork.SaveChangesAsync();
                    }

                    //if (transaction.Account != null)
                    //{
                    //    var localAccount = _uow.Set<DomainClasses.Accounting.Account>()
                    //      .Local
                    //      .FirstOrDefault(f => f.ID == transaction.Account.ID);
                    //    if (localAccount != null)
                    //    {
                    //        _uow.Entry(localAccount).State = EntityState.Detached;
                    //    }
                    //}

                    //var local = _uow.Set<Transaction>()
                    //        .Local
                    //        .FirstOrDefault(f => f.ID == transaction.ID);
                    //if (local != null)
                    //{
                    //    _uow.Entry(local).State = EntityState.Detached;
                    //}


                    unitOfWork.RepositoryAsync<Transaction>().Update(transaction);//.State = EntityState.Modified;

                    var bank = await this.unitOfWork.RepositoryAsync<Bank>().Queryable().Where(x => x.OrganId == organId && x.Code == item.DetailAccount.Code).SingleOrDefaultAsync2();
                    //var chequeBank = await _chequeBanks.Where(x => x.OrganId == organId && x.ChequeId == item.Cheque.ID).SingleOrDefaultAsync();
                    var chequeBank = await this.UnitOfWork.RepositoryAsync<ChequeBank>().Queryable().Where(x => x.OrganId == organId && x.ChequeId == item.Cheque.ID).SingleOrDefaultAsync2();
                    chequeBank.BankId = bank.ID;

                    unitOfWork.RepositoryAsync<ChequeBank>().Update(chequeBank);//.State = EntityState.Modified;
                }


                var parentAccount = await this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId && x.ComplteCoding == "3101" && x.Level == DomainClasses.ZhivarEnums.AccountType.Moen).SingleOrDefaultAsync2();
                var shareholdersQuery = this.unitOfWork.RepositoryAsync<Shareholder>().Queryable().Where(x => x.OrganId == organId && x.IsActive == true);
                var contactQuery = this.unitOfWork.RepositoryAsync<Contact>().Queryable();
                var accountsQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId && x.ParentId == parentAccount.ID).AsQueryable();

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
                    unitOfWork.RepositoryAsync<Transaction>().Insert(transaction2);
                    //_uow.Entry<Transaction>(transaction2).State = EntityState.Added;
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
            var finanYear = await this.unitOfWork.RepositoryAsync<FinanYear>().Queryable().Where(x => x.OrganId == organId && x.Closed == false).SingleOrDefaultAsync2();

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
                    unitOfWork.RepositoryAsync<Transaction>().Insert(transaction);
                   // _uow.Entry<Transaction>(transaction).State = EntityState.Added;
                    await unitOfWork.SaveChangesAsync();

                }
                else
                {
                    var relTransactions = await this.unitOfWork.RepositoryAsync<Transaction>().Queryable().Where(x => x.RefTrans == transaction.ID).ToListAsync2();

                    foreach (var relTransaction in relTransactions)
                    {
                        await unitOfWork.RepositoryAsync<Transaction>().DeleteAsync(relTransaction);
                       // _uow.Entry<Transaction>(relTransaction).State = EntityState.Deleted;
                        await unitOfWork.SaveChangesAsync();
                    }

                    //if (transaction.Account != null)
                    //{
                    //    var localAccount = _uow.Set<DomainClasses.Accounting.Account>()
                    //      .Local
                    //      .FirstOrDefault(f => f.ID == transaction.Account.ID);
                    //    if (localAccount != null)
                    //    {
                    //        _uow.Entry(localAccount).State = EntityState.Detached;
                    //    }
                    //}

                    //var local = _uow.Set<Transaction>()
                    //        .Local
                    //        .FirstOrDefault(f => f.ID == transaction.ID);
                    //if (local != null)
                    //{
                    //    _uow.Entry(local).State = EntityState.Detached;
                    //}

                    unitOfWork.RepositoryAsync<Transaction>().Update(transaction);
                    //_uow.Entry(transaction).State = EntityState.Modified;

                }


                var parentAccount = await this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId && x.ComplteCoding == "3101" && x.Level == DomainClasses.ZhivarEnums.AccountType.Moen).SingleOrDefaultAsync2();
                var shareholdersQuery = this.unitOfWork.RepositoryAsync<Shareholder>().Queryable().Where(x => x.OrganId == organId && x.IsActive == true);
                var contactQuery = this.unitOfWork.RepositoryAsync<Contact>().Queryable();
                var accountsQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId && x.ParentId == parentAccount.ID).AsQueryable();

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
                    unitOfWork.RepositoryAsync<Transaction>().Insert(transaction2);
                   // _uow.Entry<Transaction>(transaction2).State = EntityState.Added;
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

        public async Task<bool> SaveDocument(DocumentVM documentVM, int organId)
        {

            var transactions = documentVM.Transactions.Where(x => x.DetailAccount != null || x.Account != null).ToList();
            var finanYear = await this.unitOfWork.RepositoryAsync<FinanYear>().Queryable().Where(x => x.OrganId == organId && x.Closed == false).SingleOrDefaultAsync2();

            var document = new Document()
            {

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
                Status = documentVM.Status,
                StatusString = documentVM.Status.ToString(),
                Type = documentVM.Type

            };
            document.Transactions = new List<Transaction>();
            var transaction = new Transaction();

            foreach (var item in transactions ?? new List<TransactionVM>())
            {
                transaction = new Transaction();
                //transaction.AccDocument = document;
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

            var credit = document.Transactions.Sum(x => x.Credit);
            var debit = document.Transactions.Sum(x => x.Debit);

            document.Credit = credit;
            document.Debit = debit;

            if (document.ID > 0)
                 Update(document);
            else
                await InsertAsync(document, organId);

            return true;

        }



        public async Task<List<Document>> GetChequeRelatedDocuments(int chequeId)
        {

            var chequeQuery = this.unitOfWork.RepositoryAsync<Cheque>().Queryable().Where(x => x.ID == chequeId);
            var documentQuery = this.Queryable();
            var payReceviesQuery = this.unitOfWork.RepositoryAsync<PayRecevie>().Queryable();
            var detailPayRecevieQuery = this.unitOfWork.RepositoryAsync<DetailPayRecevie>().Queryable().Where(x => x.Type == ZhivarEnums.DetailPayReceiveType.Cheque);

            var joinQuery = from payRecevies in payReceviesQuery
                            join detailPayRecevie in detailPayRecevieQuery
                            on payRecevies.ID equals detailPayRecevie.PayRecevieId
                            join cheque in chequeQuery
                            on detailPayRecevie.ChequeId equals cheque.ID
                            join document in documentQuery
                            on payRecevies.DocumentId equals document.ID
                            select document;

            return await joinQuery.OrderByDescending(x => x.ID).ToListAsync2();

        }

        public async Task<List<DocumentVM>> GetDocumentsByDocumentIDs(List<int> lstDocId)
        {
            var documentQuery = this.Queryable().Where(x => lstDocId.Contains(x.ID));
            var transactionQuery = this.unitOfWork.RepositoryAsync<Transaction>().Queryable();
            var accountQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable();

            var joinQuery = from document in documentQuery
                            join transaction2 in (from transaction in transactionQuery
                                                  join account in accountQuery
                                                  on transaction.AccountId equals account.ID
                                                  select new TransactionVM
                                                  {
                                                      Date = transaction.Date,
                                                      //AccDocument = transaction.AccDocument,
                                                      Account = new AccountVM
                                                      {
                                                          Code = account.Coding,
                                                          ComplteCoding = account.ComplteCoding,
                                                          ID = account.ID,
                                                          Level = account.Level,
                                                          Name = account.Name,
                                                          OrganId = account.OrganId,
                                                          ParentId = account.ParentId
                                                      }, 
                                                      AccountId = account.ID,
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
                                                  }) on document.ID equals transaction2.DocumentId into transactionGroup
                            select new DocumentVM
                            {

                                Transactions = transactionGroup.ToList(),
                                Credit = document.Credit,
                                DateTime = document.DateTime,
                                Debit = document.Debit,
                                Description = document.Description,
                                DisplayDate = document.DisplayDate,
                                //FinanYear = document.FinanYear,
                                FinanYearId = document.FinanYearId,
                                ID = document.ID,
                                IsFirsDocument = document.IsFirsDocument,
                                IsManual = document.IsManual,
                                Number = document.Number,
                                Number2 = document.Number2,
                                OrganId = document.OrganId,
                                Status = document.Status,
                                StatusString = document.StatusString,
                                Type = document.Type
                            };

            return await joinQuery.OrderBy(x => x.ID).ToListAsync2();
             //   .Include(x => x.Transactions).Include(x => x.Transactions.Select(y => y.Account)).OrderByDescending(x => x.ID).ToListAsync();
        }

        protected override Document FindEntity(params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;

            if (entity.Transactions == null)
            {
                this.LoadCollection<Transaction>(entity, dtd => dtd.Transactions);

                foreach (var transaction in entity.Transactions)
                {
                    transaction.Account = this.UnitOfWork.Repository<Account>()
                                                  .Queryable()
                                                  .SingleOrDefault(x => x.ID == transaction.AccountId);

                }


            }

            return entity;
        }

        protected async override Task<Document> FindEntityAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;

            if (entity.Transactions == null)
            {
                await this.LoadCollectionAsync<Transaction>(entity, dtd => dtd.Transactions);

                foreach (var transaction in entity.Transactions)
                {
                    transaction.Account = this.UnitOfWork.Repository<Account>()
                                                  .Queryable()
                                                  .SingleOrDefault(x => x.ID == transaction.AccountId);

                }


            }

            return entity;
        }
    }
}