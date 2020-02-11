using System.Collections.Generic;
using TodoWithControllersAuthJWT;

namespace Todos
{
    internal class AuthService : IAuthService
    {
        private readonly Dictionary<string, (string Password, string[] Claims)> _users;

        public AuthService(string issuer, byte[] key, Dictionary<string, (string Password, string[] Claims)> users)
        {
            Issuer = issuer;
            Key = key;
            _users = users;
        }

        public string Issuer { get; private set; }

        public byte[] Key { get; private set; }

        public string[] GetUserClaims(string username)
        {
            return _users[username].Claims;
        }

        public bool IsValid(string username, string password)
        {
            return _users.ContainsKey(username) && _users[username].Password.CompareTo(password) == 0;
        }
    }
}