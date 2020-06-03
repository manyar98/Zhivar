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
    public class Contarct_Saze_BazareabService : IContarct_Saze_Bazareab
    {
        IUnitOfWork _uow;
        readonly IDbSet<Contract_Saze_Bazareab> _contarct_Saze_Bazareabs;
        public Contarct_Saze_BazareabService(IUnitOfWork uow)
        {
            _uow = uow;
            _contarct_Saze_Bazareabs = _uow.Set<Contract_Saze_Bazareab>();
        }
        public bool Delete(int id)
        {
            try
            {
                var contarct_Saze_Bazareab = GetById(id);
                Delete(contarct_Saze_Bazareab);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Contract_Saze_Bazareab contarct_Saze_Bazareab)
        {
            try
            {
                _contarct_Saze_Bazareabs.Attach(contarct_Saze_Bazareab);
                _contarct_Saze_Bazareabs.Remove(contarct_Saze_Bazareab);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<Contract_Saze_Bazareab> GetAll()
        {
            return _contarct_Saze_Bazareabs.ToList();
        }
        //public IList<Contarct_Saze_Bazareab> GetAllByOrganId(int organId)
        //{
        //    return _contarct_Saze_Bazareabs.AsQueryable().Where(x => x.Person.ID == organId).ToList();
        //}
        //public async Task<IList<Contarct_Saze_Bazareab>> GetAllByOrganIdAsync(int organId)
        //{
        //    return await _contarct_Saze_Bazareabs.AsQueryable().Where(x => x.Person.ID == organId).ToListAsync();
        //}
        public async Task<IList<Contract_Saze_Bazareab>> GetAllAsync()
        {
            return await _contarct_Saze_Bazareabs.ToListAsync();
        }
        public Contract_Saze_Bazareab GetById(int id)
        {
            return _contarct_Saze_Bazareabs.Find(id);
        }

        public bool Insert(Contract_Saze_Bazareab contarct_Saze_Bazareab)
        {
            try
            {
                _contarct_Saze_Bazareabs.Add(contarct_Saze_Bazareab);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Contract_Saze_Bazareab contarct_Saze_Bazareab)
        {
            try
            {
                var local = _uow.Set<Contract_Saze_Bazareab>()
                     .Local
                     .FirstOrDefault(f => f.ID == contarct_Saze_Bazareab.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _contarct_Saze_Bazareabs.Attach(contarct_Saze_Bazareab);

                _uow.Entry(contarct_Saze_Bazareab).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Contract_Saze_Bazareab> GetByIdAsync(int id)
        {
            return await _contarct_Saze_Bazareabs.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }
    }
}
