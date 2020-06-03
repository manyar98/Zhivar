using System.Collections.Generic;
using static OMF.Workflow.Enums;

namespace OMF.Workflow.Cartable.Model
{
    public class NextStepInfo
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public StepType Type { get; set; }

        public int? RoleId { get; set; }

        public string RoleName { get; set; }

        public List<NextStepUserInfo> UserInfoes { get; set; }
    }
}
