using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OMF.Common;
using OMF.Enterprise.MVC;
using OMF.Security;
using OMF.Business;
using OMF.Security.Model;
using OMF.Common.Configuration;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.Query;
using static OMF.Common.Enums;


namespace Zhivar.Web.Controllers.Security
{
    public class MenuController : NewApiControllerBaseAsync<Operation, OperationVM>
    {
        protected override IBusinessRuleBaseAsync<Operation> CreateBusinessRule()
        {
            UnitOfWork uow = new UnitOfWork(new SecurityDbContext());
            return BusinessContext.GetBusinessRule<Operation>(uow);
        }
        protected override Expression<Func<OperationVM, bool>> CreateDefaultSearchExpression()
        {
            if (SecurityManager.CurrentUserContext.AuthenticationType != (int)Zhivar.DomainClasses.ZhivarEnums.ZhivarUserType.Developers)
                return opr => !opr.IsSystem &&
                              opr.OperationType != OperationType.Entity &&
                              opr.OperationType != OperationType.Other &&
                              opr.ApplicationId == ConfigurationController.ApplicationID;

            return opr => opr.OperationType != OperationType.Entity &&
                          opr.OperationType != OperationType.Other &&
                          opr.ApplicationId == ConfigurationController.ApplicationID;
        }
        protected override IOrderedQueryable<OperationVM> CreateOrderedQuery(IQueryable<OperationVM> query, List<SortInfo> sortInfoList)
        {
            if (sortInfoList != null && sortInfoList.Any())
                return base.CreateOrderedQuery(query, sortInfoList);
            return query.OrderBy(op => op.OrderNo);
        }
        protected override Operation TranslateEntityVMToEntity(OperationVM entityVM)
        {
            var opr = base.TranslateEntityVMToEntity(entityVM);
            opr.ApplicationId = ConfigurationController.ApplicationID.Value;
            return opr;
        }
        protected override OperationVM TranslateEntityToEntityVM(Operation entity)
        {
            return new OperationVM()
            {
                ID = entity.ID,
                Code = entity.Code,
                Name = entity.Name,
                IsSystem = entity.IsSystem,
                ApplicationId = entity.ApplicationId,
                OperationType = entity.OperationType,
                ParentId = entity.ParentId,
                IsActive = entity.IsActive,
                OrderNo = entity.OrderNo,
                IsDeleted = entity.IsDeleted,
                Tag1 = entity.Tag1,
                Tag2 = entity.Tag2,
                Tag3 = entity.Tag3,
                Tag4 = entity.Tag4,
            };
        }
    }
}