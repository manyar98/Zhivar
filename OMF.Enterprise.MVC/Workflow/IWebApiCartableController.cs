using OMF.Workflow;
using OMF.Workflow.Cartable.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace OMF.Enterprise.MVC.Workflow
{
    public interface IWebApiCartableController : IHttpController, IDisposable
    {
        Task<HttpResponseMessage> GetMenuItems(MenuItemsRequest request);

        Task<HttpResponseMessage> GetSubMenuItems(SubMenuItemsRequest request);

        Task<HttpResponseMessage> GetMessages(GetMessagesRequest request);

        Task<HttpResponseMessage> ContinueWorkflow(
          List<WorkflowContinueInfo> continueInfo);

        Task<HttpResponseMessage> GetMessageInfoHistories(
          MessageInfoHistoryRequest messageHistoryRequest);

        Task<HttpResponseMessage> GetWorkflowStepAction(
          MessageActionsRequest messageActionRequest);

        Task<HttpResponseMessage> GetNextSteps(NextStepsRequest nextStepsRequest);

        Task<HttpResponseMessage> GetCurrentUserRoles();
    }
}
