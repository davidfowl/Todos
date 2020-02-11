namespace TodoWithControllersAuthJWT
{
    public interface IAuthService
    {
        bool IsValid(string username, string password);
        string[] GetUserClaims(string username);
        string Issuer { get; }
        byte[] Key { get; }
    }
}