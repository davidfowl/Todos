using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Todos
{
    public partial class TodoApi
    {
        public class Service
        {
            private readonly TodoDbContext _db;

            public Service(TodoDbContext db)
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
                _db.Todos.Add(todo);
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
}
