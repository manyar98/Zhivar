using System;
using System.Collections.Generic;
using OMF.Business;
using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.Query;
using OMF.EntityFramework.UnitOfWork;
using OMF.Security.Model;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCValidation = OMF.Common.Validation;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.Business.BaseInfo
{
    public partial class VahedTolRule : BusinessRuleBase<VahedTol>
    {
        public VahedTolRule()
            : base()
        {

        }

        public VahedTolRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public VahedTolRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public IList<VahedTol> GetAllByOrganId(int organId)
        {
            var VahedTols = this.Queryable().Where(x => x.OrganId == organId).ToList();

            return VahedTols;
        }
        public async Task<IList<VahedTol>> GetAllByOrganIdAsync(int organId)
        {
            var VahedTols = await this.Queryable().Where(x => x.OrganId == organId).ToListAsync2();

            return VahedTols;

        }

    }
}