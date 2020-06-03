using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;

namespace OMF.Common.Web.HttpModules
{
    public class SessionEndModule : IHttpModule
    {
        private HttpApplication m_HttpApplication;
        private static string m_SessionObjectKey;

        public static string SessionObjectKey
        {
            get
            {
                return SessionEndModule.m_SessionObjectKey;
            }
            set
            {
                SessionEndModule.m_SessionObjectKey = value;
            }
        }

        public void Init(HttpApplication context)
        {
            this.m_HttpApplication = context;
            this.m_HttpApplication.PostRequestHandlerExecute += new EventHandler(this.OnPostRequestHandlerExecute);
        }

        public void Dispose()
        {
        }

        public static event SessionEndEventHandler SessionEnd;

        private void OnPostRequestHandlerExecute(object sender, EventArgs e)
        {
            if (Path.GetExtension(this.m_HttpApplication.Context.Request.Path).ToLower() == ".axd" || this.m_HttpApplication.Context.Request.Path.ToLower().Contains("login/getauthenticateduser") || HttpContext.Current == null)
                return;
            HttpSessionState session = HttpContext.Current.Session;
            if (session == null)
                return;
            TimeSpan slidingExpiration = new TimeSpan(0, 0, session.Timeout, 0, 0);
            object obj = session[SessionEndModule.SessionObjectKey];
            if (obj == null)
                return;
            HttpContext.Current.Cache.Insert(session.SessionID, obj, (CacheDependency)null, DateTime.MaxValue, slidingExpiration, CacheItemPriority.NotRemovable, new CacheItemRemovedCallback(this.CacheItemRemovedCallbackMethod));
        }

        private void CacheItemRemovedCallbackMethod(
          string key,
          object value,
          CacheItemRemovedReason reason)
        {
            if (reason != CacheItemRemovedReason.Expired || SessionEndModule.SessionEnd == null)
                return;
            SessionEndedEventArgs e = new SessionEndedEventArgs(key, value);
            SessionEndModule.SessionEnd((object)this, e);
        }
    }
    public delegate void SessionEndEventHandler(object sender, SessionEndedEventArgs e);
}
