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

namespace Zhivar.Business.BaseInfo
{
    public partial class GoroheSazeRule : BusinessRuleBase<GoroheSaze>
    {
        public GoroheSazeRule()
            : base()
        {

        }

        public GoroheSazeRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public GoroheSazeRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        //public IList<GoroheSaze> GetAllByPersonId(int personId)
        //{
        //    return this.Queryable().Where(x => x.Person.ID == personId).ToList();
        //}
        //public async Task<IList<GoroheSaze>> GetAllByPersonIdAsync(int personId)
        //{
        //    return await this.Queryable().Where(x => x.Person.ID == personId).ToListAsync2();
        //}

        public async Task<List<GoroheSazeVM>> GetAllByOrganIdAsync(int organId)
        {
            var itemGroupQuery = this.unitOfWork.Repository<GoroheSaze>().Queryable().Where(x => x.OrganID == organId);
            var itemQuery = this.unitOfWork.Repository<Saze>().Queryable();
            var noeSazeQuery = this.unitOfWork.Repository<NoeSaze>().Queryable();
            var noeEjareQuery = this.unitOfWork.Repository<NoeEjare>().Queryable();
            //var vahedTolQuery = this.unitOfWork.Repository<VahedTol>().Queryable();


            var joinQuery = from itemGroup in itemGroupQuery
                            join saze in
                            ( from item in itemQuery
                            join noeSaze in noeSazeQuery
                            on item.NoeSazeId equals noeSaze.ID
                            join noeEjare in noeEjareQuery
                            on item.NoeEjareID equals noeEjare.ID
                            //join vahedTol in vahedTolQuery
                            //on item.VahedId equals vahedTol.ID 
                            select new
                            {
                                Title = item.Title,
                                Address = item.Address,
                                Arz = item.Arz,
                                GoroheSazeID = item.GoroheSazeID,
                                ID = item.ID,
                                Tol = item.Tol,
                                NoeSazeId = item.NoeSazeId,
                                NoeEjare = noeEjare,
                                Code = item.Code,
                                OrganId = item.OrganId,
                                NoeEjareName = noeEjare.Title,
                                NoeEjareID = noeEjare.ID,
                                NoeSazeName = noeSaze.Title,
                                Latitude = item.Latitude,
                                Longitude = item.Longitude,
                                NoorDard = item.NoorDard
                            }
                            )
                            on itemGroup.ID equals saze.GoroheSazeID into itemGroups
                            select new GoroheSazeVM
                            {
                                ID = itemGroup.ID,

                                Items = itemGroups.Select(s => new SazeVM
                                {
                                    Title = s.Title,
                                    Address = s.Address,
                                    Arz = s.Arz,
                                    GoroheSazeID = s.GoroheSazeID,
                                    ID = s.ID,
                                    Tol = s.Tol,
                                    NoeSazeId = s.NoeSazeId,
                                    NoeEjareID = s.NoeEjareID,
                                    Code = s.Code,
                                    OrganId = s.OrganId,
                                    GoroheName = itemGroup.Title,
                                    NoeEjareName = s.NoeEjareName,
                                    NoeSazeName = s.NoeSazeName,
                                    Latitude = s.Latitude,
                                    Longitude = s.Longitude,
                                    NoorDard = s.NoorDard,
                                    NoeEjare = s.NoeEjare
                                    

                                }).ToList(),

                                Title = itemGroup.Title,
                                OrganID = itemGroup.OrganID
                            };

            return await joinQuery.ToListAsync2();

        }

    }
}