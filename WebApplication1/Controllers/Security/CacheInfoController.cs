using Zhivar.DomainClasses;
using Zhivar.Common.Security;
using OMF.Common;
using OMF.Common.Cache;
using OMF.Common.Extensions;
using OMF.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using OMF.Common.ExceptionManagement.Exceptions;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.Web.Controllers.Security
{
    public class CacheInfoController : ApiController
    {
        public async Task<HttpResponseMessage> GetCacheInfoOperationAccess()
        {
            var oprAccess = new OperationAccess();

            oprAccess.CanView = await SecurityManager.HasAccessAsync(ZhivarResourceIds.Zhivar_Security_CacheInfo_View);
            oprAccess.CanDelete = await SecurityManager.HasAccessAsync(ZhivarResourceIds.Zhivar_Security_CacheInfo_Delete);

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = oprAccess });
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetCacheInfoList()
        {
            try
            {
                SecurityManager.ThrowIfUserContextNull();
                if (!(await SecurityManager.HasAccessAsync(ZhivarResourceIds.Zhivar_Security_CacheInfo_View)))
                    throw new OperationAccessException(ZhivarResourceIds.Zhivar_Security_CacheInfo_View);

                List<CacheInfo> cacheInfoList = new List<CacheInfo>();
                if (CacheManager.CacheProvider != null)
                {
                    //if (SecurityManager.CurrentUserContext.IsDeveloperUser())
                    //    cacheInfoList = CacheManager.CacheProvider.Keys.Select(key => new CacheInfo() { Key = key }).ToList();
                    //else
                    //    cacheInfoList = CacheManager.CacheProvider.Keys.Where(key => key.Contains("__CRM_"))
                    //                                                   .Select(key => new CacheInfo() { Key = key })
                    //                                                   .ToList();
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = new { records = cacheInfoList, count = cacheInfoList.Count } });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteItemInListCache(string key)
        {
            try
            {
                if (!(await SecurityManager.HasAccessAsync(ZhivarResourceIds.Zhivar_Security_CacheInfo_Delete)))
                    throw new OperationAccessException(ZhivarResourceIds.Zhivar_Security_CacheInfo_Delete);

                CacheManager.Remove(key);
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = "" });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }
    }
}
