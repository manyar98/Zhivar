using OMF.Common.Configuration;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OMF.Workflow.Model.MapConfiguration
{
    public class WorkflowInfoConfig : BaseEntityTypeConfig<WorkflowInfo>
    {
        public WorkflowInfoConfig()
        {
            this.ToTable("WF_WORKFLOW");//, ConfigurationController.SecurityDbSchema);
            this.Property((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Title)).HasColumnName("TITLE").HasMaxLength(new int?(200));
            this.Property((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Version)).HasColumnName("VERSION").HasMaxLength(new int?(20));
            this.Property((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Code)).HasColumnName("CODE").HasMaxLength(new int?(50));
            this.Property((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Icon)).HasColumnName("ICON").HasMaxLength(new int?(150));
            this.Property((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Tag01)).HasColumnName("TAG01");
            this.Property((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Tag02)).HasColumnName("TAG02");
            this.Property((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Tag03)).HasColumnName("TAG03");
            this.Property((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Tag04)).HasColumnName("TAG04");
            this.Property((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Tag05)).HasColumnName("TAG05");
            this.Property((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Tag06)).HasColumnName("TAG06");
            this.Property((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Tag07)).HasColumnName("TAG07");
            this.Property((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Tag08)).HasColumnName("TAG08");
            this.Property((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Tag09)).HasColumnName("TAG09");
            this.Property((Expression<Func<WorkflowInfo, string>>)(workflowInfo => workflowInfo.Tag10)).HasColumnName("TAG10");
            this.HasMany<WorkflowInstance>((Expression<Func<WorkflowInfo, ICollection<WorkflowInstance>>>)(workflowInfo => workflowInfo.WorkflowInstances)).WithRequired().HasForeignKey<int>((Expression<Func<WorkflowInstance, int>>)(WFI => WFI.WorkflowInfoId));
            this.HasMany<WorkflowStep>((Expression<Func<WorkflowInfo, ICollection<WorkflowStep>>>)(workflowInfo => workflowInfo.WorkflowSteps)).WithRequired().HasForeignKey<int>((Expression<Func<WorkflowStep, int>>)(WFS => WFS.WorkflowInfoId));
            this.HasMany<WorkflowStepAction>((Expression<Func<WorkflowInfo, ICollection<WorkflowStepAction>>>)(workflowInfo => workflowInfo.WorkflowStepActions)).WithRequired().HasForeignKey<int>((Expression<Func<WorkflowStepAction, int>>)(WFS => WFS.SubWorkflowInfoId));
        }
    }
}
