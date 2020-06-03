using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.Contract;
using Zhivar.ServiceLayer.Contracts.Contract;
using System.Data.Entity;
using Zhivar.ViewModel.Contract;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.Contract
{
    public class ContractService :IContract
    {
        IUnitOfWork _uow;
        readonly IDbSet<DomainClasses.Contract.Contract> _contracts;

        public ContractService(IUnitOfWork uow)
        {
            _uow = uow;
            _contracts = _uow.Set<DomainClasses.Contract.Contract>();
        }
        public bool Delete(int id)
        {
            try
            {
                var contract = GetById(id);
                Delete(contract);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(DomainClasses.Contract.Contract contract)
        {
            try
            {
                _contracts.Attach(contract);
                _contracts.Remove(contract);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<DomainClasses.Contract.Contract> GetAll()
        {
            return _contracts.ToList();
        }
        public IList<DomainClasses.Contract.Contract> GetAllByOrganId(int organId)
        {
            return _contracts.AsQueryable().Where(x => x.OrganId == organId).ToList();
        }
        public DomainClasses.Contract.Contract GetAllSazeByContractId(int contractId)
        {
            return _contracts.AsQueryable().Where(x => x.ID == contractId)
                     //.Include(x => x.Contract_Sazes)
                     //.Include(x => x.Contract_Sazes.Select(y => y.Contarct_Saze_Bazareabs))
                     // .Include(x => x.Contract_Sazes.Select(y => y.Contract_Saze_Nasabs))
                     //  .Include(x => x.Contract_Sazes.Select(y => y.Contract_Saze_Tarahs))
                     //.Include(x => x.Contract_Sazes.Select(y => y.Contract_Saze_Chapkhanes))
                    .SingleOrDefault();

        }
        public async Task<DomainClasses.Contract.Contract> GetAllSazeByContractIdAsync(int contractId)
        {
            return await _contracts.AsQueryable().Where(x => x.ID == contractId)
                .Include("Contract_Sazes")
               // .Include(x => x.Contract_Sazes.Select(y => y.Contarct_Saze_Bazareabs))
               // .Include(x => x.Contract_Sazes.Select(y => y.Contract_Saze_Nasabs))
               // .Include(x => x.Contract_Sazes.Select(y => y.Contract_Saze_Tarahs))
               // .Include(x => x.Contract_Sazes.Select(y => y.Contract_Saze_Chapkhanes))
                .SingleOrDefaultAsync();
        }
        public async Task<IList<DomainClasses.Contract.Contract>> GetAllByOrganIdAsync(int organId)
        {
            return await _contracts.AsQueryable().Where(x => x.OrganId == organId)
          // .Include(x => x.Contract_Sazes)
          // .Include(x => x.Contract_Sazes.Select(y => y.Contarct_Saze_Bazareabs))
          // .Include(x => x.Contract_Sazes.Select(y => y.Contract_Saze_Nasabs))
          //  .Include(x => x.Contract_Sazes.Select(y => y.Contract_Saze_Tarahs))
          // .Include(x => x.Contract_Sazes.Select(y => y.Contract_Saze_Chapkhanes))
           .ToListAsync();
        }
        public async Task<IList<DomainClasses.Contract.Contract>> GetAllAsync()
        {
            return await _contracts.ToListAsync();
        }
        public DomainClasses.Contract.Contract GetById(int id)
        {
            return _contracts.Find(id);
        }

        public bool Insert(DomainClasses.Contract.Contract contract)
        {
            try
            {
                _contracts.Add(contract);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(DomainClasses.Contract.Contract contract)
        {
            try
            {
                var oldContract = _uow.Set<DomainClasses.Contract.Contract>()
                     .Local
                     .FirstOrDefault(f => f.ID == contract.ID);
                if (oldContract != null)
                {
                    _uow.Entry(oldContract).State = EntityState.Detached;
                }

                //foreach (var contract_Saze in contract.Contract_Sazes ?? new List<Contract_Saze>())
                //{
                //    var oldContract_Saze = _uow.Set<Contract_Saze>().Local.FirstOrDefault(f => f.ID == contract_Saze.ID);
                //    if (oldContract_Saze != null)
                //    {
                //        _uow.Entry(oldContract_Saze).State = EntityState.Detached;

                //        //if (contract_Saze.Contarct_Saze_Bazareabs != null)
                //        //{
                //        //    var oldContarct_Saze_Bazareab = _uow.Set<Contract_Saze_Bazareab>().Local.ToList(f => f.ID == contract_Saze.Contarct_Saze_Bazareabs.ID);
                //        //    if (oldContarct_Saze_Bazareab != null)
                //        //    {
                //        //        _uow.Entry(oldContarct_Saze_Bazareab).State = EntityState.Detached;
                //        //    }
                //        //}

                //        //if (contract_Saze.Contract_Saze_Tarahs != null)
                //        //{
                //        //    var oldContract_Saze_Tarah = _uow.Set<Contract_Saze_Tarah>().Local.FirstOrDefault(f => f.ID == contract_Saze.Contract_Saze_Tarah.ID);
                //        //    if (oldContract_Saze_Tarah != null)
                //        //    {
                //        //        _uow.Entry(oldContract_Saze_Tarah).State = EntityState.Detached;
                //        //    }
                //        //}

                //        //if (contract_Saze.Contract_Saze_Nasabs != null)
                //        //{
                //        //    var oldContract_Saze_Nasab = _uow.Set<Contract_Saze_Nasab>().Local.FirstOrDefault(f => f.ID == contract_Saze.Contract_Saze_Nasab.ID);
                //        //    if (oldContract_Saze_Nasab != null)
                //        //    {
                //        //        _uow.Entry(oldContract_Saze_Nasab).State = EntityState.Detached;
                //        //    }
                //        //}

                //        //if (contract_Saze.Contract_Saze_Chapkhanes != null)
                //        //{
                //        //    var oldContract_Saze_Chapkhane = _uow.Set<Contract_Saze_Chapkhane>().Local.FirstOrDefault(f => f.ID == contract_Saze.Contract_Saze_Chapkhane.ID);
                //        //    if (oldContract_Saze_Chapkhane != null)
                //        //    {
                //        //        _uow.Entry(oldContract_Saze_Chapkhane).State = EntityState.Detached;
                //        //    }
                //        //}
                //    }
                //}
                _contracts.Attach(contract);

                _uow.Entry(contract).State = EntityState.Modified;

                //foreach (var Contract_Saze in contract.Contract_Sazes)
                //{
               
                //        foreach (var Contarct_Saze_Bazareab in Contract_Saze.Contarct_Saze_Bazareabs)
                //        {
                //            if (Contarct_Saze_Bazareab.ID > 0)
                //            {
                //                _uow.Entry(Contarct_Saze_Bazareab).State = EntityState.Modified;
                //            }
                //            else
                //            {
                //                _uow.Entry(Contarct_Saze_Bazareab).State = EntityState.Added;
                //            }
                //        }

                //        foreach (var Contract_Saze_Nasab in Contract_Saze.Contract_Saze_Nasabs)
                //        {


                //            if (Contract_Saze_Nasab.ID > 0)
                //            {
                //                _uow.Entry(Contract_Saze_Nasab).State = EntityState.Modified;
                //            }
                //            else
                //            {
                //                _uow.Entry(Contract_Saze_Nasab).State = EntityState.Added;
                //            }
                //        }

                //        foreach (var Contract_Saze_Chapkhane in Contract_Saze.Contract_Saze_Chapkhanes)
                //        {
                //            if (Contract_Saze_Chapkhane.ID > 0)
                //            {
                //                _uow.Entry(Contract_Saze_Chapkhane).State = EntityState.Modified;
                //            }
                //            else
                //            {
                //                _uow.Entry(Contract_Saze_Chapkhane).State = EntityState.Added;
                //            }
                //        }

                //        foreach (var Contract_Saze_Tarah in Contract_Saze.Contract_Saze_Tarahs)
                //        {
                //            if (Contract_Saze_Tarah.ID > 0)
                //            {
                //                _uow.Entry(Contract_Saze_Tarah).State = EntityState.Modified;
                //            }
                //            else
                //            {
                //                _uow.Entry(Contract_Saze_Tarah).State = EntityState.Added;
                //            }
                //        }
                //    if (Contract_Saze.ID > 0)
                //    {
                //        _uow.Entry(Contract_Saze).State = EntityState.Modified;
                //    }
                //    else
                    
                //        _uow.Entry(Contract_Saze).State = EntityState.Added;
                //    }
                
                return true;


            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<DomainClasses.Contract.Contract> GetByIdAsync(int id)
        {
            return await _contracts.AsQueryable().Where(x => x.ID == id)
               // .Include(x => x.Contract_Sazes.Select(y => y.Contarct_Saze_Bazareabs))
               // .Include(x => x.Contract_Sazes.Select(y => y.Contract_Saze_Nasabs))
               // .Include(x => x.Contract_Sazes.Select(y => y.Contract_Saze_Tarahs))
              //  .Include(x => x.Contract_Sazes.Select(y => y.Contract_Saze_Chapkhanes))
                .SingleOrDefaultAsync();
        }
    }
}