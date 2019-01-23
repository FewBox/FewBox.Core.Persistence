using FewBox.Core.Persistence.Orm;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace FewBox.Core.Persistence.Cache
{
    public class MemoryTokenCache : ITokenService
    {
        private IMemoryCache MemoryCache { get; set; }

        public MemoryTokenCache(IMemoryCache memoryCache)
        {
            this.MemoryCache = memoryCache;
        }

        public string GenerateToken(CurrentUser currentUser, TimeSpan expiredTime)
        {
            string token = Guid.NewGuid().ToString();
            this.MemoryCache.Set<string>(token, currentUser.Id, expiredTime);
            return token;
        }

        public string GetUserIdByToken(string token)
        {
            return this.MemoryCache.Get<string>(token);
        }
    }
}
