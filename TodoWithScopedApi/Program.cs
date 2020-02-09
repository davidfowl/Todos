using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Todos
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("Todos"));
            builder.Services.AddScoped<TodoApi>();
            builder.Services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var app = builder.Build();

            TodoApi.MapRoutes(app, Activator);

            await app.RunAsync();
        }

        private static RequestDelegate Activator<T>(Func<T, RequestDelegate> handler)
        {
            return context =>
            {
                var api = context.RequestServices.GetRequiredService<T>();
                return handler(api)(context);
            };
        }
    }

    public delegate RequestDelegate ApiActivator<T>(Func<T, RequestDelegate> inp);
}
