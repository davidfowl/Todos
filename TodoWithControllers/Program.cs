using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Todos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("Todos"));
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

await app.RunAsync();