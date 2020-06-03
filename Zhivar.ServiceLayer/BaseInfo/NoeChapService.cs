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
    public class NoeChapService : INoeChap
    {
        IUnitOfWork _uow;
        readonly IDbSet<NoeChap> _noeChaps;
        public NoeChapService(IUnitOfWork uow)
        {
            _uow = uow;
            _noeChaps = _uow.Set<NoeChap>();
        }
        public bool Delete(int id)
        {
            try
            {
                var noeChap = GetById(id);
                Delete(noeChap);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(NoeChap noeChap)
        {
            try
            {
                _noeChaps.Attach(noeChap);
                _noeChaps.Remove(noeChap);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<NoeChap> GetAll()
        {
            return _noeChaps.ToList();
        }
        public IList<NoeChap> GetAllByOrganId(int organId)
        {
            return _noeChaps.AsQueryable().Where(x => x.OrganId == organId).ToList();
        }
        public async Task<IList<NoeChap>> GetAllByOrganIdAsync(int organId)
        {
            return await _noeChaps.AsQueryable().Where(x => x.OrganId == organId).ToListAsync();
        }
        public async Task<IList<NoeChap>> GetAllAsync()
        {
            return await _noeChaps.ToListAsync();
        }
        public NoeChap GetById(int id)
        {
            return _noeChaps.Find(id);
        }

        public bool Insert(NoeChap noeChap)
        {
            try
            {
                _noeChaps.Add(noeChap);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(NoeChap noeChap)
        {
            try
            {
                var local = _uow.Set<NoeChap>()
                     .Local
                     .FirstOrDefault(f => f.ID == noeChap.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _noeChaps.Attach(noeChap);

                _uow.Entry(noeChap).State = EntityState.Modified;

                _uow.Entry(noeChap).Property(p => p.OrganId).IsModified = false;

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<NoeChap> GetByIdAsync(int id)
        {
            return await _noeChaps.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }
    }
}