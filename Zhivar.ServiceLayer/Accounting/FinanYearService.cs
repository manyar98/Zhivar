using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ServiceLayer.Contracts.BaseInfo;

namespace Zhivar.ServiceLayer.Accunting
{
    public class FinanYearService : IFinanYear
    {
        IUnitOfWork _uow;
        readonly IDbSet<FinanYear> _finanYears;
        public FinanYearService(IUnitOfWork uow)
        {
            _uow = uow;
            _finanYears = _uow.Set<FinanYear>();
        }
        public bool Delete(int id)
        {
            try
            {
                var finanYear = GetById(id);
                Delete(finanYear);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Delete(FinanYear finanYear)
        {
            try
            {
                _finanYears.Attach(finanYear);
                _finanYears.Remove(finanYear);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        public IList<FinanYear> GetAll()
        {
            return _finanYears.ToList();
        }
        public IList<FinanYear> GetAllByOrganId(int organId)
        {
            var finanYears = _finanYears.AsQueryable().Where(x => x.OrganId == organId).ToList();

            return finanYears;
        }
        public async Task<IList<FinanYear>> GetAllByOrganIdAsync(int organId)
        {
            var finanYears = await _finanYears.AsQueryable().Where(x => x.OrganId == organId).ToListAsync();

            return finanYears;

        }
        public async Task<IList<FinanYear>> GetAllAsync()
        {
            return await _finanYears.ToListAsync();
        }
        public FinanYear GetById(int id)
        {
            return _finanYears.Find(id);
        }

        public bool Insert(FinanYear finanYear)
        {
            try
            {
                _finanYears.Add(finanYear);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(FinanYear finanYear)
        {
            try
            {
                _finanYears.Attach(finanYear);

                _uow.Entry(finanYear).State = EntityState.Modified;
                //_uow.Entry(finanYear).Property(p => p.CreatedOn).IsModified = false;
                //_uow.Entry(finanYear).Property(p => p.CreatedBy).IsModified = false;
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<FinanYear> GetByIdAsync(int id)
        {
            return await _finanYears.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task<FinanYear> GetCurrentFinanYear(int organId)
        {
            return await _finanYears.AsQueryable().Where(x => x.OrganId == organId && x.Closed == false).SingleOrDefaultAsync();
        }
    }
}
