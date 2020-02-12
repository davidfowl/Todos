using System.Security.Cryptography;

namespace Todos
{
    // In real life this would come from configuration
    public class JwtSettings
    {
        public static JwtSettings Instance = new JwtSettings();

        public JwtSettings()
        {
            RandomNumberGenerator.Create().GetBytes(Key);
        }

        public string Issuer { get; private set; } = "iamissuer";

        public byte[] Key { get; private set; } = new byte[100];
    }
}
