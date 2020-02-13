using System.Collections.Generic;

namespace Todos
{
    internal class UserService
    {
        // This service should talk to whatever backend is storing your users
        private readonly Dictionary<string, (string Password, string[] Claims)> _users = new Dictionary<string, (string Password, string[] Claims)>
        {
            ["admin"] = ("123456", new[] { "can_delete", "can_view" }),
            ["user"] = ("hunter2", new[] { "can_view" })
        };

        public string[] GetUserClaims(string username)
        {
            return _users[username].Claims;
        }

        public bool IsValid(string username, string password)
        {
            // TODO: A real implementation
            return _users.TryGetValue(username, out var data) && string.Equals(data.Password, password);
        }
    }
}