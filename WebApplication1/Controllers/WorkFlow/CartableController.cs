using OMF.Common;
using OMF.Common.Extensions;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.Query;
using OMF.Security.Model;
using OMF.Workflow.Cartable.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using OMF.Workflow;
using OMF.Common.Security;
using System.Data.Entity;
using System.Collections.Generic;
using System.Reflection;
using OMF.Enterprise.MVC.Workflow;
using Behsho.Common.Security;
using OMF.Common.ExceptionManagement.Exceptions;
using Zhivar.DomainClasses.BaseInfo;
using OMF.Workflow.Cartable;
using Behsho.Common;
using Zhivar.Business.Workflows;
using static Zhivar.DomainClasses.ZhivarEnums;
using static OMF.Workflow.Enums;

namespace Zhivar.Web.Controllers.WorkFlow
{
    public class CartableController : ApiController, IWebApiCartableController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> GetMenuItems([FromBody]MenuItemsRequest request)
        {
            try
            {
                SecurityManager.ThrowIfUserContextNull();

                if (!request.RoleId.HasValue)
                {
                    if (SecurityManager.CurrentUserContext.IsOperatorPersonnelUser())//|| SecurityManager.CurrentUserContext.IsKargozariPersonnelUser())
                        request.RoleId = SecurityManager.CurrentUserContext.Roles[0].RoleID;

                    else
                        throw new OMFValidationException("نقش یا سمت کاربر مشخص نمی باشد");
                }

                if (!request.UserId.HasValue)
                    request.UserId = SecurityManager.CurrentUserContext.UserId;

                List<CtbMenuItem> menuItems = new List<CtbMenuItem>();
                menuItems = await CartableManager.GetMenuItemsAsync(request);

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = menuItems });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetSubMenuItems([FromBody]SubMenuItemsRequest request)
        {
            try
            {
                SecurityManager.ThrowIfUserContextNull();

                if (!request.RoleId.HasValue)
                {
                    if (SecurityManager.CurrentUserContext.IsOperatorPersonnelUser())// || SecurityManager.CurrentUserContext.IsKargozariPersonnelUser())
                        request.RoleId = SecurityManager.CurrentUserContext.Roles[0].RoleID;

                    else
                        throw new OMFValidationException("نقش یا سمت کاربر مشخص نمی باشد");
                }

                if (!request.UserId.HasValue)
                    request.UserId = SecurityManager.CurrentUserContext.UserId;

                List<CtbSubMenuItem> subMenuItems = new List<CtbSubMenuItem>();
                subMenuItems = await CartableManager.GetSubMenuItemsAsync(request);

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = subMenuItems });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

        public async Task<HttpResponseMessage> GetMessages([FromUri]GetMessagesRequest request)
        {
            try
            {
                SecurityManager.ThrowIfUserContextNull();

                if (!request.RoleId.HasValue)
                {
                    if (SecurityManager.CurrentUserContext.IsOperatorPersonnelUser())// || SecurityManager.CurrentUserContext.IsKargozariPersonnelUser())
                        request.RoleId = SecurityManager.CurrentUserContext.Roles[0].RoleID;

                    else
                        throw new OMFValidationException("نقش یا سمت کاربر مشخص نمی باشد");
                }

                if (!request.UserId.HasValue)
                    request.UserId = SecurityManager.CurrentUserContext.UserId;

                List<MessageInfo> messageInfoList = await CartableManager.GetMessagesAsync(new MessagesInfoesRequest()
                {
                    RoleId = request.RoleId.Value,
                    SearchRequestInfo = request.searchRequestInfo,
                    UserId = request.UserId.Value,
                    WorkflowStepId = request.WorkflowStepId
                });

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = messageInfoList });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetWorkflowStepAction([FromBody]MessageActionsRequest messageActionRequest)
        {
            try
            {
                SecurityManager.ThrowIfUserContextNull();
                List<CtbMessageAction> result = await CartableManager.GetMessageStepActionsAsync(messageActionRequest);
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = result });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetNextSteps([FromBody]NextStepsRequest nextStepsRequest)
        {
            try
            {
                SecurityManager.ThrowIfUserContextNull();
                List<NextStepInfo> result = await CartableManager.GetNextStepsAsync(nextStepsRequest);
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = result });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

        public async Task<HttpResponseMessage> GetMessageInfoHistories([FromUri]MessageInfoHistoryRequest messageHistoryRequest)
        {
            try
            {
                SecurityManager.ThrowIfUserContextNull();
                messageHistoryRequest.FetchHistoryMode = FetchHistoryMode.UpToCurrentState;
                List<MessageInfoHistory> messageInfoHistoryList = await CartableManager.GetMessageInfoHistoriesAsync(messageHistoryRequest);
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = messageInfoHistoryList });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

        public async Task<HttpResponseMessage> ContinueWorkflow(List<WorkflowContinueInfo> continueInfo)
        {
            try
            {
                foreach (WorkflowContinueInfo workflowContinueInfo in continueInfo)
                {
                    if (workflowContinueInfo.ExchangeData == null)
                        workflowContinueInfo.ExchangeData = new WFExchangeData();
                    await WorkflowManager.ContinueWorkflowAsync(workflowContinueInfo);
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = "" });
            }
            catch (TargetInvocationException ex)
            {
                return await this.HandleExceptionAsync(ex.InnerException);
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

        public async Task<HttpResponseMessage> GetCurrentUserRoles()
        {
            try
            {
                SecurityManager.ThrowIfUserContextNull();
                var rdc = await new ZhivarCartableHandler().GetCurrentUserRoles();
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = rdc });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

    }
}