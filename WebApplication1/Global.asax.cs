using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using Zhivar.DataLayer.Context;
using System.Configuration;
using System.Data.Entity.Infrastructure.Interception;
using Newtonsoft.Json;
using System.Web.Security;
using System.Web.SessionState;
using OMF.Common.Web;
using OMF.Common.Web.HttpModules;
using OMF.Common.Security;
//using OMF.EntityFramework;
using OMF.Common.Cache;
using OMF.Common.Sms;
using OMF.Security;
//using OMF.Security.ForgotPassword;
//using Behsho.Common;
using OMF.Common.ActivityLog;
//using OMF.EntityFramework.Ef6;
using OMF.Common;
using OMF.EntityFramework;
using OMF.EntityFramework.Ef6;

namespace Zhivar.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Re‌​ferenceLoopHandling = ReferenceLoopHandling.Ignore;
            // BundleConfig.RegisterBundles(BundleTable.Bundles);

            OMFAppContext.DataContextCreator = () => new ZhivarContext();

            SessionManager.InitiateSessionProvider(new HttpSessionProvider());
            SecurityManager.InitiateSecurityProvider(OMFSecurityProvider.Instance);
            CacheManager.InitiateCacheProvider(new HttpMemoryCacheProvider());
            // SmsManager.InitiateSmsProvider(new BehshoSmsProvider());
            // ForgotPasswordManager.InitiateForgotPasswordHandler(new BehshoForgotPassHandler());

            ActivityLogManager.ClientIpCatcher = () =>
            {
                if (HttpContext.Current == null) return "Undefined";
                return HttpContext.Current.Request.UserHostAddress;
            };

            // در ابتداي کار يک کليد براي شي سشن در نظر گرفته مي شود
            SessionEndModule.SessionObjectKey = Constants.UserContextSessionKey;
            // براي اتمام سشن هم رويدادي معين مي شود 
            // که با اتمام آن مجموعه عملياتي رخ دهد
            SessionEndModule.SessionEnd += new SessionEndEventHandler(SessionTimoutModule_SessionEnd);

            new ZhivarContext().Database.Initialize(false);
            AutoMapperConfig.Config();

            //Database.SetInitializer<DataContext>(null);

            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext,
            //    DataLayer.Migrations.Configuration>());

        }

        protected void Application_PostAuthorizeRequest()
        {
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }
        private const string ROOT_DOCUMENT = "~/app/index.html";

        private static void SessionTimoutModule_SessionEnd(object sender, SessionEndedEventArgs e)
        {
            // This will be the value in the session for the key specified in Application_Start
            UserContext sessionObject = e.SessionObject as UserContext;

            if (sessionObject == null)
                return;

            //SecurityManager.Logoff(sessionObject.Token);
        }
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            try
            {
                string url = Request.Url.LocalPath;
                if (url.Contains("api"))
                    return;
                if (url.ToLower() == "/rd")
                    return;
                if (url.ToLower().Contains("routedebugger"))
                    return;
                if (url.ToLower().Contains("reportgenerator"))
                    return;
                if (url.ToLower().Contains("pishkhanoffice"))
                    return;
                if (url.ToLower().Contains("resultpayment"))
                    return;
                if (url.ToLower().Contains("openid"))
                    return;
                if (url.ToLower().Contains("default/index"))
                    return;

                if (!File.Exists(Context.Server.MapPath(url)))
                    Context.RewritePath(ROOT_DOCUMENT);
            }
            catch (Exception)
            {
                Context.RewritePath(ROOT_DOCUMENT);
            }
        }
    }
}
