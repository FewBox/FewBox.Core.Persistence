using FewBox.Core.Persistence.Orm;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text;

namespace FewBox.Core.Persistence.Cache
{
    public class DistributedTokenCache : ITokenService
    {
        private IDistributedCache DistributedCache { get; set; }

        public DistributedTokenCache(IDistributedCache distributedCache)
        {
            this.DistributedCache = distributedCache;
        }

        public string GenerateToken(CurrentUser currentUser, TimeSpan expiredTime)
        {
            string token = Guid.NewGuid().ToString();
            DistributedCacheEntryOptions distributedCacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiredTime
            };
            this.DistributedCache.SetString(token.ToString(), currentUser.Id.ToString(), distributedCacheEntryOptions);
            return token;
        }

        public string GetUserIdByToken(string token)
        {
            string userIdString = this.DistributedCache.GetString(token.ToString());
            return userIdString;
        }
    }
}
