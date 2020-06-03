using OMF.Common;
using System.Collections.Generic;

namespace OMF.Workflow.Model
{
    public class WorkflowInfo : LoggableEntityName
    {
        public string Title { get; set; }

        public string Version { get; set; }

        public string Tag01 { get; set; }

        public string Tag02 { get; set; }

        public string Tag03 { get; set; }

        public string Tag04 { get; set; }

        public string Tag05 { get; set; }

        public string Tag06 { get; set; }

        public string Tag07 { get; set; }

        public string Tag08 { get; set; }

        public string Tag09 { get; set; }

        public string Tag10 { get; set; }

        public string Code { get; set; }

        public string Icon { get; set; }

        public List<WorkflowStepAction> WorkflowStepActions { get; set; }

        public List<WorkflowInstance> WorkflowInstances { get; set; }

        public List<WorkflowStep> WorkflowSteps { get; set; }
    }
}
