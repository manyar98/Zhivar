using OMF.Workflow.Model;

namespace OMF.Workflow
{
    public interface IWorkflowHandler
    {
        WorkflowInfo FindWorkflow(WorkflowFindData workflowFindData);

        WorkflowHandlingResponse StartWorkflow(WorkflowStartInfo startInfo);

        WorkflowHandlingResponse ContinueWorkflow(
          WorkflowContinueInfo continueInfo);

        void SetStatus(WorkflowSetStatusData statusData);
    }
}
