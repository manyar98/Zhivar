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

namespace Zhivar.Business.Accounting
{
    public partial class TransferMoneyRule : BusinessRuleBase<TransferMoney>
    {
        public TransferMoneyRule()
            : base()
        {

        }

        public TransferMoneyRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public TransferMoneyRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public async Task<IList<TransferMoney>> GetAllByOrganIdAsync(int organId)
        {
            var finanYear = await this.unitOfWork.RepositoryAsync<FinanYear>().Queryable().Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync2();

            var transferMoneys = await this.unitOfWork.RepositoryAsync<TransferMoney>().Queryable().Where(x => x.OrganId == organId).ToListAsync2();

            foreach (var transfer in transferMoneys)
            {
                if (transfer.From == "bank")
                {
                    int id = Convert.ToInt32(transfer.FromDetailAccountId);
                    transfer.FromDetailAccountName = this.unitOfWork.RepositoryAsync<Account>().Queryable().SingleOrDefault(x => x.ID == id).Name;
                }
                else
                {
                    int id = Convert.ToInt32(transfer.FromDetailAccountId);
                    transfer.FromDetailAccountName = this.unitOfWork.RepositoryAsync<Account>().Queryable().SingleOrDefault(x => x.ID == id).Name;

                }
                if (transfer.To == "bank")
                {
                    int id = Convert.ToInt32(transfer.ToDetailAccountId);
                    transfer.ToDetailAccountName = this.unitOfWork.RepositoryAsync<Account>().Queryable().SingleOrDefault(x => x.ID == id).Name;
                }
                else
                {
                    int id = Convert.ToInt32(transfer.ToDetailAccountId);
                    transfer.ToDetailAccountName = this.unitOfWork.RepositoryAsync<Account>().Queryable().SingleOrDefault(x => x.ID == id).Name;

                }
                if (transfer.DocumentId != null)
                {
                    transfer.DocumentNumber = this.unitOfWork.RepositoryAsync<Document>().Queryable().SingleOrDefault(x => x.ID == transfer.DocumentId).Number;
                }
            }
            return transferMoneys;
        }

        public async Task<TransferMoney> GetByDocIdAsync(int docId)
        {
            var transferQuery = this.Queryable().Where(x => x.DocumentId == docId);
            var documentQuery = this.unitOfWork.RepositoryAsync<Document>().Queryable();
            var transactionQuery = this.unitOfWork.RepositoryAsync<Transaction>().Queryable();

            var joinQuery = from transfer1 in transferQuery
                            join document2 in (from document in documentQuery
                                               join transaction in transactionQuery
                                               on document.ID equals transaction.DocumentId into transactionGroup
                                               select new Document()
                                               {
                                                   Transactions = transactionGroup.ToList(),
                                                   Credit = document.Credit,
                                                   DateTime = document.DateTime,
                                                   Debit = document.Debit,
                                                   Description = document.Description,
                                                   DisplayDate = document.DisplayDate,
                                                   FinanYear = document.FinanYear,
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
                                               }) on transfer1.DocumentId equals document2.ID
                            select new TransferMoney()
                            {
                                Amount = transfer1.Amount,
                                Document = document2,
                                ID = transfer1.ID,
                                Date = transfer1.Date,
                                Description = transfer1.Description,
                                DisplayDate = transfer1.DisplayDate,
                                DocumentId = transfer1.DocumentId,
                                DocumentNumber = transfer1.DocumentNumber,
                                From = transfer1.From,
                                FromDetailAccountId = transfer1.FromDetailAccountId,
                                FromDetailAccountName = transfer1.FromDetailAccountName,
                                FromReference = transfer1.FromReference,
                                OrganId = transfer1.OrganId,
                                To = transfer1.To,
                                ToDetailAccountId = transfer1.ToDetailAccountId,
                                ToDetailAccountName = transfer1.ToDetailAccountName,
                                ToReference = transfer1.ToReference
                            };

            var transfer = await joinQuery.SingleOrDefaultAsync2();


            if (transfer.From == "bank")
            {
                int id = Convert.ToInt32(transfer.FromDetailAccountId);
                transfer.FromDetailAccountName = this.unitOfWork.RepositoryAsync<Account>().Queryable().SingleOrDefault(x => x.ID == id).Name;
            }
            else
            {
                int id = Convert.ToInt32(transfer.FromDetailAccountId);
                transfer.FromDetailAccountName = this.unitOfWork.RepositoryAsync<Account>().Queryable().SingleOrDefault(x => x.ID == id).Name;

            }
            if (transfer.To == "bank")
            {
                int id = Convert.ToInt32(transfer.ToDetailAccountId);
                transfer.ToDetailAccountName = this.unitOfWork.RepositoryAsync<Account>().Queryable().SingleOrDefault(x => x.ID == id).Name;
            }
            else
            {
                int id = Convert.ToInt32(transfer.ToDetailAccountId);
                transfer.ToDetailAccountName = this.unitOfWork.RepositoryAsync<Account>().Queryable().SingleOrDefault(x => x.ID == id).Name;

            }
            if (transfer.DocumentId != null)
            {
                transfer.DocumentNumber = this.unitOfWork.RepositoryAsync<Document>().Queryable().SingleOrDefault(x => x.ID == transfer.DocumentId).Number;
            }

            return transfer;
        }

        protected override void DeleteEntity(TransferMoney entity)
        {
            if (entity.Document != null)
            {
                foreach (var transaction in entity.Document.Transactions.ToList())
                {
                    transaction.ObjectState = Enums.ObjectState.Deleted;
                    //_transactions.Remove(transaction);
                }

                entity.Document.ObjectState = Enums.ObjectState.Deleted;

                //_documents.Remove(transferMoney.Document);
            }

           // _transferMoneys.Remove(transferMoney);

            //return transferMoney;

            base.DeleteEntity(entity);
        }
    }
}