using myYSTU.Utils;
using NLog;

namespace myYSTU.Views;

public partial class AuthPage : ContentPage
{
    private readonly NetUtils _netUtils = DependencyService.Get<NetUtils>();
    private readonly ILogger logger = DependencyService.Get<Logger>();

    public AuthPage()
    {
        InitializeComponent();
    }

    private async Task HandleAuthorization(string Login = "", string Password = "")
    {
        LoginBtn.IsEnabled = false; //Выключаем, чтобы пользователь не нажал на кнопку дважды
        
        bool authResult;

        try
        {
            authResult = await _netUtils.AuthorizeWithPassword(Login, Password);
        }
        catch (Exception ex)
        {
            errorLabel.Text = "Произошла ошибка / отсутствует подключение к интернету";
            errorLabel.TextColor = Colors.Red;
            LoginBtn.IsEnabled = true;

            logger.Error(ex, "Auth error");
            return;
        }

        if (authResult)
        {
            App.Current.MainPage = new NavigationPage(new MainPage());
        }
        else
        {
            errorLabel.Text = "Неправильный логин или пароль";
            errorLabel.TextColor = Colors.Red;
        }

        LoginBtn.IsEnabled = true;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string Login = LoginEntry.Text;
        string Password = PasswordEntry.Text;

        await HandleAuthorization(Login, Password);
    }
}