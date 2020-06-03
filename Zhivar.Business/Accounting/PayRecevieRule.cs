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
using Zhivar.DomainClasses.Common;
using System.Threading;
using Zhivar.DomainClasses;

namespace Zhivar.Business.Accounting
{
    public partial class PayRecevieRule : BusinessRuleBase<PayRecevie>
    {
        public PayRecevieRule()
            : base()
        {

        }

        public PayRecevieRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public PayRecevieRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public async Task<IList<PayRecevieVM>> GetAllByOrganIdAsync(int organId, bool? isReceive=null)
        {
            var finanYear = await this.unitOfWork.RepositoryAsync<FinanYear>().Queryable().Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync2();
            var documentQuery = this.unitOfWork.RepositoryAsync<Document>().Queryable().Where(x => x.OrganId == organId && x.FinanYearId == finanYear.ID);
            var payRecevieQuery = this.Queryable().Where(x => x.OrganId == organId);
            var accountsQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId);
            var baseAccountsQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId);

            var transactionsQuery = this.unitOfWork.RepositoryAsync<Transaction>().Queryable();

            if (isReceive == true)
                transactionsQuery = transactionsQuery.Where(x => x.IsCredit == true);
            else if( isReceive == false)
                transactionsQuery = transactionsQuery.Where(x => x.IsDebit == true);


            var joinQuery = from document in documentQuery
                            join payRecevie in payRecevieQuery
                            on document.ID equals payRecevie.DocumentId
                            join transaction in (from transaction2  in transactionsQuery
                                                 join account in accountsQuery
                                                 on transaction2.AccountId equals account.ID
                                                 join baseAccount in baseAccountsQuery
                                                 on account.ParentId equals baseAccount.ID
                                                 select new TransactionVM
                                                 {
                                                     AccountName = baseAccount.Name,
                                                     DetailAccount = new DetailAccount
                                                     {
                                                         Name = account.Name
                                                     },
                                                     Amount = transaction2.Amount,
                                                     DocumentId = transaction2.DocumentId
                                                 })
                            on document.ID equals transaction.DocumentId into transactionGroup
                           
                            select new PayRecevieVM()
                            {
                                AccountName = transactionGroup.Any()?transactionGroup.FirstOrDefault().AccountName:"", //baseAccount.Name,
                                Amount = transactionGroup.Any() ? transactionGroup.Sum(x=> x.Amount): 0,// transaction.Amount,
                                Description = payRecevie.Description,
                                DetailAccountName = transactionGroup.Any()?transactionGroup.FirstOrDefault().DetailAccount.Name:"",//account.Name,
                                DisplayDate = payRecevie.DisplayDate,
                                ID = payRecevie.ID,
                                Number = payRecevie.Number,
                                NumberDocument = document.Number,
                                NumberDocument2 = document.Number2,
                                Type2 = payRecevie.Type,
                                //IsCredit = transaction.IsCredit,
                                //IsDebit = transaction.IsDebit,
                                IsRecevie = payRecevie.IsReceive

                            };
            return await joinQuery.Distinct().ToListAsync2();
            //return await _payRecevies.AsQueryable().Where(x => x.OrganId == organId).ToListAsync();
        }
        public IList<PayRecevieVM> GetAllByOrganId(int organId, bool? isReceive = null)
        {
            var finanYear = this.unitOfWork.RepositoryAsync<FinanYear>().Queryable().Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefault();
            var documentQuery = this.unitOfWork.RepositoryAsync<Document>().Queryable().Where(x => x.OrganId == organId && x.FinanYearId == finanYear.ID);
            var payRecevieQuery = this.Queryable().Where(x => x.OrganId == organId);
            var accountsQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId);
            var baseAccountsQuery = this.unitOfWork.RepositoryAsync<Account>().Queryable().Where(x => x.OrganId == organId);

            var transactionsQuery = this.unitOfWork.RepositoryAsync<Transaction>().Queryable();

            if (isReceive == true)
                transactionsQuery = transactionsQuery.Where(x => x.IsCredit == true);
            else if (isReceive == false)
                transactionsQuery = transactionsQuery.Where(x => x.IsDebit == true);


            var joinQuery = from document in documentQuery
                            join payRecevie in payRecevieQuery
                            on document.ID equals payRecevie.DocumentId
                            join transaction in (from transaction2 in transactionsQuery
                                                 join account in accountsQuery
                                                 on transaction2.AccountId equals account.ID
                                                 join baseAccount in baseAccountsQuery
                                                 on account.ParentId equals baseAccount.ID
                                                 select new TransactionVM
                                                 {
                                                     AccountName = baseAccount.Name,
                                                     DetailAccount = new DetailAccount
                                                     {
                                                         Name = account.Name
                                                     },
                                                     Amount = transaction2.Amount,
                                                     DocumentId = transaction2.DocumentId
                                                 })
                            on document.ID equals transaction.DocumentId into transactionGroup

                            select new PayRecevieVM()
                            {
                                AccountName = transactionGroup.Any() ? transactionGroup.FirstOrDefault().AccountName : "", //baseAccount.Name,
                                Amount = transactionGroup.Any() ? transactionGroup.Sum(x => x.Amount) : 0,// transaction.Amount,
                                Description = payRecevie.Description,
                                DetailAccountName = transactionGroup.Any() ? transactionGroup.FirstOrDefault().DetailAccount.Name : "",//account.Name,
                                DisplayDate = payRecevie.DisplayDate,
                                ID = payRecevie.ID,
                                Number = payRecevie.Number,
                                NumberDocument = document.Number,
                                NumberDocument2 = document.Number2,
                                Type2 = payRecevie.Type,
                                //IsCredit = transaction.IsCredit,
                                //IsDebit = transaction.IsDebit,
                                IsRecevie = payRecevie.IsReceive

                            };
            return joinQuery.Distinct().ToList();
            //return await _payRecevies.AsQueryable().Where(x => x.OrganId == organId).ToListAsync();
        }


        public async Task<IList<PayRecevie>> GetByInvoiceIdAsync(int invoiceId)
        {
            return await this.Queryable().Where(x => x.InvoiceId == invoiceId).ToListAsync2();
        }
        public async Task<List<DetailPayRecevieVM>> GetTempPaymentsByInvoiceIdAsync(int invoiceId)
        {
            using (var uow = new UnitOfWork())
            {
                var payRecevieQuery = uow.Repository<PayRecevie>().Queryable().Where(x => x.InvoiceId == invoiceId && x.Status == DomainClasses.ZhivarEnums.Status.Temporary);
                var payRecevieDetailQuery = uow.Repository<DetailPayRecevie>().Queryable();

                var joinQuery = from payRecevie in payRecevieQuery
                                join payRecevieDetail in payRecevieDetailQuery
                                on payRecevie.ID equals payRecevieDetail.PayRecevieId
                                select new DetailPayRecevieVM
                                {
                                    Amount = payRecevieDetail.Amount,
                                    ID = payRecevieDetail.ID,
                                    Type = payRecevieDetail.Type,
                                   
                                    //BankId = payRecevieDetail.BankId,
                                    //CashId = payRecevieDetail.CashId,
                                    //ChequeBankId = payRecevieDetail.ChequeBankId,
                                    //ChequeId = payRecevieDetail.ChequeId,
                                    PayRecevieId = payRecevieDetail.PayRecevieId,
                                    Reference = payRecevieDetail.Reference

                                };

                return await joinQuery.ToListAsync();
            }
            
        }
        public async Task<IList<PayRecevie>> GetByCostIdAsync(int costId)
        {
            return await this.Queryable().Where(x => x.CostId == costId).ToListAsync2();
        }
        public async Task<List<Document>> GetDoucumentIDByChequeIdAsync(int chequeId)
        {
            var payReceviesDetailQuery = this.unitOfWork.Repository<DetailPayRecevie>().Queryable().Where(x => x.ChequeId == chequeId && x.Type == ZhivarEnums.DetailPayReceiveType.Cheque);
            var payReceviesQuery = this.Queryable();
            var documents = this.unitOfWork.Repository<Document>().Queryable();

            var joinQuery = from payReceviesDetail in payReceviesDetailQuery
                            join payRecevies in payReceviesQuery
                            on payReceviesDetail.PayRecevieId equals payRecevies.ID
                            join document in documents
                            on payRecevies.DocumentId equals document.ID
                            select document;

            return await joinQuery.ToListAsync2();
        }

        protected override PayRecevie FindEntity(params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;

            if (entity.Items == null)
            {
                this.LoadCollection<DetailPayRecevie>(entity, dtd => dtd.Items);

                foreach (var detailPayRecevie in entity.Items)
                {
                    if (detailPayRecevie.ChequeId > 0)
                    {
                        detailPayRecevie.Cheque = this.UnitOfWork.Repository<Cheque>()
                                                                        .Queryable()
                                                                        .Where(dr => dr.ID == detailPayRecevie.ChequeId)
                                                                        .SingleOrDefault();
                    }

                    if (detailPayRecevie.BankId> 0)
                    {
                        detailPayRecevie.Bank = this.UnitOfWork.Repository<Bank>()
                                                                    .Queryable()
                                                                    .Where(dr => dr.ID == detailPayRecevie.BankId)
                                                                    .SingleOrDefault();
                    }

                    if (detailPayRecevie.CashId > 0)
                    {
                        detailPayRecevie.Cash = this.UnitOfWork.Repository<Cash>()
                                                                    .Queryable()
                                                                    .Where(dr => dr.ID == detailPayRecevie.CashId)
                                                                    .SingleOrDefault();
                    }

            
                }
            }

            if (entity.Contact == null)
            {
                this.LoadReference<Contact>(entity, dtd => dtd.Contact);
            }

            if (entity.Account == null)
            {
                this.LoadReference<Account>(entity, dtd => dtd.Account);
            }
            if (entity.Cost == null && entity.CostId > 0)
            {
                this.LoadReference<Cost>(entity, dtd => dtd.Cost);
            }

            if (entity.Invoice == null && entity.InvoiceId > 0)
            {
                this.LoadReference<Invoice>(entity, dtd => dtd.Invoice);
            }

            if (entity.Document == null)
            {
                this.LoadReference<Document>(entity, dtd => dtd.Document);

                entity.Document.Transactions = this.UnitOfWork.Repository<Transaction>()
                                                                                    .Queryable()
                                                                                    .Where(dr => dr.DocumentId == entity.Document.ID)
                                                                                    .ToList();
            }

            return entity;
        }

        protected async override Task<PayRecevie> FindEntityAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;

            if (entity.Items == null)
            {
                await this.LoadCollectionAsync<DetailPayRecevie>(entity, dtd => dtd.Items);

                foreach (var detailPayRecevie in entity.Items)
                {
                    if (detailPayRecevie.ChequeId > 0)
                    {
                        detailPayRecevie.Cheque = this.UnitOfWork.Repository<Cheque>()
                                                                        .Queryable()
                                                                        .Where(dr => dr.ID == detailPayRecevie.ChequeId)
                                                                        .SingleOrDefault();
                    }

                    if (detailPayRecevie.BankId > 0)
                    {
                        detailPayRecevie.Bank = this.UnitOfWork.Repository<Bank>()
                                                                    .Queryable()
                                                                    .Where(dr => dr.ID == detailPayRecevie.BankId)
                                                                    .SingleOrDefault();
                    }

                    if (detailPayRecevie.CashId > 0)
                    {
                        detailPayRecevie.Cash = this.UnitOfWork.Repository<Cash>()
                                                                    .Queryable()
                                                                    .Where(dr => dr.ID == detailPayRecevie.CashId)
                                                                    .SingleOrDefault();
                    }


                }
            }

            if (entity.Contact == null)
            {
                await this.LoadReferenceAsync<Contact>(entity, dtd => dtd.Contact);
            }

            if (entity.Account == null)
            {
                await this.LoadReferenceAsync<Account>(entity, dtd => dtd.Account);
            }
            if (entity.Cost == null && entity.CostId > 0)
            {
                await this.LoadReferenceAsync<Cost>(entity, dtd => dtd.Cost);
            }

            if (entity.Invoice == null && entity.InvoiceId > 0)
            {
                await this.LoadReferenceAsync<Invoice>(entity, dtd => dtd.Invoice);
            }

            if (entity.Document == null)
            {
                await this.LoadReferenceAsync<Document>(entity, dtd => dtd.Document);

                entity.Document.Transactions = this.UnitOfWork.Repository<Transaction>()
                                                                                    .Queryable()
                                                                                    .Where(dr => dr.DocumentId == entity.Document.ID)
                                                                                    .ToList();
            }

            return entity;
        }

        protected override void InsertOrUpdateGraphEntity(PayRecevie entity)
        {
            
            base.InsertOrUpdateGraphEntity(entity);
        }

        protected override void DeleteEntity(PayRecevie entity)
        {
            if (entity.Document != null)
            {
                foreach (var transaction in entity.Document.Transactions.ToList() ?? new List<Transaction>())
                {
                    transaction.ObjectState = Enums.ObjectState.Deleted;
                }

                entity.Document.ObjectState = Enums.ObjectState.Deleted;
            }

            if (entity.Items != null)
            {
                foreach (var item in entity.Items.ToList() ?? new List<DetailPayRecevie>())
                {
                    if (item.Cheque != null)
                    {
                        item.Cheque.ObjectState = Enums.ObjectState.Deleted;
                    }
                    item.ObjectState = Enums.ObjectState.Deleted;
                }
            }

            if (entity.Invoice != null)
            {

                entity.Invoice.ObjectState = Enums.ObjectState.Modified;

                //var invoice = payRecevie.Invoice;
                entity.Invoice.Rest += entity.Amount;
                entity.Invoice.Paid -= entity.Amount;
               // _invoices.Attach(invoice);
            }

            if (entity.Cost != null)
            {

                entity.Cost.ObjectState = Enums.ObjectState.Modified;

             
                entity.Cost.Rest += entity.Amount;
                entity.Cost.Paid -= entity.Amount;
             
            }

            base.DeleteEntity(entity);
        }
    }
}