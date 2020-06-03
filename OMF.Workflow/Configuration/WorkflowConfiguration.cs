using OMF.Workflow.Model;
using System;

namespace OMF.Workflow.Configuration
{
    public class WorkflowConfiguration
    {
        private static WorkflowConfiguration current = new WorkflowConfiguration();
        private Func<WorkflowFindData, WorkflowInfo> WorkflowFinder;

        public static WorkflowConfiguration Current
        {
            get
            {
                return WorkflowConfiguration.current;
            }
        }

        private WorkflowConfiguration()
        {
        }

        public void SetWorkflowFinder(
          Func<WorkflowFindData, WorkflowInfo> workflowFinder)
        {
        }
    }
}
