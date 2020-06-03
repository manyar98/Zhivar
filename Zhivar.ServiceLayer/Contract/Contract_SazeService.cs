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
    public class ContractsazeRule : IContract_Saze
    {
        IUnitOfWork _uow;
        readonly IDbSet<Contract_Saze> _contract_Sazes;
        public ContractsazeRule(IUnitOfWork uow)
        {
            _uow = uow;
            _contract_Sazes = _uow.Set<Contract_Saze>();
        }
        public bool Delete(int id)
        {
            try
            {
                var contract_Saze = GetById(id);
                Delete(contract_Saze);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Contract_Saze contract_Saze)
        {
            try
            {
                _contract_Sazes.Attach(contract_Saze);

                

                _contract_Sazes.Remove(contract_Saze);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<Contract_Saze> GetAll()
        {
            return _contract_Sazes.ToList();
        }
        //public IList<Contract_Saze> GetAllByOrganId(int organId)
        //{
        //    return _contract_Sazes.AsQueryable().Where(x => x.Person.ID == organId).ToList();
        //}
        //public async Task<IList<Contract_Saze>> GetAllByOrganIdAsync(int organId)
        //{
        //    return await _contract_Sazes.AsQueryable().Where(x => x.Person.ID == organId).ToListAsync();
        //}
        public async Task<IList<Contract_Saze>> GetAllAsync()
        {
            return await _contract_Sazes.ToListAsync();
         
        }
        public Contract_Saze GetById(int id)
        {
            return _contract_Sazes.AsQueryable().Where(x => x.ID == id)
                .Include(x => x.Contarct_Saze_Bazareabs)
                .Include(x => x.Contract_Saze_Chapkhanes)
                .Include(x => x.Contract_Saze_Nasabs)
                 .Include(x => x.Contract_Saze_Tarahs)
                .SingleOrDefault();
        }

        public bool Insert(Contract_Saze contract_Saze)
        {
            try
            {
                _contract_Sazes.Add(contract_Saze);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Contract_Saze contract_Saze)
        {
            try
            {
                var local = _uow.Set<Contract_Saze>()
                     .Local
                     .FirstOrDefault(f => f.ID == contract_Saze.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _contract_Sazes.Attach(contract_Saze);

                _uow.Entry(contract_Saze).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Contract_Saze> GetByIdAsync(int id)
        {
            return await _contract_Sazes.AsQueryable().Where(x => x.ID == id)
           .Include(x => x.Contarct_Saze_Bazareabs)
           .Include(x => x.Contract_Saze_Chapkhanes)
           .Include(x => x.Contract_Saze_Nasabs)
            .Include(x => x.Contract_Saze_Tarahs)
           .SingleOrDefaultAsync();

        }
    }
}
