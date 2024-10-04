using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using myYSTU.Services.Auth;
using Serilog;

namespace myYSTU.ViewModels;

public partial class AuthPageViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    public AuthPageViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    #region ObservableProperties

    [ObservableProperty]
    private string _errorLabel;

    [ObservableProperty]
    private bool _isErrorLabelEnabled = false;

    [ObservableProperty]
    private string _login = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _welcomeLabelText = "Введите логин и пароль";
    #endregion

    [RelayCommand]
    private async Task LoginAsync()
    {
        bool authResult;

        try
        {
            authResult = await _authService.AuthorizeWithPassword(Login, Password);
        }
        catch (Exception ex)
        {
            IsErrorLabelEnabled = true;
            ErrorLabel = "Произошла ошибка / отсутствует подключение к интернету";

            Log.Error(ex, "[AuthPageViewModel] Auth error");
            return;
        }

        if (authResult)
        {
            Log.Debug("[AuthPageViewModel] Successfully logging in");
            await Shell.Current.GoToAsync("/MainPage");
        }
        else
        {
            Log.Debug("[AuthPageViewModel] Entered wrong password");
            WelcomeLabelText = "Неправильный логин или пароль";
        }
    }
}
