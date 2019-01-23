using FewBox.Core.Persistence.Cache;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Security.Claims;

namespace FewBox.Core.Persistence.Orm
{
    public class DapperSession : IDapperSession
    {
        public IUnitOfWork UnitOfWork { get; set; }
        public Guid Id { get; set; }
        private IDbConnection Connection { get; set; }

        public DapperSession(IHttpContextAccessor httpContextAccessor, IOrmConfiguration ormConfiguration, ITokenService tokenService)
        {
            var user = new CurrentUser();
            if (httpContextAccessor != null)
            {
                var currentUser = httpContextAccessor.HttpContext.User;
                if (currentUser.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                {
                    user.Id = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                }
            }
            this.Connection = new MySqlConnection(ormConfiguration.GetConnectionString());
            this.UnitOfWork = new UnitOfWork(this.Connection, user);
            this.Id = Guid.NewGuid();
        }

        public DapperSession(IOrmConfiguration ormConfiguration, ITokenService tokenService) : 
            this(null, ormConfiguration, tokenService)
        {
        }

        public void Dispose()
        {
            this.UnitOfWork.Dispose();
        }

        public void Close()
        {
            this.Dispose();
        }
    }
}
