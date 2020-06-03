using OMF.Common;
using System.Collections.Generic;
using static OMF.Workflow.Enums;

namespace OMF.Workflow.Model
{
    public class WorkflowStep : LoggableEntityName
    {
        public int WorkflowInfoId { get; set; }

        public string Title { get; set; }

        public string Code { get; set; }

        public string Icon { get; set; }

        public bool MultiCheckAction { get; set; }

        public int? OrganizationId { get; set; }

        public int? RoleId { get; set; }

        public bool NeedToSign { get; set; }

        public int StepNo { get; set; }

        public int StepLevel { get; set; }

        public MessageType MessageType { get; set; }

        public Priority MessagePriority { get; set; }

        public StepType StepType { get; set; }

        public string DecisionLogic { get; set; }

        public string ActionUriRoute { get; set; }

        public string MasterUriRoute { get; set; }

        public string PreActionMethod { get; set; }

        public string PostActionMethod { get; set; }

        public string DesignMetaData { get; set; }

        public List<WorkflowStepAction> WorkflowStepActions { get; set; }

        public List<WorkflowInstanceState> WorkflowInstanceStates { get; set; }
    }
}
