using OMF.Business;
using OMF.EntityFramework.UnitOfWork;
using OMF.Workflow.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.Business.Workflows
{

    public class WorkflowRule : BusinessRuleBase<WorkflowInfo>
    {
        public WorkflowRule() : base() { }
        public WorkflowRule(IUnitOfWorkAsync uow) : base(uow) { }
    }

}
