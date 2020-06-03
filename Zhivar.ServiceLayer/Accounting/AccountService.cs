using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ServiceLayer.Contracts.Accunting;
using System.Data.Entity;

namespace Zhivar.ServiceLayer.Accunting
{
    public class AccountService : IAccount
    {
        IUnitOfWork _uow;
        readonly IDbSet<DomainClasses.Accounting.Account> _accounts;
        public AccountService(IUnitOfWork uow)
        {
            _uow = uow;
            _accounts = _uow.Set<DomainClasses.Accounting.Account>();
        }
        public bool Delete(int id)
        {
            try
            {
                var account = GetById(id);
                Delete(account);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(DomainClasses.Accounting.Account account)
        {
            try
            {
                _accounts.Attach(account);
                _accounts.Remove(account);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<DomainClasses.Accounting.Account> GetAll()
        {

            return _accounts.ToList();
        }
       
        public async Task<List<DomainClasses.Accounting.Account>> GetAllByOrganIdAsync(int organId)
        {
            return await _accounts.AsQueryable().Where(x => x.OrganId == organId).ToListAsync();
        }
        public async Task<IList<DomainClasses.Accounting.Account>> GetAllAsync()
        {
            return await _accounts.ToListAsync();
        }
        public DomainClasses.Accounting.Account GetById(int id)
        {
            return _accounts.Find(id);
        }

        public bool Insert(DomainClasses.Accounting.Account account)
        {
            try
            {
                _accounts.Add(account);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(DomainClasses.Accounting.Account account)
        {
            try
            {
                var local = _uow.Set<DomainClasses.Accounting.Account>()
                     .Local
                     .FirstOrDefault(f => f.ID == account.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _accounts.Attach(account);

                _uow.Entry(account).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<DomainClasses.Accounting.Account> GetByIdAsync(int id)
        {
            return await _accounts.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task DeleteAccountByComplteCodingAsync(string completeCode)
        {
            var account = await _accounts.AsQueryable().Where(x => x.ComplteCoding == completeCode).SingleOrDefaultAsync();

            if(account != null)
            {
                Delete(account);
            }
        }
    }
}
