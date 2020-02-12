using System;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Todos
{
    public class JwtSettings
    {
        public JwtSettings(byte[] key, string issuer, string audience)
        {
            Key = key;
            Issuer = issuer;
            Audience = audience;
        }

        public string Issuer { get; }

        public string Audience { get; }

        public byte[] Key { get; }

        public TokenValidationParameters TokenValidationParameters => new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Issuer,
            ValidAudience = Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Key)
        };

        public static JwtSettings FromConfiguration(IConfiguration configuration)
        {
            // In real life this would come from configuration
            var key = new byte[100];

            var issuser = configuration["jwt:issuer"] ?? "defaultissuer";
            var auidence = configuration["jwt:auidence"] ?? "defaultauidence";
            var base64Key = configuration["jwt:key"];

            if (!string.IsNullOrEmpty(base64Key))
            {
                key = Convert.FromBase64String(base64Key);
            }
            else
            {
                RandomNumberGenerator.Create().GetBytes(key);
            }

            return new JwtSettings(key, issuser, auidence);
        }
    }
}
