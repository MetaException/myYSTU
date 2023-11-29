using myYSTU.Utils;

namespace myYSTU.Views;

public partial class AuthPage : ContentPage
{
    private readonly INetUtils _netUtil = DependencyService.Get<INetUtils>();

    public AuthPage()
    {
        InitializeComponent();
        handleAuthorization();
    }

    private async Task handleAuthorization(string Login = "", string Password = "")
    {
        string savedLogin = await SecureStorage.Default.GetAsync("login");
        string savedPassword = await SecureStorage.Default.GetAsync("password");

        if (savedLogin is not null && savedPassword is not null)
        {
            Login = savedLogin;
            Password = savedPassword;
        }

        var authResult = await _netUtil.Authorize(Login, Password);

        if (authResult == 1)
        {
            await SecureStorage.Default.SetAsync("login", Login);
            await SecureStorage.Default.SetAsync("password", Password);

            App.Current.MainPage = new NavigationPage(new MainPage());
        }
        else if (authResult == 0)
        {
            SecureStorage.Default.Remove("login");
            SecureStorage.Default.Remove("password");

            errorLabel.Text = "Неправильный логин или пароль";
            errorLabel.TextColor = Colors.Red;
        }
        else
        {
            //TODO: Сделать оповещение на экране об ошибке (всплывающий элемент для всех окон)
            errorLabel.Text = "Произошла ошибка / отсутствует подключение к интернету";
            errorLabel.TextColor = Colors.Red;
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string Login = LoginEntry.Text;
        string Password = PasswordEntry.Text;

        LoginBtn.IsEnabled = false; //Выключаем, чтобы пользователь не нажал на кнопку дважды
        await handleAuthorization(Login, Password);
        LoginBtn.IsEnabled = true;
    }
}