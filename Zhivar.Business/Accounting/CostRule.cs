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
using Zhivar.DomainClasses.Common;
using System.Threading;
using Zhivar.ViewModel.Accunting;
using Zhivar.DomainClasses;
using Zhivar.Utilities;

namespace Zhivar.Business.Accounting
{
    public partial class CostRule : BusinessRuleBase<Cost>
    {
        public CostRule()
            : base()
        {

        }

        public CostRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public CostRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public IList<Cost> GetAllByOrganId(int organId)
        {
            var Costs = this.Queryable().Where(x => x.OrganId == organId).ToList();

            return Costs;
        }
        public async Task<IList<Cost>> GetAllByOrganIdAsync(int organId)
        {
            var Costs = await this.Queryable().Where(x => x.OrganId == organId).OrderByDescending(x => x.ID).ToListAsync2();

            return Costs;

        }
         private async Task<int> createNumberDocument(int organId)
        {
            var count = 0;
            DocumentRule documentRule = new DocumentRule();

            var documentQuery = await documentRule.GetAllByOrganIdAsync(organId);

            count = documentQuery.Count();

            return count++;
        }
        public async Task<Document> RegisterDocument(CostVM cost, int personId)
        {
            var documentNumber = await createNumberDocument(personId);
            Document document = new Document();
            document.Credit = cost.Sum;
            document.Debit = cost.Sum;
            document.IsManual = false;
            document.Number = documentNumber;
            document.Number2 = documentNumber;
            document.Status = ZhivarEnums.DocumentStatus.TaeedShode;
            document.StatusString = "تایید شده";
            document.DisplayDate = cost.DisplayDate;
            document.DateTime = PersianDateUtils.ToDateTime(cost.DisplayDate);
            document.Type = ZhivarEnums.NoeDoc.Cost;

            List<Transaction> transactions = new List<Transaction>();

            document.Description = cost.Explain;

            transactions = await RegisterTransaction(document, cost, personId);
            document.Transactions = transactions;


            if (document.Transactions != null && document.Transactions.Any())
            {
                var credit = document.Transactions.Sum(x => x.Credit);
                var debit = document.Transactions.Sum(x => x.Debit);

                document.Credit = credit;
                document.Debit = debit;
            }
            return document;
        }

        public async Task<List<Transaction>> RegisterTransaction(Document document, CostVM cost, int organId)
        {
            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
          
            string contactCode = "2101" + cost.Contact.Code;
            var accountHesabPardakhtani = accounts.Where(x => x.ComplteCoding == contactCode).SingleOrDefault();

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction()
            {
                AccountId = accountHesabPardakhtani.ID,
                ContactId = cost.Contact.ID,
                DocumentId = document.ID,
                CostId = cost.ID,
                //AccDocument = document,
                Account = accountHesabPardakhtani,
                Amount = cost.Sum,
                //Contact = cost.Contact,
                //Cost = cost,
                IsDebit = false,
                IsCredit = true,
                Debit = 0,
                Credit = cost.Sum,
                Description = document.Description,
                RowNumber = 1,
                DisplayDate = document.DisplayDate,
                Date = PersianDateUtils.ToDateTime(document.DisplayDate),

            });


            foreach (var item in cost.CostItems)
            {

                var accoubtItem = new DomainClasses.Accounting.Account();

                accoubtItem = accounts.Where(x => x.ComplteCoding == "8205").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtItem.ID,
                    ContactId = cost.Contact.ID,
                    DocumentId = document.ID,
                    CostId = cost.ID,
                    //AccDocument = document,
                    Account = accoubtItem,
                    Amount = item.Sum,
                    //Contact = cost.Contact,
                    //Cost = cost,
                    IsDebit = true,
                    IsCredit = false,
                    Debit = item.Sum,
                    Credit = 0,
                    Description = item.Description,
                    //RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }


            return transactions;
        }
        protected override Cost FindEntity(params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;

            if (entity.CostItems == null)
            {
                this.LoadCollection<CostItem>(entity, dtd => dtd.CostItems);

                foreach (var CostItem in entity.CostItems)
                {
                    //if (CostItem.Item == null)
                    //{

                    //    CostItem.Item = this.UnitOfWork.Repository<Account>()
                    //                                               .Queryable()
                    //                                               .SingleOrDefault(dr => dr.ID == CostItem.ItemId);


                    //}
                }
            }
            //if (entity.Contact == null)
            //{
            //    this.LoadReference<Contact>(entity, dtd => dtd.Contact);
            //}

            return entity;
        }

        protected async override Task<Cost> FindEntityAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;

            if (entity.CostItems == null)
            {
                await this.LoadCollectionAsync<CostItem>(entity, dtd => dtd.CostItems);

                foreach (var CostItem in entity.CostItems)
                {
                    //if (CostItem.Item == null)
                    //{

                    //    CostItem.Item =  this.UnitOfWork.Repository<Account>()
                    //                                               .Queryable()
                    //                                               .SingleOrDefault(dr => dr.ID == CostItem.ItemId);


                    //}
                }
            }
            //if (entity.Contact == null)
            //{
            //   // await this.LoadReferenceAsync<Contact>(entity, dtd => dtd.Contact);
            //    entity.Contact = this.UnitOfWork.Repository<Contact>()
            //                                                    .Queryable()
            //                                                    .SingleOrDefault(dr => dr.ID == entity.ContactId);
            //}

            return entity;
        }
    }
}