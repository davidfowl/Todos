using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace TodoWithControllersAuthJWT
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/todos")]
    public class TodoController : ControllerBase
    {
        private readonly TodoDbContext _db;

        public TodoController(TodoDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [HttpGet]
        public async Task<ActionResult<List<Todo>>> GetAll()
        {
            return await _db.Todos.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize("user")]
        public async Task<ActionResult<Todo>> Get(long id)
        {
            var todo = await _db.Todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            return todo;
        }

        [HttpPost]
        public async Task Post(Todo todo)
        {
            _db.Todos.Add(todo);
            await _db.SaveChangesAsync();
        }

        [HttpDelete("{id}")]
        [Authorize("admin")]
        public async Task<IActionResult> Delete(long id)
        {
            var todo = await _db.Todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            _db.Todos.Remove(todo);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
