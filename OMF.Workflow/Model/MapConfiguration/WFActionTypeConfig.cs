using OMF.Common.Configuration;
using OMF.Common.Validation;
using OMF.EntityFramework.Ef6;
using OMF.Workflow.Model.Validators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OMF.Workflow.Model.MapConfiguration
{
    public class WFActionTypeConfig : BaseEntityTypeConfig<WFActionType>
    {
        public WFActionTypeConfig()
        {
            this.ToTable("WF_ACTION_TYPES");
            //this.ToTable("WF_ACTION_TYPES", ConfigurationController.SecurityDbSchema);
            this.Property((Expression<Func<WFActionType, string>>)(actionType => actionType.Title)).HasColumnName("TITLE").HasMaxLength(new int?(100));
            this.Property<bool>((Expression<Func<WFActionType, bool>>)(actionType => actionType.NeedConfirm)).HasColumnName("NEED_CONFIRM");
            this.Property((Expression<Func<WFActionType, string>>)(actionType => actionType.ConfirmMessage)).HasColumnName("CONFIRM_MESSAGE").HasMaxLength(new int?(200));
            this.HasMany<WorkflowStepAction>((Expression<Func<WFActionType, ICollection<WorkflowStepAction>>>)(actionType => actionType.WorkflowStepActions)).WithRequired().HasForeignKey<int>((Expression<Func<WorkflowStepAction, int>>)(WFS => WFS.WFActionTypeId));
            this.MapEntityValidator((EntityValidator<WFActionType>)new WFActionTypeValidator());
        }
    }
}
