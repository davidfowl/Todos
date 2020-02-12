using System.Collections.Generic;

namespace Todos
{
    internal class UserService
    {
        private readonly Dictionary<string, (string Password, string[] Claims)> _users = new Dictionary<string, (string Password, string[] Claims)>
        {
            ["user"] = ("123456", new[] { "can_delete", "can_view" }),
        };

        public string[] GetUserClaims(string username)
        {
            return _users[username].Claims;
        }

        public bool IsValid(string username, string password)
        {
            return _users.TryGetValue(username, out var data) && string.Equals(data.Password, password);
        }
    }
}