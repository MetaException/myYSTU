using Microsoft.Datasync.Client;
using myYSTU.Model;
using myYSTU.Parsers;
using NLog;

namespace myYSTU.Views;

public partial class StaffPage : ContentPage
{
    private readonly ILogger _logger = DependencyService.Get<Logger>();

    private ConcurrentObservableCollection<Staff> infoList = new ConcurrentObservableCollection<Staff>();

    public StaffPage()
    {
        InitializeComponent();
        _ = UpdateInfo();
    }

    private async Task UpdateInfo()
    {
        try
        {
            await ParseAsync();
            internetError.IsVisible = false;
        }
        catch (Exception ex)
        {
            internetError.IsVisible = true;
            _logger.Error(ex, "Staff parsing error");
            return;
        }
        activityIndicator.IsVisible = false;
        contentGrid.IsVisible = true;
    }

    private async Task ParseAsync()
    {
        var parser = ParserFactory.CreateParser<Staff>();
        infoList = await parser.ParallelParseInfo();

        StaffTable.ItemsSource = infoList;

        _ = parser.ParseAvatarsAsync(infoList);

        activityIndicator.IsVisible = false;
        contentGrid.IsVisible = true;
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;

        if (searchBar.Text == "")
        {
            StaffTable.ItemsSource = infoList;
        }
        else
        {
            StaffTable.ItemsSource = infoList.Where(x => x.Name.ToLower().Contains(searchBar.Text.ToLower()));
        }
    }
}
