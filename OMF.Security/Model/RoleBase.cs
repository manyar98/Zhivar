using OMF.Common;
using System;
using System.Collections.Generic;
using static OMF.Common.Enums;

namespace OMF.Security.Model
{
    public abstract class RoleBase : LoggableEntity, ISelfReferenceEntity, ILogicalDeletable, IAggregateRoot, IActivatable, IActivityLoggable, IEntity, IViewEntity, IObjectState, ICloneable, ICustomIdentity
    {
        public ActionLog ActionsToLog
        {
            get
            {
                return ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
            }
        }

        public int? ApplicationId { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public int? ParentId { get; set; }

        public List<RoleOperation> RoleOperations { get; set; }

        public bool? IsMultiAssignable { get; set; } = new bool?(true);

        public string IdentityGeneratorSequenceName
        {
            get
            {
                return "tbl_role_info_seq";
            }
        }
    }
}
