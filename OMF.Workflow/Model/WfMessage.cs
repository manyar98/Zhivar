using OMF.Common;
using System;
using static OMF.Workflow.Enums;

namespace OMF.Workflow.Model
{
    public class WfMessage : LoggableEntityName
    {
        public int WorkflowId { get; set; }

        public int WorkflowInstanceId { get; set; }

        public int WorkflowStepId { get; set; }

        public string Title { get; set; }

        public MessageType MessageType { get; set; }

        public string ExchangeDataString { get; set; }

        public int MasterId { get; set; }

        public string MasterUriRoute { get; set; }

        public string ActionUriRoute { get; set; }

        public DateTime? ViewDateTime { get; set; }

        public DateTime SendDateTime { get; set; }

        public string WorkflowTitle { get; set; }

        public Priority Priority { get; set; }

        public int SenderUserId { get; set; }

        public int SenderRoleId { get; set; }

        public bool NeedToSign { get; set; }

        public WfStateStatus WorkflowStatus { get; set; }

        public MessageCategory Category { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }

        public WFExchangeData ExchangeData
        {
            get
            {
                return (WFExchangeData)this.ExchangeDataString;
            }
        }
    }
}
