using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Todos
{
    public partial class TodoApi
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private async Task GetAllAsync(HttpContext context)
        {
            var todos = await GetAllAsync();

            await context.Response.WriteAsJsonAsync(todos, _options);
        }

        private async Task GetAsync(HttpContext context)
        {
            if (!context.Request.RouteValues.TryGet("id", out int id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            var todo = await GetAsync(id);
            if (todo == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            await context.Response.WriteAsJsonAsync(todo, _options);
        }

        private async Task PostAsync(HttpContext context)
        {
            var todo = await context.Request.ReadFromJsonAsync<Todo>(_options);

            await PostAsync(todo);
        }

        private async Task DeleteAsync(HttpContext context)
        {
            if (!context.Request.RouteValues.TryGet("id", out int id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            if (!await DeleteAsync(id))
            {
                context.Response.StatusCode = 404;
                return;
            }
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
