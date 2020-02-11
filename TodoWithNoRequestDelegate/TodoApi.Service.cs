using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Todos
{
    public partial class TodoApi
    {
        private readonly TodoDbContext _db;

        public TodoApi(TodoDbContext db)
        {
            _db = db;
        }

        public async Task<List<Todo>> GetAllAsync()
        {
            return await _db.Todos.ToListAsync();
        }

        public async Task<Todo> GetAsync(long id)
        {
            return await _db.Todos.FindAsync(id);
        }

        public async Task PostAsync(Todo todo)
        {
            await _db.Todos.AddAsync(todo);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var todo = await _db.Todos.FindAsync(id);
            if (todo == null)
            {
                return false;
            }

            _db.Todos.Remove(todo);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
