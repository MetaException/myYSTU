using myYSTU.Utils;
using myYSTU.ViewModels;

namespace myYSTU.Views;

public partial class MainPage : ContentPage
{
    private readonly NetUtils _netUtils;

    public MainPage(MainPageViewModel viewModel, NetUtils netUtils)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
        _netUtils = netUtils;
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (!await _netUtils.TryAuthorize())
        {
            await Shell.Current.GoToAsync("AuthPage");
        }
    }
}