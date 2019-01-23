using System;
using FewBox.Core.Persistence.Cache;
using FewBox.Core.Persistence.Orm;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FewBox.Core.Persistence.UnitTest
{
    [TestClass]
    public class MemoryTokenCacheUnitTest
    {
        private ITokenService TokenService { get; set; }
        private CurrentUser CurrentUser { get; set; }

        [TestInitialize]
        public void Init()
        {
            this.TokenService = new MemoryTokenCache(new MemoryCache(new MemoryCacheOptions{})); // MVC: services.AddMemoryCache();
            this.CurrentUser = new CurrentUser { Id = Guid.NewGuid().ToString() };
        }

        [TestMethod]
        public void TestToken()
        {
            string token = this.TokenService.GenerateToken(this.CurrentUser, TimeSpan.FromMinutes(5));
            Assert.AreEqual(this.CurrentUser.Id, this.TokenService.GetUserIdByToken(token));
        }
    }
}
