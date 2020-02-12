using System;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TodoWithControllersAuthJWT
{
    public class JwtSettings
    {
        public JwtSettings(byte[] key, string issuer)
        {
            Key = key;
            Issuer = issuer;
        }

        public string Issuer { get; }

        public byte[] Key { get; }

        public TokenValidationParameters TokenValidationParameters => new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Key)
        };

        public static JwtSettings FromConfiguration(IConfiguration configuration)
        {
            // In real life this would come from configuration
            var key = new byte[100];

            var issuser = configuration["jwt:issuer"] ?? "defaultissuer";
            var base64Key = configuration["jwt:key"];

            if (!string.IsNullOrEmpty(base64Key))
            {
                key = Convert.FromBase64String(base64Key);
            }
            else
            {
                RandomNumberGenerator.Create().GetBytes(key);
            }

            return new JwtSettings(key, issuser);
        }
    }
}
