using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FewBox.App.Demo.Dtos;
using FewBox.Core.Persistence.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FewBox.Core.Persistence.Orm;
using System.Security.Claims;
using FewBox.App.Demo.Configs;

namespace FewBox.App.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TokenController : ControllerBase
    {
        private ITokenService TokenService { get; set; }
        private JWTConfig JWTConfig { get; set; }

        public TokenController(ITokenService tokenService, JWTConfig jWTConfig)
        {
            this.TokenService = tokenService;
            this.JWTConfig = jWTConfig;
        }

        [AllowAnonymous]
        [HttpPost]
        public SignInResponseDto CreateToken([FromBody]SignInRequestDto signInRequestDto)
        {
            if(signInRequestDto.Username=="landpy" && signInRequestDto.Password=="fewbox")
            {
                var userInfo = new UserInfo { 
                    Id = Guid.NewGuid().ToString(),
                    Key = this.JWTConfig.Key,
                    Issuer = this.JWTConfig.Issuer,
                    Claims = new List<Claim>{ 
                        new Claim( ClaimTypes.Role, "Admin"), 
                        new Claim(ClaimTypes.Role,"Normal") }
                };
                string token = this.TokenService.GenerateToken(userInfo, TimeSpan.FromMinutes(5));
                return new SignInResponseDto { IsValid = true, Token = token };
            }
            else if(signInRequestDto.Username=="fewbox" && signInRequestDto.Password=="landpy")
            {
                var userInfo = new UserInfo { 
                    Id = Guid.NewGuid().ToString(),
                    Key = this.JWTConfig.Key,
                    Issuer = this.JWTConfig.Issuer,
                    Claims = new List<Claim>{ new Claim(ClaimTypes.Role,"Normal") }
                };
                string token = this.TokenService.GenerateToken(userInfo, TimeSpan.FromMinutes(5));
                return new SignInResponseDto { IsValid = true, Token = token };
            }
            else
            {
                return new SignInResponseDto { IsValid = false };
            }
        }

        [AllowAnonymous]
        [HttpGet("claims")]
        public object Claims()
        {
            return User.Claims.Select(c =>
            new
            {
                Type = c.Type,
                Value = c.Value
            });
        }
    }
}