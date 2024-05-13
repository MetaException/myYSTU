using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using myYSTU.Models;
using myYSTU.Parsers;
using myYSTU.Utils;
using myYSTU.Views;
using NLog;

namespace myYSTU.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly NetUtils _netUtils;
    private readonly ILogger _logger;

    public MainPageViewModel(NetUtils netUtils, ILogger logger)
    {
        _netUtils = netUtils;
        _logger = logger;

        _ = UpdateInfo();
    }

    #region ObservableProperties
    [ObservableProperty]
    private bool _isInternetErrorVisible = false;

    [ObservableProperty]
    private bool _isActivityIndicatorVisible = true;

    [ObservableProperty]
    private bool _isContentGridVisible = false;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _group;

    [ObservableProperty]
    private ImageSource _avatar;
    #endregion

    private Person _person;

    private async Task UpdateInfo()
    {
        try
        {
            await ParseAsync();
            IsInternetErrorVisible = false;
        }
        catch (NetUtils.AuthException)
        {
            await Shell.Current.GoToAsync("AuthPage");
            return;
        }
        catch (Exception ex)
        {
            IsInternetErrorVisible = true;
            _logger.Error(ex, "Main profile parsing error");
            return;
        }
        IsActivityIndicatorVisible = false;
        IsContentGridVisible = true;
    }

    private async Task ParseAsync()
    {
        _person = (await ParserFactory.CreateParser<Person>().ParseInfo())[0];

        Name = _person.ShortName;
        Group = _person.Group;

        _ = Task.Run(async () =>
        {
            Avatar = await _netUtils.GetImage(_person.AvatarUrl);
        });
    }

    [RelayCommand]
    private void ShowInfoButtomSheet()
    {
        var sheet = new InfoBottomSheet(); //Пока автор библиотеки не пофиксит ошибку
        sheet.HasBackdrop = true;
        //TODO: переделать
        sheet.setInfo(_person);
        sheet.ShowAsync();
    }

    [RelayCommand]
    private void EnterToGradesPage()
    {
        Shell.Current.GoToAsync("GradesPage");
    }

    [RelayCommand]
    private void EnterToTimeTablePage()
    {
        Shell.Current.GoToAsync("TimeTablePage");
    }

    [RelayCommand]
    private void EnterToStaffPage()
    {
        Shell.Current.GoToAsync("StaffPage");
    }
}
