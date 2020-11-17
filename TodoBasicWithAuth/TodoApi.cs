using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Todos
{
    public class TodoApi
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
             PropertyNameCaseInsensitive = true,
             PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task GetAllAsync(TodoDbContext db, HttpContext context)
        {
            var todos = await db.Todos.ToListAsync();

            await context.Response.WriteAsJsonAsync(todos, _options);
        }

        public async Task GetAsync(TodoDbContext db, HttpContext context)
        {
            if (!context.Request.RouteValues.TryGet("id", out int id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            var todo = await db.Todos.FindAsync(id);
            if (todo == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            await context.Response.WriteAsJsonAsync(todo, _options);
        }

        public async Task PostAsync(TodoDbContext db, HttpContext context)
        {
            var todo = await context.Request.ReadFromJsonAsync<Todo>(_options);

            await db.Todos.AddAsync(todo);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TodoDbContext db, HttpContext context)
        {
            if (!context.Request.RouteValues.TryGet("id", out int id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            var todo = await db.Todos.FindAsync(id);
            if (todo == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
        }

        public void MapRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/todos", WithDbContext(GetAllAsync)).RequireAuthorization();
            endpoints.MapGet("/api/todos/{id}", WithDbContext(GetAsync)).RequireAuthorization("user");
            endpoints.MapPost("/api/todos", WithDbContext(PostAsync)).RequireAuthorization();
            endpoints.MapDelete("/api/todos/{id}", WithDbContext(DeleteAsync)).RequireAuthorization("admin");
        }

        private RequestDelegate WithDbContext(Func<TodoDbContext, HttpContext, Task> handler)
        {
            return context =>
            {
                // Resolve the service from the container
                var db = context.RequestServices.GetRequiredService<TodoDbContext>();
                return handler(db, context);
            };
        }
    }
}
