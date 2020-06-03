using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.DataContext;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.Repositories;
using OMF.Security.Model;
using OMF.Workflow.Cartable;
using OMF.Workflow.Cartable.Model;
using OMF.Workflow.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using static OMF.Common.Enums;
using static OMF.Workflow.Enums;

namespace OMF.Workflow
{
    public class OMFWorkflowHandler : IWorkflowHandlerAsync, IWorkflowHandler
    {
        public WorkflowHandlingResponse StartWorkflow(
          WorkflowStartInfo startInfo)
        {
            WorkflowFindData workflowFindData = new WorkflowFindData()
            {
                StartType = startInfo.StartType,
                Code = startInfo.Code
            };
            WorkflowInfo wfInfo = this.FindWorkflow(workflowFindData);
            if (wfInfo == null)
                throw new WorkflowException(string.Format("workflow with code: '{0}' not found", (object)workflowFindData.Code));
            using (UnitOfWork unitOfWork = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                WorkflowStep workflowStep = unitOfWork.RepositoryAsync<WorkflowStep>().Queryable(false, true, (List<Expression<Func<WorkflowStep, object>>>)null).Where<WorkflowStep>((Expression<Func<WorkflowStep, bool>>)(wfStep => (int)wfStep.StepType == 1 && wfStep.WorkflowInfoId == wfInfo.ID)).SingleOrDefault<WorkflowStep>();
                if (workflowStep == null)
                    throw new WorkflowException(string.Format("no start step for workflow with code '{0}'", (object)startInfo.Code));
                int? nullable = startInfo.StarterOrganizationId;
                if (!nullable.HasValue && SecurityManager.CurrentUserContext != null)
                    startInfo.StarterOrganizationId = SecurityManager.CurrentUserContext.OrganizationId;
                if (startInfo.ExchangeData == null)
                    startInfo.ExchangeData = new WFExchangeData();
                List<WorkflowInstanceState> workflowInstanceStateList = new List<WorkflowInstanceState>();
                WorkflowInstanceState workflowInstanceState1 = new WorkflowInstanceState();
                workflowInstanceState1.AccomplishTime = new DateTime?(DateTime.Now);
                workflowInstanceState1.ExchangeData = (string)startInfo.ExchangeData;
                workflowInstanceState1.InstantiationTime = DateTime.Now;
                workflowInstanceState1.ObjectState = ObjectState.Added;
                nullable = new int?();
                workflowInstanceState1.SenderWorkflowInstanceStateId = nullable;
                workflowInstanceState1.UserComment = "شروع فرایند";
                workflowInstanceState1.StateStatus = WfStateStatus.Close;
                workflowInstanceState1.Title = string.Format("شروع فرایند {0} {1}", (object)wfInfo.Title, (object)startInfo.InstanceTitle);
                workflowInstanceState1.UserId = startInfo.StarterUserId;
                workflowInstanceState1.WorkflowStepId = workflowStep.ID;
                WorkflowInstanceState workflowInstanceState2 = workflowInstanceState1;
                workflowInstanceStateList.Add(workflowInstanceState2);
                WFExchangeData wfExchangeData1 = new WFExchangeData();
                WFExchangeData wfExchangeData2 = wfExchangeData1;
                nullable = startInfo.StarterOrganizationId;
                ref int? local1 = ref nullable;
                int num;
                string str1;
                if (!local1.HasValue)
                {
                    str1 = (string)null;
                }
                else
                {
                    num = local1.GetValueOrDefault();
                    str1 = num.ToString();
                }
                wfExchangeData2.Add("_StarterOrganizationId_", str1);
                WFExchangeData wfExchangeData3 = wfExchangeData1;
                num = startInfo.StarterUserId;
                string str2 = num.ToString();
                wfExchangeData3.Add("_StarterUserId_", str2);
                wfExchangeData1.Add("_InstanceTitle_", string.Format("{0}", (object)startInfo.InstanceTitle));
                wfExchangeData1.Add("_WorkflowTitle_", string.Format("{0}", (object)wfInfo.Title));
                wfExchangeData1.Add("_RelatedRecordId_", string.Format("{0}", (object)startInfo.RelatedRecordId));
                bool flag = false;
                WorkflowStep currentStep = workflowStep;
                WFExchangeData exchangeData = startInfo.ExchangeData;
                WorkflowInstanceState senderInstanceState = workflowInstanceState2;
                WFExchangeData initialExchangeData = wfExchangeData1;
                nullable = new int?();
                int? actionId = nullable;
                ref bool local2 = ref flag;
                List<WorkflowInstanceState> nextStateList = this.CreateNextStateList(currentStep, exchangeData, senderInstanceState, initialExchangeData, actionId, out local2);
                workflowInstanceStateList.AddRange((IEnumerable<WorkflowInstanceState>)nextStateList);
                WorkflowInstance workflowInstance = new WorkflowInstance();
                workflowInstance.RelatedRecordId = startInfo.RelatedRecordId;
                workflowInstance.InitialExchangeData = (string)wfExchangeData1;
                workflowInstance.StartTime = DateTime.Now;
                workflowInstance.Status = WfStateStatus.Open;
                workflowInstance.ObjectState = ObjectState.Added;
                workflowInstance.WorkflowInfoId = wfInfo.ID;
                workflowInstance.WorkflowInstanceStates = workflowInstanceStateList;
                WorkflowInstance entity = workflowInstance;
                if (flag)
                {
                    entity.FinishTime = new DateTime?(DateTime.Now);
                    entity.Status = WfStateStatus.Close;
                }
                unitOfWork.RepositoryAsync<WorkflowInstance>().InsertOrUpdateGraph(entity);
                unitOfWork.SaveChanges();
                return new WorkflowHandlingResponse()
                {
                    Code = 1,
                    Message = "جریان کاری با موفقیت آغاز گردید"
                };
            }
        }

        public async Task<WorkflowHandlingResponse> StartWorkflowAsync(
          WorkflowStartInfo startInfo)
        {
            WorkflowFindData wfFindData = new WorkflowFindData()
            {
                StartType = startInfo.StartType,
                Code = startInfo.Code
            };
            WorkflowInfo wfInfo ;
           // WorkflowInfo workflowInfo = wfInfo;
            wfInfo = await this.FindWorkflowAsync(wfFindData);
            if (wfInfo == null)
                throw new WorkflowException(string.Format("workflow with code: '{0}' not found", (object)wfFindData.Code));
            WorkflowHandlingResponse handlingResponse;
            using (UnitOfWork uow = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                WorkflowStep wfStartStep = await uow.RepositoryAsync<WorkflowStep>().Queryable(false, true, (List<Expression<Func<WorkflowStep, object>>>)null).Where<WorkflowStep>((Expression<Func<WorkflowStep, bool>>)(wfStep => (int)wfStep.StepType == 1 && wfStep.WorkflowInfoId == wfInfo.ID)).SingleOrDefaultAsync<WorkflowStep>();
                if (wfStartStep == null)
                    throw new WorkflowException(string.Format("no start step for workflow with code '{0}'", (object)startInfo.Code));
                int? nullable = startInfo.StarterOrganizationId;
                if (!nullable.HasValue && SecurityManager.CurrentUserContext != null)
                    startInfo.StarterOrganizationId = SecurityManager.CurrentUserContext.OrganizationId;
                if (startInfo.ExchangeData == null)
                    startInfo.ExchangeData = new WFExchangeData();
                List<WorkflowInstanceState> instanceStateList = new List<WorkflowInstanceState>();
                WorkflowInstanceState workflowInstanceState = new WorkflowInstanceState();
                workflowInstanceState.AccomplishTime = new DateTime?(DateTime.Now);
                workflowInstanceState.ExchangeData = (string)startInfo.ExchangeData;
                workflowInstanceState.InstantiationTime = DateTime.Now;
                workflowInstanceState.ObjectState = ObjectState.Added;
                nullable = new int?();
                workflowInstanceState.SenderWorkflowInstanceStateId = nullable;
                workflowInstanceState.UserComment = "شروع فرایند";
                workflowInstanceState.StateStatus = WfStateStatus.Close;
                workflowInstanceState.Title = string.Format("شروع فرایند {0} {1}", (object)wfInfo.Title, (object)startInfo.InstanceTitle);
                workflowInstanceState.UserId = startInfo.StarterUserId;
                workflowInstanceState.WorkflowStepId = wfStartStep.ID;
                WorkflowInstanceState wfStartInstaceState = workflowInstanceState;
                instanceStateList.Add(wfStartInstaceState);
                WFExchangeData initialExchangeData = new WFExchangeData();
                WFExchangeData wfExchangeData1 = initialExchangeData;
                nullable = startInfo.StarterOrganizationId;
               // ref int? local1 = ref nullable;
                int num1;
                string str1;
                if (!nullable.HasValue)
                {
                    str1 = (string)null;
                }
                else
                {
                    num1 = nullable.GetValueOrDefault();
                    str1 = num1.ToString();
                }
                wfExchangeData1.Add("_StarterOrganizationId_", str1);
                WFExchangeData wfExchangeData2 = initialExchangeData;
                num1 = startInfo.StarterUserId;
                string str2 = num1.ToString();
                wfExchangeData2.Add("_StarterUserId_", str2);
                initialExchangeData.Add("_InstanceTitle_", string.Format("{0}", (object)startInfo.InstanceTitle));
                initialExchangeData.Add("_WorkflowTitle_", string.Format("{0}", (object)wfInfo.Title));
                initialExchangeData.Add("_RelatedRecordId_", string.Format("{0}", (object)startInfo.RelatedRecordId));
                bool hasEndStep = false;
                WorkflowStep currentStep = wfStartStep;
                WFExchangeData exchangeData = startInfo.ExchangeData;
                WorkflowInstanceState senderInstanceState = wfStartInstaceState;
                WFExchangeData initialExchangeData1 = initialExchangeData;
                nullable = new int?();
                int? actionId = nullable;
                //ref bool local2 = ref hasEndStep;
                List<WorkflowInstanceState> nextStateList = this.CreateNextStateList(currentStep, exchangeData, senderInstanceState, initialExchangeData1, actionId, out hasEndStep);
                instanceStateList.AddRange((IEnumerable<WorkflowInstanceState>)nextStateList);
                WorkflowInstance workflowInstance = new WorkflowInstance();
                workflowInstance.RelatedRecordId = startInfo.RelatedRecordId;
                workflowInstance.InitialExchangeData = (string)initialExchangeData;
                workflowInstance.StartTime = DateTime.Now;
                workflowInstance.Status = WfStateStatus.Open;
                workflowInstance.ObjectState = ObjectState.Added;
                workflowInstance.WorkflowInfoId = wfInfo.ID;
                workflowInstance.WorkflowInstanceStates = instanceStateList;
                WorkflowInstance wfInstance = workflowInstance;
                if (hasEndStep)
                {
                    wfInstance.FinishTime = new DateTime?(DateTime.Now);
                    wfInstance.Status = WfStateStatus.Close;
                }
                IRepositoryAsync<WorkflowInstance> workFlowInstanceRep = uow.RepositoryAsync<WorkflowInstance>();
                workFlowInstanceRep.InsertOrUpdateGraph(wfInstance);
                int num2 = await uow.SaveChangesAsync();
                handlingResponse = new WorkflowHandlingResponse()
                {
                    Code = 1,
                    Message = "جریان کاری با موفقیت آغاز گردید"
                };
            }
            return handlingResponse;
        }

        public WorkflowHandlingResponse ContinueWorkflow(
          WorkflowContinueInfo continueInfo)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                //using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions()
                //{
                //    IsolationLevel = IsolationLevel.ReadCommitted,
                //    Timeout = TimeSpan.MaxValue
                //}))
                //{
                    IRepositoryAsync<WorkflowInstanceState> repositoryAsync1 = unitOfWork.RepositoryAsync<WorkflowInstanceState>();
                    WorkflowInstanceState wfCurrentInstanceState = repositoryAsync1.Queryable(false, true, (List<Expression<Func<WorkflowInstanceState, object>>>)null).Where<WorkflowInstanceState>((Expression<Func<WorkflowInstanceState, bool>>)(wfInstanceState => wfInstanceState.ID == continueInfo.CurrentStateID)).SingleOrDefault<WorkflowInstanceState>();
                    if (wfCurrentInstanceState == null)
                        throw new WorkflowException(string.Format("the state not found. stateId:{0}", (object)continueInfo.CurrentStateID));
                    if (wfCurrentInstanceState.StateStatus == WfStateStatus.Close)
                        throw new OMFValidationException("این مرحله از فرایند قبلا انجام شده است ");
                    IRepositoryAsync<WorkflowInstance> repositoryAsync2 = unitOfWork.RepositoryAsync<WorkflowInstance>();
                    WorkflowInstance entity = repositoryAsync2.Queryable(false, true, (List<Expression<Func<WorkflowInstance, object>>>)null).Where<WorkflowInstance>((Expression<Func<WorkflowInstance, bool>>)(wfi => wfi.ID == wfCurrentInstanceState.WorkflowInstanceId)).SingleOrDefault<WorkflowInstance>();
                    WorkflowStep currentStep = unitOfWork.RepositoryAsync<WorkflowStep>().Queryable(false, true, (List<Expression<Func<WorkflowStep, object>>>)null).Where<WorkflowStep>((Expression<Func<WorkflowStep, bool>>)(wfStep => wfStep.ID == wfCurrentInstanceState.WorkflowStepId)).SingleOrDefault<WorkflowStep>();
                    if (currentStep == null)
                        throw new WorkflowException(string.Format("no step found with code '{0}'", (object)wfCurrentInstanceState.WorkflowStepId));
                    if (!CartableManager.HasUserAccessTo(new HasUserAccessToRequest()
                    {
                        UserId = wfCurrentInstanceState.UserId
                    }))
                        throw new WorkflowException("عدم مطابقت شناسه کاربر در ادامه فرایند");
                    wfCurrentInstanceState.WorkflowInstance = entity;
                    WFExchangeData wfExchangeData1 = this.CallMethod(currentStep.PreActionMethod, new ActionMethodParams()
                    {
                        ContinueInfo = continueInfo,
                        InitialExchangeData = (WFExchangeData)entity.InitialExchangeData,
                        WorkflowInstanceState = wfCurrentInstanceState
                    });
                    if (wfExchangeData1 != null)
                    {
                        foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>)wfExchangeData1)
                        {
                            if (continueInfo.ExchangeData.ContainsKey(keyValuePair.Key))
                                continueInfo.ExchangeData[keyValuePair.Key] = keyValuePair.Value;
                            else
                                continueInfo.ExchangeData.Add(keyValuePair.Key, keyValuePair.Value);
                        }
                    }
                    wfCurrentInstanceState.AccomplishTime = new DateTime?(DateTime.Now);
                    wfCurrentInstanceState.AccomplishActionId = new int?(continueInfo.ActionId);
                    wfCurrentInstanceState.StateStatus = WfStateStatus.Close;
                    wfCurrentInstanceState.UserComment = continueInfo.UserComment;
                    repositoryAsync1.Update(wfCurrentInstanceState);
                    if (continueInfo.ActionId == -100)
                        this.CallMethod(currentStep.PostActionMethod, new ActionMethodParams()
                        {
                            ContinueInfo = continueInfo,
                            InitialExchangeData = (WFExchangeData)entity.InitialExchangeData,
                            WorkflowInstanceState = wfCurrentInstanceState
                        });
                    else if (continueInfo.ActionId > 0)
                    {
                        List<WorkflowInstanceState> workflowInstanceStateList = new List<WorkflowInstanceState>();
                        bool hasEndStep = false;
                        int? targetUserId = continueInfo.TargetUserId;
                        if (targetUserId.HasValue && !continueInfo.ExchangeData.ContainsKey("_NextUserId_"))
                        {
                            WFExchangeData exchangeData = continueInfo.ExchangeData;
                            targetUserId = continueInfo.TargetUserId;
                            string str = targetUserId.ToString();
                            exchangeData.Add("_NextUserId_", str);
                        }
                        List<WorkflowInstanceState> nextStateList = this.CreateNextStateList(currentStep, continueInfo.ExchangeData, wfCurrentInstanceState, (WFExchangeData)entity.InitialExchangeData, new int?(continueInfo.ActionId), out hasEndStep);
                        repositoryAsync1.InsertRange((IEnumerable<WorkflowInstanceState>)nextStateList);
                        if (hasEndStep)
                        {
                            entity.FinishTime = new DateTime?(DateTime.Now);
                            entity.Status = WfStateStatus.Close;
                            repositoryAsync2.Update(entity);
                        }
                        WFExchangeData wfExchangeData2 = this.CallMethod(currentStep.PostActionMethod, new ActionMethodParams()
                        {
                            ContinueInfo = continueInfo,
                            InitialExchangeData = (WFExchangeData)entity.InitialExchangeData,
                            WorkflowInstanceState = wfCurrentInstanceState
                        });
                        if (wfExchangeData2 != null)
                        {
                            foreach (WorkflowInstanceState workflowInstanceState in nextStateList)
                            {
                                WFExchangeData exchangeData = (WFExchangeData)workflowInstanceState.ExchangeData;
                                foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>)wfExchangeData2)
                                {
                                    if (!exchangeData.ContainsKey(keyValuePair.Key))
                                        exchangeData.Add(keyValuePair.Key, keyValuePair.Value);
                                    else
                                        exchangeData[keyValuePair.Key] = keyValuePair.Value;
                                }
                                workflowInstanceState.ExchangeData = (string)exchangeData;
                            }
                        }
                    }
                    unitOfWork.SaveChangesAsync();
                  //  transactionScope.Complete();
                    return new WorkflowHandlingResponse()
                    {
                        Code = 1,
                        Message = "جریان کاری با موفقیت آغاز گردید"
                    };
                //}
            }
        }

        public async Task<WorkflowHandlingResponse> ContinueWorkflowAsync(
          WorkflowContinueInfo continueInfo)
        {
            WorkflowHandlingResponse handlingResponse;
            using (UnitOfWork uow = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                //using (TransactionScope trans = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions()
                //{
                //    IsolationLevel = IsolationLevel.ReadCommitted,
                //    Timeout = TimeSpan.MaxValue
                //}))
                {
                    IRepositoryAsync<WorkflowInstanceState> wfInstanceStateRep = uow.RepositoryAsync<WorkflowInstanceState>();
                    //  WorkflowInstanceState wfCurrentInstanceState;
                    // WorkflowInstanceState workflowInstanceState1 = wfCurrentInstanceState;
                    WorkflowInstanceState wfCurrentInstanceState = await wfInstanceStateRep.Queryable(false, true, (List<Expression<Func<WorkflowInstanceState, object>>>)null).Where<WorkflowInstanceState>((Expression<Func<WorkflowInstanceState, bool>>)(wfInstanceState => wfInstanceState.ID == continueInfo.CurrentStateID)).SingleOrDefaultAsync<WorkflowInstanceState>();
                    if (wfCurrentInstanceState == null)
                        throw new WorkflowException(string.Format("the state not found. stateId:{0}", (object)continueInfo.CurrentStateID));
                    if (wfCurrentInstanceState.StateStatus == WfStateStatus.Close)
                        throw new OMFValidationException("این مرحله از فرایند قبلا انجام شده است ");
                    IRepositoryAsync<WorkflowInstance> wfInstanceRep = uow.RepositoryAsync<WorkflowInstance>();
                    WorkflowInstance wfInstance = await wfInstanceRep.Queryable(false, true, (List<Expression<Func<WorkflowInstance, object>>>)null).Where<WorkflowInstance>((Expression<Func<WorkflowInstance, bool>>)(wfi => wfi.ID == wfCurrentInstanceState.WorkflowInstanceId)).SingleOrDefaultAsync<WorkflowInstance>();
                    WorkflowStep wfCurrentStep = await uow.RepositoryAsync<WorkflowStep>().Queryable(false, true, (List<Expression<Func<WorkflowStep, object>>>)null).Where<WorkflowStep>((Expression<Func<WorkflowStep, bool>>)(wfStep => wfStep.ID == wfCurrentInstanceState.WorkflowStepId)).SingleOrDefaultAsync<WorkflowStep>();
                    if (wfCurrentStep == null)
                        throw new WorkflowException(string.Format("no step found with code '{0}'", (object)wfCurrentInstanceState.WorkflowStepId));
                    bool async = await CartableManager.HasUserAccessToAsync(new HasUserAccessToRequest()
                    {
                        UserId = wfCurrentInstanceState.UserId
                    });
                    if (!async)
                        throw new WorkflowException("عدم مطابقت شناسه کاربر در ادامه فرایند");
                    wfCurrentInstanceState.WorkflowInstance = wfInstance;
                    WFExchangeData preActionExchangeData = this.CallMethod(wfCurrentStep.PreActionMethod, new ActionMethodParams()
                    {
                        ContinueInfo = continueInfo,
                        InitialExchangeData = (WFExchangeData)wfInstance.InitialExchangeData,
                        WorkflowInstanceState = wfCurrentInstanceState
                    });
                    if (preActionExchangeData != null)
                    {
                        foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>)preActionExchangeData)
                        {
                            KeyValuePair<string, string> item = keyValuePair;
                            if (continueInfo.ExchangeData.ContainsKey(item.Key))
                                continueInfo.ExchangeData[item.Key] = item.Value;
                            else
                                continueInfo.ExchangeData.Add(item.Key, item.Value);
                            item = new KeyValuePair<string, string>();
                        }
                    }
                    wfCurrentInstanceState.AccomplishTime = new DateTime?(DateTime.Now);
                    wfCurrentInstanceState.AccomplishActionId = new int?(continueInfo.ActionId);
                    wfCurrentInstanceState.StateStatus = WfStateStatus.Close;
                    wfCurrentInstanceState.UserComment = continueInfo.UserComment;
                    wfInstanceStateRep.Update(wfCurrentInstanceState);
                    if (continueInfo.ActionId == -100)
                        this.CallMethod(wfCurrentStep.PostActionMethod, new ActionMethodParams()
                        {
                            ContinueInfo = continueInfo,
                            InitialExchangeData = (WFExchangeData)wfInstance.InitialExchangeData,
                            WorkflowInstanceState = wfCurrentInstanceState
                        });
                    else if (continueInfo.ActionId > 0)
                    {
                        int? targetUserId = continueInfo.TargetUserId;
                        if (targetUserId.HasValue && !continueInfo.ExchangeData.ContainsKey("_NextUserId_"))
                        {
                            WFExchangeData exchangeData = continueInfo.ExchangeData;
                            targetUserId = continueInfo.TargetUserId;
                            string str = targetUserId.ToString();
                            exchangeData.Add("_NextUserId_", str);
                        }
                        List<WorkflowInstanceState> instanceStateList = new List<WorkflowInstanceState>();
                        bool hasEndStep = false;
                        List<WorkflowInstanceState> nextStateList = this.CreateNextStateList(wfCurrentStep, continueInfo.ExchangeData, wfCurrentInstanceState, (WFExchangeData)wfInstance.InitialExchangeData, new int?(continueInfo.ActionId), out hasEndStep);
                        wfInstanceStateRep.InsertRange((IEnumerable<WorkflowInstanceState>)nextStateList);
                        if (hasEndStep)
                        {
                            wfInstance.FinishTime = new DateTime?(DateTime.Now);
                            wfInstance.Status = WfStateStatus.Close;
                            wfInstanceRep.Update(wfInstance);
                        }
                        WFExchangeData postActionExchangeData = this.CallMethod(wfCurrentStep.PostActionMethod, new ActionMethodParams()
                        {
                            ContinueInfo = continueInfo,
                            InitialExchangeData = (WFExchangeData)wfInstance.InitialExchangeData,
                            WorkflowInstanceState = wfCurrentInstanceState
                        });
                        if (postActionExchangeData != null)
                        {
                            foreach (WorkflowInstanceState workflowInstanceState2 in nextStateList)
                            {
                                WorkflowInstanceState nextState = workflowInstanceState2;
                                WFExchangeData nextStateExchangeData = (WFExchangeData)nextState.ExchangeData;
                                foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>)postActionExchangeData)
                                {
                                    KeyValuePair<string, string> item = keyValuePair;
                                    if (!nextStateExchangeData.ContainsKey(item.Key))
                                        nextStateExchangeData.Add(item.Key, item.Value);
                                    else
                                        nextStateExchangeData[item.Key] = item.Value;
                                    item = new KeyValuePair<string, string>();
                                }
                                nextState.ExchangeData = (string)nextStateExchangeData;
                                nextStateExchangeData = (WFExchangeData)null;
                                nextState = (WorkflowInstanceState)null;
                            }
                        }
                        instanceStateList = (List<WorkflowInstanceState>)null;
                        nextStateList = (List<WorkflowInstanceState>)null;
                        postActionExchangeData = (WFExchangeData)null;
                    }
                    int num = await uow.SaveChangesAsync();
                  //  trans.Complete();
                    handlingResponse = new WorkflowHandlingResponse()
                    {
                        Code = 1,
                        Message = "جریان کاری با موفقیت آغاز گردید"
                    };
                }
            }
            return handlingResponse;
        }

        public void SetStatus(WorkflowSetStatusData statusData)
        {
            if (statusData.StateStatus == WfStateStatus.Open || statusData.StateStatus == WfStateStatus.Close)
                throw new WorkflowException("input data is not valid for setStatus");
            using (UnitOfWork unitOfWork = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                IRepositoryAsync<WorkflowInstance> repositoryAsync = unitOfWork.RepositoryAsync<WorkflowInstance>();
                WorkflowInstance entity = repositoryAsync.Queryable(false, true, (List<Expression<Func<WorkflowInstance, object>>>)null).SingleOrDefault<WorkflowInstance>((Expression<Func<WorkflowInstance, bool>>)(wfi => wfi.ID == statusData.WfInstanceId));
                if (entity == null)
                    throw new WorkflowException(string.Format("workflow instance doesn't exist. WorkflowInstanceId: {0}", (object)statusData.WfInstanceId));
                repositoryAsync.LoadCollection<WorkflowInstanceState>(entity, (Expression<Func<WorkflowInstance, ICollection<WorkflowInstanceState>>>)(wfi => wfi.WorkflowInstanceStates), false);
                if (entity.WorkflowInstanceStates.Any<WorkflowInstanceState>() && !entity.WorkflowInstanceStates.Any<WorkflowInstanceState>((Func<WorkflowInstanceState, bool>)(item => CartableManager.HasUserAccessTo(new HasUserAccessToRequest()
                {
                    UserId = item.UserId
                }))))
                    throw new WorkflowException("عدم مطابقت شناسه کاربر در ثبت وضعیت فرایند");
                entity.UserComment = statusData.UserComment;
                entity.Status = statusData.StateStatus;
                repositoryAsync.Update(entity);
                unitOfWork.SaveChanges();
            }
        }

        public WorkflowInfo FindWorkflow(WorkflowFindData workflowFindData)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
                return unitOfWork.Repository<WorkflowInfo>().Queryable(false, true, (List<Expression<Func<WorkflowInfo, object>>>)null).OrderByDescending<WorkflowInfo, int>((Expression<Func<WorkflowInfo, int>>)(wf => wf.ID)).Where<WorkflowInfo>((Expression<Func<WorkflowInfo, bool>>)(wf => wf.Code == workflowFindData.Code)).FirstOrDefault<WorkflowInfo>();
        }

        public async Task SetStatusAsync(WorkflowSetStatusData statusData)
        {
            if (statusData.StateStatus == WfStateStatus.Open || statusData.StateStatus == WfStateStatus.Close)
                throw new WorkflowException("input data is not valid for setStatus");
            using (UnitOfWork uow = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                IRepositoryAsync<WorkflowInstance> workFlowInstanceRep = uow.RepositoryAsync<WorkflowInstance>();
                WorkflowInstance wfInstance = await workFlowInstanceRep.Queryable(false, true, (List<Expression<Func<WorkflowInstance, object>>>)null).SingleOrDefaultAsync<WorkflowInstance>((Expression<Func<WorkflowInstance, bool>>)(wfi => wfi.ID == statusData.WfInstanceId));
                if (wfInstance == null)
                    throw new WorkflowException(string.Format("workflow instance doesn't exist. WorkflowInstanceId: {0}", (object)statusData.WfInstanceId));
                await workFlowInstanceRep.LoadCollectionAsync<WorkflowInstanceState>(wfInstance, (Expression<Func<WorkflowInstance, ICollection<WorkflowInstanceState>>>)(wfi => wfi.WorkflowInstanceStates), false);
                if (wfInstance.WorkflowInstanceStates.Any<WorkflowInstanceState>() && !wfInstance.WorkflowInstanceStates.Any<WorkflowInstanceState>((Func<WorkflowInstanceState, bool>)(item => CartableManager.HasUserAccessTo(new HasUserAccessToRequest()
                {
                    UserId = item.UserId
                }))))
                    throw new WorkflowException("عدم مطابقت شناسه کاربر در ثبت وضعیت فرایند");
                wfInstance.UserComment = statusData.UserComment;
                wfInstance.Status = statusData.StateStatus;
                workFlowInstanceRep.Update(wfInstance);
                int num = await uow.SaveChangesAsync();
                workFlowInstanceRep = (IRepositoryAsync<WorkflowInstance>)null;
                wfInstance = (WorkflowInstance)null;
            }
        }

        public async Task<WorkflowInfo> FindWorkflowAsync(
          WorkflowFindData workflowFindData)
        {
            WorkflowInfo workflowInfo;
            using (UnitOfWork uow = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                WorkflowInfo wfInfo = await uow.RepositoryAsync<WorkflowInfo>().Queryable(false, true, (List<Expression<Func<WorkflowInfo, object>>>)null).OrderByDescending<WorkflowInfo, int>((Expression<Func<WorkflowInfo, int>>)(wf => wf.ID)).Where<WorkflowInfo>((Expression<Func<WorkflowInfo, bool>>)(wf => wf.Code == workflowFindData.Code)).FirstOrDefaultAsync<WorkflowInfo>();
                workflowInfo = wfInfo;
            }
            return workflowInfo;
        }

        private List<WorkflowInstanceState> CreateNextStateList(
          WorkflowStep currentStep,
          WFExchangeData currentExchangeData,
          WorkflowInstanceState senderInstanceState,
          WFExchangeData initialExchangeData,
          int? actionId,
          out bool hasEndStep)
        {
            hasEndStep = false;
            using (UnitOfWork unitOfWork = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                IQueryable<WorkflowStepAction> source = unitOfWork.RepositoryAsync<WorkflowStepAction>().Queryable(false, true, (List<Expression<Func<WorkflowStepAction, object>>>)null).Where<WorkflowStepAction>((Expression<Func<WorkflowStepAction, bool>>)(wfsa => wfsa.WorkflowStepId == currentStep.ID));
                if (actionId.HasValue)
                    source = source.Where<WorkflowStepAction>((Expression<Func<WorkflowStepAction, bool>>)(wfsa => (int?)wfsa.WFActionTypeId == actionId));
                IQueryable<WorkflowStep> queryable = unitOfWork.RepositoryAsync<WorkflowStep>().Queryable(false, true, (List<Expression<Func<WorkflowStep, object>>>)null).Join<WorkflowStep, WorkflowStepAction, int, WorkflowStep>((IEnumerable<WorkflowStepAction>)source, (Expression<Func<WorkflowStep, int>>)(wfStep => wfStep.ID), (Expression<Func<WorkflowStepAction, int>>)(wfStepAction => wfStepAction.NextWorkflowStepId), (Expression<Func<WorkflowStep, WorkflowStepAction, WorkflowStep>>)((wfStep, wfStepAction) => wfStep));
                List<WorkflowInstanceState> workflowInstanceStateList = new List<WorkflowInstanceState>();
                foreach (WorkflowStep workflowStep in (IEnumerable<WorkflowStep>)queryable)
                {
                    if (workflowStep.StepType == StepType.Start)
                        throw new WorkflowException(string.Format("گام شروع فرایند به درستی طراحی نشده است. شناسه فرایند: {0}", (object)currentStep.WorkflowInfoId));
                    WorkflowInstanceState workflowInstanceState1;
                    if (workflowStep.StepType == StepType.End)
                    {
                        hasEndStep = workflowStep.StepType == StepType.End;
                        WorkflowInstanceState workflowInstanceState2 = new WorkflowInstanceState();
                        workflowInstanceState2.ExchangeData = (string)null;
                        workflowInstanceState2.InstantiationTime = DateTime.Now;
                        workflowInstanceState2.ObjectState = ObjectState.Added;
                        workflowInstanceState2.SenderWorkflowInstanceStateId = new int?(senderInstanceState.ID);
                        workflowInstanceState2.Title = string.Format("{0} {1}", (object)initialExchangeData["_WorkflowTitle_"], (object)initialExchangeData["_InstanceTitle_"]);
                        workflowInstanceState2.WorkflowStepId = workflowStep.ID;
                        workflowInstanceState2.SenderWorkflowInstanceState = senderInstanceState;
                        workflowInstanceState2.WorkflowInstanceId = senderInstanceState.WorkflowInstanceId;
                        workflowInstanceState2.AccomplishTime = new DateTime?(DateTime.Now);
                        workflowInstanceState2.StateStatus = WfStateStatus.Close;
                        workflowInstanceState2.UserComment = "خاتمه فرایند";
                        workflowInstanceState1 = workflowInstanceState2;
                    }
                    else if (workflowStep.StepType == StepType.SendToStarter)
                    {
                        if (hasEndStep && workflowStep.MessageType == MessageType.Task)
                            throw new WorkflowException(string.Format("گام پایانی فرایند به درستی طراحی نشده است. شناسه فرایند: {0}", (object)currentStep.WorkflowInfoId));
                        if (currentExchangeData.ContainsKey("_NextUserId_"))
                            currentExchangeData.Remove("_NextUserId_");
                        WorkflowInstanceState workflowInstanceState2 = new WorkflowInstanceState();
                        workflowInstanceState2.AccomplishTime = new DateTime?();
                        workflowInstanceState2.ExchangeData = (string)currentExchangeData;
                        workflowInstanceState2.InstantiationTime = DateTime.Now;
                        workflowInstanceState2.ObjectState = ObjectState.Added;
                        workflowInstanceState2.SenderWorkflowInstanceStateId = new int?(senderInstanceState.ID);
                        workflowInstanceState2.StateStatus = WfStateStatus.Open;
                        workflowInstanceState2.Title = string.Format("{0} {1}", (object)initialExchangeData["_WorkflowTitle_"], (object)initialExchangeData["_InstanceTitle_"]);
                        workflowInstanceState2.UserId = initialExchangeData["_StarterUserId_"].ConvertTo<int>();
                        workflowInstanceState2.WorkflowStepId = workflowStep.ID;
                        workflowInstanceState2.SenderWorkflowInstanceState = senderInstanceState;
                        workflowInstanceState2.WorkflowInstanceId = senderInstanceState.WorkflowInstanceId;
                        workflowInstanceState1 = workflowInstanceState2;
                    }
                    else
                    {
                        if (hasEndStep && workflowStep.MessageType == MessageType.Task)
                            throw new WorkflowException(string.Format("گام پایانی فرایند به درستی طراحی نشده است. شناسه فرایند: {0}", (object)currentStep.WorkflowInfoId));
                        int userIdFromRoleId;
                        if (currentExchangeData.ContainsKey("_NextUserId_"))
                        {
                            userIdFromRoleId = currentExchangeData["_NextUserId_"].ConvertTo<int>();
                            currentExchangeData.Remove("_NextUserId_");
                        }
                        else
                            userIdFromRoleId = this.GetUserIdFromRoleId(workflowStep.RoleId, new int?(workflowStep.OrganizationId ?? initialExchangeData["_StarterOrganizationId_"].ConvertTo<int>()));
                        WorkflowInstanceState workflowInstanceState2 = new WorkflowInstanceState();
                        workflowInstanceState2.AccomplishTime = new DateTime?();
                        workflowInstanceState2.ExchangeData = (string)currentExchangeData;
                        workflowInstanceState2.InstantiationTime = DateTime.Now;
                        workflowInstanceState2.ObjectState = ObjectState.Added;
                        workflowInstanceState2.SenderWorkflowInstanceStateId = new int?(senderInstanceState.ID);
                        workflowInstanceState2.StateStatus = WfStateStatus.Open;
                        workflowInstanceState2.Title = string.Format("{0} {1}", (object)initialExchangeData["_WorkflowTitle_"], (object)initialExchangeData["_InstanceTitle_"]);
                        workflowInstanceState2.UserId = userIdFromRoleId;
                        workflowInstanceState2.WorkflowStepId = workflowStep.ID;
                        workflowInstanceState2.SenderWorkflowInstanceState = senderInstanceState;
                        workflowInstanceState2.WorkflowInstanceId = senderInstanceState.WorkflowInstanceId;
                        workflowInstanceState1 = workflowInstanceState2;
                    }
                    workflowInstanceStateList.Add(workflowInstanceState1);
                }
                return workflowInstanceStateList;
            }
        }

        private int GetUserIdFromRoleId(int? roleId, int? organId)
        {
            if (!roleId.HasValue)
                throw new WorkflowException("roleId is null.");
            int? nullable1 = organId;
            int num1 = 0;
            if (nullable1.GetValueOrDefault() == num1 && nullable1.HasValue)
                organId = new int?();
            int? nullable2 = organId;
            int num2 = -1;
            if (nullable2.GetValueOrDefault() == num2 && nullable2.HasValue)
                organId = new int?();
            using (UnitOfWork unitOfWork = new UnitOfWork((IDataContextAsync)new WorkflowDbContext()))
            {
                IQueryable<UserInfo> queryable = unitOfWork.RepositoryAsync<UserInfo>().Queryable(false, true, (List<Expression<Func<UserInfo, object>>>)null).Where<UserInfo>((Expression<Func<UserInfo, bool>>)(user => user.OrganizationId == organId && user.IsActive && !user.IsDeleted));
                IQueryable<UserRole> outer = unitOfWork.RepositoryAsync<UserRole>().Queryable(false, true, (List<Expression<Func<UserRole, object>>>)null).Where<UserRole>((Expression<Func<UserRole, bool>>)(ur => (int?)ur.RoleId == roleId));
                var roleData = unitOfWork.RepositoryAsync<RoleBase>().Queryable(false, true, (List<Expression<Func<RoleBase, object>>>)null).Where<RoleBase>((Expression<Func<RoleBase, bool>>)(role => (int?)role.ID == roleId)).Select(r => new
                {
                    RoleId = r.ID,
                    RoleName = r.Name
                }).FirstOrDefault();
                if (roleData == null)
                {
                    ExceptionManager.SaveException((Exception)new WorkflowException(string.Format("role with Id: '{0}' not found.", (object)roleId)));
                    throw new OMFValidationException("نقش مورد نظر برای ادامه فرایند تعریف نشده است");
                }
                var data1 = outer.Join((IEnumerable<UserInfo>)queryable, (Expression<Func<UserRole, int>>)(userRole => userRole.UserId), (Expression<Func<UserInfo, int>>)(user => user.ID), (userRole, user) => new
                {
                    userRole = userRole,
                    user = user
                }).Where(data => data.userRole.RoleId == roleData.RoleId).Select(data => new
                {
                    UserId = data.user.ID
                }).FirstOrDefault();
                if (data1 != null)
                    return data1.UserId;
                ExceptionManager.SaveException((Exception)new WorkflowException(string.Format("user with roleId: '{0}' and roleName:'{1}' in organId: {2} not found.", (object)roleId, (object)roleData.RoleName, (object)organId)));
                throw new OMFValidationException(string.Format("هیچ کاربر فعالی برای نقش یا سمت {0} تعریف نشده است", (object)roleData.RoleName));
            }
        }

        private WFExchangeData CallMethod(
          string actionMethod,
          ActionMethodParams actionMethodParams)
        {
            if (string.IsNullOrEmpty(actionMethod))
                return (WFExchangeData)null;
            string[] strArray = actionMethod.Split(',');
            if (strArray.Length != 2)
                throw new WorkflowException(string.Format("actionMethod field is not correct, actionMethod: {0}, stepId: {1} ", (object)actionMethod, (object)actionMethodParams.WorkflowInstanceState.WorkflowStepId));
            string methodName = strArray[0].Trim().ToLower();
            string className = strArray[1].Trim().ToLower();
            if (string.IsNullOrEmpty(ConfigurationController.BusinessAssemblyName))
                throw new WorkflowException("BusinessAssemblyName is not set in config file.");
            Assembly assembly = ConfigurationController.GetAssembly(ConfigurationController.BusinessAssemblyName);
            Type type = ((IEnumerable<Type>)assembly.GetExportedTypes()).FirstOrDefault<Type>((Func<Type, bool>)(t => t.FullName.ToLower() == className));
            if (type == (Type)null)
                throw new WorkflowException(string.Format("className is not found in assembly, className: {0}, assemblyName: {1} ", (object)strArray[1], (object)assembly.GetName().Name));
            MethodInfo methodInfo = ((IEnumerable<MethodInfo>)type.GetMethods()).FirstOrDefault<MethodInfo>((Func<MethodInfo, bool>)(m => m.Name.ToLower() == methodName));
            if (methodInfo == (MethodInfo)null)
                throw new WorkflowException(string.Format("methodName is not found in class, methodName: {0}, className: {1}", (object)strArray[0], (object)strArray[1]));
            object[] parameters = new object[1]
            {
        (object) actionMethodParams
            };
            return (WFExchangeData)methodInfo.Invoke((object)null, parameters);
        }
    }
}
