using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using myYSTU.Models;
using myYSTU.Parsers;

namespace myYSTU.ViewModels;

public partial class StaffPageViewModel : ObservableObject
{
    private IEnumerable<Staff> _dataList;

    // https://github.com/dotnet/maui/issues/8994
    private string _searchText;
    public string SearchText
    {
        get
        {
            return _searchText;
        }
        set
        {
            _searchText = value;

            if (_searchText.Length > 2)
            {
                DisplayDataList = _dataList.Where(info => info.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
            }
            else
                DisplayDataList = _dataList;
        }
    }

    #region ObservableProperties
    [ObservableProperty]
    private IEnumerable<Staff> _displayDataList;

    [ObservableProperty]
    private bool _isInternetErrorVisible = false;

    [ObservableProperty]
    private string _internetErrorText;

    #endregion

    #region RelayCommands

    [RelayCommand]
    private async Task OnAppearing()
    {
        Log.Debug("[StaffPageViewModel] [OnAppearing] Staff Page is loading...");
        try
        {
            await ParseAsync();
        }
        catch (ArgumentNullException ex)
        {
            Log.Error(ex, "[StaffPageViewModel] [OnAppearing] Staff profile parsing error");
            IsInternetErrorVisible = true;
            InternetErrorText = "Ошибка получения данных с сервера";
            return;
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "[StaffPageViewModel] [OnAppearing] Staff profile parsing error");
            IsInternetErrorVisible = true;
            InternetErrorText = "Ошибка подключения к серверу";
            return;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[StaffPageViewModel] [OnAppearing] Staff profile parsing error");
            IsInternetErrorVisible = true;
            InternetErrorText = "Неизвестная ошибка";
            return;
        }
        Log.Debug("[StaffPageViewModel] [OnAppearing] Staff Page is loaded successfully...");
    }
    #endregion

    private async Task ParseAsync()
    {
        var parser = ParserFactory.CreateParser<Staff>();

        DisplayDataList = _dataList = await parser.ParallelParseInfo();
        await parser.ParseAvatarsAsync(DisplayDataList);
    }
}
