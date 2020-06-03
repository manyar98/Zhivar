using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace OMF.Workflow.Cartable.Model
{
    public class NextStepUserInfo
    {
        public int UserId { get; set; }

        public Gender? Gender { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
