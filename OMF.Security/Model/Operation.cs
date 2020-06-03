using OMF.Common;
using System;
using static OMF.Common.Enums;

namespace OMF.Security.Model
{
    public class Operation : LoggableEntity, ISelfReferenceEntity, ILogicalDeletable, IActivatable, IActivityLoggable, IEntity, IViewEntity, IObjectState, ICloneable//, ICustomIdentity
    {
        public ActionLog ActionsToLog
        {
            get
            {
                return ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
            }
        }

        public int? ParentId { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public int ApplicationId { get; set; }

        public OperationType OperationType { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsSystem { get; set; }

        public int OrderNo { get; set; }

        public string Tag1 { get; set; }

        public string Tag2 { get; set; }

        public string Tag3 { get; set; }

        public string Tag4 { get; set; }

        public string Tag5 { get; set; }

        public int? TagInt1 { get; set; }

        public int? TagInt2 { get; set; }

        public int? TagInt3 { get; set; }

        //public string IdentityGeneratorSequenceName
        //{
        //    get
        //    {
        //        return "tbl_operation_seq";
        //    }
        //}
    }
}
