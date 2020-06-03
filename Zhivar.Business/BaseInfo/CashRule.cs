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
    public partial class CashRule : BusinessRuleBase<Cash>
    {
        public CashRule()
            : base()
        {

        }

        public CashRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public CashRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        //public async Task<Cash> GetByAccountId(int accountId)
        //{

        //    var account = await _accounts.Where(x => x.ID == accountId).SingleOrDefaultAsync();

        //    return await _cashs.SingleOrDefaultAsync(x => x.Code == account.Coding);

        //}

        public IList<Cash> GetAllByOrganId(int organId)
        {
            return this.Queryable().Where(x => x.OrganId == organId).ToList();
        }
        public async Task<IList<Cash>> GetAllByOrganIdAsync(int organId)
        {
            return await this.Queryable().Where(x => x.OrganId == organId).ToListAsync2();
        }
    }
}