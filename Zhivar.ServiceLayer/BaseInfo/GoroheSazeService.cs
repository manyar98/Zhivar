using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using System.Data.Entity;


namespace Zhivar.ServiceLayer.BaseInfo
{
    public class GoroheSazeService : IGoroheSaze
    {
        IUnitOfWork _uow;
        readonly IDbSet<GoroheSaze> _goroheSazes;
        public GoroheSazeService(IUnitOfWork uow)
        {
            _uow = uow;
            _goroheSazes = _uow.Set<GoroheSaze>();
        }
        public bool Delete(int id)
        {
            try
            {
                var goroheSaze = GetById(id);
                Delete(goroheSaze);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(GoroheSaze goroheSaze)
        {
            try
            {
                _goroheSazes.Attach(goroheSaze);
                _goroheSazes.Remove(goroheSaze);
        
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<GoroheSaze> GetAll()
        {
            return _goroheSazes.ToList();
        }
        public IList<GoroheSaze> GetAllByPersonId(int personId)
        {
            return _goroheSazes.AsQueryable().Where(x => x.Person.ID == personId).ToList();
        }
        public async Task<IList<GoroheSaze>> GetAllByPersonIdAsync(int personId)
        {
            return await _goroheSazes.AsQueryable().Where(x => x.Person.ID == personId).ToListAsync();
        }
        public async Task<IList<GoroheSaze>> GetAllAsync()
        {
            return await _goroheSazes.ToListAsync();
        }
        public GoroheSaze GetById(int id)
        {
            return _goroheSazes.Find(id);
        }

        public bool Insert(GoroheSaze goroheSaze)
        {
            try
            {
                _goroheSazes.Add(goroheSaze);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(GoroheSaze goroheSaze)
        {
            try
            {
                var local = _uow.Set<GoroheSaze>()
                     .Local
                     .FirstOrDefault(f => f.ID == goroheSaze.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                //var local2 = _uow.Set<Saze>()
                //  .Local
                //  .FirstOrDefault(f => f.ID == goroheSaze.Sazes.ID);
                //if (local2 != null)
                //{
                //    _uow.Entry(local).State = EntityState.Detached;
                //}
                _goroheSazes.Attach(goroheSaze);

                _uow.Entry(goroheSaze).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<GoroheSaze> GetByIdAsync(int id)
        {
            return await _goroheSazes.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }
    }
}
