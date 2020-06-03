using static OMF.Workflow.Enums;

namespace OMF.Workflow
{
    public class WorkflowStartInfo
    {
        public StartType StartType { get; set; }

        public string Code { get; set; }

        public WFExchangeData ExchangeData { get; set; }

        public int RelatedRecordId { get; set; }

        public int? StarterOrganizationId { get; set; }

        public int StarterUserId { get; set; }

        public string InstanceTitle { get; set; }
    }
}
