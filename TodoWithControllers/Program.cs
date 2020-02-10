using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace TodoWithControllers
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("Todos"));
            builder.Services.AddControllers();

            var app = builder.Build();
            if (args?.Length > 0)
                app.Listen($"https://localhosts:{args[0]}");

            app.MapControllers();

            await app.RunAsync();
        }
    }
}