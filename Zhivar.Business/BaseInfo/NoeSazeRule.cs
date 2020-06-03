﻿using System;
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
    public partial class NoeSazeRule : BusinessRuleBase<NoeSaze>
    {
        public NoeSazeRule()
            : base()
        {

        }

        public NoeSazeRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public NoeSazeRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public IList<NoeSaze> GetAllByOrganId(int organId)
        {
            var NoeSazes = this.Queryable().Where(x => x.OrganId == organId).ToList();

            return NoeSazes;
        }
        public async Task<IList<NoeSaze>> GetAllByOrganIdAsync(int organId)
        {
            var NoeSazes = await this.Queryable().Where(x => x.OrganId == organId).ToListAsync2();

            return NoeSazes;

        }

    }
}