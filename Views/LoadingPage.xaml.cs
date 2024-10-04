using myYSTU.Services.Auth;
using Serilog;

namespace myYSTU.Views;

public partial class LoadingPage : ContentPage
{
    private readonly IAuthService _authService;

    public LoadingPage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        Log.Information("Application started successfully");
        if (!_authService.IsAuthorized && !await _authService.TryAuthorize())
        {
            await Shell.Current.GoToAsync("AuthPage");
        }
        else
        {
            await Shell.Current.GoToAsync("/MainPage");
        }
    }

    protected async override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);

        //Shell.Current.Navigation.PopAsync();
    }
}