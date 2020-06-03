using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Zhivar.DomainClasses;

namespace Zhivar.Web.Controllers
{
    [RoutePrefix("api/Test")]
    public class TestController : ApiController
    {

        [HttpPost]
        [Route("Hello")]
        public async Task<HttpResponseMessage> Hello([FromBody] int id)
        {
           
            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = "hi" });
        }
    }
}
