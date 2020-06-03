using OMF.Common.Cache;
using OMF.Common.Configuration;
using OMF.Common.Security;
using System;

namespace OMF.Security.TokenManagement
{
    public class TokenInfo
    {
        public string Token { get; set; }

        public string ClientIP { get; set; }

        public DateTime LastAccess { get; set; }

        public UserContext UserContext { get; set; }

        public string SessionId { get; set; }

        public bool IsExpired
        {
            get
            {
                return (DateTime.Now - this.LastAccess).Minutes > ConfigurationController.TokenExpireMins;
            }
        }

        public static TokenInfo CreateInstance(UserContext context)
        {
            return new TokenInfo()
            {
                UserContext = context,
                ClientIP = context.ClientIP,
                Token = Guid.NewGuid().ToString(),
                LastAccess = DateTime.Now,
                SessionId = SessionManager.SessionId
            };
        }
    }
}
