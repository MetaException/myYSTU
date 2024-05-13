using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Datasync.Client;
using myYSTU.Models;
using myYSTU.Parsers;
using NLog;

namespace myYSTU.ViewModels;

public partial class StaffPageViewModel : ObservableObject
{
    private readonly ILogger _logger;

    #region ObservableProperties
    [ObservableProperty]
    private ConcurrentObservableCollection<Staff> _infoList = new ConcurrentObservableCollection<Staff>();

    [ObservableProperty]
    private bool _isInternetErrorVisible = false;

    [ObservableProperty]
    private bool _isActivityIndicatorVisible = true;

    [ObservableProperty]
    private bool _isContentGridVisible = false;
    #endregion

    private ConcurrentObservableCollection<Staff> _tempList;

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
                Task.Run(() => Search(SearchText));
            }
            else
                InfoList = _tempList;
        }
    }

    public StaffPageViewModel(ILogger logger)
    {
        _logger = logger;

        _ = UpdateInfo();
    }

    private async Task UpdateInfo()
    {
        try
        {
            await ParseAsync();
            IsInternetErrorVisible = false;
        }
        catch (Exception ex)
        {
            IsInternetErrorVisible = true;
            _logger.Error(ex, "Staff parsing error");
            return;
        }
        IsActivityIndicatorVisible = false;
        IsContentGridVisible = true;
    }

    private async Task ParseAsync()
    {
        var parser = ParserFactory.CreateParser<Staff>();
        InfoList = await parser.ParallelParseInfo();

        _ = parser.ParseAvatarsAsync(InfoList);

        _tempList = InfoList;

        IsActivityIndicatorVisible = false;
        IsContentGridVisible = true;
    }

    private void Search(string text)
    {
        var filtered = new ConcurrentObservableCollection<Staff>();
        foreach (var info in _tempList)
        {
            if (info.Name.ToLower().Contains(text.ToLower()))
            {
                filtered.Add(info);
            }
        }

        InfoList = filtered;
    }
}
