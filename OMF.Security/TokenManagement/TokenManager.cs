using OMF.Common.Security;
using System.Collections.Generic;

namespace OMF.Security.TokenManagement
{
    public static class TokenManager
    {
        public static ITokenCacheProvider TokenCacheProvider = (ITokenCacheProvider)new MemoryTokenCacheProvider();

        public static string AddUser(UserContext userContext)
        {
            return TokenManager.TokenCacheProvider.AddUser(userContext);
        }

        public static void AddToken(TokenInfo tokenInfo)
        {
            TokenManager.TokenCacheProvider.AddToken(tokenInfo);
        }

        public static void TokenizeUser(UserContext userContext)
        {
            userContext.Token = TokenManager.AddUser(userContext);
        }

        public static void TokenizeUser(UserContext userContext, string token)
        {
            TokenInfo instance = TokenInfo.CreateInstance(userContext);
            instance.Token = token;
            TokenManager.AddToken(instance);
            userContext.Token = token;
        }

        public static IEnumerable<TokenInfo> GetTokenInfoListByUserName(
          string userName)
        {
            return TokenManager.TokenCacheProvider.GetTokenInfoListByUserName(userName);
        }

        public static TokenInfo GetTokenInfoByToken(string token)
        {
            return TokenManager.TokenCacheProvider.GetTokenInfoByToken(token);
        }

        public static UserContext GetUser(string token)
        {
            return TokenManager.TokenCacheProvider.GetUserByToken(token);
        }

        public static void DeleteToken(string token)
        {
            TokenManager.TokenCacheProvider.DeleteToken(token);
        }

        public static List<UserContext> ListUsers()
        {
            return TokenManager.TokenCacheProvider.List();
        }

        public static bool Exists(string userName)
        {
            return TokenManager.TokenCacheProvider.Contains(userName);
        }
    }
}
