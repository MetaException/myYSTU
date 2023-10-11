//"baranovea.21" "OO5n8n#1NP"

using MauiApp1.Model;
using MauiApp1.Parsers;

namespace MauiApp1.Views;

public partial class MainPage : ContentPage
{
    private Person _person;
     
    public MainPage()
    {
        InitializeComponent();
        InitAsync();
    }

    private async void InitAsync()
    {
        await PersonParser.ParseInfo();
        _person = DependencyService.Get<Person>();

        Fullname.Text = _person.Name[.._person.Name.LastIndexOf(' ')];
        GroupName.Text = _person.Group;
        avatar.Source = _person.Avatar;
    }

    private void ProfileInfo_Tapped(object sender, TappedEventArgs e)
    {
        var sheet = new InfoBottomSheet();

        sheet.ShowAsync();
    }

    private void EnterToGradesPageButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new GradesPage());
    }

    private void EnterToTimeTablePageButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new TimeTablePage());
    }

    private void EnterToStaffPageButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new StaffPage());
    }
}