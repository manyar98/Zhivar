using System;
using System.Collections.Generic;
using System.Linq;
using OMF.Workflow.Model;
using System.Threading.Tasks;

namespace OMF.Workflow
{
    public static class WorkflowManager
    {
        private static IWorkflowHandlerAsync wfHandler = (IWorkflowHandlerAsync)new OMFWorkflowHandler();

        public static void InitiateWorkflowHandler(IWorkflowHandlerAsync workflowHandler)
        {
            WorkflowManager.wfHandler = workflowHandler;
        }

        public static WorkflowHandlingResponse StartWorkflow(
          WorkflowStartInfo startInfo)
        {
            return WorkflowManager.wfHandler.StartWorkflow(startInfo);
        }

        public static WorkflowHandlingResponse ContinueWorkflow(
          WorkflowContinueInfo continueInfo)
        {
            return WorkflowManager.wfHandler.ContinueWorkflow(continueInfo);
        }

        public static void SetStatus(WorkflowSetStatusData statusData)
        {
            WorkflowManager.wfHandler.SetStatus(statusData);
        }

        public static async Task<WorkflowHandlingResponse> StartWorkflowAsync(
          WorkflowStartInfo startInfo)
        {
            WorkflowHandlingResponse handlingResponse = await WorkflowManager.wfHandler.StartWorkflowAsync(startInfo);
            return handlingResponse;
        }

        public static async Task<WorkflowHandlingResponse> ContinueWorkflowAsync(
          WorkflowContinueInfo continueInfo)
        {
            WorkflowHandlingResponse handlingResponse = await WorkflowManager.wfHandler.ContinueWorkflowAsync(continueInfo);
            return handlingResponse;
        }

        public static async Task SetStatusAsync(WorkflowSetStatusData statusData)
        {
            await WorkflowManager.wfHandler.SetStatusAsync(statusData);
        }

        public static WorkflowInfo FindWorkflow(WorkflowFindData workflowFindData)
        {
            return WorkflowManager.wfHandler.FindWorkflow(workflowFindData);
        }

        public static async Task<WorkflowInfo> FindWorkflowAsync(
          WorkflowFindData workflowFindData)
        {
            WorkflowInfo workflowAsync = await WorkflowManager.wfHandler.FindWorkflowAsync(workflowFindData);
            return workflowAsync;
        }
    }
}
