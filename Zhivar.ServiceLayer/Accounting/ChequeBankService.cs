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
    public class ChequeBankService : IChequeBank
    {
        IUnitOfWork _uow;
        readonly IDbSet<ChequeBank> _chequeBanks;
        public ChequeBankService(IUnitOfWork uow)
        {
            _uow = uow;
            _chequeBanks = _uow.Set<ChequeBank>();
        }
        public bool Delete(int id)
        {
            try
            {
                var chequeBank = GetById(id);
                Delete(chequeBank);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Delete(ChequeBank chequeBank)
        {
            try
            {
                _chequeBanks.Attach(chequeBank);
                _chequeBanks.Remove(chequeBank);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        public IList<ChequeBank> GetAll()
        {
            return _chequeBanks.ToList();
        }
        public IList<ChequeBank> GetAllByOrganId(int organId)
        {
            var chequeBanks = _chequeBanks.AsQueryable().Where(x => x.OrganId == organId).ToList();

            return chequeBanks;
        }
        public async Task<IList<ChequeBank>> GetAllByOrganIdAsync(int organId)
        {
            var chequeBanks = await _chequeBanks.AsQueryable().Where(x => x.OrganId == organId).ToListAsync();

            return chequeBanks;

        }
        public async Task<IList<ChequeBank>> GetAllAsync()
        {
            return await _chequeBanks.ToListAsync();
        }
        public ChequeBank GetById(int id)
        {
            return _chequeBanks.Find(id);
        }

        public bool Insert(ChequeBank chequeBank)
        {
            try
            {
                _chequeBanks.Add(chequeBank);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(ChequeBank chequeBank)
        {
            try
            {
                _chequeBanks.Attach(chequeBank);

                _uow.Entry(chequeBank).State = EntityState.Modified;
                //_uow.Entry(chequeBank).Property(p => p.CreatedOn).IsModified = false;
                //_uow.Entry(chequeBank).Property(p => p.CreatedBy).IsModified = false;
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ChequeBank> GetByIdAsync(int id)
        {
            return await _chequeBanks.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }


    }
}
