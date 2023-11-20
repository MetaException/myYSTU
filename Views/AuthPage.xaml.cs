﻿using myYSTU.Utils;

namespace myYSTU.Views;

public partial class AuthPage : ContentPage
{
    private readonly INetUtils _netUtil = DependencyService.Get<INetUtils>();

    public AuthPage()
    {
        InitializeComponent();

        TryAuthorizeWithSavedCredentials();
    }

    private async void TryAuthorizeWithSavedCredentials()
    {
        string login = await SecureStorage.Default.GetAsync("login");
        string password = await SecureStorage.Default.GetAsync("password");

        if (login != null && password != null)
            await handleAuthorization(login, password);
    }

    private async Task handleAuthorization(string Login, string Password)
    {
        LoginBtn.IsEnabled = false; //Выключаем, чтобы пользователь не нажал на кнопку дважды

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

            errorLabel.TextColor = Colors.Red;
            errorLabel.Text = "Неправильный логин или пароль";
        }
        else
        {
            //TODO: Сделать оповещение на экране об ошибке (всплывающий элемент для всех окон)
            errorLabel.TextColor = Colors.Red;
            errorLabel.Text = "Произошла ошибка / отсутствует подключение к интернету";
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