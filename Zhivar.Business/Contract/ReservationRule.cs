using OMF.Business;
using OMF.Common;
using OMF.EntityFramework.UnitOfWork;
using OMF.Security.Model;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Zhivar.DomainClasses.Contract;
using System.Collections.Generic;
using Zhivar.ViewModel.Contract;
using Zhivar.DomainClasses.Common;
using AutoMapper;
using System;
using OMF.EntityFramework.Ef6;

namespace Zhivar.Business.Contract
{
    public partial class ReservationRule : BusinessRuleBase<Reservation>
    {
        public ReservationRule()
            : base()
        {

        }

        public ReservationRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public ReservationRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

      
        protected override Reservation FindEntity(params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity.ReservationDetails == null)
            {
                this.LoadCollection<Reservation_Detail>(entity, x => x.ReservationDetails);

               
            }
            return entity;
        }

        protected async override Task<Reservation> FindEntityAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity.ReservationDetails == null)
            {
                await this.LoadCollectionAsync<Reservation_Detail>(entity, x => x.ReservationDetails);
            }

            return entity;
        }

        protected override void DeleteEntity(Reservation entity)
        {
            //entity.ReservationDetails = this.UnitOfWork.RepositoryAsync<Reservation_Detail>().Queryable().Where(x => x.ReservationID == entity.ID).ToList();

            //foreach (var reservationDetail in entity.ReservationDetails)
            //{
            //    reservationDetail.ObjectState = Enums.ObjectState.Deleted;
            //}

            //base.DeleteEntity(entity);

            using (var uow = new UnitOfWork())
            {
                entity.ReservationDetails = uow.RepositoryAsync<Reservation_Detail>().Queryable().Where(x => x.ReservationID == entity.ID).ToList();

                foreach (var reservationDetail in entity.ReservationDetails)
                {
                    reservationDetail.ObjectState = Enums.ObjectState.Deleted;
                }
                
                uow.RepositoryAsync<Reservation>().DeleteAsync(entity);
                uow.SaveChangesAsync();
            }
        }

        protected override List<string> CheckDeleteRules(Reservation entity)
        {
            return base.CheckDeleteRules(entity);
        }
        public async Task<IList<ReservationVM>> GetAllByOrganIdAsync(int organId)
        {
            try
            {
               
                var contactQuery = this.unitOfWork.RepositoryAsync<Contact>().Queryable();
                var reservationsQuery = this.Queryable().Where(x => x.OrganID == organId);

                var joinQuery = from reservation in reservationsQuery
                                join contact in contactQuery
                                on reservation.ContactID equals contact.ID
                                select new ReservationVM
                                {
                                    ID = reservation.ID,
                                    ContactID = reservation.ContactID,
                                    ContactCommon = contact,
                                    DisplayRegisterDate = reservation.DisplayRegisterDate,
                                    OrganID = reservation.OrganID,
                                    RegisterDate = reservation.RegisterDate,
                                    ContactTitle = contact.Name ,
                                    ValiditDuration = reservation.ValiditDuration
                                    // Number = reservation.ID

                                };


                return await joinQuery.ToListAsync();  //reservationVMs;
            }
            catch (Exception ex)
            {

                throw;
            }


        }

    }
}
