using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace TodoWithControllersAuthJWT
{
    [ApiController]
    [Route("/api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(IUserService userService, JwtSettings jwtSettings)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
        }

        [HttpPost("token")]
        public IActionResult GenerateToken(UserInfo userInfo)
        {
            bool isValidUser = _userService.IsValid(userInfo.UserName, userInfo.Password);
            if (!isValidUser)
            {
                return BadRequest("invalid user/pass combination");
            }

            var claims = _userService.GetUserClaims(userInfo.UserName).Select(name => new Claim(name, "true"));

            var key = new SymmetricSecurityKey(_jwtSettings.Key);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
                );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
