using OMF.Workflow.Model;

namespace OMF.Workflow
{
    public class ActionMethodParams
    {
        public WorkflowContinueInfo ContinueInfo { get; set; }

        public WFExchangeData InitialExchangeData { get; set; }

        public WorkflowInstanceState WorkflowInstanceState { get; set; }
    }
}
