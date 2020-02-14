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

        private readonly TodoDbContext _db;

        public TodoApi(TodoDbContext db)
        {
            _db = db;
        }

        public async Task GetAllAsync(HttpContext context)
        {
            var todos = await _db.Todos.ToListAsync();

            await context.Response.WriteJsonAsync(todos, _options);
        }

        public async Task GetAsync(HttpContext context)
        {
            if (!context.Request.RouteValues.TryGet("id", out int id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            var todo = await _db.Todos.FindAsync(id);
            if (todo == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            await context.Response.WriteJsonAsync(todo, _options);
        }

        public async Task PostAsync(HttpContext context)
        {
            var todo = await context.Request.ReadJsonAsync<Todo>(_options);

            await _db.Todos.AddAsync(todo);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(HttpContext context)
        {
            if (!context.Request.RouteValues.TryGet("id", out int id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            var todo = await _db.Todos.FindAsync(id);
            if (todo == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            _db.Todos.Remove(todo);
            await _db.SaveChangesAsync();
        }

        public static void MapRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/todos", WithServices(api => api.GetAllAsync));
            endpoints.MapGet("/api/todos/{id}", WithServices(api => api.GetAsync));
            endpoints.MapPost("/api/todos", WithServices(api => api.PostAsync));
            endpoints.MapDelete("/api/todos/{id}", WithServices(api => api.DeleteAsync));
        }

        private static RequestDelegate WithServices(Func<TodoApi, RequestDelegate> handler)
        {
            return context =>
            {
                var db = context.RequestServices.GetRequiredService<TodoDbContext>();
                var api = new TodoApi(db);
                return handler(api)(context);
            };
        }
    }
}
