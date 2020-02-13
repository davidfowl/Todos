using System.ComponentModel.DataAnnotations;

namespace TodoWithControllersAuthJWT
{
    public class LoginInfo
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
