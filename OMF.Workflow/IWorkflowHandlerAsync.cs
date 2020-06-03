using OMF.Workflow.Model;
using System.Threading.Tasks;

namespace OMF.Workflow
{
    public interface IWorkflowHandlerAsync : IWorkflowHandler
    {
        Task<WorkflowInfo> FindWorkflowAsync(WorkflowFindData workflowFindData);

        Task<WorkflowHandlingResponse> StartWorkflowAsync(
          WorkflowStartInfo startInfo);

        Task<WorkflowHandlingResponse> ContinueWorkflowAsync(
          WorkflowContinueInfo continueInfo);

        Task SetStatusAsync(WorkflowSetStatusData statusData);
    }
}
