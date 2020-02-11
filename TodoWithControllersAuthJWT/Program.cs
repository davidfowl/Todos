using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TodoWithControllersAuthJWT;

namespace Todos
{
    class Program
    {
        public static string Issuer = "iamissuer";
        public static byte[] Key = new byte[100];

        public static Dictionary<string, (string Password, string[] Claims)>  validUsers = new Dictionary<string, (string Password, string[] Claims)>
        {
            ["user"] = ("123456", new[] { "can_delete", "can_view" }),
        };
        static async Task Main(string[] args)
        {
            RandomNumberGenerator.Create().GetBytes(Key);
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("Todos"));
            builder.Services.AddSingleton<IAuthService, AuthService>(service => new AuthService(Issuer, Key, validUsers));
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy => policy.RequireClaim("can_delete", "true"));
                options.AddPolicy("user", policy => policy.RequireClaim("can_view", "true"));
            });
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Key)
                    };
                });
            builder.Services.AddControllers();

            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            await app.RunAsync();
        }
    }
}
