using OMF.Common;
using System;
using static OMF.Common.Enums;

namespace OMF.Security.Model
{
    public class ForgotPasswordEntity : Entity, IActivityLoggable, IEntity, IViewEntity, IObjectState, ICloneable, ICustomIdentity
    {
        public string Code { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string UserMobile { get; set; }

        public DateTime InsertDateTime { get; set; }

        public DateTime UpdateDateTime { get; set; }

        public bool IsActive { get; set; }

        public string ClientIP { get; set; }

        public ActionLog ActionsToLog
        {
            get
            {
                return ActionLog.Insert;
            }
        }

        public string IdentityGeneratorSequenceName
        {
            get
            {
                return "tbl_forgotpassword_seq";
            }
        }
    }
}
