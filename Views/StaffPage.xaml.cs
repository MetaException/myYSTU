using MauiApp1.Model;
using MauiApp1.Parsers;
using System.Collections.ObjectModel;

namespace MauiApp1.Views;

public partial class StaffPage : ContentPage
{
    private ObservableCollection<Staff> _staffList { get; set; }

    public StaffPage()
    {
        InitializeComponent();

        _staffList = new ObservableCollection<Staff>();
        ParseAsync();
    }


    private async void ParseAsync()
    {
        var staffParser = StaffParser.ParseInfo();

        await foreach (var staffInfo in staffParser)
        {
            _staffList.Add(staffInfo);
            if (string.IsNullOrEmpty(SearchBar.Text))
                StaffTable.ItemsSource = _staffList;
        }
    }

    private ObservableCollection<Staff> GetSearchResults(string query)
    {
        var ret = new ObservableCollection<Staff>();
        var to_ret = _staffList.Where(x => x.Name.ToLower().Contains(query));
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
            StaffTable.ItemsSource = _staffList;
        }
        else
        {
            StaffTable.ItemsSource = GetSearchResults(searchBar.Text.ToLower());
        }
    }
}
