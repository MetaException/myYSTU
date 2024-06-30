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
    private readonly ILogger _logger;

    public MainPageViewModel(ILogger logger)
    {
        _logger = logger;
    }

    #region ObservableProperties
    [ObservableProperty]
    private bool _isInternetErrorVisible = false;

    [ObservableProperty]
    private bool _isDataLoaded = false;

    [ObservableProperty]
    private Person _person;
    #endregion

    #region RelayCommands

    [RelayCommand]
    private async Task OnAppearing()
    {
        try
        {
            await ParseAsync();
            IsDataLoaded = true;
        }
        catch (NetUtils.AuthException)
        {
            await Shell.Current.GoToAsync("AuthPage");
        }
        catch (Exception ex) //TODO: обработать ошибки
        {
            IsInternetErrorVisible = true;
            _logger.Error(ex, "Main profile parsing error");
        }
    }

    [RelayCommand] 
    private void ShowInfoBottomSheet()
    {
        var sheet = new InfoBottomSheet(); //Пока автор библиотеки не пофиксит ошибку
        sheet.HasBackdrop = true;
        //TODO: переделать
        sheet.setInfo(Person);
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

    #endregion

    private async Task ParseAsync()
    {
        var parser = ParserFactory.CreateParser<Person>();

        Person = (await parser.ParseInfo()).First();
        await parser.UpdateAvatarAsync(Person);
    }
}
