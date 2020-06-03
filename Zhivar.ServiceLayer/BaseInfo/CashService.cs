using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ServiceLayer.Contracts.Accunting;
using System.Data.Entity;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.BaseInfo
{
    public class CashService : ICash
    {
        IUnitOfWork _uow;
        readonly IDbSet<Cash> _cashs;
        readonly IDbSet<DomainClasses.Accounting.Account> _accounts;
        public CashService(IUnitOfWork uow)
        {
            _uow = uow;
            _cashs = _uow.Set<Cash>();
            _accounts = _uow.Set<DomainClasses.Accounting.Account>();
        }
        public bool Delete(int id)
        {
            try
            {
                var cash = GetById(id);
                Delete(cash);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Cash cash)
        {
            try
            {
                _cashs.Attach(cash);
                _cashs.Remove(cash);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<Cash> GetAll()
        {
            return _cashs.ToList();
        }
        public IList<Cash> GetAllByOrganId(int organId)
        {
            return _cashs.AsQueryable().Where(x => x.OrganId == organId).ToList();
        }
        public async Task<IList<Cash>> GetAllByOrganIdAsync(int organId)
        {
            return await _cashs.AsQueryable().Where(x => x.OrganId == organId).ToListAsync();
        }
        public async Task<IList<Cash>> GetAllAsync()
        {
            return await _cashs.ToListAsync();
        }
        public Cash GetById(int id)
        {
            return _cashs.Find(id);
        }

        public bool Insert(Cash cash)
        {
            try
            {
                _cashs.Add(cash);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Cash cash)
        {
            try
            {
                var local = _uow.Set<Cash>()
                     .Local
                     .FirstOrDefault(f => f.ID == cash.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _cashs.Attach(cash);

                _uow.Entry(cash).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Cash> GetByIdAsync(int id)
        {
            return await _cashs.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task<Cash> GetByAccountId(int accountId)
        {

            var account = await _accounts.Where(x => x.ID == accountId).SingleOrDefaultAsync();

            return await _cashs.SingleOrDefaultAsync(x => x.Code == account.Coding);

        }
    }
}
