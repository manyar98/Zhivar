using OMF.Common;
using OMF.Enterprise.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using OMF.Common.Extensions;
using Zhivar.DomainClasses;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.Web.Controllers.Enum
{
    //TODO add to Template project for SG
    public class EnumController : ApiController
    {
        private HttpResponseMessage EnumKeyValueReponse<TEnum>() where TEnum : struct, IConvertible => Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = EnumHelper.GetKeyValues<TEnum>() });

        public async Task<HttpResponseMessage> GetAuthenticationTypes()
        {
            try
            {
                var userTypes = EnumHelper.GetKeyValues<DomainClasses.ZhivarEnums.ZhivarUserType>();
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = userTypes });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }
     
        public HttpResponseMessage GetActivityTypes() => EnumKeyValueReponse<ActionType>();

        public HttpResponseMessage GetExceptionTypes() => EnumKeyValueReponse<OMF.Common.Enums.ExceptionType>();

    }
}
