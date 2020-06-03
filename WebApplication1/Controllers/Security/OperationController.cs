using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OMF.Common;
using OMF.Enterprise.MVC;
using OMF.Security;
using OMF.Security.Model;
using OMF.Common.Configuration;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.Query;
using Zhivar.Common;
using Zhivar.Business.Security;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Web.Http;
using static OMF.Common.Enums;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.Web.Controllers.Security
{
    public class OperationController : NewApiControllerBaseAsync<Operation, OperationVM>
    {
        protected override OMF.Business.IBusinessRuleBaseAsync<Operation> CreateBusinessRule()
        {
            UnitOfWork uow = new UnitOfWork(new SecurityDbContext());
            return new OperationRule(uow);
        }

        protected override Expression<Func<OperationVM, bool>> CreateDefaultSearchExpression()
        {
          //  if (!SecurityManager.CurrentUserContext.IsDeveloperUser())
               // return opr => !opr.IsSystem &&
                          //    opr.ApplicationId == ConfigurationController.ApplicationID &&
                           //   opr.OperationType == OperationType.Entity;
            return opr => opr.ApplicationId == ConfigurationController.ApplicationID && opr.OperationType == OperationType.Entity;
        }

        protected override IOrderedQueryable<OperationVM> CreateOrderedQuery(IQueryable<OperationVM> query, List<SortInfo> sortInfoList)
        {
            if (sortInfoList != null && sortInfoList.Any())
                return base.CreateOrderedQuery(query, sortInfoList);
            return query.OrderBy(op => op.ID);
        }

        protected override Operation TranslateEntityVMToEntity(OperationVM entityVM)
        {
            var opr = base.TranslateEntityVMToEntity(entityVM);
            opr.ApplicationId = ConfigurationController.ApplicationID.Value;
            opr.OperationType = OperationType.Entity;
            return opr;
        }

        protected override OperationVM TranslateEntityToEntityVM(Operation entity)
        {
            return new OperationVM()
            {
                ID = entity.ID,
                Code = entity.Code,
                Name = entity.Name,
                OperationType = entity.OperationType,
                ApplicationId = entity.ApplicationId,
                IsSystem = entity.IsSystem,
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

        public async Task<HttpResponseMessage> GetOperationAndMenusAsync([FromUri]QueryInfo searchRequestInfo)
        {
            try
            {
                using (var operationRule = new OperationRule())
                {
                    var operationList = await operationRule.GetOperationAndMenusAsync(searchRequestInfo);
                    var operationVmList = operationList.ConvertAll(en => TranslateEntityToEntityVM(en));
                    operationVmList = await base.PostExecuteQueryAsync(operationVmList, searchRequestInfo);
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = operationVmList });
                }
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }           
        }
    }
}