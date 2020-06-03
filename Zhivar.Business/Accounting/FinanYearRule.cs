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
using Zhivar.DomainClasses.Accounting;

namespace Zhivar.Business.Accounting
{
    public partial class FinanYearRule : BusinessRuleBase<FinanYear>
    {
        public FinanYearRule()
            : base()
        {

        }

        public FinanYearRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public FinanYearRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public IList<FinanYear> GetAllByOrganId(int organId)
        {
            var finanYears = this.Queryable().Where(x => x.OrganId == organId).ToList();

            return finanYears;
        }
        public async Task<IList<FinanYear>> GetAllByOrganIdAsync(int organId)
        {
            var finanYears = await this.Queryable().Where(x => x.OrganId == organId).ToListAsync2();

            return finanYears;

        }

        public async Task<FinanYear> GetCurrentFinanYear(int organId)
        {
            return await this.Queryable().Where(x => x.OrganId == organId && x.Closed == false).SingleOrDefaultAsync2();
        }

    }
}