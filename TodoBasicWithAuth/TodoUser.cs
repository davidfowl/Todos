using Microsoft.AspNetCore.Identity;

namespace TodoBasicWithAuth
{
    public class TodoUser : IdentityUser
    {
        public bool IsAdmin { get; set; }
    }
}
