using OMF.Common;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Zhivar.DataLayer.Validation;
using Zhivar.DomainClasses.Account;
using Zhivar.ServiceLayer.Contracts.Account;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.Web.Controllers.Account
{
    [RoutePrefix("api/Register")]
    public class RegisterController : ApiController
    {
        [Route("InitiateRegister")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> InitiateRegister([FromBody]BussinessRegisteration registerationRequest)
        {
            try
            {
                BussinessRegisteration registerData = registerationRequest.Translate<BussinessRegisteration>();

                #region Request Validation
                RegisterationValidator validator = new RegisterationValidator();
                var validationResult = await validator.ValidateAsync(registerData);

                if (!validationResult.IsValid)
                {
                    List<string> failures = validationResult.Errors.Select(failure => failure.ErrorMessage).ToList();
                    throw new OMFValidationException(failures);
                }
                #endregion

                // await darkhastRule.InitiateRegisterMotaghaziAsync(registerData);

               // await _userManager.InitiateRegisterAsync(registerData);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = (int)ResultCode.Successful,
                    data = new { }
                });

            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
            finally
            {
                //BusinessRule.Dispose();
            }
        }
        [Route("FinalRegister")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> FinalRegister([FromBody]BussinessRegisteration registerationRequest)
        {
            try
            {
                //using (var darkhastRule = new DarkhastRule())
                //{
                //    BussinessRegisteration registerData = registerationRequest.Translate<BussinessRegisteration>();

                //    UserContext userContext = await darkhastRule.FinalRegisterMotaghaziAsync(registerData);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, new
                {
                    resultCode = (int)ResultCode.Successful,
                    data = new
                    {
                        //UserId = userContext.UserId,
                        //FullName = userContext.FullName,
                        //UserName = userContext.UserName,
                        //AuthenticationType = userContext.AuthenticationType,
                        //MobileNo = userContext.MobileNo,
                        //Tag1 = userContext.Tag1,
                        //Tag2 = userContext.Tag2,
                        //Tag3 = userContext.Tag3,
                        //Tag4 = userContext.Tag4,
                        //Tag5 = userContext.Tag5,
                        //Tag6 = userContext.Tag6,
                        //Tag7 = userContext.Tag7,
                        //Tag8 = userContext.Tag8,
                        //Tag9 = userContext.Tag9,
                        //Tag10 = userContext.Tag10
                    }
                });
                // }
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
            finally
            {
                //BusinessRule.Dispose();
            }
        }
    }
}
