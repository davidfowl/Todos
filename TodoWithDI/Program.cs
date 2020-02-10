using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace TodoWithDi
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("Todos"));

            var app = builder.Build();
            if (args?.Length > 0)
                app.Listen($"https://localhosts:{args[0]}");

            var todos = new TodoApi();
            todos.MapRoutes(app);

            await app.RunAsync();
        }
    }
}