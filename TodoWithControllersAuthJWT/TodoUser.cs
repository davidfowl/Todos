using Microsoft.AspNetCore.Identity;

namespace TodoWithControllersAuthJWT
{
    public class TodoUser : IdentityUser
    {
        public bool IsAdmin { get; set; }
    }
}
