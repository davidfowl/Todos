using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;

namespace TodoWithRoutes
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var app = WebApplication.Create(args);
            if (args?.Length > 0)
                app.Listen($"https://localhosts:{args[0]}");

            var todos = new TodoApi();
            todos.MapRoutes(app);

            await app.RunAsync();
        }
    }
}