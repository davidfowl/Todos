using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoWithControllersAuthJWT;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = JwtSettings.FromConfiguration(builder.Configuration);

builder.Services.AddIdentityCore<TodoUser>()
                .AddEntityFrameworkStores<TodoDbContext>();

builder.Services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("Todos"));
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy.RequireClaim("can_delete", "true"));
    options.AddPolicy("user", policy => policy.RequireClaim("can_view", "true"));
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = jwtSettings.TokenValidationParameters);

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();