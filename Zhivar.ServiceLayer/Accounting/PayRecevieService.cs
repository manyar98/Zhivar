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
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ServiceLayer.Accunting
{
    public class PayRecevieService : IPayRecevie
    {
        IUnitOfWork _uow;
        IMappingEngine Mapper;
        readonly IDbSet<PayRecevie> _payRecevies;
        readonly IDbSet<DomainClasses.Accounting.Account> _accounts;
        readonly IDbSet<FinanYear> _finanYears;
        readonly IDbSet<Document> _documents;
        readonly IDbSet<Transaction> _transactions;
        readonly IDbSet<Invoice> _invoices;
        readonly IDbSet<DetailPayRecevie> _detailPayRecevies;

        public PayRecevieService(IUnitOfWork uow, IMappingEngine mappingEngine)
        {
            _uow = uow;
            Mapper = mappingEngine;
            _payRecevies = _uow.Set<PayRecevie>();
            _accounts = _uow.Set<DomainClasses.Accounting.Account>();
            _documents = _uow.Set<Document>();
            _finanYears = _uow.Set<FinanYear>();
            _transactions = _uow.Set<Transaction>();
            _invoices = _uow.Set<Invoice>();
            _detailPayRecevies = _uow.Set<DetailPayRecevie>();
        }
        //public async Task<bool> Delete(int id)
        //{
        //    try
        //    {
        //        var payRecevie = await GetByIdAsync(id);
        //        Delete(payRecevie);
        //        return true;

        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        //public bool Delete(PayRecevie payRecevie)
        //{
        //    try
        //    {
        //        _payRecevies.Attach(payRecevie);

        //        if(payRecevie.Document != null)
        //        {
        //            foreach (var transaction in payRecevie.Document.Transactions.ToList() ?? new List<Transaction>())
        //            {
        //                _transactions.Remove(transaction);
        //            }
               
        //            _uow.Entry(payRecevie.Document).State = EntityState.Deleted;
        //        }

        //        if (payRecevie.Items != null)
        //        {
        //            foreach (var item in payRecevie.Items.ToList() ?? new List<DetailPayRecevie>())
        //            {
        //                if (item.Cheque != null)
        //                {
        //                    _uow.Entry(item.Cheque).State = EntityState.Deleted;
        //                }
        //                _detailPayRecevies.Remove(item);
        //            }
        //        }



        //        _payRecevies.Remove(payRecevie);

        //        if (payRecevie.Invoice != null)
        //        {
        //            var local = _uow.Set<Invoice>()
        //           .Local
        //           .FirstOrDefault(f => f.ID == payRecevie.Invoice.ID);
        //            if (local != null)
        //            {
        //                _uow.Entry(local).State = EntityState.Detached;
        //            }

        //            _invoices.Attach(payRecevie.Invoice);

        //            _uow.Entry(payRecevie.Invoice).State = EntityState.Modified;

        //            var invoice = payRecevie.Invoice;
        //            invoice.Rest += payRecevie.Amount;
        //            invoice.Paid -= payRecevie.Amount;
        //            _invoices.Attach(invoice);
        //        }

        //        return true;

        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //public IList<PayRecevie> GetAll()
        //{
        //    return _payRecevies.ToList();
        //}
        //public IList<PayRecevie> GetAllByOrganId(int organId)
        //{
        //    return _payRecevies.AsQueryable().Where(x => x.OrganId == organId).ToList();
        //}
        public async Task<IList<PayRecevieVM>> GetAllByOrganIdAsync(int organId)
        {
            var finanYear = await _finanYears.Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync();
            var documentQuery = _documents.Where(x => x.OrganId == organId && x.FinanYearId == finanYear.ID);
            var payRecevieQuery = _payRecevies.Where(x => x.OrganId == organId);
            var accountsQuery = _accounts.Where(x => x.OrganId == organId);
            var baseAccountsQuery = _accounts.Where(x => x.OrganId == organId);
            var transactionsQuery = _transactions;

            var joinQuery = from document in documentQuery
                            join payRecevie in payRecevieQuery
                            on document.ID equals payRecevie.DocumentId
                            join transaction in transactionsQuery
                            on document.ID equals transaction.DocumentId
                            join account in accountsQuery
                            on transaction.AccountId equals account.ID
                            join baseAccount in baseAccountsQuery
                            on account.ParentId equals baseAccount.ID
                            select new PayRecevieVM()
                            {
                                AccountName = baseAccount.Name,
                                Amount = transaction.Amount,
                                Description = payRecevie.Description,
                                DetailAccountName = account.Name,
                                DisplayDate = payRecevie.DisplayDate,
                                ID = payRecevie.ID,
                                Number = payRecevie.Number,
                                NumberDocument = document.Number,
                                NumberDocument2 = document.Number2,
                                Type2 = payRecevie.Type,
                                IsCredit = transaction.IsCredit,
                                IsDebit = transaction.IsDebit,
                                IsRecevie = payRecevie.IsReceive

                            };
            return await joinQuery.Distinct().ToListAsync();
            //return await _payRecevies.AsQueryable().Where(x => x.OrganId == organId).ToListAsync();
        }

        //public async Task<IList<PayRecevie>> GetAllAsync()
        //{
        //    return await _payRecevies.ToListAsync();
        //}
        //public PayRecevie GetById(int id)
        //{
        //    return _payRecevies.Find(id);
        //}

        //public bool Insert(PayRecevie payRecevie)
        //{
        //    try
        //    {
        //        _payRecevies.Add(payRecevie);

        //        // if(payRecevie.Contact != null)
        //        //    _uow.Entry(payRecevie.Contact).State = EntityState.Unchanged;
        //        if (payRecevie.Account != null)
        //        {
        //            payRecevie.AccountId = payRecevie.Account.ID;

        //            var localAccount = _uow.Set<DomainClasses.Accounting.Account>().Local.FirstOrDefault(x => x.ID == payRecevie.Account.ID);

        //            if (localAccount != null)
        //            {
        //                _uow.Entry(localAccount).State = EntityState.Detached;
        //            }
        //            //_uow.Entry(payRecevie.Account).State = EntityState.Unchanged;
        //        }

        //        //if (payRecevie.FinanYear != null)
        //        //{
        //        //    var localFinanYear = _uow.Set<FinanYear>().Local.FirstOrDefault(x => x.ID == payRecevie.FinanYear.ID);

        //        //    if (localFinanYear != null)
        //        //    {
        //        //        _uow.Entry(localFinanYear).State = EntityState.Detached;
        //        //    }

        //        //    _uow.Entry(payRecevie.FinanYear).State = EntityState.Unchanged;
        //        //}
                    
        //        if (payRecevie.Invoice != null)
        //        {
        //            payRecevie.InvoiceId = payRecevie.Invoice.ID;

        //            var local = _uow.Set<Contact>()
        //                 .Local
        //                 .FirstOrDefault(f => f.ID == payRecevie.Invoice.Contact.ID);
        //            if (local != null)
        //            {
        //                _uow.Entry(local).State = EntityState.Detached;

        //            }
        //          //  _uow.Entry(payRecevie.Invoice).State = EntityState.Unchanged;

        //            if (payRecevie.Invoice.Contact != null)
        //            {
        //                payRecevie.Invoice.ContactId = payRecevie.Invoice.Contact.ID;

        //                var localContact = _uow.Set<Contact>()
        //                    .Local
        //                    .FirstOrDefault(f => f.ID == payRecevie.Invoice.Contact.ID);
        //                if (localContact != null)
        //                {
        //                    _uow.Entry(localContact).State = EntityState.Detached;

        //                }
        //                //_uow.Entry(payRecevie.Contact).State = EntityState.Unchanged;
        //            }
                    
        //        }

        //        if (payRecevie.Cost != null)
        //        {
        //            payRecevie.CostId = payRecevie.Cost.ID;

        //            var local = _uow.Set<Contact>()
        //                 .Local
        //                 .FirstOrDefault(f => f.ID == payRecevie.Cost.Contact.ID);
        //            if (local != null)
        //            {
        //                _uow.Entry(local).State = EntityState.Detached;

        //            }
        //            //  _uow.Entry(payRecevie.Invoice).State = EntityState.Unchanged;

        //            if (payRecevie.Cost.Contact != null)
        //            {
        //                payRecevie.Cost.ContactId = payRecevie.Cost.Contact.ID;

        //                var localContact = _uow.Set<Contact>()
        //                    .Local
        //                    .FirstOrDefault(f => f.ID == payRecevie.Cost.Contact.ID);
        //                if (localContact != null)
        //                {
        //                    _uow.Entry(localContact).State = EntityState.Detached;

        //                }
        //                //_uow.Entry(payRecevie.Contact).State = EntityState.Unchanged;
        //            }

        //        }
        //        if (payRecevie.Contact != null)
        //        {
        //            payRecevie.ContactId = payRecevie.Contact.ID;

        //            var localContact = _uow.Set<Contact>()
        //                    .Local
        //                    .FirstOrDefault(f => f.ID == payRecevie.Contact.ID);
        //            if (localContact != null)
        //            {
        //                _uow.Entry(localContact).State = EntityState.Detached;

        //            }
        //            //_uow.Entry(payRecevie.Contact).State = EntityState.Modified;
        //        }
                    

        //        foreach (var item in payRecevie.Items)
        //        {
        //            if(item.Cash!= null)
        //            {
        //                item.CashId = item.Cash.ID;

        //                var local = _uow.Set<Cash>()
        //                   .Local
        //                   .FirstOrDefault(f => f.ID == item.Cash.ID);
        //                if (local != null)
        //                {
        //                    _uow.Entry(local).State = EntityState.Detached;

        //                }

        //                //_uow.Entry(item.Cash).State = EntityState.Unchanged;
        //            }
                        
        //            if (item.Bank != null)
        //            {
        //                item.BankId = item.Bank.ID;

        //                var local = _uow.Set<Bank>()
        //                   .Local
        //                   .FirstOrDefault(f => f.ID == item.Bank.ID);
        //                if (local != null)
        //                {
        //                    _uow.Entry(local).State = EntityState.Detached;

        //                }

        //                //_uow.Entry(item.Bank).State = EntityState.Unchanged;
        //            }
        //            if (item.Cheque != null && item.Type == 3)
        //            {
        //                item.ChequeId = item.Cheque.ID;

        //                if(item.Cheque.ID > 0)
        //                {
        //                    var local = _uow.Set<Cheque>()
        //                  .Local
        //                  .FirstOrDefault(f => f.ID == item.Cheque.ID);
        //                    if (local != null)
        //                    {
        //                        _uow.Entry(local).State = EntityState.Detached;

        //                    }
        //                }
                       

        //                //_uow.Entry(item.Cheque).State = EntityState.ش;
        //            }

        //            if (item.Cheque != null && item.Type == 4)
        //            {
        //                item.ChequeId = item.Cheque.ID;

        //                var localContact = _uow.Set<Contact>()
        //                 .Local
        //                 .FirstOrDefault(f => f.ID == item.Cheque.Contact.ID);
        //                if (localContact != null)
        //                {
        //                    _uow.Entry(localContact).State = EntityState.Detached;

        //                }

        //                var local = _uow.Set<Cheque>()
        //                   .Local
        //                   .FirstOrDefault(f => f.ID == item.Cheque.ID);
        //                if (local != null)
        //                {
        //                    _uow.Entry(local).State = EntityState.Detached;

        //                }

                       

        //                //_uow.Entry(item.Cheque).State = EntityState.Unchanged;
        //            }
        //        }

        //        if (payRecevie.Document != null)
        //        {
        //            if(payRecevie.Document.FinanYear != null)
        //            {
        //                payRecevie.Document.FinanYearId = payRecevie.Document.FinanYear.ID;

        //                var localfinanYear = _uow.Set<FinanYear>()
        //                    .Local
        //                    .FirstOrDefault(f => f.ID == payRecevie.Document.FinanYear.ID);
        //                if (localfinanYear != null)
        //                {
        //                    _uow.Entry(localfinanYear).State = EntityState.Detached;

        //                }
        //               // _uow.Entry(payRecevie.Document.FinanYear).State = EntityState.Unchanged;
        //            }

        //            foreach (var transaction in payRecevie.Document.Transactions)
        //            {
        //                if (transaction.Account != null)
        //                {
        //                    transaction.AccountId = transaction.Account.ID;

        //                    var localAccount = _uow.Set<DomainClasses.Accounting.Account>().Local.FirstOrDefault(x => x.ID == transaction.Account.ID);

        //                    if (localAccount != null)
        //                    {
        //                        _uow.Entry(localAccount).State = EntityState.Detached;
        //                    }
        //                   // _uow.Entry(transaction.Account).State = EntityState.Unchanged;
        //                }
        //            }
        //        }
                
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {

        //        return false;
        //    }
        //}

        public bool Update(PayRecevie payRecevie)
        {
            try
            {
                var local = _uow.Set<PayRecevie>()
                     .Local
                     .FirstOrDefault(f => f.ID == payRecevie.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _payRecevies.Attach(payRecevie);

                _uow.Entry(payRecevie).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //public async Task<PayRecevie> GetByIdAsync(int id)
        //{
        //    return await _payRecevies.AsQueryable().Where(x => x.ID == id).Include(x => x.Items).Include(x => x.Items.Select(c => c.Bank)).Include(x => x.Items.Select(c => c.Cash)).Include(x => x.Items.Select(c => c.Cheque)).Include(x => x.Contact).Include(x => x.Account).Include(x => x.Document).Include(x => x.Document.Transactions).FirstOrDefaultAsync();
        //}

        public async Task<IList<PayRecevie>> GetAllByOrganIdAsync(int organId,bool isRecevie)
        {
            var finanYear = await _finanYears.Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync();
            var documentQuery = _documents.Where(x => x.OrganId == organId && x.FinanYearId == finanYear.ID);
            var payRecevieQuery = _payRecevies.Where(x => x.OrganId == organId && x.IsReceive == isRecevie);

            var joinQuery = from document in documentQuery
                            join payRecevie in payRecevieQuery
                            on document.ID equals payRecevie.DocumentId
                            select payRecevie;
         //   return await joinQuery.ToListAsync();

         //   var payReceviesQuery = _payRecevies.AsQueryable().Where(x => x.OrganId == organId && x.IsReceive == isRecevie);//.ToListAsync();
         //   var hesabQuery = _account.AsQueryable();//.ToListAsync();

          //  var resultQuery = from payRecevies in payReceviesQuery
                              //join hesab in hesabQuery
                              //on payRecevies.HesabId equals hesab.ID into hesabGroup
                              //join hesab2 in hesabQuery
                              //on payRecevies.HesabTafziliId equals hesab2.ID into hesabTafziliIdGroup
                              //join hesab3 in hesabQuery
                              //on payRecevies.DaramadHazine equals hesab3.ID into daramadHazineGroup
               //               select new PayRecevieVM
                 //             {
                                
                                  //Amount = payRecevies.Amount,
                                  //DaramadHazine = payRecevies.DaramadHazine,
                                  //DaramadHazineOnvan = daramadHazineGroup.Any() ? daramadHazineGroup.FirstOrDefault().Onvan : "",
                                  //HesabId = payRecevies.HesabId,
                                  //HesabOnvan = hesabGroup.Any()? hesabGroup.FirstOrDefault().Onvan: "",
                                  //HesabTafziliId = payRecevies.HesabTafziliId,
                                  //HesabTafziliOnvan = hesabTafziliIdGroup.Any() ? hesabTafziliIdGroup.FirstOrDefault().Onvan : "",
                                  //ID = payRecevies.ID,
                                  //Noe = payRecevies.Noe,
                                  //NoeDreaftPardakht = payRecevies.NoeDreaftPardakht,
                                  //OrganId = payRecevies.OrganId,
                                  //RezDaramadHazine = payRecevies.RezDaramadHazine,
                                  //ShakhsId = payRecevies.ShakhsId,
                                  //Sharh = payRecevies.Sharh,
                                  //TarikhMiladi = payRecevies.Tarikh
                    //          };

            //List <PayRecevieVM> payRecevieVMs = new List<PayRecevieVM>();

            //Mapper.Map(payRecevies, payRecevieVMs);

            return await joinQuery.OrderByDescending(x => x.ID).ToListAsync();

        }

        public async Task<IList<PayRecevie>> GetByInvoiceIdAsync(int invoiceId)
        {
            return await _payRecevies.Where(x => x.InvoiceId == invoiceId).ToListAsync();
        }
        public async Task<IList<PayRecevie>> GetByCostIdAsync(int costId)
        {
            return await _payRecevies.Where(x => x.CostId == costId).ToListAsync();
        }
        public async Task<List<Document>> GetDoucumentIDByChequeIdAsync(int chequeId)
        {
            var payReceviesDetailQuery = _detailPayRecevies.Where(x => x.ChequeId == chequeId && x.Type == ZhivarEnums.DetailPayReceiveType.Cheque);
            var payReceviesQuery = _payRecevies;
            var documents = _documents;

            var joinQuery = from payReceviesDetail in payReceviesDetailQuery
                            join payRecevies in payReceviesQuery
                            on payReceviesDetail.PayRecevieId equals payRecevies.ID
                            join document in documents
                            on payRecevies.DocumentId equals document.ID
                            select document;

            return await joinQuery.ToListAsync();
        }
    }
}
