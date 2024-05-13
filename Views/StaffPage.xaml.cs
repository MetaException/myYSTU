using myYSTU.ViewModels;

namespace myYSTU.Views;

public partial class StaffPage : ContentPage
{
    public StaffPage(StaffPageViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}
