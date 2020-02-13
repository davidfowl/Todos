using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Todos
{
    public class TodoDbContext : IdentityDbContext<TodoUser>
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {

        }

        public DbSet<Todo> Todos { get; set; }
    }
}
