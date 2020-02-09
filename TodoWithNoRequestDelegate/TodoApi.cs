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
        private readonly JsonSerializerOptions _options;
        private readonly Service _todoApiService;

        public TodoApi(Service todoApiService, JsonSerializerOptions options)
        {
            _todoApiService = todoApiService ?? throw new ArgumentNullException(nameof(todoApiService));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        private async Task GetAll(HttpContext context)
        {
            var todos = await _todoApiService.GetAll();
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

            var todo = await _todoApiService.Get(todoId);
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

            await _todoApiService.Post(todo);
        }

        private async Task Delete(HttpContext context)
        {
            var id = (string)context.Request.RouteValues["id"];
            if (id == null || !long.TryParse(id, out var todoId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            if (!await _todoApiService.Delete(todoId))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
        }

        public static void MapRoutes(IEndpointRouteBuilder endpoints, ApiActivator<TodoApi> activator)
        {
            endpoints.MapGet("/api/todos", activator(api => api.GetAll));
            endpoints.MapGet("/api/todos/{id}", activator(api => api.Get));
            endpoints.MapPost("/api/todos", activator(api => api.Post));
            endpoints.MapDelete("/api/todos/{id}", activator(api => api.Delete));
        }
    }
}
