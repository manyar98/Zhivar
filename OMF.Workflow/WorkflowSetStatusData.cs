using static OMF.Workflow.Enums;

namespace OMF.Workflow
{
    public class WorkflowSetStatusData
    {
        public int WfInstanceId { get; set; }

        public WfStateStatus StateStatus { get; set; }

        public string UserComment { get; set; }
    }
}
