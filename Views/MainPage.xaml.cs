using myYSTU.Model;
using myYSTU.Parsers;
using myYSTU.Utils;

namespace myYSTU.Views;

public partial class MainPage : ContentPage
{
    private Person person;
    private readonly NetUtils _netUtils = DependencyService.Get<NetUtils>();

    public MainPage()
    {
        InitializeComponent();
        UpdateInfo();
    }

    private async Task UpdateInfo()
    {
        await _netUtils.UseSavedSession();
        try
        {
            await ParseAsync();
            internetError.IsVisible = false;
        }
        catch (HttpRequestException ex)
        {
            internetError.IsVisible = true;
            //Log.Error("", ex);
            return;
        }
        activityIndicator.IsVisible = false;
        contentGrid.IsVisible = true;
    }

    private async Task ParseAsync()
    {
        person = await new PersonParser().ParseInfo();

        Fullname.Text = person.Name[..person.Name.LastIndexOf(' ')];
        GroupName.Text = person.Group;

        avatar.Source = await _netUtils.GetImage(person.AvatarUrl);
    }

    private void ProfileInfo_Tapped(object sender, TappedEventArgs e)
    {
        InfoBottomSheet sheet = new InfoBottomSheet(); //Пока автор библиотеки не пофиксит ошибку
        sheet.HasBackdrop = true;
        //TODO: переделать
        sheet.setInfo(person);
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