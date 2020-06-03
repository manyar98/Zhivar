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
    public class NoeSazeService : INoeSaze
    {
        IUnitOfWork _uow;
        readonly IDbSet<NoeSaze> _noeSazes;
        public NoeSazeService(IUnitOfWork uow)
        {
            _uow = uow;
            _noeSazes = _uow.Set<NoeSaze>();
        }
        public bool Delete(int id)
        {
            try
            {
                var noeSaze = GetById(id);
                Delete(noeSaze);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(NoeSaze noeSaze)
        {
            try
            {
                _noeSazes.Attach(noeSaze);
                _noeSazes.Remove(noeSaze);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<NoeSaze> GetAll()
        {
            return _noeSazes.ToList();
        }
        public IList<NoeSaze> GetAllByOrganId(int organId)
        {
            return _noeSazes.AsQueryable().Where(x => x.OrganId == organId).ToList();
        }
        public async Task<IList<NoeSaze>> GetAllByOrganIdAsync(int organId)
        {
            return await _noeSazes.AsQueryable().Where(x => x.OrganId == organId).ToListAsync();
        }
        public async Task<IList<NoeSaze>> GetAllAsync()
        {
            return await _noeSazes.ToListAsync();
        }
        public NoeSaze GetById(int id)
        {
            return _noeSazes.Find(id);
        }

        public bool Insert(NoeSaze noeSaze)
        {
            try
            {
                _noeSazes.Add(noeSaze);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(NoeSaze noeSaze)
        {
            try
            {
                var local = _uow.Set<NoeSaze>()
                     .Local
                     .FirstOrDefault(f => f.ID == noeSaze.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _noeSazes.Attach(noeSaze);

                _uow.Entry(noeSaze).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<NoeSaze> GetByIdAsync(int id)
        {
            return await _noeSazes.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }
    }
}