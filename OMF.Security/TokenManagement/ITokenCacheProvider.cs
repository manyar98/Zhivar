using OMF.Common.Security;
using System.Collections;
using System.Collections.Generic;

namespace OMF.Security.TokenManagement
{
    public interface ITokenCacheProvider : IEnumerable<TokenInfo>, IEnumerable
    {
        string AddUser(UserContext userContext);

        UserContext GetUserByToken(string token);

        void DeleteToken(string token);

        List<UserContext> List();

        void ClearCache();

        bool Contains(string userName);

        IEnumerable<TokenInfo> GetTokenInfoListByUserName(string userName);

        TokenInfo GetTokenInfoByToken(string token);

        void AddToken(TokenInfo tokenInfo);
    }
}
