using System;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TodoWithControllersAuthJWT
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
            var issuser = configuration["jwt:issuer"] ?? "defaultissuer";
            var auidence = configuration["jwt:auidence"] ?? "defaultauidence";
            var base64Key = configuration["jwt:key"];

            byte[] key;
            if (!string.IsNullOrEmpty(base64Key))
            {
                key = Convert.FromBase64String(base64Key);
            }
            else
            {
                // In real life this would come from configuration
                key = new byte[32];
                RandomNumberGenerator.Fill(key);
            }

            return new JwtSettings(key, issuser, auidence);
        }
    }
}
