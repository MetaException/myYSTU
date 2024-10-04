using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using myYSTU.Models;
using myYSTU.Parsers;
using myYSTU.Views;
using Serilog;

namespace myYSTU.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    #region ObservableProperties
    [ObservableProperty]
    private bool _isInternetErrorVisible = false;

    [ObservableProperty]
    private Person _person;

    [ObservableProperty]
    private string _internetErrorText;
    #endregion

    #region RelayCommands

    [RelayCommand]
    private async Task OnAppearing()
    {
        Log.Debug("[MainPageViewModel] [OnAppearing] Main Page is loading...");
        try
        {
            await ParseAsync();
            Log.Debug("[MainPageViewModel] [OnAppearing] Main Page is loaded successfully...");
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "[MainPageViewModel] [OnAppearing] Main Page parsing error");
            InternetErrorText = "Ошибка подключения к серверу";
            IsInternetErrorVisible = true;
        }
        catch (ArgumentNullException ex)
        {
            Log.Error(ex, "[MainPageViewModel] [OnAppearing] Main profile parsing error");
            InternetErrorText = "Ошибка парсинга данных";
            IsInternetErrorVisible = true;
        }
        catch (Exception ex) //TODO: обработать ошибки
        {
            Log.Error(ex, "[MainPageViewModel] [OnAppearing] Main profile parsing error");
            InternetErrorText = "Неизвестная ошибка";
            IsInternetErrorVisible = true;
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
