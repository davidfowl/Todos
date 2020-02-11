using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TodoWithControllersAuthJWT;

namespace Todos
{
    internal class AuthService : IAuthService
    {
        public AuthService()
        {
            Key = Enumerable.Range(0, 100).Select(Convert.ToByte).ToArray();
        }
        private readonly Dictionary<string, (string Password, string[] Claims)> _users = new Dictionary<string, (string Password, string[] Claims)>
        {
            ["user"] = ("123456", new[] { "can_delete", "can_view" }),
        };

        public string Issuer { get; private set; } = "iamissuer";

        public byte[] Key { get; private set; } = new byte[100];

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