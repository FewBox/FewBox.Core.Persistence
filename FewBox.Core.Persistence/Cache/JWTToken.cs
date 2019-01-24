using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FewBox.Core.Persistence.Orm;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;

namespace FewBox.Core.Persistence.Cache
{
    public class JWTToken : ITokenService
    {
        public string GenerateToken(UserInfo userInfo, TimeSpan expiredTime)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(userInfo.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                userInfo.Issuer,
                userInfo.Issuer,
                userInfo.Claims.Union( new List<Claim>{ new Claim("Id", userInfo.Id.ToString(), ClaimTypes.NameIdentifier)} ),
                expires: DateTime.Now.AddTicks(expiredTime.Ticks),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetUserIdByToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            return jsonToken.Claims.FirstOrDefault(c => c.Type == "Id").Value;
        }
    }
}