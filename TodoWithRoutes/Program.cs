using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Todos
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var app = WebApplication.Create(args);

            var todos = new TodoApi();
            todos.MapRoutes(app);

            await app.RunAsync();
        }
    }
}
