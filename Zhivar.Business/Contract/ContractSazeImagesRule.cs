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
using Zhivar.DomainClasses.Common;
using System.Threading;
using Zhivar.ViewModel.Accunting;
using AutoMapper;
using Zhivar.DomainClasses;
using Zhivar.Utilities;
using Zhivar.ViewModel.Contract;
using Zhivar.Business.Accounting;
using Zhivar.DomainClasses.Contract;
using Zhivar.Business.BaseInfo;
using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.Business.Contract
{
    public partial class ContractSazeImagesRule : BusinessRuleBase<ContractSazeImages>
    {
        public ContractSazeImagesRule()
            : base()
        {

        }

        public ContractSazeImagesRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public ContractSazeImagesRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }


   

    }
}