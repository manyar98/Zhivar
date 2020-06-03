using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ServiceLayer.Contracts.BaseInfo;

namespace Zhivar.ServiceLayer.Accunting
{
    public class BankService : IBank
    {
        IUnitOfWork _uow;
        readonly IDbSet<Bank> _banks;
        readonly IDbSet<DomainClasses.Accounting.Account> _accounts;

        public BankService(IUnitOfWork uow)
        {
            _uow = uow;
            _banks = _uow.Set<Bank>();
            _accounts = _uow.Set<DomainClasses.Accounting.Account>();
        }
        public bool Delete(int id)
        {
            try
            {
                var bank = GetById(id);
                Delete(bank);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Delete(Bank bank)
        {
            try
            {
                _banks.Attach(bank);
                _banks.Remove(bank);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        public IList<Bank> GetAll()
        {
            return _banks.ToList();
        }
        public IList<Bank> GetAllByOrganId(int organId)
        {
            var banks = _banks.AsQueryable().Where(x => x.OrganId == organId).ToList();

            return banks;
        }
        public async Task<IList<Bank>> GetAllByOrganIdAsync(int organId)
        {
            var banks = await _banks.AsQueryable().Where(x => x.OrganId == organId).ToListAsync();

            return banks;

        }
        public async Task<IList<Bank>> GetAllAsync()
        {
            return await _banks.ToListAsync();
        }
        public Bank GetById(int id)
        {
            return _banks.Find(id);
        }

        public bool Insert(Bank bank)
        {
            try
            {
                _banks.Add(bank);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Bank bank)
        {
            try
            {
                _banks.Attach(bank);

                _uow.Entry(bank).State = EntityState.Modified;
                //_uow.Entry(bank).Property(p => p.CreatedOn).IsModified = false;
                //_uow.Entry(bank).Property(p => p.CreatedBy).IsModified = false;
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Bank> GetByIdAsync(int id)
        {
            return await _banks.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task<Bank> GetByAccountId(int accountId)
        {
            var account = await _accounts.Where(x => x.ID == accountId).SingleOrDefaultAsync();

            return await _banks.SingleOrDefaultAsync(x => x.Code == account.Coding);
        }
    }
}
