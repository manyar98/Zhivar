
using System;
using static OMF.Workflow.Enums;

namespace OMF.Workflow.Cartable.Model
{
    public class MessageInfo
    {
        public int ID { get; set; }

        public int WorkflowId { get; set; }

        public int WorkflowInstanceId { get; set; }

        public int WorkflowStepId { get; set; }

        public string Title { get; set; }

        public MessageType MessageType { get; set; }

        internal string ExchangeDataString { get; set; }

        public WFExchangeData ExchangeData
        {
            get
            {
                return (WFExchangeData)this.ExchangeDataString;
            }
            internal set
            {
                this.ExchangeData = value;
            }
        }

        public int MasterId { get; set; }

        public string MasterUriRoute { get; set; }

        public string ActionUriRoute { get; set; }

        public DateTime SendDateTime { get; set; }

        public string WorkflowTitle { get; set; }

        public Priority Priority { get; set; }

        public int SenderUserId { get; set; }

        public string SenderFullName { get; set; }

        public int SenderRoleId { get; set; }

        public string SenderRoleName { get; set; }

        public bool NeedToSign { get; set; }

        public WfStateStatus WorkflowStatus { get; set; }

        public string UserComment { get; set; }
    }
}
