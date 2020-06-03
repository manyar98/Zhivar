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
    public partial class AccountRule : BusinessRuleBase<Account>
    {
        public AccountRule()
            : base()
        {

        }

        public AccountRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public AccountRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public IList<Account> GetAllByOrganId(int organId)
        {
            var Accounts = this.Queryable().Where(x => x.OrganId == organId).ToList();

            return Accounts;
        }
        public async Task<List<Account>> GetAllByOrganIdAsync(int organId)
        {
            var Accounts = await this.Queryable().Where(x => x.OrganId == organId).ToListAsync2();

            return Accounts;

        }
      
        public void DeleteAccountByComplteCoding(string completeCode)
        {
            var account = this.Queryable().Where(x => x.ComplteCoding == completeCode).SingleOrDefault();

            if (account != null)
            {
                Delete(account);
            }
        }
        public async Task DeleteAccountByComplteCodingAsync(string completeCode)
        {
            var account = await this.Queryable().Where(x => x.ComplteCoding == completeCode).SingleOrDefaultAsync2();

            if (account != null)
            {
                Delete(account);
            }
        }
    }
}