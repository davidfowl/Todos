using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Todos
{
    public class TodoApi
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
             PropertyNameCaseInsensitive = true,
             PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task GetAllAsync(HttpContext context)
        {
            using var db = new TodoDbContext();
            var todos = await db.Todos.ToListAsync();

            await context.Response.WriteJsonAsync(todos, _options);
        }

        public async Task GetAsync(HttpContext context)
        {
            var id = (string)context.Request.RouteValues["id"];
            if (id == null || !long.TryParse(id, out var todoId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            using var db = new TodoDbContext();
            var todo = await db.Todos.FindAsync(todoId);
            if (todo == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            await context.Response.WriteJsonAsync(todo, _options);
        }

        public async Task PostAsync(HttpContext context)
        {
            var todo = await context.Request.ReadJsonAsync<Todo>(_options);

            using var db = new TodoDbContext();
            await db.Todos.AddAsync(todo);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(HttpContext context)
        {
            var id = (string)context.Request.RouteValues["id"];
            if (id == null || !long.TryParse(id, out var todoId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            using var db = new TodoDbContext();
            var todo = await db.Todos.FindAsync(todoId);
            if (todo == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
        }

        public void MapRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/todos", GetAllAsync);
            endpoints.MapGet("/api/todos/{id}", GetAsync);
            endpoints.MapPost("/api/todos", PostAsync);
            endpoints.MapDelete("/api/todos/{id}", DeleteAsync);
        }
    }
}
