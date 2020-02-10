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

            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, todos, _options);
        }

        public async Task GetAsync(TodoDbContext db, HttpContext context)
        {
            var id = (string)context.Request.RouteValues["id"];
            if (id == null || !long.TryParse(id, out var todoId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var todo = await db.Todos.FindAsync(todoId);
            if (todo == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, todo, _options);
        }

        public async Task PostAsync(TodoDbContext db, HttpContext context)
        {
            var todo = await JsonSerializer.DeserializeAsync<Todo>(context.Request.Body, _options);

            await db.Todos.AddAsync(todo);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TodoDbContext db, HttpContext context)
        {
            var id = (string)context.Request.RouteValues["id"];
            if (id == null || !long.TryParse(id, out var todoId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

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
            endpoints.MapGet("/api/todos", WithDbContext(GetAllAsync));
            endpoints.MapGet("/api/todos/{id}", WithDbContext(GetAsync));
            endpoints.MapPost("/api/todos", WithDbContext(PostAsync));
            endpoints.MapDelete("/api/todos/{id}", WithDbContext(DeleteAsync));
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
