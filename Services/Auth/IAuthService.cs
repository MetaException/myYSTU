namespace myYSTU.Services.Auth;

public interface IAuthService
{
    public bool IsAuthorized { get; }
    public Task<bool> CheckIfAuthorized();
    public Task<bool> TryAuthorize();
    public Task<bool> AuthorizeWithPassword(string login, string password);
}
