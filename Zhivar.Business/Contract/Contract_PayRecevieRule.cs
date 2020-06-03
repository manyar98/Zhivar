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
using Zhivar.DomainClasses.Contract;
using Zhivar.DomainClasses;

namespace Zhivar.Business.Contract
{
    public partial class Contract_PayRecevieRule : BusinessRuleBase<Contract_PayRecevies>
    {
        public Contract_PayRecevieRule()
            : base()
        {

        }

        public Contract_PayRecevieRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public Contract_PayRecevieRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

   
        public async Task<IList<Contract_PayRecevies>> GetByContractIdAsync(int contractId)
        {
            return await this.Queryable().Where(x => x.ContractId == contractId).ToListAsync2();
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

        protected override Contract_PayRecevies FindEntity(params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;

            if (entity.Contract_DetailPayRecevies == null)
            {
                this.LoadCollection<DomainClasses.Contract.Contract_DetailPayRecevies>(entity, dtd => dtd.Contract_DetailPayRecevies);

                foreach (var detailPayRecevie in entity.Contract_DetailPayRecevies)
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
                this.LoadReference<Contact>(entity, dtd => dtd.Contact);
            }

            if (entity.Account == null)
            {
                this.LoadReference<Account>(entity, dtd => dtd.Account);
            }

            if (entity.Contract == null && entity.ContractId > 0)
            {
                this.LoadReference<DomainClasses.Contract.Contract>(entity, dtd => dtd.Contract);
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

        protected async override Task<Contract_PayRecevies> FindEntityAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;

            if (entity.Contract_DetailPayRecevies == null)
            {
                await this.LoadCollectionAsync<Contract_DetailPayRecevies>(entity, dtd => dtd.Contract_DetailPayRecevies);

                foreach (var detailPayRecevie in entity.Contract_DetailPayRecevies)
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
         

            if (entity.Contract == null && entity.ContractId > 0)
            {
                await this.LoadReferenceAsync<DomainClasses.Contract.Contract>(entity, dtd => dtd.Contract);
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

        protected override void InsertOrUpdateGraphEntity(Contract_PayRecevies entity)
        {

            base.InsertOrUpdateGraphEntity(entity);
        }

        protected override void DeleteEntity(Contract_PayRecevies entity)
        {
            if (entity.Document != null)
            {
                foreach (var transaction in entity.Document.Transactions.ToList() ?? new List<Transaction>())
                {
                    transaction.ObjectState = Enums.ObjectState.Deleted;
                }

                entity.Document.ObjectState = Enums.ObjectState.Deleted;
            }

            if (entity.Contract_DetailPayRecevies != null)
            {
                foreach (var item in entity.Contract_DetailPayRecevies.ToList() ?? new List<Contract_DetailPayRecevies>())
                {
                    if (item.Cheque != null)
                    {
                        item.Cheque.ObjectState = Enums.ObjectState.Deleted;
                    }
                    item.ObjectState = Enums.ObjectState.Deleted;
                }
            }

            if (entity.Contract != null)
            {

                entity.Contract.ObjectState = Enums.ObjectState.Modified;

                //var invoice = payRecevie.Invoice;
                entity.Contract.Rest += entity.Amount;
                entity.Contract.Paid -= entity.Amount;
                // _invoices.Attach(invoice);
            }

          

            base.DeleteEntity(entity);
        }
    }
}