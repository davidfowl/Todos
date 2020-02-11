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
        private readonly IAuthService _userService;

        public AuthController(IAuthService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));;
        }
        [HttpGet(nameof(GenerateToken))]
        public IActionResult GenerateToken([FromQuery] string username, [FromQuery] string password)
        {
            bool isValidUser = _userService.IsValid(username, password);
            if (!isValidUser)
            {
                return BadRequest("invalid user/pass combination");
            }
            var claims = _userService.GetUserClaims(username).Select(name => new Claim(name, "true"));

            var key = new SymmetricSecurityKey(_userService.Key);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _userService.Issuer,
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
