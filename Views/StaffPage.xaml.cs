using Microsoft.Datasync.Client;
using myYSTU.Model;
using myYSTU.Parsers;

namespace myYSTU.Views;

public partial class StaffPage : ContentPage
{
    private readonly ConcurrentObservableCollection<Staff> staffList = new ConcurrentObservableCollection<Staff>();

    public StaffPage()
    {
        InitializeComponent();
        ParseAsync();
    }

    private async Task ParseAsync()
    {
        var staffInfoParser = StaffParser.ParseInfo();

        await foreach (var staffInfoList in staffInfoParser)
        {
            await Parallel.ForEachAsync(staffInfoList, async (staffInfo, ct) =>
            {
                staffInfo.Avatar = await StaffParser.ParseAvatar(staffInfo.AvatarUrl);
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
