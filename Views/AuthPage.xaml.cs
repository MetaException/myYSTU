using myYSTU.Utils;

namespace myYSTU.Views;

public partial class AuthPage : ContentPage
{
    string Login = "";
    string Password = "";

    private readonly INetUtils _netUtil;

    public AuthPage()
    {
        InitializeComponent();
        _netUtil = DependencyService.Get<INetUtils>();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        Login = LoginEntry.Text;
        Password = PasswordEntry.Text;

        //Для отладки
        //TODO: убрать
        Login = "baranovea.21";
        Password = "OO5n8n#1NP";

        var authResult = await _netUtil.Authorize(Login, Password);

        if (authResult == 1)
        {
            App.Current.MainPage = new NavigationPage(new MainPage());
        }
        else if (authResult == 0)
        {
            App.Current.MainPage = new NavigationPage(new MainPage());
            errorLabel.Text = "Неправильный логин или пароль";
            errorLabel.TextColor = Colors.Red;
        }
        else
        {
            //TODO: Сделать оповещение на экране об ошибке
        }
    }
}