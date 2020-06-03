using OMF.Common;
using System;
using static OMF.Common.Enums;

namespace OMF.Security.Model
{
    public class Organization : LoggableEntityName, ISelfReferenceEntity, ILogicalDeletable, IActivityLoggable, IEntity, IViewEntity, IObjectState, ICloneable, ICustomIdentity
    {
        public ActionLog ActionsToLog
        {
            get
            {
                return ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
            }
        }

        public string Title { get; set; }

        public string Alias { get; set; }

        public string Address { get; set; }

        public string Website { get; set; }

        public string Email { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }

        public byte[] Logo { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public int? ParentId { get; set; }

        public int OrganizationUnitChartId { get; set; }

        public int CityId { get; set; }

        public string Tag1 { get; set; }

        public string Tag2 { get; set; }

        public string Tag3 { get; set; }

        public string Tag4 { get; set; }

        public string Tag5 { get; set; }

        public string IdentityGeneratorSequenceName
        {
            get
            {
                return "tbl_organization_seq";
            }
        }
    }
}
