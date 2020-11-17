using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Todos;

var app = WebApplication.Create(args);

var todos = new TodoApi();
todos.MapRoutes(app);

await app.RunAsync();