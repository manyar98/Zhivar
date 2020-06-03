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
    public partial class NoeEjareRule : BusinessRuleBase<NoeEjare>
    {
        public NoeEjareRule()
            : base()
        {

        }

        public NoeEjareRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public NoeEjareRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public List<NoeEjare> GetAllByOrganId(int organId)
        {
            var NoeEjares = this.Queryable().Where(x => x.OrganId == organId).ToList();

            return NoeEjares;
        }
        public async Task<List<NoeEjare>> GetAllByOrganIdAsync(int organId)
        {
            var NoeEjares = await this.Queryable().Where(x => x.OrganId == organId).ToListAsync2();

            return NoeEjares;

        }

    }
}