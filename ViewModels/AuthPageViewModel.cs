using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using myYSTU.Utils;
using NLog;

namespace myYSTU.ViewModels;

public partial class AuthPageViewModel : ObservableObject
{
    private readonly NetUtils _netUtils;
    private readonly ILogger _logger;

    public AuthPageViewModel(NetUtils netUtils, ILogger logger)
    {
        _netUtils = netUtils;
        _logger = logger;
    }

    #region ObservableProperties
    [ObservableProperty]
    private bool _isLoginButtonEnabled = true;

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
        IsLoginButtonEnabled = false; //Выключаем, чтобы пользователь не нажал на кнопку дважды

        bool authResult;

        try
        {
            authResult = await _netUtils.AuthorizeWithPassword(Login, Password);
        }
        catch (Exception ex)
        {
            IsErrorLabelEnabled = true;
            ErrorLabel = "Произошла ошибка / отсутствует подключение к интернету";

            _logger.Error(ex, "Auth error");
            return;
        }
        finally
        {
            IsLoginButtonEnabled = true;
        }

        if (authResult)
        {
            await Shell.Current.GoToAsync("MainPage");
        }
        else
        {
            WelcomeLabelText = "Неправильный логин или пароль";
        }
    }
}
