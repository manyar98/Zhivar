using OMF.Common.Cache;
using OMF.Common.Extensions;
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Zhivar.Web.Controllers.Security
{
    public class AuthenticateUrlController : ApiController
    {
        [HttpPost]
        public void SetUrl([FromBody]ParameterUrl url)
        {
            SessionManager.Add("authenticateUrl", url);
        }

        public string GetUrl()
        {
            ParameterUrl parameterUrl = SessionManager.GetData("authenticateUrl") as ParameterUrl ?? new ParameterUrl();
            return parameterUrl.Url;
        }

        public HttpResponseMessage GetVersion()
        {
            var version = VersionConfigController.LoadVersion;
            return new HttpResponseMessage() { Content = new StringContent(version, Encoding.UTF8, "text/html") };
        }
    }

    public class ParameterUrl
    {
        public string Url { get; set; }
    }

    public static class VersionConfigController
    {
        public static string LoadVersion
        {
            get
            {
                try
                {
                    string loadVer = ConfigurationManager.AppSettings["LoadVersion"].ConvertTo<string>();

                    if (string.IsNullOrEmpty(loadVer))
                        loadVer = DateTime.Now.Hour.ToString();

                    return loadVer;
                }
                catch
                {
                    return DateTime.Now.Hour.ToString();
                }
            }
        }
    }
    }
