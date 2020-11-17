using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Todos
{
    public class AuthApi
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly JwtSettings _jwtSettings;

        public AuthApi(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public async Task CreateUser(UserManager<TodoUser> userManager, HttpContext context)
        {
            var loginInfo = await context.Request.ReadFromJsonAsync<LoginInfo>(_options);

            var result = await userManager.CreateAsync(new TodoUser { UserName = loginInfo.UserName }, loginInfo.Password);

            if (result.Succeeded)
            {
                context.Response.StatusCode = StatusCodes.Status202Accepted;
                return;
            }

            context.Response.StatusCode = 400;
        }

        public async Task GenerateTokenAsync(UserManager<TodoUser> userManager, HttpContext context)
        {
            var loginInfo = await context.Request.ReadFromJsonAsync<LoginInfo>(_options);

            var user = await userManager.FindByNameAsync(loginInfo?.UserName);
            if (user == null || !await userManager.CheckPasswordAsync(user, loginInfo.Password))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
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

            await context.Response.WriteAsJsonAsync(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        public void MapRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/api/auth", WithUserManager(CreateUser));
            endpoints.MapPost("/api/auth/token", WithUserManager(GenerateTokenAsync));
        }

        private RequestDelegate WithUserManager(Func<UserManager<TodoUser>, HttpContext, Task> handler)
        {
            return context =>
            {
                // Resolve the service from the container
                var userManager = context.RequestServices.GetRequiredService<UserManager<TodoUser>>();
                return handler(userManager, context);
            };
        }
    }
}
