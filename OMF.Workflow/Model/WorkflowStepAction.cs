using OMF.Common;

namespace OMF.Workflow.Model
{
    public class WorkflowStepAction : LoggableEntityName
    {
        public bool? TerminationStatus { get; set; }

        public int WorkflowStepId { get; set; }

        public int NextWorkflowStepId { get; set; }

        public int SubWorkflowInfoId { get; set; }

        public int WFActionTypeId { get; set; }

        public string DesignMetaData { get; set; }
    }
}
