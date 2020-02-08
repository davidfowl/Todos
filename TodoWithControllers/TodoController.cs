using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Todos
{
    [ApiController]
    [Route("/api/todos")]
    public class TodoController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Todo>>> GetAll([FromServices]TodoDbContext db)
        {
            var todos = await db.Todos.ToListAsync();

            return todos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> Get(long id, [FromServices]TodoDbContext db)
        {
            var todo = await db.Todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            return todo;
        }

        [HttpPost]
        public async Task Post(Todo todo, [FromServices]TodoDbContext db)
        {
            db.Todos.Add(todo);
            await db.SaveChangesAsync();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id, [FromServices]TodoDbContext db)
        {
            var todo = await db.Todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return Ok();
        }
    }
}
