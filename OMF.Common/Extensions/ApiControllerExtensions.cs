using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement;
using OMF.Common.ExceptionManagement.Exceptions;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace OMF.Common.Extensions
{
    public static class ApiControllerExtensions
    {
        public static async Task<HttpResponseMessage> HandleExceptionAsync(
          this ApiController apiController,
          Exception ex)
        {
            if (ex is OperationAccessException)
                return apiController.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 2,
                    message = ex.Message
                });
            if (ex is OMFValidationException)
                return apiController.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 1,
                    failures = ((OMFValidationException)ex).Failures
                });
            if (ex is UserContextNullException)
                return apiController.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 3
                });
            if (!ex.Data.Keys.OfType<string>().Contains<string>("SourceName"))
                ex.Data.Add((object)"SourceName", (object)apiController.GetType().FullName);
            await ExceptionManager.HandleExceptionAsync(ex);
            return apiController.Request.CreateResponse(HttpStatusCode.OK, new
            {
                resultCode = 4,
                message = ExceptionManager.GetExceptionMessageWithoutDebugInfo(ex),
                stackTrace = ConfigurationController.WithDebugInfo ? ExceptionManager.GetExceptionMessageWithDebugInfo(ex) : ""
            });
        }

        public static HttpResponseMessage HandleException(
          this ApiController apiController,
          Exception ex)
        {
            if (ex is OperationAccessException)
                return apiController.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 2,
                    message = ex.Message
                });
            if (ex is OMFValidationException)
                return apiController.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 1,
                    failures = ((OMFValidationException)ex).Failures
                });
            if (ex is UserContextNullException)
                return apiController.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 3
                });
            if (!ex.Data.Keys.OfType<string>().Contains<string>("SourceName"))
                ex.Data.Add((object)"SourceName", (object)apiController.GetType().FullName);
            ExceptionManager.HandleException(ex);
            return apiController.Request.CreateResponse(HttpStatusCode.OK, new
            {
                resultCode = 4,
                message = ExceptionManager.GetExceptionMessageWithoutDebugInfo(ex),
                stackTrace = ConfigurationController.WithDebugInfo ? ExceptionManager.GetExceptionMessageWithDebugInfo(ex) : ""
            });
        }
    }
}
