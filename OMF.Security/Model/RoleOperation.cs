using OMF.Common;
using System;
using static OMF.Common.Enums;

namespace OMF.Security.Model
{
    public class RoleOperation : LoggableEntity, IActivityLoggable, IEntity, IViewEntity, IObjectState, ICloneable, ICustomIdentity
    {
        public ActionLog ActionsToLog
        {
            get
            {
                return ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
            }
        }

        public int IsPermision { get; set; }

        public int RoleId { get; set; }

        public int OperationId { get; set; }

        public string IdentityGeneratorSequenceName
        {
            get
            {
                return "tbl_role_operation_seq";
            }
        }
    }
}
