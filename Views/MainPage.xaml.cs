//"baranovea.21" "OO5n8n#1NP"
using myYSTU.Parsers;

namespace myYSTU.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        ParseAsync();
    }

    private async void ParseAsync()
    {
        var person = await PersonParser.ParseInfo();

        Fullname.Text = person.Name[..person.Name.LastIndexOf(' ')];
        GroupName.Text = person.Group;
        avatar.Source = person.Avatar;
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