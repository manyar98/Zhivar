using OMF.Business;
using BPJ.Common.ExceptionManagement.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPJ.Common;
using OMF.EntityFramework.Repositories;
using OMF.Workflow.Cartable;
using OMF.Workflow.Cartable.Model;
using OMF.Workflow.Model;
using OMF.Workflow;
using OMF.Common.Security;
using OMF.EntityFramework.UnitOfWork;
using Zhivar.DomainClasses.BaseInfo;
using OMF.Common.ExceptionManagement.Exceptions;
using static OMF.Workflow.Enums;
using OMF.Security.Model;

namespace Zhivar.Business.BaseInfo
{
    public class PersonelRule : BusinessRuleBase<Personel>
    {
        public PersonelRule()
               : base()
        {

        }

        public PersonelRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public PersonelRule(OperationAccess operationAccess, IUnitOfWorkAsync uow)
            : base(uow)
        {
            OperationAccess = operationAccess;
        }

        public PersonelRule(OperationAccess operationAccess, IUnitOfWorkAsync uow, bool useForAnonymousUser)
            : base(uow)
        {
            OperationAccess = operationAccess;
            UseForAnonymousUser = useForAnonymousUser;
        }

        public PersonelRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        protected override Personel FindEntity(params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;

            if (entity.User == null)
                this.LoadReference(entity, en => en.User);

            if (entity.User == null)
                return entity;


            return entity;
        }

        protected async override Task<Personel> FindEntityAsync(System.Threading.CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;

            if (entity.User == null)
                await this.LoadReferenceAsync(entity, en => en.User);

            if (entity.User == null)
                return entity;

            return entity;
        }

        public async Task<List<PersonnelUserResponse>> GetAllUserIdsInSamePositionAsync(int personelId)
        {
            if (personelId <= default(int))
                throw new OMFValidationException("اطلاعات کارمند ارسالی صحیح نمی باشد");

            var selectedPersonnelInfo = this.Queryable()
                                            .Where(p => p.ID == personelId)
                                            .Select(p => new { p.RoleID, p.OrganizationId })
                                            .SingleOrDefault();

            var stepQuery = this.UnitOfWork.RepositoryAsync<WorkflowStep>().Queryable();
            var instanceStateQuery = this.UnitOfWork.RepositoryAsync<WorkflowInstanceState>().Queryable();

            var messageQuery = from instanceState in instanceStateQuery
                               join step in stepQuery
                               on instanceState.WorkflowStepId equals step.ID
                               where instanceState.StateStatus == WfStateStatus.Open && step.RoleId == selectedPersonnelInfo.RoleID
                               select instanceState;

            if (selectedPersonnelInfo == null)
                throw new OMFValidationException("کارمند یافت نشد");

            var personnelInSamePositionQuery = this.Queryable()
                                                   .Where(p => p.RoleID == selectedPersonnelInfo.RoleID &&
                                                               p.OrganizationId == selectedPersonnelInfo.OrganizationId &&
                                                               !p.DarHalKhedmat);

            IQueryable<UserInfo> userQuery = this.UnitOfWork.RepositoryAsync<UserInfo>().Queryable();

            IQueryable<PersonnelUserResponse> userCartableInfoQuery = from personnel in personnelInSamePositionQuery
                                                                      join user in userQuery
                                                                      on personnel.UserID equals user.ID
                                                                      join instanceState in messageQuery
                                                                      on user.ID equals instanceState.UserId into instanceStateGroup
                                                                      select new PersonnelUserResponse
                                                                      {
                                                                          ID = user.ID,
                                                                          PositionId = personnel.RoleID,
                                                                          FirstName = user.FirstName,
                                                                          LastName = user.LastName,
                                                                          MessageCount = instanceStateGroup.Count()
                                                                      };

            List<PersonnelUserResponse> userInfoList = await userCartableInfoQuery.ToListAsync();

            if (userInfoList == null || userInfoList.Count == 0) return null;

            return userInfoList;
        }

        //public async Task PersonnelCartableMessageMoving(PersonnelCartableMessageBusiClass request)
        //{
        //    if (request.OldUserIds == null || request.OldUserIds.Count == 0)
        //        throw new OMFValidationException("کارمندی انتخاب نشده است");

        //    var user = await this.UnitOfWork
        //                         .RepositoryAsync<UserInfo>()
        //                         .Queryable()
        //                         .Where(u => u.PersonnelID == request.NewPersonnelId)
        //                         .Select(u => new { u.ID })
        //                         .SingleOrDefaultAsync();

        //    if (user == null)
        //        throw new OMFValidationException("کارمندی جهت انتقال اطلاعات به وی یافت نشد");

        //    List<PersonnelUserResponse> personnelUserList = await this.GetAllUserIdsInSamePositionAsync(request.NewPersonnelId);

        //    IEnumerable<int> personnelUserIds = personnelUserList.Select(p => p.ID);

        //    foreach (var id in request.OldUserIds.ToList())
        //    {
        //        if (!personnelUserIds.Contains(id))
        //            request.OldUserIds.Remove(id);
        //    }

        //    if (request.OldUserIds == null || request.OldUserIds.Count == 0)
        //        throw new OMFValidationException("کارمندی انتخاب نشده است");

        //    ChangeReceiverRequest crr = new ChangeReceiverRequest()
        //    {
        //        FromUsersId = request.OldUserIds,
        //        RoleId = personnelUserList.FirstOrDefault().PositionId,
        //        ToUserId = user.ID
        //    };
        //    await MessageManager.ChangeReceiverAsync(crr);
        //}

        public int GetPersonelIDByUserID(int userID)
        {
            return this.Queryable()
                       .Where(person => person.UserID == userID)
                       .Select(person => person.ID)
                       .FirstOrDefault();
        }

        public async Task<Personel> GetPersonelInfo(PersonelRequest request)
        {
            Personel personel = await this.Queryable()
                                          .Where(p => p.UserID == request.UserId &&
                                                      p.OrganizationId == request.OrganizationId &&
                                                      p.RoleID == request.PositionId)
                                          .SingleOrDefaultAsync();

            return personel;
        }

        protected override void DeleteEntity(Personel entity)
        {
           var user = this.UnitOfWork.Repository<UserInfo>().Queryable()
                       .Where(x => x.ID == entity.UserID)
                       .FirstOrDefault();

            if (user != null)
            {
                //var userRole = this.UnitOfWork.Repository<UserRole>().Queryable()
                //     .Where(x => x.ID == entity.UserID)
                //     .FirstOrDefault();

                //this.UnitOfWork.Repository<UserRole>().Delete(userRole);

                user.IsActive = false;
                user.IsDeleted = true;
                this.UnitOfWork.Repository<UserInfo>().Update(user);
            }
            base.DeleteEntity(entity);

        }
    }

    public class PersonelRequest
    {
        public int UserId { get; set; }
        public int PositionId { get; set; }
        public int OrganizationId { get; set; }
    }

    public class PersonnelUserResponse
    {
        public int ID { get; set; }
        public int PositionId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int MessageCount { get; set; }
    }

    public class PersonnelCartableMessageBusiClass
    {
        public List<int> OldUserIds { get; set; }
        public int NewPersonnelId { get; set; }
    }
}
