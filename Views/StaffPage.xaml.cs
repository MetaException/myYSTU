using myYSTU.Model;
using myYSTU.Parsers;
using System.Collections.ObjectModel;

namespace myYSTU.Views;

public partial class StaffPage : ContentPage
{
    private readonly ObservableCollection<Staff> staffList = new ObservableCollection<Staff>();

    public StaffPage()
    {
        InitializeComponent();
    }


    private async void ParseAsync()
    {
        var staffParser = StaffParser.ParseInfo();

        await foreach (var staffInfo in staffParser)
        {
            //TODO: если во время загрузки выйти, то приложение выкинет исключение
            staffList.Add(staffInfo);
            if (string.IsNullOrEmpty(SearchBar.Text))
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

    private void SearchBar_Loaded(object sender, EventArgs e)
    {
        ParseAsync();
    }
}
