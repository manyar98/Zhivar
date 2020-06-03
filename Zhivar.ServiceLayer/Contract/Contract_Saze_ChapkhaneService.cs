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

    public class Contract_Saze_ChapkhaneService : IContract_Saze_Chapkhane
    {
        IUnitOfWork _uow;
        readonly IDbSet<Contract_Saze_Chapkhane> _contract_Saze_Chapkhanes;
        public Contract_Saze_ChapkhaneService(IUnitOfWork uow)
        {
            _uow = uow;
            _contract_Saze_Chapkhanes = _uow.Set<Contract_Saze_Chapkhane>();
        }
        public bool Delete(int id)
        {
            try
            {
                var contract_Saze_Chapkhane = GetById(id);
                Delete(contract_Saze_Chapkhane);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Contract_Saze_Chapkhane contract_Saze_Chapkhane)
        {
            try
            {
                _contract_Saze_Chapkhanes.Attach(contract_Saze_Chapkhane);
                _contract_Saze_Chapkhanes.Remove(contract_Saze_Chapkhane);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<Contract_Saze_Chapkhane> GetAll()
        {
            return _contract_Saze_Chapkhanes.ToList();
        }
        //public IList<Contract_Saze_Chapkhane> GetAllByOrganId(int organId)
        //{
        //    return _contract_Saze_Chapkhanes.AsQueryable().Where(x => x.Person.ID == organId).ToList();
        //}
        //public async Task<IList<Contract_Saze_Chapkhane>> GetAllByOrganIdAsync(int organId)
        //{
        //    return await _contract_Saze_Chapkhanes.AsQueryable().Where(x => x.Person.ID == organId).ToListAsync();
        //}
        public async Task<IList<Contract_Saze_Chapkhane>> GetAllAsync()
        {
            return await _contract_Saze_Chapkhanes.ToListAsync();
        }
        public Contract_Saze_Chapkhane GetById(int id)
        {
            return _contract_Saze_Chapkhanes.Find(id);
        }

        public bool Insert(Contract_Saze_Chapkhane contract_Saze_Chapkhane)
        {
            try
            {
                _contract_Saze_Chapkhanes.Add(contract_Saze_Chapkhane);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Contract_Saze_Chapkhane contract_Saze_Chapkhane)
        {
            try
            {
                var local = _uow.Set<Contract_Saze_Chapkhane>()
                     .Local
                     .FirstOrDefault(f => f.ID == contract_Saze_Chapkhane.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _contract_Saze_Chapkhanes.Attach(contract_Saze_Chapkhane);

                _uow.Entry(contract_Saze_Chapkhane).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Contract_Saze_Chapkhane> GetByIdAsync(int id)
        {
            return await _contract_Saze_Chapkhanes.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }
    }
}
