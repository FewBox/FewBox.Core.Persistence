using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.Core.Persistence.Cache
{
    public interface ITokenService
    {
        string GenerateToken(UserInfo userInfo, TimeSpan expiredTime);
        string GetUserIdByToken(string token);
        UserProfile GetUserProfileByToken(string token);
    }
}