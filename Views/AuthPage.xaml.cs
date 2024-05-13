using myYSTU.ViewModels;

namespace myYSTU.Views;

public partial class AuthPage : ContentPage
{
    public AuthPage(AuthPageViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}