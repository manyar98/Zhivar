using OMF.Common.Cache;
using OMF.Common.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OMF.Security.TokenManagement
{
    internal class MemoryTokenCacheProvider : ITokenCacheProvider, IEnumerable<TokenInfo>, IEnumerable
    {
        private static ICacheProvider CacheProvider = (ICacheProvider)new MemoryCacheProvider(nameof(MemoryTokenCacheProvider));

        public string AddUser(UserContext userContext)
        {
            TokenInfo instance = TokenInfo.CreateInstance(userContext);
            this.AddToken(instance);
            return instance.Token;
        }

        public void AddToken(TokenInfo tokenInfo)
        {
            MemoryTokenCacheProvider.CacheProvider.Add(tokenInfo.Token, (object)tokenInfo);
        }

        public UserContext GetUserByToken(string token)
        {
            return this.GetTokenInfoByToken(token)?.UserContext;
        }

        public IEnumerable<TokenInfo> GetTokenInfoListByUserName(string userName)
        {
            userName = userName.ToLower();
            return this.Where<TokenInfo>((Func<TokenInfo, bool>)(item => item.UserContext.UserName.ToLower() == userName));
        }

        public TokenInfo GetTokenInfoByToken(string token)
        {
            return this.SingleOrDefault<TokenInfo>((Func<TokenInfo, bool>)(tInfo => tInfo.Token == token));
        }

        public void DeleteToken(string token)
        {
            MemoryTokenCacheProvider.CacheProvider.Remove(token);
        }

        public List<UserContext> List()
        {
            return this.Select<TokenInfo, UserContext>((Func<TokenInfo, UserContext>)(tInfo => tInfo.UserContext)).ToList<UserContext>();
        }

        public void ClearCache()
        {
            MemoryTokenCacheProvider.CacheProvider.Clear();
        }

        public bool Contains(string userName)
        {
            return this.Any<TokenInfo>((Func<TokenInfo, bool>)(tInfo => tInfo.UserContext.UserName.ToLower() != userName.ToLower()));
        }

        private IEnumerator<TokenInfo> GetEnumerator()
        {
            return MemoryTokenCacheProvider.CacheProvider.OfType<TokenInfo>().GetEnumerator();
        }

        IEnumerator<TokenInfo> IEnumerable<TokenInfo>.GetEnumerator()
        {
            return MemoryTokenCacheProvider.CacheProvider.OfType<TokenInfo>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return MemoryTokenCacheProvider.CacheProvider.GetEnumerator();
        }
    }
}
