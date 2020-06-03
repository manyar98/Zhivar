using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ServiceLayer.Contracts.BaseInfo;

namespace Zhivar.ServiceLayer.Accunting
{
    public class TransferMoneyService : ITransferMoney
    {
        IUnitOfWork _uow;
        readonly IDbSet<TransferMoney> _transferMoneys;
        readonly IDbSet<Bank> _banks;
        readonly IDbSet<Cash> _cash;
        readonly IDbSet<Document> _documents;
        readonly IDbSet<Transaction> _transactions;
        readonly IDbSet<DomainClasses.Accounting.Account> _accounts;
        readonly IDbSet<FinanYear> _finanYears;

        public TransferMoneyService(IUnitOfWork uow)
        {
            _uow = uow;
            _transferMoneys = _uow.Set<TransferMoney>();
            _banks = _uow.Set<Bank>();
            _cash = _uow.Set<Cash>();
            _documents = _uow.Set<Document>();
            _transactions = _uow.Set<Transaction>();
            _accounts = _uow.Set<DomainClasses.Accounting.Account>();
            _finanYears = _uow.Set<FinanYear>();

        }
        public async Task<TransferMoney> Delete(int id)
        {
            try
            {
                var transferMoney = await GetByIdAsync(id);

               //if (transferMoney.Document != null)
               //  {
               //    foreach (var transaction in transferMoney.Document.Transactions.ToList())
               //     {
               //         _uow.Entry(transaction).State = EntityState.Deleted;
               //     }
               //     _uow.Entry(transferMoney).State = EntityState.Deleted;
               // }
                



                await Delete(transferMoney);
                return transferMoney;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<TransferMoney> Delete(TransferMoney transferMoney)
        {
            try
            {
                _transferMoneys.Attach(transferMoney);


                if (transferMoney.Document != null)
                {
                    foreach (var transaction in transferMoney.Document.Transactions.ToList())
                    {
                        _transactions.Remove(transaction);
                    }
                    _documents.Remove(transferMoney.Document);
                }

                _transferMoneys.Remove(transferMoney);

                return transferMoney;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //public IList<TransferMoney> GetAll()
        //{
        //    return _transferMoneys.ToList();
        //}
        public async Task<IList<TransferMoney>> GetAllByOrganIdAsync(int organId)
        {
            var finanYear = await _finanYears.Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync();
          

            var transferMoneys = await _transferMoneys.AsQueryable().Where(x => x.OrganId == organId ).ToListAsync();

            foreach (var transfer in transferMoneys)
            {
                if (transfer.From == "bank")
                {
                    int id = Convert.ToInt32(transfer.FromDetailAccountId);
                    transfer.FromDetailAccountName = _accounts.SingleOrDefault(x => x.ID == id).Name;
                }
                else
                {
                    int id = Convert.ToInt32(transfer.FromDetailAccountId);
                    transfer.FromDetailAccountName = _accounts.SingleOrDefault(x => x.ID == id).Name;
                    
                }
                if (transfer.To == "bank")
                {
                    int id = Convert.ToInt32(transfer.ToDetailAccountId);
                    transfer.ToDetailAccountName = _accounts.SingleOrDefault(x => x.ID == id).Name;
                }
                else
                {
                    int id = Convert.ToInt32(transfer.ToDetailAccountId);
                    transfer.ToDetailAccountName = _accounts.SingleOrDefault(x => x.ID == id).Name;

                }
                if (transfer.DocumentId != null)
                {
                    transfer.DocumentNumber = _documents.SingleOrDefault(x => x.ID == transfer.DocumentId).Number;
                }
            }
            return transferMoneys;
        }
     
        //public async Task<IList<TransferMoney>> GetAllAsync()
        //{
        //    return await _transferMoneys.ToListAsync();
        //}
        //public TransferMoney GetById(int id)
        //{
        //    return _transferMoneys.Where(x => x.ID == id).Include(x => x.Document).Include(x => x.Document.Transactions).SingleOrDefault();
        //}

        public async Task<bool> Insert(TransferMoney transferMoney)
        {
            try
            {
                //var finanYear = await _finanYears.Where(x => x.Closed == false).SingleOrDefaultAsync();
                //transferMoney.FinanYear = finanYear;
                //transferMoney.FinanYearId = finanYear.ID;
                _transferMoneys.Add(transferMoney);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(TransferMoney transferMoney)
        {
            try
            {
                if (transferMoney.Document.ID > 0)
                {

                    var local = _uow.Set<Document>()
                         .Local
                         .FirstOrDefault(f => f.ID == transferMoney.Document.ID);
                    if (local != null)
                    {
                        _uow.Entry(local).State = EntityState.Detached;
                    }

                    foreach (var transaction in transferMoney.Document.Transactions.ToList() ?? new List<Transaction>())
                    {
                        if (transaction.ID > 0)
                        {

                            var localTransaction = _uow.Set<Transaction>()
                                 .Local
                                 .FirstOrDefault(f => f.ID == transaction.ID);
                            if (localTransaction != null)
                            {
                                _uow.Entry(local).State = EntityState.Detached;
                            }

                            _uow.Entry(transaction).State = EntityState.Modified;
                        }
                        else
                        {

                            _uow.Entry(transaction).State = EntityState.Added;
                        }
                    }

                    _uow.Entry(transferMoney.Document).State = EntityState.Modified;
                }

               // _transferMoneys.Attach(transferMoney);

                _uow.Entry(transferMoney).State = EntityState.Modified;
                //_uow.Entry(transferMoney).Property(p => p.CreatedOn).IsModified = false;
                //_uow.Entry(transferMoney).Property(p => p.CreatedBy).IsModified = false;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<TransferMoney> GetByIdAsync(int id)
        {
            return await _transferMoneys.AsQueryable().Where(x => x.ID == id).Include(x => x.Document).Include(x => x.Document.Transactions).FirstOrDefaultAsync();
        }

        public async Task<TransferMoney> GetByDocIdAsync(int docId)
        {
            var transfer = await _transferMoneys.AsQueryable().Where(x => x.DocumentId == docId).Include(x => x.Document.Transactions).SingleOrDefaultAsync();


                if (transfer.From == "bank")
                {
                    int id = Convert.ToInt32(transfer.FromDetailAccountId);
                    transfer.FromDetailAccountName = _accounts.SingleOrDefault(x => x.ID == id).Name;
                }
                else
                {
                    int id = Convert.ToInt32(transfer.FromDetailAccountId);
                    transfer.FromDetailAccountName = _accounts.SingleOrDefault(x => x.ID == id).Name;

                }
                if (transfer.To == "bank")
                {
                    int id = Convert.ToInt32(transfer.ToDetailAccountId);
                    transfer.ToDetailAccountName = _accounts.SingleOrDefault(x => x.ID == id).Name;
                }
                else
                {
                    int id = Convert.ToInt32(transfer.ToDetailAccountId);
                    transfer.ToDetailAccountName = _accounts.SingleOrDefault(x => x.ID == id).Name;

                }
                if (transfer.DocumentId != null)
                {
                    transfer.DocumentNumber = _documents.SingleOrDefault(x => x.ID == transfer.DocumentId).Number;
                }
            
            return transfer;
        }


    }
}
