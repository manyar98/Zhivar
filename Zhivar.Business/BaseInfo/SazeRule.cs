using System;
using System.Collections.Generic;
using OMF.Business;
using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.Query;
using OMF.EntityFramework.UnitOfWork;
using OMF.Security.Model;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCValidation = OMF.Common.Validation;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.BaseInfo;
using System.Threading;
using AutoMapper;

namespace Zhivar.Business.BaseInfo
{
    public partial class SazeRule : BusinessRuleBase<Saze>
    {
        public SazeRule()
            : base()
        {

        }

        public SazeRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public SazeRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public IList<Saze> GetAll()
        {
            return this.Queryable().ToList();
        }
        public IList<SazeVM> GetAllByPersonId(int personId)
        {
            var goroheSazes = this.unitOfWork.Repository<GoroheSaze>().Queryable().Where(x => x.Person.ID == personId).ToList();
            //var sazeQuery = this.Queryable();
            
            //var joinQuery = from goroheSaze in goroheSazeQuery
            //                join saze in sazeQuery
            //                on goroheSaze.saz .Include("Saze").ToList();


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
            var sazesQuery = this.Queryable().Where(x => x.OrganId == personId);
            var goroheSazeQuery = this.unitOfWork.Repository<GoroheSaze>().Queryable();
            var noeSazeQuery = this.unitOfWork.Repository<NoeSaze>().Queryable();
            var noeEjareQuery = this.unitOfWork.Repository<NoeEjare>().Queryable();
            var vahedQuery = this.unitOfWork.Repository<VahedTol>().Queryable();

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

            return await joinQuery.ToListAsync2();

        }

        public async Task<IList<Saze>> GetAllByGorohIdAsync(int gorohId)
        {
            return await this.Queryable().Where(x => x.GoroheSazeID == gorohId).ToListAsync2();
        }
        public IList<Saze> GetAllByGorohId(int gorohId)
        {
            return this.Queryable().Where(x => x.GoroheSazeID == gorohId).ToList();
        }
        public async Task<IList<Saze>> GetAllAsync()
        {
            return await this.Queryable().ToListAsync2();
        }

        public async Task<SazeVM> GetSazeForEdit(int id)
        {
            var sazesQuery = this.Queryable().Where(x => x.ID == id);
            var goroheSazeQuery = this.unitOfWork.RepositoryAsync<GoroheSaze>().Queryable();
            var noeSazeQuery = this.unitOfWork.RepositoryAsync<NoeSaze>().Queryable();
            var noeEjareQuery = this.unitOfWork.RepositoryAsync <NoeEjare>().Queryable();
            var vahedQuery = this.unitOfWork.RepositoryAsync<VahedTol>().Queryable();
            var sazeImageQuery = this.unitOfWork.RepositoryAsync<SazeImage>().Queryable();


            var joinQuery = from saze in sazesQuery
                            join goroheSaze in goroheSazeQuery
                            on saze.GoroheSazeID equals goroheSaze.ID
                            join noeSaze in noeSazeQuery
                            on saze.NoeSazeId equals noeSaze.ID
                            join noeEjare in noeEjareQuery
                            on saze.NoeEjareID equals noeEjare.ID
                            join sazeImage in sazeImageQuery
                            on saze.ID equals sazeImage.SazeId into groupSazeImages
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
                                NoorDard = saze.NoorDard,
                                ImagesCommon = groupSazeImages.ToList()

                            };

            return await joinQuery.SingleOrDefaultAsync2();


        }

        protected override Saze FindEntity(params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;



            if (entity.Images == null)
                this.LoadCollection<SazeImage>(entity, dtd => dtd.Images);


            return entity;
        }

        protected async override Task<Saze> FindEntityAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;

            if (entity.Images == null)
                await this.LoadCollectionAsync<SazeImage>(entity, dtd => dtd.Images);


            return entity;
        }

        protected override void DeleteEntity(Saze entity)
        {
            foreach (var image in entity.Images ?? new List<SazeImage>())
            {
                image.ObjectState = Enums.ObjectState.Deleted;
                image.SazeId = entity.ID;
            }
            base.DeleteEntity(entity);
        }
    }
}