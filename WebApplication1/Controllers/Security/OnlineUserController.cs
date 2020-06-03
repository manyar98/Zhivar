using OMF.Common;
using OMF.Common.Cache;
using OMF.Common.Security;
using OMF.Enterprise.MVC;
using Behsho.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using OMF.Common.Extensions;
using Zhivar.Common;
using System.Linq.Expressions;
using OMF.Business;
using Zhivar.ViewModel.Security;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.Web.Controllers.Security
{
    public class OnlineUserController : ApiControllerBaseAsync<OnlineUser, OnlineUserVM>
    {
        protected override Expression<Func<OnlineUser, bool>> CreateDefaultSearchExpression()
        {
            SecurityManager.ThrowIfUserContextNull();
            //if (SecurityManager.CurrentUserContext.IsDeveloperUser())
            //    return base.CreateDefaultSearchExpression();
            //if (SecurityManager.CurrentUserContext.IsSystemUser())
            //    return onLineuser => onLineuser.UserType != BehshoEnums.BehshoUserType.Developers;
            return base.CreateDefaultSearchExpression();
        }
        protected override IBusinessRuleBaseAsync<OnlineUser> CreateBusinessRule()
        {
            return new Zhivar.Business.Security.OnlineUserRule();
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Logoff([FromBody]LogoffData request)
        {
            try
            {
                await SecurityManager.LogoffAsync(request.UserToken);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = "عملیات با موفقیت انجام شد." });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }            
        }

        public class LogoffData
        {
            public string UserToken { get; set; }
        }
    }
}