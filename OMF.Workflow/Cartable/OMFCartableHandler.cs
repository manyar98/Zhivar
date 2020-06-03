using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.DataContext;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model;
using OMF.Workflow.Cartable.Model;
using OMF.Workflow.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using static OMF.Workflow.Enums;

namespace OMF.Workflow.Cartable
{
    public class OMFCartableHandler : ICartableHandlerAsync, ICartableHandler
    {
        public List<CtbMenuItem> GetMenuItems(MenuItemsRequest menuRequest)
        {
            this.CheckUserAccessTo(menuRequest.UserId.Value, "منوی کارتابل");
            using (UnitOfWork unitOfWork = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                IQueryable<WorkflowInfo> workflowInfoQuery = unitOfWork.RepositoryAsync<WorkflowInfo>().Queryable(false, true, (List<Expression<Func<WorkflowInfo, object>>>)null);
                IQueryable<WorkflowStep> workflowStepQuery = unitOfWork.RepositoryAsync<WorkflowStep>().Queryable(false, true, (List<Expression<Func<WorkflowStep, object>>>)null).Where<WorkflowStep>((Expression<Func<WorkflowStep, bool>>)(wfs => wfs.RoleId == menuRequest.RoleId && ((int)wfs.StepType == 10 || (int)wfs.StepType == 30)));
                IQueryable<WorkflowInstanceState> workflowInstanceStateQuery = unitOfWork.RepositoryAsync<WorkflowInstanceState>().Queryable(false, true, (List<Expression<Func<WorkflowInstanceState, object>>>)null).Where<WorkflowInstanceState>((Expression<Func<WorkflowInstanceState, bool>>)(wfis => (int?)wfis.UserId == menuRequest.UserId && (int)wfis.StateStatus == 1));


                var joinQuery = (from workflowInfo in workflowInfoQuery
                                 join wf in (from workflowStep in workflowStepQuery
                                             join workflowInstanceState in workflowInstanceStateQuery
                                             on workflowStep.ID equals workflowInstanceState.WorkflowStepId
                                             select new
                                             {
                                                 WorkflowInfoId = workflowStep.WorkflowInfoId
                                             }
                                 ) on workflowInfo.ID equals wf.WorkflowInfoId into workflowInstanceStateGroups
                                 select new CtbMenuItem
                                 {
                                     Code = workflowInfo.Code,
                                     Title = workflowInfo.Title,
                                     Icon = workflowInfo.Icon,
                                     MessageCount = workflowInstanceStateGroups.Count(),
                                     WorkflowId = workflowInfo.ID
                                 });


                return joinQuery.Where(c => c.MessageCount > 0).ToList();


            }
        }

        public async Task<List<CtbMenuItem>> GetMenuItemsAsync(
          MenuItemsRequest menuRequest)
        {
            await this.CheckUserAccessToAsync(menuRequest.UserId.Value, "منوی کارتابل");
            List<CtbMenuItem> ctbMenuItemList = new List<CtbMenuItem>();
            using (UnitOfWork uow = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                IQueryable<WorkflowInfo> workflowInfoQuery = uow.RepositoryAsync<WorkflowInfo>().Queryable();
                IQueryable<WorkflowStep> workflowStepQuery = uow.RepositoryAsync<WorkflowStep>().Queryable().Where(wfs => wfs.RoleId == menuRequest.RoleId && ((int)wfs.StepType == 10 || (int)wfs.StepType == 30));
                IQueryable<WorkflowInstanceState> workflowInstanceStateQuery = uow.RepositoryAsync<WorkflowInstanceState>().Queryable().Where(wfis => (int?)wfis.UserId == menuRequest.UserId && (int)wfis.StateStatus == 1);

                var joinQuery = (from workflowInfo in workflowInfoQuery
                                join wf in (from workflowStep in workflowStepQuery
                                join workflowInstanceState in workflowInstanceStateQuery
                                on workflowStep.ID equals workflowInstanceState.WorkflowStepId 
                                select new
                                {
                                    WorkflowInfoId= workflowStep.WorkflowInfoId
                                }
                                ) on workflowInfo.ID equals wf.WorkflowInfoId into workflowInstanceStateGroups
                                select new CtbMenuItem
                                {
                                    Code = workflowInfo.Code,
                                    Title = workflowInfo.Title,
                                    Icon = workflowInfo.Icon,
                                    MessageCount = workflowInstanceStateGroups.Count(),
                                    WorkflowId = workflowInfo.ID
                                });

             
                return await joinQuery.Where(c => c.MessageCount > 0).ToListAsync2();
              
            }
        }
        public List<CtbSubMenuItem> GetSubMenuItems(SubMenuItemsRequest subMenuRequest)
        {
            this.CheckUserAccessTo(subMenuRequest.UserId.Value, "منوی کارتابل");
            using (UnitOfWork unitOfWork = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                IQueryable<WorkflowInstanceState> queryable = unitOfWork.RepositoryAsync<WorkflowInstanceState>().Queryable(false, true, (List<Expression<Func<WorkflowInstanceState, object>>>)null).Where<WorkflowInstanceState>((Expression<Func<WorkflowInstanceState, bool>>)(wfis => (int?)wfis.UserId == subMenuRequest.UserId && (int)wfis.StateStatus == 1));
 

                return unitOfWork.RepositoryAsync<WorkflowStep>().
                            Queryable(false, true, (List<Expression<Func<WorkflowStep, object>>>)null).
                            Where<WorkflowStep>((Expression<Func<WorkflowStep, bool>>)
                                (wfs => wfs.WorkflowInfoId == subMenuRequest.WorkflowId && wfs.RoleId == subMenuRequest.RoleId &&
                                ((int)wfs.StepType == 10 || (int)wfs.StepType == 30)))
                                      .GroupJoin(queryable, p => p.ID, c => c.WorkflowStepId, (p, c) => new CtbSubMenuItem
                                      {
                                          Code = p.Code,
                                          Icon = p.Icon,
                                          MessageCount = c.Count(),
                                          MultiCheckAction = p.MultiCheckAction,
                                          StepId = c.OrderByDescending(x => x.ID).FirstOrDefault().WorkflowStepId,
                                          Title = p.Title,
                                          WorkflowId = p.WorkflowInfoId
                                          
                                      }).ToList();

            }
        }

        public async Task<List<CtbSubMenuItem>> GetSubMenuItemsAsync(
          SubMenuItemsRequest subMenuRequest)
        {
            try
            {
                await this.CheckUserAccessToAsync(subMenuRequest.UserId.Value, "منوی کارتابل");
                List<CtbSubMenuItem> ctbSubMenuItemList;
                using (UnitOfWork uow = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
                {
                    IQueryable<WorkflowInstanceState> workflowInstanceStateQuery = uow.RepositoryAsync<WorkflowInstanceState>().Queryable(false, true, (List<Expression<Func<WorkflowInstanceState, object>>>)null).Where<WorkflowInstanceState>((Expression<Func<WorkflowInstanceState, bool>>)(wfis => (int?)wfis.UserId == subMenuRequest.UserId && (int)wfis.StateStatus == 1));
                    IQueryable<WorkflowStep> workflowStepQuery = uow.RepositoryAsync<WorkflowStep>().Queryable(false, true, (List<Expression<Func<WorkflowStep, object>>>)null).Where<WorkflowStep>((Expression<Func<WorkflowStep, bool>>)(wfs => wfs.WorkflowInfoId == subMenuRequest.WorkflowId && wfs.RoleId == subMenuRequest.RoleId && ((int)wfs.StepType == 10 || (int)wfs.StepType == 30)));

                    IQueryable<CtbSubMenuItem> subMenuItemsQuery = workflowStepQuery
                               .GroupJoin(workflowInstanceStateQuery, workflowStep => 
                               workflowStep.ID, workflowInstanceState => workflowInstanceState.WorkflowStepId, (p, c) => new CtbSubMenuItem
                               {
                                   Code = p.Code,
                                   Icon = p.Icon,
                                   MessageCount = c.Count(),
                                   MultiCheckAction = p.MultiCheckAction,
                                   StepId = c.Any()? c.OrderByDescending(x => x.ID).FirstOrDefault().WorkflowStepId:-1,
                                   Title =  p.Title,
                                   WorkflowId = p.WorkflowInfoId

                               });


                    List<CtbSubMenuItem> list = await subMenuItemsQuery.ToListAsync<CtbSubMenuItem>();
                    ctbSubMenuItemList = list;
                }
                return ctbSubMenuItemList;
            }
            catch (Exception ex)
            {

                throw;
            }
       
        }

        public List<MessageInfo> GetMessages(MessagesInfoesRequest messagesRequest)
        {
           
            this.CheckUserAccessTo(messagesRequest.UserId, "کارتابل");
            using (UnitOfWork unitOfWork = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                IQueryable<WorkflowInstanceState> queryable1 = unitOfWork.Repository<WorkflowInstanceState>().Queryable(false, true, (List<Expression<Func<WorkflowInstanceState, object>>>)null).Where<WorkflowInstanceState>((Expression<Func<WorkflowInstanceState, bool>>)(wfis => wfis.WorkflowStepId == messagesRequest.WorkflowStepId && (int)wfis.StateStatus == 1 && wfis.UserId == messagesRequest.UserId));
                IQueryable<WorkflowInstanceState> outer1 = unitOfWork.Repository<WorkflowInstanceState>().Queryable(false, true, (List<Expression<Func<WorkflowInstanceState, object>>>)null);
                IQueryable<WorkflowInfo> queryable2 = unitOfWork.Repository<WorkflowInfo>().Queryable(false, true, (List<Expression<Func<WorkflowInfo, object>>>)null);
                IQueryable<WorkflowInstance> outer2 = unitOfWork.Repository<WorkflowInstance>().Queryable(false, true, (List<Expression<Func<WorkflowInstance, object>>>)null);
                IQueryable<UserInfo> queryable3 = unitOfWork.Repository<UserInfo>().Queryable(true, true, (List<Expression<Func<UserInfo, object>>>)null);
                IQueryable<RoleBase> queryable4 = unitOfWork.Repository<RoleBase>().Queryable(true, true, (List<Expression<Func<RoleBase, object>>>)null);
                IQueryable<WorkflowStep> outer3 = unitOfWork.Repository<WorkflowStep>().Queryable(false, true, (List<Expression<Func<WorkflowStep, object>>>)null);
       

                return outer1.Join((IEnumerable<WorkflowInstanceState>)queryable1, (Expression<Func<WorkflowInstanceState, int?>>)(senderWfInstanceState => (int?)senderWfInstanceState.ID), (Expression<Func<WorkflowInstanceState, int?>>)(wfInstanceState => wfInstanceState.SenderWorkflowInstanceStateId), (senderWfInstanceState, wfInstanceState) => new
                {
                    senderWfInstanceState = senderWfInstanceState,
                    wfInstanceState = wfInstanceState
                }).Join((IEnumerable<UserInfo>)queryable3, data => data.senderWfInstanceState.UserId,
                (Expression<Func<UserInfo, int>>)(user => user.ID), (data, user) => new
                {
                    data,
                    user = user
                }).Join(outer3.Join((IEnumerable<RoleBase>)queryable4, (Expression<Func<WorkflowStep, int?>>)(wfStep => wfStep.RoleId), (Expression<Func<RoleBase, int?>>)(role => (int?)role.ID), (wfStep, role) => new
                {
                    StepId = wfStep.ID,
                    RoleId = role.ID,
                    RoleName = role.Name
                }), data => data.data.senderWfInstanceState.WorkflowStepId, senderWfStepRoleData => senderWfStepRoleData.StepId, (data, senderWfStepRoleData) => new
                {
                    ID = data.data.wfInstanceState.ID,
                    WorkflowInstanceId = data.data.wfInstanceState.WorkflowInstanceId,
                    WorkflowStepId = data.data.wfInstanceState.WorkflowStepId,
                    ExchangeData = data.data.wfInstanceState.ExchangeData,
                    InstantiationTime = data.data.wfInstanceState.InstantiationTime,
                    Title = data.data.wfInstanceState.Title,
                    SenderUserId = data.data.senderWfInstanceState.UserId,
                    SenderFullName = data.user.FirstName + DbFunctions.AsUnicode(" ") + data.user.LastName,
                    SenderRoleId = senderWfStepRoleData.RoleId,
                    SenderRoleName = senderWfStepRoleData.RoleName
                }).Join(outer2.Join((IEnumerable<WorkflowInfo>)queryable2, (Expression<Func<WorkflowInstance, int>>)(wfInstance => wfInstance.WorkflowInfoId), (Expression<Func<WorkflowInfo, int>>)(workflow => workflow.ID), (wfInstance, workflow) => new
                {
                    WorkflowInstanceId = wfInstance.ID,
                    WorkflowId = workflow.ID,
                    WorkflowTitle = workflow.Title,
                    MasterId = wfInstance.RelatedRecordId,
                    WfStatus = wfInstance.Status,
                    UserComment = wfInstance.UserComment
                }), wfInstanceStateData => wfInstanceStateData.WorkflowInstanceId, wfInstanceData => wfInstanceData.WorkflowInstanceId, (wfInstanceStateData, wfInstanceData) => new
                {
                    wfInstanceStateData = wfInstanceStateData,
                    wfInstanceData = wfInstanceData
                })
                .Join(outer3, data => data.wfInstanceStateData.WorkflowStepId, wfStep => wfStep.ID, (p, c) => new MessageInfo
                {
                    ActionUriRoute = c.ActionUriRoute,
                    ExchangeData = (WFExchangeData)p.wfInstanceStateData.ExchangeData,
                    ExchangeDataString = p.wfInstanceStateData.ExchangeData,
                    ID = p.wfInstanceStateData.ID,
                    MasterId = p.wfInstanceData.MasterId,
                    MasterUriRoute = c.MasterUriRoute,
                    MessageType = c.MessageType,
                    NeedToSign = c.NeedToSign,
                    Title = c.Title,
                    Priority = c.MessagePriority,
                    SenderFullName = p.wfInstanceStateData.SenderFullName,
                    SenderRoleId = p.wfInstanceStateData.SenderRoleId,
                    SenderRoleName = p.wfInstanceStateData.SenderRoleName,
                    SenderUserId = p.wfInstanceStateData.SenderUserId,
                    UserComment = p.wfInstanceData.UserComment,
                    WorkflowTitle = p.wfInstanceData.WorkflowTitle,
                    WorkflowStatus = p.wfInstanceData.WfStatus,
                    WorkflowId = p.wfInstanceData.WorkflowId,
                    WorkflowInstanceId = p.wfInstanceData.WorkflowInstanceId,
                    WorkflowStepId = p.wfInstanceStateData.WorkflowStepId,
                    //SendDateTime
                }).ToList();

            }
        }

        public async Task<List<MessageInfo>> GetMessagesAsync(
          MessagesInfoesRequest messagesRequest)
        {
            
            await this.CheckUserAccessToAsync(messagesRequest.UserId, "کارتابل");
            List<MessageInfo> messageInfoList;
            using (UnitOfWork uow = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                IQueryable<WorkflowInstanceState> receiverWorkflowInstanceStateQuery = uow.RepositoryAsync<WorkflowInstanceState>().Queryable(false, true, (List<Expression<Func<WorkflowInstanceState, object>>>)null).Where<WorkflowInstanceState>((Expression<Func<WorkflowInstanceState, bool>>)(wfis => wfis.WorkflowStepId == messagesRequest.WorkflowStepId && (int)wfis.StateStatus == 1 && wfis.UserId == messagesRequest.UserId));
                IQueryable<WorkflowInstanceState> senderWorkflowInstanceStateQuery = uow.RepositoryAsync<WorkflowInstanceState>().Queryable(false, true, (List<Expression<Func<WorkflowInstanceState, object>>>)null);
                IQueryable<WorkflowInfo> workflowQuery = uow.RepositoryAsync<WorkflowInfo>().Queryable(false, true, (List<Expression<Func<WorkflowInfo, object>>>)null);
                IQueryable<WorkflowInstance> workflowInstanceQuery = uow.RepositoryAsync<WorkflowInstance>().Queryable(false, true, (List<Expression<Func<WorkflowInstance, object>>>)null);
                IQueryable<UserInfo> senderUserQuery = uow.RepositoryAsync<UserInfo>().Queryable(true, true, (List<Expression<Func<UserInfo, object>>>)null);
                IQueryable<RoleBase> senderRoleQuery = uow.RepositoryAsync<RoleBase>().Queryable(true, true, (List<Expression<Func<RoleBase, object>>>)null);
                IQueryable<WorkflowStep> workflowStepQuery = uow.RepositoryAsync<WorkflowStep>().Queryable(false, true, (List<Expression<Func<WorkflowStep, object>>>)null);


                IQueryable<MessageInfo> messageQuery = senderWorkflowInstanceStateQuery.Join((IEnumerable<WorkflowInstanceState>)receiverWorkflowInstanceStateQuery, (Expression<Func<WorkflowInstanceState, int?>>)(senderWfInstanceState => (int?)senderWfInstanceState.ID), (Expression<Func<WorkflowInstanceState, int?>>)(receiverInstanceState => receiverInstanceState.SenderWorkflowInstanceStateId), (senderWfInstanceState, receiverInstanceState) => new
                {
                    senderWfInstanceState = senderWfInstanceState,
                    receiverInstanceState = receiverInstanceState
                }).Join((IEnumerable<UserInfo>)senderUserQuery, data => data.senderWfInstanceState.UserId, (Expression<Func<UserInfo, int>>)(user => user.ID), (data, user) => new
                {
                    data,
                    user = user
                }).Join(workflowStepQuery.Join((IEnumerable<RoleBase>)senderRoleQuery, (Expression<Func<WorkflowStep, int?>>)(wfStep => wfStep.RoleId), (Expression<Func<RoleBase, int?>>)(role => (int?)role.ID), (wfStep, role) => new
                {
                    StepId = wfStep.ID,
                    RoleId = role.ID,
                    RoleName = role.Name
                }), data => data.data.senderWfInstanceState.WorkflowStepId, senderWfStepRoleData => senderWfStepRoleData.StepId, (data, senderWfStepRoleData) => new
                {
                    ID = data.data.receiverInstanceState.ID,
                    WorkflowInstanceId = data.data.receiverInstanceState.WorkflowInstanceId,
                    WorkflowStepId = data.data.receiverInstanceState.WorkflowStepId,
                    ExchangeData = data.data.receiverInstanceState.ExchangeData,
                    InstantiationTime = data.data.receiverInstanceState.InstantiationTime,
                    Title = data.data.receiverInstanceState.Title,
                    SenderUserId = data.data.senderWfInstanceState.UserId,
                    SenderFullName = data.user.FirstName + DbFunctions.AsUnicode(" ") + data.user.LastName,
                    SenderRoleId = senderWfStepRoleData.RoleId,
                    SenderRoleName = senderWfStepRoleData.RoleName
                }).Join(workflowInstanceQuery.Join((IEnumerable<WorkflowInfo>)workflowQuery, (Expression<Func<WorkflowInstance, int>>)(wfInstance => wfInstance.WorkflowInfoId), (Expression<Func<WorkflowInfo, int>>)(workflow => workflow.ID), (wfInstance, workflow) => new
                {
                    WorkflowInstanceId = wfInstance.ID,
                    WorkflowId = workflow.ID,
                    WorkflowTitle = workflow.Title,
                    MasterId = wfInstance.RelatedRecordId,
                    WfStatus = wfInstance.Status,
                    UserComment = wfInstance.UserComment
                }), wfInstanceStateData => wfInstanceStateData.WorkflowInstanceId, wfInstanceData => wfInstanceData.WorkflowInstanceId, (wfInstanceStateData, wfInstanceData) => new
                {
                    wfInstanceStateData = wfInstanceStateData,
                    wfInstanceData = wfInstanceData
                }).Join(workflowStepQuery,
                        data => data.wfInstanceStateData.WorkflowStepId,
                        wfStep => wfStep.ID,
                        (p, c) => new MessageInfo
                        {
                            ActionUriRoute = c.ActionUriRoute,
                           // ExchangeData = (WFExchangeData)p.wfInstanceStateData.ExchangeData,
                            ExchangeDataString = p.wfInstanceStateData.ExchangeData,
                            ID = p.wfInstanceStateData.ID,
                            MasterId = p.wfInstanceData.MasterId,
                            MasterUriRoute = c.MasterUriRoute,

                            MessageType = c.MessageType,
                            NeedToSign = c.NeedToSign,
                            Title = p.wfInstanceStateData.Title,
                            Priority = c.MessagePriority,
                            SenderFullName = p.wfInstanceStateData.SenderFullName,
                            SenderRoleId = p.wfInstanceStateData.SenderRoleId,
                            SenderRoleName = p.wfInstanceStateData.SenderRoleName,
                            SenderUserId = p.wfInstanceStateData.SenderUserId,
                            UserComment = p.wfInstanceData.UserComment,
                            WorkflowTitle = p.wfInstanceData.WorkflowTitle,
                            WorkflowStatus = p.wfInstanceData.WfStatus,
                            WorkflowId = p.wfInstanceData.WorkflowId,
                            WorkflowInstanceId = p.wfInstanceData.WorkflowInstanceId,
                            WorkflowStepId = p.wfInstanceStateData.WorkflowStepId,
                            SendDateTime = p.wfInstanceStateData.InstantiationTime

                        });


               // List<MessageInfo> result = await messageQuery.ToListAsync<MessageInfo>();
                List<MessageInfo> result = await messageQuery.OrderByDescending<MessageInfo, DateTime>((Expression<Func<MessageInfo, DateTime>>)(msg => msg.SendDateTime)).ToListAsync<MessageInfo>();
                messageInfoList = result;
            }
            return messageInfoList;
        }

        public List<CtbMessageAction> GetMessageStepActions(
          MessageActionsRequest messageActionRequest)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                CtbMessageAction ctbMessageAction = (CtbMessageAction)null;
                if (messageActionRequest.StateId.HasValue)
                {
                    var data1 = unitOfWork.RepositoryAsync<WorkflowInstance>().Queryable(false, true, (List<Expression<Func<WorkflowInstance, object>>>)null).Join((IEnumerable<WorkflowInstanceState>)unitOfWork.RepositoryAsync<WorkflowInstanceState>().Queryable(false, true, (List<Expression<Func<WorkflowInstanceState, object>>>)null), (Expression<Func<WorkflowInstance, int>>)(wfInstance => wfInstance.ID), (Expression<Func<WorkflowInstanceState, int>>)(wfInstanceState => wfInstanceState.WorkflowInstanceId), (wfInstance, wfInstanceState) => new
                    {
                        wfInstance = wfInstance,
                        wfInstanceState = wfInstanceState
                    }).Where(data => (int?)data.wfInstanceState.ID == messageActionRequest.StateId).Select(data => new
                    {
                        UserId = data.wfInstanceState.UserId,
                        WfStateStatus = data.wfInstance.Status
                    }).SingleOrDefault();
                    if (data1 == null)
                        throw new WorkflowException("No instance state found");
                    this.CheckUserAccessTo(data1.UserId, "اقدامات");
                    if (data1.WfStateStatus == WfStateStatus.Refuse)
                        ctbMessageAction = new CtbMessageAction()
                        {
                            StepId = messageActionRequest.StepId,
                            ActionId = -100,
                            ActionTitle = "تایید انصراف",
                            NeedConfirm = true,
                            ConfirmMessage = "آیا از تایید انصراف فرایند اطمینان دارید؟"
                        };
                }
                IQueryable<WFActionType> queryable = unitOfWork.RepositoryAsync<WFActionType>().Queryable(false, true, (List<Expression<Func<WFActionType, object>>>)null);
   

                List<CtbMessageAction> list = unitOfWork.RepositoryAsync<WorkflowStepAction>().
                        Queryable(false, true, (List<Expression<Func<WorkflowStepAction, object>>>)null).
                        Where<WorkflowStepAction>((Expression<Func<WorkflowStepAction, bool>>)
                        (wfsa => wfsa.WorkflowStepId == messageActionRequest.StepId)).
                        Join(queryable,
                        wfStepAction => wfStepAction.WFActionTypeId,
                        wfActionType => wfActionType.ID, (p, c) => new CtbMessageAction
                        {
                            ActionId = c.ID,
                            ActionTitle = c.Title,
                            ConfirmMessage = c.ConfirmMessage,
                            NeedConfirm = c.NeedConfirm,
                            StepId = p.WorkflowStepId
                        }).ToList();
                       
                if (ctbMessageAction != null)
                    list.Add(ctbMessageAction);
                return list;
            }
        }

        public async Task<List<CtbMessageAction>> GetMessageStepActionsAsync(
          MessageActionsRequest messageActionRequest)
        {
            List<CtbMessageAction> ctbMessageActionList;
            using (UnitOfWork uow = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                CtbMessageAction refuseAction = (CtbMessageAction)null;
                if (messageActionRequest.StateId.HasValue)
                {
                    IQueryable<WorkflowInstance> workflowInstanceQuery = uow.RepositoryAsync<WorkflowInstance>().Queryable(false, true, (List<Expression<Func<WorkflowInstance, object>>>)null);
                    IQueryable<WorkflowInstanceState> workflowInstanceStateQuery = uow.RepositoryAsync<WorkflowInstanceState>().Queryable(false, true, (List<Expression<Func<WorkflowInstanceState, object>>>)null);
                    var data1 = await workflowInstanceQuery.Join((IEnumerable<WorkflowInstanceState>)workflowInstanceStateQuery, (Expression<Func<WorkflowInstance, int>>)(wfInstance => wfInstance.ID), (Expression<Func<WorkflowInstanceState, int>>)(wfInstanceState => wfInstanceState.WorkflowInstanceId), (wfInstance, wfInstanceState) => new
                    {
                        wfInstance = wfInstance,
                        wfInstanceState = wfInstanceState
                    }).Where(data => (int?)data.wfInstanceState.ID == messageActionRequest.StateId).Select(data => new
                    {
                        UserId = data.wfInstanceState.UserId,
                        WfStateStatus = data.wfInstance.Status
                    }).SingleOrDefaultAsync();
                    var stateData = data1;
                    if (stateData == null)
                        throw new WorkflowException("No instance state found");
                    await this.CheckUserAccessToAsync(stateData.UserId, "اقدامات");
                    if (stateData.WfStateStatus == WfStateStatus.Refuse)
                        refuseAction = new CtbMessageAction()
                        {
                            StepId = messageActionRequest.StepId,
                            ActionId = -100,
                            ActionTitle = "تایید انصراف",
                            NeedConfirm = true,
                            ConfirmMessage = "آیا از تایید انصراف فرایند اطمینان دارید؟"
                        };
                    workflowInstanceQuery = (IQueryable<WorkflowInstance>)null;
                    workflowInstanceStateQuery = (IQueryable<WorkflowInstanceState>)null;
                    stateData = null;
                }
                IQueryable<WFActionType> workflowActionTypeQuery = uow.RepositoryAsync<WFActionType>().Queryable(false, true, (List<Expression<Func<WFActionType, object>>>)null);
                IQueryable<WorkflowStepAction> workflowStepActionQuery = uow.RepositoryAsync<WorkflowStepAction>().Queryable(false, true, (List<Expression<Func<WorkflowStepAction, object>>>)null).Where<WorkflowStepAction>((Expression<Func<WorkflowStepAction, bool>>)(wfsa => wfsa.WorkflowStepId == messageActionRequest.StepId));

   
                IQueryable<CtbMessageAction> resultQuery = workflowStepActionQuery.
                    Join(workflowActionTypeQuery, 
                        wfStepAction => wfStepAction.WFActionTypeId, 
                        wfActionType => wfActionType.ID ,
                        (p, c) => new CtbMessageAction
                        {
                            ActionId = c.ID,
                            ActionTitle = c.Title,
                            ConfirmMessage = c.ConfirmMessage,
                            NeedConfirm = c.NeedConfirm,
                            StepId = p.WorkflowStepId
                        });
                List<CtbMessageAction> list = await resultQuery.Distinct<CtbMessageAction>().ToListAsync<CtbMessageAction>();
                if (refuseAction != null)
                    list.Add(refuseAction);
                ctbMessageActionList = list;
            }
            return ctbMessageActionList;
        }

        public List<MessageInfoHistory> GetMessageInfoHistories(
          MessageInfoHistoryRequest messageInfoHistryRequest)
        {
       
            using (UnitOfWork unitOfWork = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                IQueryable<WorkflowInstanceState> source = unitOfWork.RepositoryAsync<WorkflowInstanceState>().Queryable(false, true, (List<Expression<Func<WorkflowInstanceState, object>>>)null);
                IQueryable<WorkflowInstanceState> outer;
                if (messageInfoHistryRequest.WorkflowInstanceId > 0)
                    outer = source.Where<WorkflowInstanceState>((Expression<Func<WorkflowInstanceState, bool>>)(wfis => wfis.WorkflowInstanceId == messageInfoHistryRequest.WorkflowInstanceId));
                else
                    outer = unitOfWork.RepositoryAsync<WorkflowInfo>().Queryable(false, true, (List<Expression<Func<WorkflowInfo, object>>>)null).Where<WorkflowInfo>((Expression<Func<WorkflowInfo, bool>>)(wf => wf.Code == messageInfoHistryRequest.WorkflowInfoCode)).Join((IEnumerable<WorkflowInstance>)unitOfWork.RepositoryAsync<WorkflowInstance>().Queryable(false, true, (List<Expression<Func<WorkflowInstance, object>>>)null).Where<WorkflowInstance>((Expression<Func<WorkflowInstance, bool>>)(wfi => wfi.RelatedRecordId == messageInfoHistryRequest.RelatedRecordId)), (Expression<Func<WorkflowInfo, int>>)(workflow => workflow.ID), (Expression<Func<WorkflowInstance, int>>)(wfInstance => wfInstance.WorkflowInfoId), (workflow, wfInstance) => new
                    {
                        workflow = workflow,
                        wfInstance = wfInstance
                    }).Join((IEnumerable<WorkflowInstanceState>)source, data => data.wfInstance.ID, (Expression<Func<WorkflowInstanceState, int>>)(wfInstanceState => wfInstanceState.WorkflowInstanceId), (data, wfInstanceState) => wfInstanceState);
                IQueryable<WorkflowStep> queryable1 = unitOfWork.RepositoryAsync<WorkflowStep>().Queryable(false, true, (List<Expression<Func<WorkflowStep, object>>>)null);
                IQueryable<RoleBase> queryable2 = unitOfWork.RepositoryAsync<RoleBase>().Queryable(true, true, (List<Expression<Func<RoleBase, object>>>)null);
                IQueryable<UserInfo> queryable3 = unitOfWork.RepositoryAsync<UserInfo>().Queryable(true, true, (List<Expression<Func<UserInfo, object>>>)null);
                IQueryable<WFActionType> queryable4 = unitOfWork.RepositoryAsync<WFActionType>().Queryable(false, true, (List<Expression<Func<WFActionType, object>>>)null);

                List<MessageInfoHistory> list = outer.Join((IEnumerable<UserInfo>)queryable3, (Expression<Func<WorkflowInstanceState, int>>)(wfInstanceState => wfInstanceState.UserId), (Expression<Func<UserInfo, int>>)(user => user.ID), (wfInstanceState, user) => new
                {
                    wfInstanceState = wfInstanceState,
                    user = user
                }).Join((IEnumerable<WorkflowStep>)queryable1, data => data.wfInstanceState.WorkflowStepId, (Expression<Func<WorkflowStep, int>>)(wfStep => wfStep.ID), (data, wfStep) => new
                {
                    data = data,
                    wfStep = wfStep
                }).Join((IEnumerable<RoleBase>)queryable2, data => data.wfStep.RoleId, (Expression<Func<RoleBase, int?>>)(role => (int?)role.ID), (data, role) => new
                {
                    data = data,
                    role = role
                })
                .GroupJoin(queryable4, data => data.data.data.wfInstanceState.AccomplishActionId,
                wfActionType => wfActionType.ID, (p, c) => new MessageInfoHistory
                {
                    AccomplishTime = p.data.data.wfInstanceState.AccomplishTime,
                    RoleId = p.role.ID,
                    RoleCode = p.role.Code,
                    UserId = p.data.data.user.ID,
                    UserFullName = p.data.data.user.FirstName + " " + p.data.data.user.LastName,
                    UserComment = p.data.data.wfInstanceState.UserComment,
                    StepTitle = p.data.wfStep.Title,
                    Title = p.data.data.wfInstanceState.Title,
                    RoleName = p.role.Name,
                    InstanceStateId = p.data.data.wfInstanceState.ID,
                    InstantiationTime = p.data.data.wfInstanceState.InstantiationTime,
                    // Action = p.data.wfStep.WorkflowStepActions.FirstOrDefault(). coorect it 
                }).ToList();

                if (SecurityManager.HasAccess("Workflow-MessageInfoHistory-View") || (!list.Any<MessageInfoHistory>() || list.Any<MessageInfoHistory>((Func<MessageInfoHistory, bool>)(item => this.HasUserAccessTo(new HasUserAccessToRequest()
                {
                    UserId = item.UserId,
                    RoleCode = item.RoleCode,
                    WorkflowInfoCode = messageInfoHistryRequest.WorkflowInfoCode
                })))))
                    return list;
                throw new WorkflowException("عدم مطابقت شناسه کاربر در گردش کار فرایند");
            }
        }

        public async Task<List<MessageInfoHistory>> GetMessageInfoHistoriesAsync(
          MessageInfoHistoryRequest messageInfoHistryRequest)
        {

            using (UnitOfWork uow = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                IQueryable<WorkflowInstanceState> workflowInstanceStateQuery = uow.RepositoryAsync<WorkflowInstanceState>().Queryable(false, true, (List<Expression<Func<WorkflowInstanceState, object>>>)null);
                if (messageInfoHistryRequest.WorkflowInstanceId > 0)
                    workflowInstanceStateQuery = workflowInstanceStateQuery.Where<WorkflowInstanceState>((Expression<Func<WorkflowInstanceState, bool>>)(wfis => wfis.WorkflowInstanceId == messageInfoHistryRequest.WorkflowInstanceId));
                else
                    workflowInstanceStateQuery = uow.RepositoryAsync<WorkflowInfo>().Queryable(false, true, (List<Expression<Func<WorkflowInfo, object>>>)null).Where<WorkflowInfo>((Expression<Func<WorkflowInfo, bool>>)(wf => wf.Code == messageInfoHistryRequest.WorkflowInfoCode)).Join((IEnumerable<WorkflowInstance>)uow.RepositoryAsync<WorkflowInstance>().Queryable(false, true, (List<Expression<Func<WorkflowInstance, object>>>)null).Where<WorkflowInstance>((Expression<Func<WorkflowInstance, bool>>)(wfi => wfi.RelatedRecordId == messageInfoHistryRequest.RelatedRecordId)), (Expression<Func<WorkflowInfo, int>>)(workflow => workflow.ID), (Expression<Func<WorkflowInstance, int>>)(wfInstance => wfInstance.WorkflowInfoId), (workflow, wfInstance) => new
                    {
                        workflow = workflow,
                        wfInstance = wfInstance
                    }).Join((IEnumerable<WorkflowInstanceState>)workflowInstanceStateQuery, data => data.wfInstance.ID, (Expression<Func<WorkflowInstanceState, int>>)(wfInstanceState => wfInstanceState.WorkflowInstanceId), (data, wfInstanceState) => wfInstanceState);
                IQueryable<WorkflowStep> workflowStepQuery = uow.RepositoryAsync<WorkflowStep>().Queryable(false, true, (List<Expression<Func<WorkflowStep, object>>>)null);
                IQueryable<RoleBase> roleQuery = uow.RepositoryAsync<RoleBase>().Queryable(true, true, (List<Expression<Func<RoleBase, object>>>)null);
                IQueryable<UserInfo> userQuery = uow.RepositoryAsync<UserInfo>().Queryable(true, true, (List<Expression<Func<UserInfo, object>>>)null);
                IQueryable<WFActionType> workflowActionTypeQuery = uow.RepositoryAsync<WFActionType>().Queryable(false, true, (List<Expression<Func<WFActionType, object>>>)null);

                IQueryable<MessageInfoHistory> historyQuery = workflowInstanceStateQuery.Join((IEnumerable<UserInfo>)userQuery, (Expression<Func<WorkflowInstanceState, int>>)(wfInstanceState => wfInstanceState.UserId), (Expression<Func<UserInfo, int>>)(user => user.ID), (wfInstanceState, user) => new
                {
                    wfInstanceState = wfInstanceState,
                    user = user
                }).Join((IEnumerable<WorkflowStep>)workflowStepQuery, data => data.wfInstanceState.WorkflowStepId, (Expression<Func<WorkflowStep, int>>)(wfStep => wfStep.ID), (data, wfStep) => new
                {
                    data = data,
                    wfStep = wfStep
                }).Join((IEnumerable<RoleBase>)roleQuery, data => data.wfStep.RoleId, (Expression<Func<RoleBase, int?>>)(role => (int?)role.ID), (data, role) => new
                {
                    data = data,
                    role = role
                })
                .GroupJoin(workflowActionTypeQuery, data => data.data.data.wfInstanceState.AccomplishActionId,
                wfActionType => (int?)wfActionType.ID, (p, c) => new MessageInfoHistory
                {
                    AccomplishTime = p.data.data.wfInstanceState.AccomplishTime,
                    RoleId = p.role.ID,
                    RoleCode = p.role.Code,
                    UserId = p.data.data.user.ID,
                    UserFullName = p.data.data.user.FirstName + " " + p.data.data.user.LastName,
                    UserComment = p.data.data.wfInstanceState.UserComment,
                    StepTitle = p.data.wfStep.Title,
                    Title = p.data.data.wfInstanceState.Title,
                    RoleName = p.role.Name,
                    InstanceStateId = p.data.data.wfInstanceState.ID,
                    InstantiationTime = p.data.data.wfInstanceState.InstantiationTime,
                    //Action = p.data.data.
                });



                List<MessageInfoHistory> list = await historyQuery.OrderBy<MessageInfoHistory, int>((Expression<Func<MessageInfoHistory, int>>)(his => his.InstanceStateId)).ToListAsync<MessageInfoHistory>();
                bool hasViewAccess = await SecurityManager.HasAccessAsync("Workflow-MessageInfoHistory-View");
                if (hasViewAccess || (!list.Any<MessageInfoHistory>() || list.Any<MessageInfoHistory>((Func<MessageInfoHistory, bool>)(item => this.HasUserAccessTo(new HasUserAccessToRequest()
                {
                    UserId = item.UserId,
                    WorkflowInfoCode = messageInfoHistryRequest.WorkflowInfoCode,
                    RoleCode = item.RoleCode
                })))))
                    return list;
                throw new WorkflowException("عدم مطابقت شناسه کاربر در گردش کار فرایند");
            }
        }

        public List<NextStepInfo> GetNextSteps(NextStepsRequest nextStepsRequest)
        {
           
            using (UnitOfWork unitOfWork = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                IQueryable<WorkflowStep> queryable1 =
                    unitOfWork.Repository<WorkflowStep>().
                    Queryable(false, true, (List<Expression<Func<WorkflowStep, object>>>)null);

                IQueryable<Temp> queryable2 = unitOfWork.Repository<WorkflowStepAction>().
                    Queryable(false, true, (List<Expression<Func<WorkflowStepAction, object>>>)null).
                    Where<WorkflowStepAction>((Expression<Func<WorkflowStepAction, bool>>)
                    (wfsa => wfsa.WorkflowStepId == nextStepsRequest.StepId && wfsa.WFActionTypeId == nextStepsRequest.ActionId)).
                    Join((IEnumerable<WorkflowStep>)queryable1,
                    (Expression<Func<WorkflowStepAction, int>>)
                    (wfStepAction => wfStepAction.NextWorkflowStepId),
                    (Expression<Func<WorkflowStep, int>>)(wfStep => wfStep.ID),
                    (wfStepAction, wfStep) => new Temp
                    {
                        Id = wfStep.ID,
                        Type = wfStep.StepType,
                        Title = wfStep.Title,
                        RoleId = wfStep.RoleId,
                        OrganizationId = wfStep.OrganizationId
                    });

                List<NextStepInfo> nextStepInfoList = new List<NextStepInfo>();
                foreach (var data in queryable2)
                {
                    var nextStepData = data;
                    IQueryable<RoleBase> source1 = unitOfWork.Repository<RoleBase>().
                        Queryable(false, true, (List<Expression<Func<RoleBase, object>>>)null).
                        Where<RoleBase>((Expression<Func<RoleBase, bool>>)(r => (int?)r.ID == nextStepData.RoleId));
                    string str = nextStepData.Title;
                    if (nextStepData.Type == StepType.HumanActivity || nextStepData.Type == StepType.SendToStarter)
                        str = source1.SingleOrDefault<RoleBase>((Expression<Func<RoleBase, bool>>)(role => (int?)role.ID == nextStepData.RoleId))?.Name;
                    IQueryable<UserInfo> source2 = unitOfWork.Repository<UserInfo>().Queryable(false, true, (List<Expression<Func<UserInfo, object>>>)null).Where<UserInfo>((Expression<Func<UserInfo, bool>>)(u => u.IsActive && !u.IsDeleted));
                    IQueryable<UserRole> outer = unitOfWork.Repository<UserRole>().Queryable(false, true, (List<Expression<Func<UserRole, object>>>)null).Where<UserRole>((Expression<Func<UserRole, bool>>)(ur => (int?)ur.RoleId == nextStepData.RoleId));
                    int? organizationId = new int?();
                    int? nullable = nextStepData.OrganizationId;
                    if (nullable.HasValue)
                    {
                        organizationId = nextStepData.OrganizationId;
                    }
                    else
                    {
                        nullable = nextStepsRequest.WfInstanceID;
                        if (nullable.HasValue)
                        {
                            var WFExchangeData = (WFExchangeData)unitOfWork.Repository<WorkflowInstance>().
                            Queryable(false, true, (List<Expression<Func<WorkflowInstance, object>>>)null).
                            SingleOrDefault<WorkflowInstance>((Expression<Func<WorkflowInstance, bool>>)
                            (wfIns => (int?)wfIns.ID == nextStepsRequest.WfInstanceID)).InitialExchangeData;

                            organizationId = Convert.ToInt32(WFExchangeData["_StarterOrganizationId_"]);
                        }
                        else
                            organizationId = SecurityManager.CurrentUserContext.OrganizationId;
                    }
                    if (organizationId.HasValue)
                        source2 = source2.Where<UserInfo>((Expression<Func<UserInfo, bool>>)(u => u.OrganizationId == organizationId));

                    IQueryable<NextStepUserInfo> source3 =
                        outer.Join(source2,
                      userRole => userRole.UserId,
                      user => user.ID,
                      (p, c) => new NextStepUserInfo
                      {
                          FirstName = c.FirstName,
                          Gender = c.Gender,
                          LastName = c.LastName,
                          UserId = c.ID
                      });

                    NextStepInfo nextStepInfo = new NextStepInfo()
                    {
                        Id = nextStepData.Id,
                        Type = nextStepData.Type,
                        Title = nextStepData.Title,
                        RoleName = str,
                        RoleId = nextStepData.RoleId,
                        UserInfoes = new List<NextStepUserInfo>()
                    };
                    int num = source3.Count<NextStepUserInfo>();
                    if (num > 0 && num < 21)
                        nextStepInfo.UserInfoes = source3.ToList<NextStepUserInfo>();
                    nextStepInfoList.Add(nextStepInfo);
                }
                return nextStepInfoList;
            }
        }

        public async Task<List<NextStepInfo>> GetNextStepsAsync(
          NextStepsRequest nextStepsRequest)
        {
            List<NextStepInfo> nextStepInfoList;
            using (UnitOfWork uow = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                IQueryable<WorkflowStep> workflowStepQuery = uow.RepositoryAsync<WorkflowStep>().Queryable(false, true, (List<Expression<Func<WorkflowStep, object>>>)null);
                IQueryable<WorkflowStepAction> workflowStepActionQuery = uow.RepositoryAsync<WorkflowStepAction>().Queryable(false, true, (List<Expression<Func<WorkflowStepAction, object>>>)null).Where<WorkflowStepAction>((Expression<Func<WorkflowStepAction, bool>>)(wfsa => wfsa.WorkflowStepId == nextStepsRequest.StepId && wfsa.WFActionTypeId == nextStepsRequest.ActionId));
                IQueryable<Temp> nextStepsQuery = workflowStepActionQuery.
                    Join(workflowStepQuery, 
                    wfStepAction => wfStepAction.NextWorkflowStepId, 
                    wfStep => wfStep.ID, 
                    (wfStepAction, wfStep) => new
                    Temp
                {
                    Id = wfStep.ID,
                    Type = wfStep.StepType,
                    Title = wfStep.Title,
                    RoleId = wfStep.RoleId,
                    OrganizationId = wfStep.OrganizationId
                });

                List<NextStepInfo> nextSteplist = new List<NextStepInfo>();

                foreach (var data in await nextStepsQuery.ToListAsync())
                {
                    var nextStepData = data;
                    IQueryable<RoleBase> roleQuery = uow.Repository<RoleBase>().Queryable(false, true, (List<Expression<Func<RoleBase, object>>>)null).Where<RoleBase>((Expression<Func<RoleBase, bool>>)(r => (int?)r.ID == nextStepData.RoleId));
                    string nextStepRoleName = nextStepData.Title;
                    if (nextStepData.Type == StepType.HumanActivity || nextStepData.Type == StepType.SendToStarter)
                    {
                        RoleBase roleBase = await roleQuery.SingleOrDefaultAsync<RoleBase>((Expression<Func<RoleBase, bool>>)(role => (int?)role.ID == nextStepData.RoleId));
                        nextStepRoleName = roleBase?.Name;
                        roleBase = (RoleBase)null;
                    }
                    IQueryable<UserInfo> userQuery = uow.RepositoryAsync<UserInfo>().Queryable(false, true, (List<Expression<Func<UserInfo, object>>>)null).Where<UserInfo>((Expression<Func<UserInfo, bool>>)(u => u.IsActive && !u.IsDeleted));
                    IQueryable<UserRole> userRoleQuery = uow.RepositoryAsync<UserRole>().Queryable(false, true, (List<Expression<Func<UserRole, object>>>)null).Where<UserRole>((Expression<Func<UserRole, bool>>)(ur => (int?)ur.RoleId == nextStepData.RoleId));
                    int? organizationId = new int?();
                    int? nullable = nextStepData.OrganizationId;
                    if (nullable.HasValue)
                    {
                        organizationId = nextStepData.OrganizationId;
                    }
                    else
                    {
                        nullable = nextStepsRequest.WfInstanceID;
                        if (nullable.HasValue)
                        {
                            WorkflowInstance workflowInstance = await uow.Repository<WorkflowInstance>().Queryable(false, true, (List<Expression<Func<WorkflowInstance, object>>>)null).SingleOrDefaultAsync<WorkflowInstance>((Expression<Func<WorkflowInstance, bool>>)(wfIns => (int?)wfIns.ID == nextStepsRequest.WfInstanceID));
                            var WFExchangeData = (WFExchangeData)workflowInstance.InitialExchangeData;

                            organizationId = WFExchangeData["_StarterOrganizationId_"].ConvertTo<int?>();
                            workflowInstance = (WorkflowInstance)null;
                        }
                        else
                            organizationId = SecurityManager.CurrentUserContext.OrganizationId;
                    }
                    if (organizationId.HasValue)
                        userQuery = userQuery.Where<UserInfo>((Expression<Func<UserInfo, bool>>)(u => u.OrganizationId == organizationId));

                    IQueryable<NextStepUserInfo> userInfoQuery =
                        userRoleQuery.Join(userQuery,
                            userRole => userRole.UserId,
                            user => user.ID,
                    (p, c) => new NextStepUserInfo
                    {
                        FirstName = c.FirstName,
                        Gender = c.Gender,
                        LastName = c.LastName,
                        UserId = c.ID
                    });

                    NextStepInfo nextStepInfo1 = new NextStepInfo()
                    {
                        Id = nextStepData.Id,
                        Type = nextStepData.Type,
                        Title = nextStepData.Title,
                        RoleName = nextStepRoleName,
                        RoleId = nextStepData.RoleId,
                        UserInfoes = new List<NextStepUserInfo>()
                    };
                    int usersCount = await userInfoQuery.CountAsync<NextStepUserInfo>();
                    if (usersCount > 0 && usersCount < 21)
                    {
                        NextStepInfo nextStepInfo2 = nextStepInfo1;
                        List<NextStepUserInfo> nextStepUserInfoList = await userInfoQuery.ToListAsync<NextStepUserInfo>();
                        nextStepInfo2.UserInfoes = nextStepUserInfoList;
                        nextStepInfo2 = (NextStepInfo)null;
                        nextStepUserInfoList = (List<NextStepUserInfo>)null;
                    }
                    nextSteplist.Add(nextStepInfo1);
                    roleQuery = (IQueryable<RoleBase>)null;
                    nextStepRoleName = (string)null;
                    userQuery = (IQueryable<UserInfo>)null;
                    userRoleQuery = (IQueryable<UserRole>)null;
                    userInfoQuery = (IQueryable<NextStepUserInfo>)null;
                    nextStepInfo1 = (NextStepInfo)null;
                }
                nextStepInfoList = nextSteplist;
            }
            return nextStepInfoList;
        }

        public virtual bool HasUserAccessTo(HasUserAccessToRequest hasUserAccessToRequest)
        {
            SecurityManager.ThrowIfUserContextNull();
            return SecurityManager.CurrentUserContext.UserId == hasUserAccessToRequest.UserId;
        }

        public virtual async Task<bool> HasUserAccessToAsync(
          HasUserAccessToRequest hasUserAccessToRequest)
        {
            SecurityManager.ThrowIfUserContextNull();
            return SecurityManager.CurrentUserContext.UserId == hasUserAccessToRequest.UserId;
        }

        private void CheckUserAccessTo(int userId, string toName)
        {
            if (!this.HasUserAccessTo(new HasUserAccessToRequest()
            {
                UserId = userId
            }))
                throw new WorkflowException(string.Format("عدم مطابقت شناسه کاربر در {0}", (object)toName));
        }

        private async Task CheckUserAccessToAsync(int userId, string toName)
        {
            bool async = await this.HasUserAccessToAsync(new HasUserAccessToRequest()
            {
                UserId = userId
            });
            if (!async)
                throw new WorkflowException(string.Format("عدم مطابقت شناسه کاربر در {0}", (object)toName));
        }
    }
}
