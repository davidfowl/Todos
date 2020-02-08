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

        private async Task GetAll(HttpContext context)
        {
            var todos = await GetAll();
            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, todos, _options);
        }

        private async Task Get(HttpContext context)
        {
            var id = (string)context.Request.RouteValues["id"];
            if (id == null || !long.TryParse(id, out var todoId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var todo = await Get(todoId);
            if (todo == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, todo, _options);
        }

        private async Task Post(HttpContext context)
        {
            var todo = await JsonSerializer.DeserializeAsync<Todo>(context.Request.Body, _options);

            await Post(todo);
        }

        private async Task Delete(HttpContext context)
        {
            var id = (string)context.Request.RouteValues["id"];
            if (id == null || !long.TryParse(id, out var todoId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            if (!await Delete(todoId))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
        }

        public static void MapRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/todos", WithServices(api => api.GetAll));
            endpoints.MapGet("/api/todos/{id}", WithServices(api => api.Get));
            endpoints.MapPost("/api/todos", WithServices(api => api.Post));
            endpoints.MapDelete("/api/todos/{id}", WithServices(api => api.Delete));
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
