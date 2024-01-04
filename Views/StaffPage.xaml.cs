using Microsoft.Datasync.Client;
using myYSTU.Model;
using myYSTU.Parsers;
using myYSTU.Utils;

namespace myYSTU.Views;

public partial class StaffPage : ContentPage
{
    private readonly ConcurrentObservableCollection<Staff> staffList = new ConcurrentObservableCollection<Staff>();
    private readonly NetUtils _netUtils = DependencyService.Get<NetUtils>();

    public StaffPage()
    {
        InitializeComponent();
        UpdateInfo();
    }

    private async Task UpdateInfo()
    {
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
        var staffInfoParser = new StaffParser().ParseInfo();

        await foreach (var staffInfoList in staffInfoParser)
        {
            await Parallel.ForEachAsync(staffInfoList, async (staffInfo, ct) =>
            {
                staffInfo.Avatar = await _netUtils.GetImage(staffInfo.AvatarUrl);
            });
            staffList.AddRange(staffInfoList);
            StaffTable.ItemsSource = staffList;
        }
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;

        if (searchBar.Text == "")
        {
            StaffTable.ItemsSource = staffList;
        }
        else
        {
            StaffTable.ItemsSource = staffList.Where(x => x.Name.ToLower().Contains(searchBar.Text.ToLower()));
        }
    }
}
