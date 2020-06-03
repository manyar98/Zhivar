using OMF.Common;
using System;
using static OMF.Common.Enums;

namespace OMF.Security.Model
{
    public class UserRole : LoggableEntity, IActivityLoggable, IEntity, IViewEntity, IObjectState, ICloneable//, ICustomIdentity
    {
        public ActionLog ActionsToLog
        {
            get
            {
                return ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
            }
        }

        public int UserId { get; set; }

        public int RoleId { get; set; }

        //public string IdentityGeneratorSequenceName
        //{
        //    get
        //    {
        //        return "tbl_user_role_seq";
        //    }
        //}
    }
}
