using System;

namespace OMF.Common.ActivityLog
{
    [Serializable]
    public class ActivityLog : Entity, ICustomIdentity
    {
        public string EntityID { get; set; }

        public string EntityName { get; set; }

        public int Action { get; set; }

        public DateTime RecordDateTime { get; set; }

        public int ApplicationID { get; set; }

        public int UserID { get; set; }

        public string UserName { get; set; }

        public string ClientIP { get; set; }

        public bool ForceLog { get; set; }

        public bool VisibleForEndUser { get; set; }

        public ActivityLogData LogData { get; set; }

        public string IdentityGeneratorSequenceName
        {
            get
            {
                return "tbl_activitylog_seq";
            }
        }
    }
}
