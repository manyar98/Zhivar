using OMF.Common;
using System.Collections.Generic;

namespace OMF.Workflow.Model
{
    public class WFActionType : LoggableEntityName
    {
        public string Title { get; set; }

        public bool NeedConfirm { get; set; }

        public string ConfirmMessage { get; set; }

        public List<WorkflowStepAction> WorkflowStepActions { get; set; }
    }
}
