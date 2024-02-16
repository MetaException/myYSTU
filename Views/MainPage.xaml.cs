using myYSTU.Model;
using myYSTU.Parsers;
using myYSTU.Utils;
using NLog;

namespace myYSTU.Views;

public partial class MainPage : ContentPage
{
    private readonly NetUtils _netUtils = DependencyService.Get<NetUtils>();
    private readonly ILogger _logger = DependencyService.Get<Logger>();

    private Person person;

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
        catch (Exception ex)
        {
            internetError.IsVisible = true;
            _logger.Error(ex, "Main profile parsing error");
            return;
        }
        activityIndicator.IsVisible = false;
        contentGrid.IsVisible = true;
    }

    private async Task ParseAsync()
    {
        var data = await ParserFactory.CreateParser<Person>().ParseInfo();

        person = data[0];

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