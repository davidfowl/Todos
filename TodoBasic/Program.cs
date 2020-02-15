using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Todos
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var app = WebApplication.Create(args);

            app.MapGet("/api/todos", GetTodos);
            app.MapGet("/api/todos/{id}", GetTodo);
            app.MapPost("/api/todos", CreateTodo);
            app.MapPost("/api/todos/{id}", UpdateCompleted);
            app.MapDelete("/api/todos/{id}", DeleteTodo);

            await app.RunAsync();
        }

        static async Task GetTodos(HttpContext context)
        {
            using var db = new TodoDbContext();
            var todos = await db.Todos.ToListAsync();

            await context.Response.WriteJsonAsync(todos);
        }

        static async Task GetTodo(HttpContext context)
        {
            if (!context.Request.RouteValues.TryGet("id", out int id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            using var db = new TodoDbContext();
            var todo = await db.Todos.FindAsync(id);
            if (todo == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            await context.Response.WriteJsonAsync(todo);
        }

        static async Task CreateTodo(HttpContext context)
        {
            var todo = await context.Request.ReadJsonAsync<Todo>();

            using var db = new TodoDbContext();
            await db.Todos.AddAsync(todo);
            await db.SaveChangesAsync();

            context.Response.StatusCode = 204;
        }

        static async Task UpdateCompleted(HttpContext context)
        {
            if (!context.Request.RouteValues.TryGet("id", out int id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            using var db = new TodoDbContext();
            var todo = await db.Todos.FindAsync(id);

            if (todo == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            var inputTodo = await context.Request.ReadJsonAsync<Todo>();
            todo.IsComplete = inputTodo.IsComplete;

            await db.SaveChangesAsync();

            context.Response.StatusCode = 204;
        }

        static async Task DeleteTodo(HttpContext context)
        {
            if (!context.Request.RouteValues.TryGet("id", out int id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            using var db = new TodoDbContext();
            var todo = await db.Todos.FindAsync(id);
            if (todo == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            db.Todos.Remove(todo);
            await db.SaveChangesAsync();

            context.Response.StatusCode = 204;
        }
    }
}
