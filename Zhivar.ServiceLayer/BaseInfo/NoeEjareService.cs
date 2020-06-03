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
    public class NoeEjareService : INoeEjare
    {
        IUnitOfWork _uow;
        readonly IDbSet<NoeEjare> _noeEjares;
        public NoeEjareService(IUnitOfWork uow)
        {
            _uow = uow;
            _noeEjares = _uow.Set<NoeEjare>();
        }
        public bool Delete(int id)
        {
            try
            {
                var noeEjare = GetById(id);
                Delete(noeEjare);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(NoeEjare noeEjare)
        {
            try
            {
                _noeEjares.Attach(noeEjare);
                _noeEjares.Remove(noeEjare);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<NoeEjare> GetAll()
        {
            return _noeEjares.ToList();
        }
        public IList<NoeEjare> GetAllByOrganId(int organId)
        {
            return _noeEjares.AsQueryable().Where(x => x.OrganId == organId).ToList();
        }
        public async Task<IList<NoeEjare>> GetAllByOrganIdAsync(int organId)
        {
            return await _noeEjares.AsQueryable().Where(x => x.OrganId == organId).ToListAsync();
        }
        public async Task<IList<NoeEjare>> GetAllAsync()
        {
            return await _noeEjares.ToListAsync();
        }
        public NoeEjare GetById(int id)
        {
            return _noeEjares.Find(id);
        }

        public bool Insert(NoeEjare noeEjare)
        {
            try
            {
                _noeEjares.Add(noeEjare);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(NoeEjare noeEjare)
        {
            try
            {
                var local = _uow.Set<NoeEjare>()
                     .Local
                     .FirstOrDefault(f => f.ID == noeEjare.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _noeEjares.Attach(noeEjare);

                _uow.Entry(noeEjare).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<NoeEjare> GetByIdAsync(int id)
        {
            return await _noeEjares.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }
    }
}