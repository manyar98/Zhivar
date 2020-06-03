using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.Contract;
using Zhivar.ServiceLayer.Contracts.Contract;
using System.Data.Entity;

namespace Zhivar.ServiceLayer.Contract
{
    public class Contract_Saze_NasabService : IContract_Saze_Nasab
    {
        IUnitOfWork _uow;
        readonly IDbSet<Contract_Saze_Nasab> _contract_Saze_Nasabs;
        public Contract_Saze_NasabService(IUnitOfWork uow)
        {
            _uow = uow;
            _contract_Saze_Nasabs = _uow.Set<Contract_Saze_Nasab>();
        }
        public bool Delete(int id)
        {
            try
            {
                var contract_Saze_Nasab = GetById(id);
                Delete(contract_Saze_Nasab);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Contract_Saze_Nasab contract_Saze_Nasab)
        {
            try
            {
                _contract_Saze_Nasabs.Attach(contract_Saze_Nasab);
                _contract_Saze_Nasabs.Remove(contract_Saze_Nasab);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<Contract_Saze_Nasab> GetAll()
        {
            return _contract_Saze_Nasabs.ToList();
        }
        //public IList<Contract_Saze_Nasab> GetAllByOrganId(int organId)
        //{
        //    return _contract_Saze_Nasabs.AsQueryable().Where(x => x.Person.ID == organId).ToList();
        //}
        //public async Task<IList<Contract_Saze_Nasab>> GetAllByOrganIdAsync(int organId)
        //{
        //    return await _contract_Saze_Nasabs.AsQueryable().Where(x => x.Person.ID == organId).ToListAsync();
        //}
        public async Task<IList<Contract_Saze_Nasab>> GetAllAsync()
        {
            return await _contract_Saze_Nasabs.ToListAsync();
        }
        public Contract_Saze_Nasab GetById(int id)
        {
            return _contract_Saze_Nasabs.Find(id);
        }

        public bool Insert(Contract_Saze_Nasab contract_Saze_Nasab)
        {
            try
            {
                _contract_Saze_Nasabs.Add(contract_Saze_Nasab);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Contract_Saze_Nasab contract_Saze_Nasab)
        {
            try
            {
                var local = _uow.Set<Contract_Saze_Nasab>()
                     .Local
                     .FirstOrDefault(f => f.ID == contract_Saze_Nasab.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _contract_Saze_Nasabs.Attach(contract_Saze_Nasab);

                _uow.Entry(contract_Saze_Nasab).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Contract_Saze_Nasab> GetByIdAsync(int id)
        {
            return await _contract_Saze_Nasabs.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }
    }
}
