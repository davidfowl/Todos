using Microsoft.AspNetCore.Identity;

namespace Todos
{
    public class TodoUser : IdentityUser
    {
        public bool IsAdmin { get; set; }
    }
}
