using OMF.Common;
using System;
using static OMF.Workflow.Enums;

namespace OMF.Workflow.Model
{
    public class WorkflowInstanceState : LoggableEntityName
    {
        public string Title { get; set; }

        public WfStateStatus StateStatus { get; set; }

        public DateTime InstantiationTime { get; set; }

        public DateTime? AccomplishTime { get; set; }

        public DateTime? ViewDateTime { get; set; }

        public int? AccomplishActionId { get; set; }

        public string UserComment { get; set; }

        public string ExchangeData { get; set; }

        public int UserId { get; set; }

        public int WorkflowInstanceId { get; set; }

        public int SubWorkflowInstanceId { get; set; }

        public int WorkflowStepId { get; set; }

        public int? SenderWorkflowInstanceStateId { get; set; }

        public WorkflowInstanceState SenderWorkflowInstanceState { get; set; }

        public WorkflowInstance WorkflowInstance { get; set; }
    }
}
