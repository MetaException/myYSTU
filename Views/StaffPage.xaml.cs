using myYSTU.Model;
using myYSTU.Parsers;
using System.Collections.ObjectModel;

namespace myYSTU.Views;

public partial class StaffPage : ContentPage
{
    private readonly ObservableCollection<Staff> staffList;

    public StaffPage()
    {
        InitializeComponent();

        staffList = new ObservableCollection<Staff>();
        ParseAsync();
    }


    private async void ParseAsync()
    {
        var staffParser = StaffParser.ParseInfo();

        await foreach (var staffInfo in staffParser)
        {
            staffList.Add(staffInfo);
            if (string.IsNullOrEmpty(SearchBar.Text))
                StaffTable.ItemsSource = staffList;
        }
    }

    private ObservableCollection<Staff> GetSearchResults(string query)
    {
        var ret = new ObservableCollection<Staff>();
        var to_ret = staffList.Where(x => x.Name.ToLower().Contains(query));
        foreach (var staff in to_ret)
        {
            ret.Add(staff);
        }
        return ret;
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
            StaffTable.ItemsSource = GetSearchResults(searchBar.Text.ToLower());
        }
    }
}
