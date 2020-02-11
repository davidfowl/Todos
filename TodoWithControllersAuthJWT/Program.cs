using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Todos
{
    class Program
    {
        public static string ISSUER = "iamissuer";
        public static string AUDIENCE = ISSUER;
        public static byte[] KEY = Enumerable.Range(0, 100).Select(Convert.ToByte).ToArray();

        public static Dictionary<string, string>  validUsers = new Dictionary<string, string>
        {
            ["user"] = "123456",
        };
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("Todos"));
            builder.Services.AddAuthorization(x =>
            {
                x.AddPolicy("admin", policy => policy.RequireClaim("can_delete", true.ToString().ToLowerInvariant()));
                x.AddPolicy("user", policy => policy.RequireClaim("can_view", true.ToString().ToLowerInvariant()));
            });
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = ISSUER,
                        ValidAudience = AUDIENCE,
                        IssuerSigningKey = new SymmetricSecurityKey(KEY)
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
