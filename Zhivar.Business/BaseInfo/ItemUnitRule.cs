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
    public partial class ItemUnitRule : BusinessRuleBase<ItemUnit>
    {
        public ItemUnitRule()
            : base()
        {

        }

        public ItemUnitRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public ItemUnitRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public List<ItemUnit> GetAllByOrganId(int organId)
        {
            var itemUnits = this.Queryable().Where(x => x.OrganID == organId).ToList();

            return itemUnits;
        }
        public async Task<List<ItemUnit>> GetAllByOrganIdAsync(int organId)
        {
            var itemUnits = await this.Queryable().Where(x => x.OrganID == organId).ToListAsync2();

            return itemUnits;

        }

    }
}