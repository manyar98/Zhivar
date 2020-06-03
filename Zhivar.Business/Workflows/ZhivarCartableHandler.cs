using OMF.Common;
using OMF.Common.Security;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model;
using OMF.Workflow.Cartable;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Security;
using OMF.Workflow.Cartable.Model;

namespace Zhivar.Business.Workflows
{
    public class ZhivarCartableHandler : OMFCartableHandler
    {
        public override bool HasUserAccessTo(HasUserAccessToRequest hasUserAccessToRequest)
        {
            List<int> userIdList = GetAllUserIds();
            return userIdList.Contains(hasUserAccessToRequest.UserId);
        }

        public async override Task<bool> HasUserAccessToAsync(HasUserAccessToRequest hasUserAccessToRequest)
        {
            List<int> userIdList = await GetAllUserIdsAsync();
            return userIdList.Contains(hasUserAccessToRequest.UserId);
        }

        public async Task<RoleDataCollection> GetCurrentUserRoles()
        {
            SecurityManager.ThrowIfUserContextNull();
            return SecurityManager.CurrentUserContext.Roles;
        }

        public static List<int> GetAllUserIds()
        {
            SecurityManager.ThrowIfUserContextNull();
            return new List<int>() { SecurityManager.CurrentUserContext.UserId };
        }

        public async static Task<List<int>> GetAllUserIdsAsync()
        {
            SecurityManager.ThrowIfUserContextNull();
            return new List<int>() { SecurityManager.CurrentUserContext.UserId };
        }

        public async static Task<OperationAccess> GetMoveMessageOperationAccess()
        {
            OperationAccess operationAccess = new OperationAccess
            {
                CanUpdate = await SecurityManager.HasAccessAsync(ResourceKeys.Workflow_StepStateInfo_ChangeReceiverUser)
            };
            return operationAccess;
        }
    }
}
