using OMF.Common.Cache;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace OMF.Common.Web
{
    public class HttpSessionProvider : ISessionProvider, ICacheProvider, IEnumerable
    {
        public HttpSessionState HttpSession
        {
            get
            {
                if (HttpContext.Current == null)
                    return (HttpSessionState)null;
                return HttpContext.Current.Session;
            }
        }

        public object this[string key]
        {
            get
            {
                if (this.HttpSession == null)
                    return (object)null;
                return this.HttpSession[key];
            }
            set
            {
                if (this.HttpSession == null)
                    return;
                this.HttpSession[key] = value;
            }
        }

        public int Count
        {
            get
            {
                if (this.HttpSession == null)
                    return 0;
                return this.HttpSession.Count;
            }
        }

        public void Add(string key, object value)
        {
            if (this.HttpSession == null)
                return;
            this.HttpSession.Add(key, value);
        }

        public bool Contains(string key)
        {
            if (this.HttpSession == null)
                return false;
            return this.Keys.Contains(key);
        }

        public object GetData(string key)
        {
            if (this.HttpSession == null)
                return (object)null;
            return this.HttpSession[key];
        }

        public void Remove(string key)
        {
            if (this.HttpSession == null)
                return;
            this.HttpSession.Remove(key);
        }

        public void Clear()
        {
            if (this.HttpSession == null)
                return;
            this.HttpSession.RemoveAll();
        }

        public void Clear(string keyLike)
        {
            foreach (string key in this.Keys)
            {
                if (key.ToString().Contains(keyLike))
                    this.Remove(key);
            }
        }

        public List<string> Keys
        {
            get
            {
                if (this.HttpSession == null)
                    return new List<string>();
                return this.HttpSession.Keys.Cast<string>().ToList<string>();
            }
        }

        public string SessionId
        {
            get
            {
                if (this.HttpSession == null)
                    return "";
                return this.HttpSession.SessionID;
            }
        }

        public IEnumerator GetEnumerator()
        {
            if (this.HttpSession == null)
                return new ArrayList().GetEnumerator();
            return this.HttpSession.GetEnumerator();
        }

        public void ChangeSessionId()
        {
            SessionIDManager sessionIdManager = new SessionIDManager();
            string sessionId = sessionIdManager.CreateSessionID(HttpContext.Current);
            bool redirected = false;
            bool cookieAdded = false;
            sessionIdManager.SaveSessionID(HttpContext.Current, sessionId, out redirected, out cookieAdded);
        }
    }
}
