using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMF.Common.Configuration;
using OMF.EntityFramework.Ef6;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using static OMF.Workflow.Enums;

namespace OMF.Workflow.Model.MappingConfiguration
{
    public class WfMessageConfig : BaseEntityTypeConfig<WfMessage>
    {
        public WfMessageConfig()
        {
            this.ToTable("WE_MESSAGE");//, ConfigurationController.SecurityDbSchema);
            this.Property<int>((Expression<Func<WfMessage, int>>)(msg => msg.ID)).HasDatabaseGeneratedOption(new DatabaseGeneratedOption?(DatabaseGeneratedOption.None));
            this.Property<int>((Expression<Func<WfMessage, int>>)(msg => msg.WorkflowId)).HasColumnName("WORKFLOW_ID");
            this.Property<int>((Expression<Func<WfMessage, int>>)(msg => msg.WorkflowInstanceId)).HasColumnName("WORKFLOWINSTANCE_ID");
            this.Property<int>((Expression<Func<WfMessage, int>>)(msg => msg.WorkflowStepId)).HasColumnName("WORKFLOWSTEP_ID");
            this.Property((Expression<Func<WfMessage, string>>)(msg => msg.Title)).HasColumnName("TITLE");
            this.Property<MessageType>((Expression<Func<WfMessage, MessageType>>)(msg => msg.MessageType)).HasColumnName("MESSAGETYPE");
            this.Property<int>((Expression<Func<WfMessage, int>>)(msg => msg.MasterId)).HasColumnName("MASTER_ID");
            this.Property((Expression<Func<WfMessage, DateTime?>>)(msg => msg.ViewDateTime)).HasColumnName("VIEW_DATETIME");
            this.Property((Expression<Func<WfMessage, DateTime>>)(msg => msg.SendDateTime)).HasColumnName("SEND_DATETIME");
            this.Property((Expression<Func<WfMessage, string>>)(msg => msg.WorkflowTitle)).HasColumnName("WORKFLOW_TITLE");
            this.Property<Priority>((Expression<Func<WfMessage, Priority>>)(msg => msg.Priority)).HasColumnName("PRIORITY");
            this.Property<int>((Expression<Func<WfMessage, int>>)(msg => msg.SenderRoleId)).HasColumnName("SENDER_ROLE_ID");
            this.Property<int>((Expression<Func<WfMessage, int>>)(msg => msg.SenderUserId)).HasColumnName("SENDER_USER_ID");
            this.Property<bool>((Expression<Func<WfMessage, bool>>)(msg => msg.NeedToSign)).HasColumnName("NEEDTOSIGN");
            this.Property<int>((Expression<Func<WfMessage, int>>)(msg => msg.RoleId)).HasColumnName("ROLE_ID");
            this.Property<int>((Expression<Func<WfMessage, int>>)(msg => msg.UserId)).HasColumnName("USER_ID");
            this.Property<WfStateStatus>((Expression<Func<WfMessage, WfStateStatus>>)(msg => msg.WorkflowStatus)).HasColumnName("WORKFLOWSTATUS");
            this.Property<MessageCategory>((Expression<Func<WfMessage, MessageCategory>>)(msg => msg.Category)).HasColumnName("CATEGORY");
        }
    }
}
