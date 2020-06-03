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
using Zhivar.DomainClasses.Common;

namespace Zhivar.Business.Common
{
    public partial class BussinessSiteRule : BusinessRuleBase<Bussiness>
    {
        public BussinessSiteRule()
            : base()
        {

        }

        public BussinessSiteRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public BussinessSiteRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public IList<Bussiness> GetAllByOrganId(int organId)
        {
            return this.Queryable().Where(x => x.OrganId == organId).ToList();
        }
        public async Task<IList<Bussiness>> GetAllByOrganIdAsync(int organId)
        {
            return await this.Queryable().Where(x => x.OrganId == organId).ToListAsync2();
        }
    }
}