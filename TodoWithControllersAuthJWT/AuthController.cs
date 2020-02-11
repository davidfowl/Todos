using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace TodoWithControllersAuthJWT
{
    [ApiController]
    [Route("/api/auth")]
    [AllowAnonymous]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));;
        }

        [HttpGet("token")]
        public IActionResult GenerateToken([FromQuery] string username, [FromQuery] string password)
        {
            bool isValidUser = _authService.IsValid(username, password);
            if (!isValidUser)
            {
                return BadRequest("invalid user/pass combination");
            }

            var claims = _authService.GetUserClaims(username).Select(name => new Claim(name, "true"));

            var key = new SymmetricSecurityKey(JwtSettings.Instance.Key);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: JwtSettings.Instance.Issuer,
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
