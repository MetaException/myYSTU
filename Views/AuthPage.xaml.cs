using myYSTU.Utils;

namespace myYSTU.Views;

public partial class AuthPage : ContentPage
{
    public AuthPage()
    {
        InitializeComponent();
        handleAuthorization();
    }

    private async Task handleAuthorization(string Login = "", string Password = "")
    {
        LoginBtn.IsEnabled = false; //Выключаем, чтобы пользователь не нажал на кнопку дважды

        string savedLogin = await SecureStorage.Default.GetAsync("login");
        string savedPassword = await SecureStorage.Default.GetAsync("password");

        if (savedLogin is not null && savedPassword is not null)
        {
            Login = savedLogin;
            Password = savedPassword;
        }

        bool authResult = false;

        try
        {
            authResult = await NetUtils.Authorize(Login, Password);
        }
        catch (HttpRequestException ex)
        {
            //Log.Error("", ex);

            //TODO: Сделать оповещение на экране об ошибке (всплывающий элемент для всех окон)
            errorLabel.Text = "Произошла ошибка / отсутствует подключение к интернету";
            errorLabel.TextColor = Colors.Red;

            LoginBtn.IsEnabled = true;
            return;
        }

        if (authResult)
        {
            await SecureStorage.Default.SetAsync("login", Login);
            await SecureStorage.Default.SetAsync("password", Password);

            App.Current.MainPage = new NavigationPage(new MainPage());
        }
        else
        {
            SecureStorage.Default.Remove("login");
            SecureStorage.Default.Remove("password");

            errorLabel.Text = "Неправильный логин или пароль";
            errorLabel.TextColor = Colors.Red;
        }

        LoginBtn.IsEnabled = true;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string Login = LoginEntry.Text;
        string Password = PasswordEntry.Text;

        await handleAuthorization(Login, Password);
    }
}