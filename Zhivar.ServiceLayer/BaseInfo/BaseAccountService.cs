using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ServiceLayer.Contracts.Accunting;

namespace Zhivar.ServiceLayer.BaseInfo
{  

    public class BaseaccountRule : IBaseAccount
    {
        IUnitOfWork _uow;
        readonly IDbSet<BaseAccount> _baseAccounts;


        public BaseaccountRule(IUnitOfWork uow)
        {
            _uow = uow;
            _baseAccounts = _uow.Set<BaseAccount>();
        }

        public bool Delete(int id)
        {
            try
            {
                var baseAccount = GetById(id);
                Delete(baseAccount);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(BaseAccount baseAccount)
        {
            try
            {
                _baseAccounts.Attach(baseAccount);
                _baseAccounts.Remove(baseAccount);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<BaseAccount> GetAll()
        {
            return _baseAccounts.ToList();
        }

        public async Task<IList<BaseAccount>> GetAllAsync()
        {
            return await _baseAccounts.ToListAsync();
        }
        public BaseAccount GetById(int id)
        {
            return _baseAccounts.Find(id);
        }

        public bool Insert(BaseAccount baseAccount)
        {
            try
            {
                _baseAccounts.Add(baseAccount);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(BaseAccount baseAccount)
        {
            try
            {
                var local = _uow.Set<BaseAccount>()
                     .Local
                     .FirstOrDefault(f => f.ID == baseAccount.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                //var local2 = _uow.Set<Saze>()
                //  .Local
                //  .FirstOrDefault(f => f.ID == baseAccount.Sazes.ID);
                //if (local2 != null)
                //{
                //    _uow.Entry(local).State = EntityState.Detached;
                //}
                _baseAccounts.Attach(baseAccount);

                _uow.Entry(baseAccount).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<BaseAccount> GetByIdAsync(int id)
        {
            return await _baseAccounts.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }
    }
}
