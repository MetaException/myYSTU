using Microsoft.Datasync.Client;
using myYSTU.Model;
using myYSTU.Parsers;
using myYSTU.Utils;

namespace myYSTU.Views;

public partial class StaffPage : ContentPage
{
    private readonly ConcurrentObservableCollection<Staff> staffList = new ConcurrentObservableCollection<Staff>();

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
        }
    }

    private async Task ParseAsync()
    {
        var staffInfoParser = StaffParser.ParseInfo();

        await foreach (var staffInfoList in staffInfoParser)
        {
            await Parallel.ForEachAsync(staffInfoList, async (staffInfo, ct) =>
            {
                staffInfo.Avatar = await NetUtils.GetImage(staffInfo.AvatarUrl);
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
