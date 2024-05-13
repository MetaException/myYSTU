using myYSTU.ViewModels;

namespace myYSTU.Views;

public partial class TimeTablePage : ContentPage
{
    public TimeTablePage(TimeTablePageViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}