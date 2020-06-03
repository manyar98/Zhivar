using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using System.Data.Entity;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.DomainClasses.Common;

namespace Zhivar.ServiceLayer.Common
{
    public class BussinessService : IBussiness
    {

        IUnitOfWork _uow;
        readonly IDbSet<Bussiness> _bussinesss;
        public BussinessService(IUnitOfWork uow)
        {
            _uow = uow;
            _bussinesss = _uow.Set<Bussiness>();
        }
        public bool Delete(int id)
        {
            try
            {
                var bussiness = GetById(id);
                Delete(bussiness);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Bussiness bussiness)
        {
            try
            {
                _bussinesss.Attach(bussiness);
                _bussinesss.Remove(bussiness);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<Bussiness> GetAll()
        {
            return _bussinesss.ToList();
        }
        public IList<Bussiness> GetAllByOrganId(int organId)
        {
            return _bussinesss.AsQueryable().Where(x => x.OrganId == organId).ToList();
        }
        public async Task<IList<Bussiness>> GetAllByOrganIdAsync(int organId)
        {
            return await _bussinesss.AsQueryable().Where(x => x.OrganId == organId).ToListAsync();
        }
        public async Task<IList<Bussiness>> GetAllAsync()
        {
            return await _bussinesss.ToListAsync();
        }
        public Bussiness GetById(int id)
        {
            return _bussinesss.Find(id);
        }

        public bool Insert(Bussiness bussiness)
        {
            try
            {
                _bussinesss.Add(bussiness);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Bussiness bussiness)
        {
            try
            {
                var local = _uow.Set<Bussiness>()
                     .Local
                     .FirstOrDefault(f => f.ID == bussiness.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _bussinesss.Attach(bussiness);

                _uow.Entry(bussiness).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Bussiness> GetByIdAsync(int id)
        {
            return await _bussinesss.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }
    }
}
