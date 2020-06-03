using System;

namespace OMF.Common.Web.HttpModules
{
    public class SessionEndedEventArgs : EventArgs
    {
        public readonly string SessionId;
        public readonly object SessionObject;

        public SessionEndedEventArgs(string sessionId, object sessionObject)
        {
            this.SessionId = sessionId;
            this.SessionObject = sessionObject;
        }
    }
}
