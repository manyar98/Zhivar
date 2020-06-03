using OMF.EntityFramework.Query;

namespace OMF.Workflow.Cartable.Model
{
    public class GetMessagesRequest
    {
        public int? RoleId { get; set; }

        public int? UserId { get; set; }

        public int WorkflowStepId { get; set; }

        public QueryInfo searchRequestInfo { get; set; }
    }
}
