using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Todos
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var jwtSettings = JwtSettings.FromConfiguration(builder.Configuration);

            builder.Services.AddIdentityCore<TodoUser>()
                            .AddEntityFrameworkStores<TodoDbContext>();

            builder.Services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("Todos"));

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy => policy.RequireClaim("can_delete", "true"));
                options.AddPolicy("user", policy => policy.RequireClaim("can_view", "true"));
            });

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = jwtSettings.TokenValidationParameters);

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetService<UserManager<TodoUser>>();
                await userManager.CreateAsync(new TodoUser { UserName = "admin", IsAdmin = true }, "Pass123456!" );
            }

            app.UseAuthentication();
            app.UseAuthorization();

            var auth = new AuthApi(jwtSettings);
            auth.MapRoutes(app);

            var todo = new TodoApi();
            todo.MapRoutes(app);

            await app.RunAsync();
        }
    }
}
