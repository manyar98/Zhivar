
using static OMF.Workflow.Enums;

namespace OMF.Workflow.Cartable.Model
{
    public class MessageInfoHistoryRequest
    {
        public int RelatedRecordId { get; set; }

        public string WorkflowInfoCode { get; set; }

        public int WorkflowInstanceId { get; set; }

        public int CurrentStateId { get; set; }

        public FetchHistoryMode FetchHistoryMode { get; set; }
    }
}
