using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.Core.Persistence.Cache
{
    public interface ITokenService
    {
        string GenerateToken(CurrentUser currentUser, TimeSpan expiredTime);
        string GetUserIdByToken(string token);
    }
}