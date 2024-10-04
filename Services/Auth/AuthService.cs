using myYSTU.Models;
using myYSTU.Services.Http;
using System.Text;
using Serilog;

namespace myYSTU.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IHttpService _httpService;
    private readonly HttpClientHandler _handler;
    private readonly HttpClient _client;

    public AuthService(IHttpService httpService, HttpClientHandler handler, HttpClient client)
    {
        _httpService = httpService;
        _handler = handler;
        _client = client;
    }

    public bool IsAuthorized { get { return _isAuthorized; } }

    private bool _isAuthorized = false;

    public async Task<bool> TryAuthorize()
    {
        _isAuthorized = await UseSavedSession() || await AuthorizeWithSavedCredentials();
        return _isAuthorized;
    }

    public async Task<bool> CheckIfAuthorized()
    {
        _isAuthorized = true;

        try
        {
            await _httpService.RequestWebData(Links.AccountInfoLink);
        }
        catch
        {
            _isAuthorized = false;
        }

        Log.Debug("[AuthService] [CheckIfAuthorized] IsAuthorized: {@IsAuthorized}", IsAuthorized);
        return _isAuthorized;
    }

    private async Task<bool> UseSavedSession()
    {
        var session_name = await SecureStorage.Default.GetAsync("session_name");
        var session_value = await SecureStorage.Default.GetAsync("session_value");

        if (session_name is null || session_value is null)
        {
            return false;
        }

        Log.Debug("[AuthService] [UseSavedSession] trying to authorize using saved session...");

        _handler.CookieContainer.SetCookies(_client.BaseAddress, $"{session_name}={session_value}");

        return await CheckIfAuthorized();
    }

    private async Task<bool> AuthorizeWithSavedCredentials()
    {
        var login = await SecureStorage.Default.GetAsync("login");
        var password = await SecureStorage.Default.GetAsync("password");

        if (login is null || password is null)
        {
            return false;
        }

        Log.Debug("[AuthService] [UseSavedSession] trying to authorize using saved credentials...");

        return await AuthorizeWithPassword(login, password);
    }

    public async Task<bool> AuthorizeWithPassword(string login, string password)
    {
        // URL для первого запроса
        string loginUrl = Links.AuthorizeLink;

        // Создаем строку с данными для первого запроса
        string loginFormData = $"login={login}&password={password}";

        // Создаем контент запроса с типом "application/x-www-form-urlencoded" для первого запроса
        var loginContent = new StringContent(loginFormData, Encoding.UTF8, "application/x-www-form-urlencoded");

        // Отправляем первый POST-запрос и получаем ответ
        var loginResponse = await _client.PostAsync(loginUrl, loginContent);

        Log.Debug("[AuthService] [AuthorizeWithPassword] [{@StatusCode}] Received login response:  {@Content}", loginResponse.StatusCode, await loginResponse.Content.ReadAsStringAsync());

        if (_handler.CookieContainer.Count == 0 || (await loginResponse.Content.ReadAsStringAsync()).Contains("auth.php"))
        {
            return false;
        }

        await SecureStorage.Default.SetAsync("login", login);
        await SecureStorage.Default.SetAsync("password", password);
        await SecureStorage.Default.SetAsync("session_name", _handler.CookieContainer.GetCookies(_client.BaseAddress)[0].Name);
        await SecureStorage.Default.SetAsync("session_value", _handler.CookieContainer.GetCookies(_client.BaseAddress)[0].Value);

        _isAuthorized = true; //TODO Refactor

        return _isAuthorized;
    }
}
