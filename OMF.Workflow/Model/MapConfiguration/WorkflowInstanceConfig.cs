using OMF.Common.Configuration;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static OMF.Workflow.Enums;

namespace OMF.Workflow.Model.MapConfiguration
{
    public class WorkflowInstanceConfig : BaseEntityTypeConfig<WorkflowInstance>
    {
        public WorkflowInstanceConfig()
        {
            this.ToTable("WF_INSTANCE");//, ConfigurationController.SecurityDbSchema);
            this.Property<int>((Expression<Func<WorkflowInstance, int?>>)(workflowInstance => workflowInstance.ParentId)).HasColumnName("PRSS_ID");
            this.Property<int>((Expression<Func<WorkflowInstance, int>>)(workflowInstance => workflowInstance.WorkflowInfoId)).HasColumnName("WRKF_ID");
            this.Property<int>((Expression<Func<WorkflowInstance, int>>)(workflowInstance => workflowInstance.RelatedRecordId)).HasColumnName("RELATED_RECORD_ID");
            this.Property<WfStateStatus>((Expression<Func<WorkflowInstance, WfStateStatus>>)(workflowInstance => workflowInstance.Status)).HasColumnName("STATUS");
            this.Property((Expression<Func<WorkflowInstance, string>>)(workflowInstance => workflowInstance.UserComment)).HasColumnName("USERCOMMENT");
            this.Property((Expression<Func<WorkflowInstance, DateTime>>)(workflowInstance => workflowInstance.StartTime)).HasColumnName("START_TIME");
            this.Property((Expression<Func<WorkflowInstance, DateTime?>>)(workflowInstance => workflowInstance.FinishTime)).HasColumnName("FINISH_TIME");
            this.Property((Expression<Func<WorkflowInstance, string>>)(workflowInstance => workflowInstance.InitialExchangeData)).HasColumnName("INITIAL_EXCHANGE_DATA");
            this.HasMany<WorkflowInstanceState>((Expression<Func<WorkflowInstance, ICollection<WorkflowInstanceState>>>)(workflowInstance => workflowInstance.WorkflowInstanceStates)).WithRequired().HasForeignKey<int>((Expression<Func<WorkflowInstanceState, int>>)(WFIS => WFIS.WorkflowInstanceId));
            this.HasMany<WorkflowInstanceState>((Expression<Func<WorkflowInstance, ICollection<WorkflowInstanceState>>>)(workflowInstance => workflowInstance.WorkflowInstanceStates)).WithRequired().HasForeignKey<int>((Expression<Func<WorkflowInstanceState, int>>)(WFIS => WFIS.SubWorkflowInstanceId));
        }
    }
}
