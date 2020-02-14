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

            await context.Response.WriteJsonAsync(todos, _options);
        }

        private async Task GetAsync(HttpContext context)
        {
            var id = (string)context.Request.RouteValues["id"];
            if (id == null || !long.TryParse(id, out var todoId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var todo = await GetAsync(todoId);
            if (todo == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            await context.Response.WriteJsonAsync(todo, _options);
        }

        private async Task PostAsync(HttpContext context)
        {
            var todo = await context.Request.ReadJsonAsync<Todo>(_options);

            await PostAsync(todo);
        }

        private async Task DeleteAsync(HttpContext context)
        {
            var id = (string)context.Request.RouteValues["id"];
            if (id == null || !long.TryParse(id, out var todoId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            if (!await DeleteAsync(todoId))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
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
