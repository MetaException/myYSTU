using myYSTU.ViewModels;

namespace myYSTU.Views;

public partial class GradesPage : ContentPage
{
    public GradesPage(GradesPageViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}