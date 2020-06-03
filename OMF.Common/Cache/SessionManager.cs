namespace OMF.Common.Cache
{
    public static class SessionManager
    {
        private static ISessionProvider session;

        public static ISessionProvider SessionProvider
        {
            get
            {
                return SessionManager.session;
            }
        }

        public static void InitiateSessionProvider(ISessionProvider sessionProvider)
        {
            SessionManager.session = sessionProvider;
        }

        public static int Count
        {
            get
            {
                if (SessionManager.SessionProvider == null)
                    return 0;
                return SessionManager.SessionProvider.Count;
            }
        }

        public static void Add(string key, object value)
        {
            if (SessionManager.SessionProvider == null)
                return;

            SessionManager.SessionProvider.Add(key, value);
        }

        public static bool Contains(string key)
        {
            if (SessionManager.SessionProvider == null)
                return false;
            return SessionManager.SessionProvider.Contains(key);
        }

        public static object GetData(string key)
        {
            if (SessionManager.SessionProvider == null)
                return (object)null;
            return SessionManager.SessionProvider.GetData(key);
        }

        public static void Remove(string key)
        {
            if (SessionManager.SessionProvider == null || !SessionManager.SessionProvider.Contains(key))
                return;
            SessionManager.SessionProvider.Remove(key);
        }

        public static void Clear()
        {
            if (SessionManager.SessionProvider == null)
                return;
            SessionManager.SessionProvider.Clear();
            SessionManager.ChangeSessionId();
        }

        public static void Clear(string keyLike)
        {
            if (SessionManager.SessionProvider == null)
                return;
            SessionManager.SessionProvider.Clear(keyLike);
        }

        public static void ChangeSessionId()
        {
            if (SessionManager.SessionProvider == null)
                return;
            SessionManager.SessionProvider.ChangeSessionId();
        }

        public static string SessionId
        {
            get
            {
                return SessionManager.session.SessionId;
            }
        }
    }
}
