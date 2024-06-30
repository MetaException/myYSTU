using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using myYSTU.Models;
using myYSTU.Parsers;
using NLog;

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

    private readonly ILogger _logger;

    public StaffPageViewModel(ILogger logger)
    {
        _logger = logger;
    }

    #region ObservableProperties
    [ObservableProperty]
    private IEnumerable<Staff> _displayDataList;

    [ObservableProperty]
    private bool _isInternetErrorVisible = false;

    #endregion

    #region RelayCommands

    [RelayCommand]
    private async Task OnAppearing()
    {
        try
        {
            await ParseAsync();
        }
        catch (Exception ex)
        {
            IsInternetErrorVisible = true;
            _logger.Error(ex, "Staff parsing error");
        }
    }
    #endregion

    private async Task ParseAsync()
    {
        var parser = ParserFactory.CreateParser<Staff>();

        DisplayDataList = await parser.ParallelParseInfo();
        await parser.ParseAvatarsAsync(DisplayDataList);

        _dataList = DisplayDataList;
    }
}
