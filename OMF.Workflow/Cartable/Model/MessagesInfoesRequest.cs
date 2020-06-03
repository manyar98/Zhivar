
using OMF.EntityFramework.Query;

namespace OMF.Workflow.Cartable.Model
{
    public class MessagesInfoesRequest
    {
        public int RoleId { get; set; }

        public int UserId { get; set; }

        public QueryInfo SearchRequestInfo { get; set; }

        public int WorkflowStepId { get; set; }
    }
}
