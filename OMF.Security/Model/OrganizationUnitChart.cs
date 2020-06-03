using OMF.Common;
using System;
using System.Collections.Generic;
using static OMF.Common.Enums;

namespace OMF.Security.Model
{
    public class OrganizationUnitChart : LoggableEntityName, ISelfReferenceEntity, ILogicalDeletable, IActivityLoggable, IEntity, IViewEntity, IObjectState, ICloneable, ICustomIdentity
    {
        public ActionLog ActionsToLog
        {
            get
            {
                return ActionLog.Insert | ActionLog.Update;
            }
        }

        public string Title { get; set; }

        public string Code { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public int? ParentId { get; set; }

        public UnitChartType UnitChartType { get; set; }

        public string Tag1 { get; set; }

        public string Tag2 { get; set; }

        public int? IntTag1 { get; set; }

        public int? IntTag2 { get; set; }

        public List<OrganizationUnitChart> ChildUnitCharts { get; set; }

        public string IdentityGeneratorSequenceName
        {
            get
            {
                return "tbl_organization_unit_chart_seq";
            }
        }
    }
}
