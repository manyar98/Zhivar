using OMF.Common;
using System;
using System.Collections.Generic;
using static OMF.Workflow.Enums;

namespace OMF.Workflow.Model
{
    public class WorkflowInstance : LoggableEntityName
    {
        public int? ParentId { get; set; }

        public int WorkflowInfoId { get; set; }

        public int RelatedRecordId { get; set; }

        public WfStateStatus Status { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? FinishTime { get; set; }

        public string InitialExchangeData { get; set; }

        public string UserComment { get; set; }

        public List<WorkflowInstanceState> WorkflowInstanceStates { get; set; }
    }
}
