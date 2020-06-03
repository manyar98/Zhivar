using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using System.Data.Entity;

namespace Zhivar.ServiceLayer.BaseInfo
{
    public class VahedTolService : IVahedTol
    {
        IUnitOfWork _uow;
        readonly IDbSet<VahedTol> _vahedTols;
        public VahedTolService(IUnitOfWork uow)
        {
            _uow = uow;
            _vahedTols = _uow.Set<VahedTol>();
        }
        public bool Delete(int id)
        {
            try
            {
                var vahedTol = GetById(id);
                Delete(vahedTol);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(VahedTol vahedTol)
        {
            try
            {
                _vahedTols.Attach(vahedTol);
                _vahedTols.Remove(vahedTol);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<VahedTol> GetAll()
        {
            return _vahedTols.ToList();
        }
        public IList<VahedTol> GetAllByOrganId(int organId)
        {
            return _vahedTols.AsQueryable().Where(x => x.OrganId == organId).ToList();
        }
        public async Task<IList<VahedTol>> GetAllByOrganIdAsync(int organId)
        {
            return await _vahedTols.AsQueryable().Where(x => x.OrganId == organId).ToListAsync();
        }
        public async Task<IList<VahedTol>> GetAllAsync()
        {
            return await _vahedTols.ToListAsync();
        }
        public VahedTol GetById(int id)
        {
            return _vahedTols.Find(id);
        }

        public bool Insert(VahedTol vahedTol)
        {
            try
            {
                _vahedTols.Add(vahedTol);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(VahedTol vahedTol)
        {
            try
            {
                var local = _uow.Set<VahedTol>()
                     .Local
                     .FirstOrDefault(f => f.ID == vahedTol.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _vahedTols.Attach(vahedTol);
                _uow.Entry(vahedTol).State = EntityState.Modified;

                _uow.Entry(vahedTol).Property(p => p.OrganId).IsModified = false;

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<VahedTol> GetByIdAsync(int id)
        {
            return await _vahedTols.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }
    }
}