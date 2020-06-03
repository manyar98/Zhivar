using System;

namespace OMF.Workflow.Cartable.Model
{
    public class MessageInfoHistory
    {
        public int InstanceStateId { get; set; }

        public string Title { get; set; }

        public DateTime InstantiationTime { get; set; }

        public DateTime? AccomplishTime { get; set; }

        public string Action { get; set; }

        public int RoleId { get; set; }

        public string RoleCode { get; set; }

        public string RoleName { get; set; }

        public int UserId { get; set; }

        public string UserFullName { get; set; }

        public string UserComment { get; set; }

        public string StepTitle { get; set; }
    }
}
