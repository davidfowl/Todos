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

            app.MapGet("/api/todos", GetAllAsync);
            app.MapGet("/api/todos/{id}", GetAsync);
            app.MapPost("/api/todos", PostAsync);
            app.MapDelete("/api/todos/{id}", DeleteAsync);

            await app.RunAsync();
        }

        static async Task GetAllAsync(HttpContext context)
        {
            using var db = new TodoDbContext();
            var todos = await db.Todos.ToListAsync();

            await context.Response.WriteJsonAsync(todos);
        }

        static async Task GetAsync(HttpContext context)
        {
            var (id, ok) = context.Request.RouteValues.Get<int>("id");
            if (!ok)
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

        static async Task PostAsync(HttpContext context)
        {
            var todo = await context.Request.ReadJsonAsync<Todo>();

            using var db = new TodoDbContext();
            await db.Todos.AddAsync(todo);
            await db.SaveChangesAsync();

            context.Response.StatusCode = 204;
        }

        static async Task DeleteAsync(HttpContext context)
        {
            var (id, ok) = context.Request.RouteValues.Get<int>("id");
            if (!ok)
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
