using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.ServiceLayer.BaseInfo
{
    public class SazeService:ISaze
    {
        IUnitOfWork _uow;
        readonly IDbSet<Saze> _sazes;
        readonly IDbSet<GoroheSaze> _goroheSazes;
        readonly IDbSet<NoeSaze> _noeSaze;
        readonly IDbSet<NoeEjare> _noeEjare;
        readonly IDbSet<VahedTol> _vahedTol;
        private readonly IMappingEngine Mapper;
        public SazeService(IUnitOfWork uow, IMappingEngine mappingEngine)
        {
            _uow = uow;
            _sazes = _uow.Set<Saze>();
            _goroheSazes = _uow.Set<GoroheSaze>();
            _noeSaze = _uow.Set<NoeSaze>();
            _noeEjare = _uow.Set<NoeEjare>();
            _vahedTol = _uow.Set<VahedTol>();

             Mapper = mappingEngine;
        }
        public bool Delete(int id)
        {
            try
            {
                var saze = GetById(id);
                Delete(saze);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Saze saze)
        {
            try
            {
                _sazes.Attach(saze);
                _sazes.Remove(saze);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<Saze> GetAll()
        {
            return _sazes.ToList();
        }
        public IList<SazeVM> GetAllByPersonId(int personId)
        {
            var goroheSazes = _goroheSazes.Where(x => x.Person.ID == personId).Include("Saze").ToList();

            List<SazeVM> listSazeVM = new List<SazeVM>();

            foreach (var goroheSaze in goroheSazes ?? new List<GoroheSaze>())
            {
                foreach (var saze in goroheSaze.Sazes ?? new List<Saze>())
                {
                    listSazeVM.Add(new SazeVM()
                    {
                        Address = saze.Address,
                        Arz = saze.Arz,
                        //GoroheSazeID = goroheSaze.ID,
                        //GoroheSazeName = goroheSaze.Title,
                        //ID = saze.ID,
                        Title = saze.Title,
                        //NoeEjare = saze.NoeEjare,
                        Tol = saze.Tol,
                        //X = saze.X,
                        //Y = saze.Y
                    });
                }
            }

            return listSazeVM.ToList();
        }
        public async Task<IList<SazeVM>> GetAllByPersonIdAsync(int personId)
        {
            var sazesQuery = _sazes.AsQueryable().Where(x => x.OrganId == personId);
            var goroheSazeQuery = _goroheSazes.AsQueryable();
            var noeSazeQuery = _noeSaze.AsQueryable();
            var noeEjareQuery = _noeEjare.AsQueryable();
            var vahedQuery = _vahedTol.AsQueryable();

            var joinQuery = from saze in sazesQuery
                            join goroheSaze in goroheSazeQuery
                            on saze.GoroheSazeID equals goroheSaze.ID
                            join noeSaze in noeSazeQuery
                            on saze.NoeSazeId equals noeSaze.ID
                            join noeEjare in noeEjareQuery
                            on saze.NoeEjareID equals noeEjare.ID
                            select new SazeVM
                            {
                                Address = saze.Address,
                                Arz = saze.Arz,
                                Code = saze.Code,
                                GoroheName = goroheSaze.Title,
                                ID = saze.ID,
                                NoeEjareName = noeEjare.Title,
                                OrganId = saze.OrganId,
                                NoeSazeName = noeSaze.Title,
                                Title = saze.Title,
                                Tol = saze.Tol,
                                Latitude = saze.Latitude,
                                Longitude = saze.Longitude,
                                NoorDard = saze.NoorDard


                            };

            return await joinQuery.ToListAsync();
         
        }

        public async Task<IList<Saze>> GetAllByGorohIdAsync(int gorohId)
        {
            return await _sazes.Where(x => x.GoroheSazeID == gorohId).ToListAsync();
        }
        public IList<Saze> GetAllByGorohId(int gorohId)
        {
            return _sazes.Where(x => x.GoroheSazeID == gorohId).ToList();
        }
        public async Task<IList<Saze>> GetAllAsync()
        {
            return await _sazes.ToListAsync();
        }
        public Saze GetById(int id)
        {
            return _sazes.Find(id);
        }

        public bool Insert(Saze saze)
        {
            try
            {
                _sazes.Add(saze);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Saze saze)
        {
            try
            {

                var oldSaze = _uow.Set<Saze>()
                     .Local
                     .FirstOrDefault(f => f.ID == saze.ID);

                if (oldSaze != null)
                {
                    _uow.Entry(oldSaze).State = EntityState.Detached;
                }

                //foreach (var sazeImage in saze.Images ?? new List<SazeImage>())
                //{
                //    var oldsazeImage = _uow.Set<SazeImage>().Local.FirstOrDefault(f => f.ID == sazeImage.ID);
                //    if (oldsazeImage != null)
                //    {
                //        _uow.Entry(oldsazeImage).State = EntityState.Detached;

                //    }
                //}
                _sazes.Attach(saze);


                _uow.Entry(saze).State = EntityState.Modified;


                //foreach (var image in saze.Images ?? new List<SazeImage>())
                //{
                //    if (image.ID > 0)
                //    {
                //        _uow.Entry(image).State = EntityState.Modified;
                //    }
                //    else
                //    {
                //        _uow.Entry(image).State = EntityState.Added;
                //    }


                //}
                    return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Saze> GetByIdAsync(int id)
        {
            return await _sazes.SingleOrDefaultAsync( x=> x.ID == id);
        }
        public async Task<SazeVM> GetSazeForEdit(int id)
        {
            var sazesQuery = _sazes.AsQueryable().Where(x => x.ID == id);
            var goroheSazeQuery = _goroheSazes.AsQueryable();
            var noeSazeQuery = _noeSaze.AsQueryable();
            var noeEjareQuery = _noeEjare.AsQueryable();
            var vahedQuery = _vahedTol.AsQueryable();

            var joinQuery = from saze in sazesQuery
                            join goroheSaze in goroheSazeQuery
                            on saze.GoroheSazeID equals goroheSaze.ID
                            join noeSaze in noeSazeQuery
                            on saze.NoeSazeId equals noeSaze.ID
                            join noeEjare in noeEjareQuery
                            on saze.NoeEjareID equals noeEjare.ID
                            select new SazeVM
                            {
                                Address = saze.Address,
                                Arz = saze.Arz,
                                Code = saze.Code,
                                GoroheName = goroheSaze.Title,
                                ID = saze.ID,
                                NoeEjareName = noeEjare.Title,
                                OrganId = saze.OrganId,
                                NoeSazeName = noeSaze.Title,
                                Title = saze.Title,
                                Tol = saze.Tol,
                                GoroheSazeID = goroheSaze.ID,
                                NoeEjareID = noeEjare.ID,
                                NoeSazeId = noeSaze.ID,
                                Latitude = saze.Latitude,
                                Longitude = saze.Longitude,
                                NoorDard = saze.NoorDard


                            };

            return await joinQuery.SingleOrDefaultAsync();

        
    }

    }
}
