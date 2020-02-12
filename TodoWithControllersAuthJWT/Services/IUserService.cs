namespace TodoWithControllersAuthJWT
{
    public interface IUserService
    {
        bool IsValid(string username, string password);
        string[] GetUserClaims(string username);
    }
}