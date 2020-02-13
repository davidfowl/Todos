using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace TodoWithControllersAuthJWT
{
    [ApiController]
    [Route("/api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<TodoUser> _userManager;
        private readonly JwtSettings _jwtSettings;

        public AuthController(UserManager<TodoUser> userManager, JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(LoginInfo loginInfo)
        {
            var result = await _userManager.CreateAsync(new TodoUser { UserName = loginInfo.UserName }, loginInfo.Password);

            if (result.Succeeded)
            {
                return Accepted();
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("token")]
        public async Task<IActionResult> GenerateToken(LoginInfo loginInfo)
        {
            var user = await _userManager.FindByNameAsync(loginInfo.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginInfo.Password))
            {
                return Unauthorized();
            }

            var claims = new List<Claim>();

            if (user.IsAdmin)
            {
                claims.Add(new Claim("can_delete", "true"));
                claims.Add(new Claim("can_view", "true"));
            }

            var key = new SymmetricSecurityKey(_jwtSettings.Key);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
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
