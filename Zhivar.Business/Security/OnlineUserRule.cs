using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Security;
using OMF.Business;
using OMF.Common.Security;
using OMF.Security.TokenManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Behsho.Common.Security;

namespace Zhivar.Business.Security
{
    public class OnlineUserRule : BusinessRuleBase<OnlineUser>
    {
        protected override OperationKeys CreateOperationKeys()
        {
            OperationKeys oprKeys = new OperationKeys();
            oprKeys.ViewKey = ZhivarResourceIds.Zhivar_Security_OnlineUser_View;
            oprKeys.UpdateKey = ZhivarResourceIds.Zhivar_Security_OnlineUser_Update;
            return oprKeys;
        }

        protected override IQueryable<OnlineUser> GetQueryableData(bool containsDeletedData = false, bool fromCache = true, List<System.Linq.Expressions.Expression<Func<OnlineUser, object>>> includes = null)
        {
            List<UserContext> userList = TokenManager.ListUsers();
            List<OnlineUser> onlineUserList = userList.ConvertAll(uc => new OnlineUser()
            {
                FirstName = uc.FirstName,
                LastName = uc.LastName,
                ID = uc.UserId,
                LastLoginDate = uc.LastLoginDateTime,
                Token = uc.Token,
                UserName = uc.UserName,
                UserType = (ZhivarEnums.ZhivarUserType)uc.AuthenticationType
            });


            return onlineUserList.AsQueryable();
        }
    }
}
