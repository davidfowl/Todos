using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Todos
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
        [AllowAnonymous]
        [HttpGet(nameof(GenerateToken))]
        public IActionResult GenerateToken([FromQuery] string username, [FromQuery] string password, [FromQuery] string[] desiredClaims)
        {
            bool isValidUser = 
                Program.validUsers.ContainsKey(username) 
                && Program.validUsers[username].CompareTo(password) == 0;
            if (!isValidUser)
            {
                return BadRequest("invalid user/pass combination");
            }
            var claims = desiredClaims.Select(name => new Claim($"can_{name}", true.ToString().ToLowerInvariant()));

            var key = new SymmetricSecurityKey(Program.KEY);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: Program.ISSUER,
                audience: Program.AUDIENCE,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
                );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
        [HttpGet]
        public async Task<ActionResult<List<Todo>>> GetAll()
        {
            var todos = await _db.Todos.ToListAsync();

            return todos;
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
