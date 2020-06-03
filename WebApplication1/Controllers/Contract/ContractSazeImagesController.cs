using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Zhivar.DataLayer.Context;
using Zhivar.DataLayer.Validation;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.Common;
using Zhivar.ServiceLayer.Contracts.Accounting;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.Utilities;
using Zhivar.ViewModel.Accunting;
using Zhivar.ViewModel.BaseInfo;
using Zhivar.ViewModel.Common;
using FluentValidation;
using Zhivar.ViewModel.Accounting;
using Zhivar.DomainClasses.BaseInfo;
using OMF.Common.Security;
using OMF.Enterprise.MVC;
using OMF.Business;
using Zhivar.Business.Accounting;
using Zhivar.Business.Common;
using Zhivar.Business.BaseInfo;
using OMF.EntityFramework.Ef6;
using static Zhivar.DomainClasses.ZhivarEnums;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Workflow;
using OMF.Workflow.Model;
using static OMF.Workflow.Enums;
using static OMF.Common.Enums;
using OMF.Common.Extensions;
using Zhivar.ViewModel.Contract;
using Zhivar.Business.Contract;
using Zhivar.Web.Controllers.Accounting;
using Zhivar.DomainClasses.Contract;
using OMF.Security.Model;
using Zhivar.ViewModel.Security;
using Zhivar.Business.Security;
using System.Linq.Expressions;
using OMF.EntityFramework.Query;

namespace Zhivar.Web.Controllers.Contract
{
    [RoutePrefix("api/ContractSazeImages")]
    public class ContractSazeImagesController : NewApiControllerBaseAsync<ContractSazeImages, ContractSazeImagesVM>
    {
        public ContractSazeImagesRule Rule => this.BusinessRule as ContractSazeImagesRule;

        protected override IBusinessRuleBaseAsync<ContractSazeImages> CreateBusinessRule()
        {
            return new ContractSazeImagesRule();
        }

        protected override Expression<Func<ContractSazeImagesVM, bool>> CreateDefaultSearchExpression()
        {
            return base.CreateDefaultSearchExpression();
        }

        protected override IOrderedQueryable<ContractSazeImagesVM> CreateOrderedQuery(IQueryable<ContractSazeImagesVM> query, List<SortInfo> sortInfoList)
        {
            return base.CreateOrderedQuery(query, sortInfoList);
        }

        protected override IQueryable<ContractSazeImages> CreateQuery(FilterInfo filter)
        {
            return base.CreateQuery(filter);
        }

        protected override Expression<Func<ContractSazeImagesVM, bool>> CreateSearchExpressionByFilterInfo(FilterInfo filterInfo)
        {
            return base.CreateSearchExpressionByFilterInfo(filterInfo);
        }

        protected override IQueryable<ContractSazeImagesVM> CreateSearchQuery(IQueryable<ContractSazeImages> query)
        {
            return base.CreateSearchQuery(query);
        }

    }
}