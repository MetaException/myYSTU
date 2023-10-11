namespace MauiApp1;

public partial class App : Application
{
	public App()
	{
        InitializeComponent();

        MainPage = new AppShell();

        //App.Current.MainPage = new NavigationPage(new AuthPage());
    }
}
