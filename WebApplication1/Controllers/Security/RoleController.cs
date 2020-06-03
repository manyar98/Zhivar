using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Web.Http;
using LinqKit;
using System.Linq.Expressions;
using OMF.Common;
using OMF.Enterprise.MVC;
using System.Threading.Tasks;
using OMF.Common.ExceptionManagement;
using OMF.Security.Model;
using OMF.Security;
using OMF.Business;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Configuration;
using OMF.Common.Security;
using System.Data.Entity;
using OMF.EntityFramework.Query;
using OMF.Common.Extensions;
using Zhivar.ViewModel.Security;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.Web.Controllers.Security
{
    public class RoleController : NewApiControllerBaseAsync<Role, RoleVM>
    {
        protected override OMF.Business.IBusinessRuleBaseAsync<Role> CreateBusinessRule()
        {
            UnitOfWork uow = new UnitOfWork(new SecurityDbContext());
            return BusinessContext.GetBusinessRule<Role>(uow);
        }

        public async Task<HttpResponseMessage> GetRoleOperationOperationAccess()
        {
            using (var roleOperationRule = BusinessContext.GetBusinessRule<RoleOperation>())
            {
                OperationAccess oprAccess = await roleOperationRule.CreateOperationAccessAsync();
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = oprAccess });
            }
        }

        public async Task<HttpResponseMessage> UpdateOperations(AccessRoleVM accessRoles)
        {
            try
            {
                using (var roleOperationBusiness = BusinessContext.GetBusinessRule<RoleOperation>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork))
                {
                    var dbRoleOprOerations = await roleOperationBusiness.Queryable().Where(roleOpr => roleOpr.RoleId == accessRoles.RoleId).ToListAsync();
                    var currentRoleOperationIds = accessRoles.RoleOperations.Select(s => s.OperationId);

                    var dbRoleOprOperationIds = dbRoleOprOerations.Select(dbRoleOpr => dbRoleOpr.OperationId);
                    var roleOprOperationIdsForDelete = dbRoleOprOperationIds.Except(currentRoleOperationIds);
                    foreach (var roleOprOperaionID in roleOprOperationIdsForDelete)
                    {
                        RoleOperation roleOperation = dbRoleOprOerations.FirstOrDefault(dbRoleOpr => dbRoleOpr.OperationId == roleOprOperaionID);
                        roleOperationBusiness.Delete(roleOperation);
                    }

                    // currentRoleOperationIds = role.RoleOperations.Where(roleOpr => currentRoleOperationIds.Contains(roleOpr.OperationId)).Select(roleOpr => roleOpr.OperationId).ToList();
                    var roleOperationIdsForInsert = accessRoles.RoleOperations.Select(currentRoleOpr => currentRoleOpr.OperationId).Except(dbRoleOprOperationIds);
                    foreach (var roleOprIdForInsert in roleOperationIdsForInsert)
                    {
                        RoleOperation roleOperationForInsert = new RoleOperation() { OperationId = roleOprIdForInsert, RoleId = accessRoles.RoleId };
                        roleOperationBusiness.Insert(roleOperationForInsert);
                    }

                    await roleOperationBusiness.SaveChangesAsync();
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = accessRoles.RoleId });
                }
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

        protected override Role TranslateEntityVMToEntity(RoleVM entityVM)
        {
            var role = base.TranslateEntityVMToEntity(entityVM);
            role.ApplicationId = ConfigurationController.ApplicationID.Value;
            return role;
        }

        protected override RoleVM TranslateEntityToEntityVM(Role entity)
        {
            RoleVM roleVM = base.TranslateEntityToEntityVM(entity);
            roleVM.RoleOperations = entity.RoleOperations == null ? null : entity.RoleOperations.Select(roleOperation => roleOperation.Translate<RoleOperationVM>()).ToList();

            return roleVM;
        }

        public async Task<HttpResponseMessage> GetRoleInfoAsync()
        {
            try
            {
                var result = await this.BusinessRule.Queryable().Select(role => new KeyValueVM() { Key = role.ID, Value = role.Name }).ToListAsync();
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = result });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }
    }
}