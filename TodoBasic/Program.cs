using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

var app = WebApplication.Create(args);

app.MapGet("/api/todos", GetTodos);
app.MapGet("/api/todos/{id}", GetTodo);
app.MapPost("/api/todos", CreateTodo);
app.MapPost("/api/todos/{id}", UpdateCompleted);
app.MapDelete("/api/todos/{id}", DeleteTodo);

await app.RunAsync();

async Task GetTodos(HttpContext context)
{
    using var db = new TodoDbContext();
    var todos = await db.Todos.ToListAsync();

    await context.Response.WriteAsJsonAsync(todos);
}

async Task GetTodo(HttpContext context)
{
    if (!context.Request.RouteValues.TryGet("id", out int id))
    {
        context.Response.StatusCode = 400;
        return;
    }

    using var db = new TodoDbContext();
    var todo = await db.Todos.FindAsync(id);
    if (todo is null)
    {
        context.Response.StatusCode = 404;
        return;
    }

    await context.Response.WriteAsJsonAsync(todo);
}

async Task CreateTodo(HttpContext context)
{
    var todo = await context.Request.ReadFromJsonAsync<Todo>();

    using var db = new TodoDbContext();
    await db.Todos.AddAsync(todo);
    await db.SaveChangesAsync();

    context.Response.StatusCode = 204;
}

async Task UpdateCompleted(HttpContext context)
{
    if (!context.Request.RouteValues.TryGet("id", out int id))
    {
        context.Response.StatusCode = 400;
        return;
    }

    using var db = new TodoDbContext();
    var todo = await db.Todos.FindAsync(id);

    if (todo is null)
    {
        context.Response.StatusCode = 404;
        return;
    }

    var inputTodo = await context.Request.ReadFromJsonAsync<Todo>();
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    context.Response.StatusCode = 204;
}

async Task DeleteTodo(HttpContext context)
{
    if (!context.Request.RouteValues.TryGet("id", out int id))
    {
        context.Response.StatusCode = 400;
        return;
    }

    using var db = new TodoDbContext();
    var todo = await db.Todos.FindAsync(id);
    if (todo is null)
    {
        context.Response.StatusCode = 404;
        return;
    }

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();

    context.Response.StatusCode = 204;
}

public class Todo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("isComplete")]
    public bool IsComplete { get; set; }
}

public class TodoDbContext : DbContext
{
    public DbSet<Todo> Todos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("Todos");
    }
}