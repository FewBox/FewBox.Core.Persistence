using System;
using System.Collections.Generic;
using System.Security.Claims;
using FewBox.Core.Persistence.Cache;
using FewBox.Core.Persistence.Orm;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FewBox.Core.Persistence.UnitTest
{
    [TestClass]
    public class JWTTokenUnitTest
    {
        private ITokenService TokenService { get; set; }
        private CurrentUser CurrentUser { get; set; }

        [TestInitialize]
        public void Init()
        {
            Guid userId = Guid.NewGuid();
            this.TokenService = new JWTToken();
            this.CurrentUser = new CurrentUser { 
                Id = userId.ToString(),
                Key = "EnVsakc0bNXs1UYHAiOjE1ND",
                Issuer = "https://fewbox.com",
                Claims = new List<Claim>{  }
                };
        }

        [TestMethod]
        public void TestToken()
        {
            string token = this.TokenService.GenerateToken(this.CurrentUser, TimeSpan.FromMinutes(5));
            Assert.AreEqual(this.CurrentUser.Id, this.TokenService.GetUserIdByToken(token));
        }
    }
}
