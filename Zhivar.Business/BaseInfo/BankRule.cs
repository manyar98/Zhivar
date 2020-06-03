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
    public partial class BankRule : BusinessRuleBase<Bank>
    {
        public BankRule()
            : base()
        {

        }

        public BankRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public BankRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public IList<Bank> GetAllByOrganId(int organId)
        {
            var banks = this.Queryable().Where(x => x.OrganId == organId).ToList();

            return banks;
        }
        public async Task<IList<Bank>> GetAllByOrganIdAsync(int organId)
        {
            var banks = await this.Queryable().Where(x => x.OrganId == organId).ToListAsync2();

            return banks;

        }

    }
}