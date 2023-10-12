using MauiApp1.Model;
using MauiApp1.Parsers;
using System.Collections.ObjectModel;

namespace MauiApp1.Views;

public partial class StaffPage : ContentPage
{
    private Staff _staff;
    private ObservableCollection<Staff> _staffList { get; set; }

    public StaffPage()
    {
        InitializeComponent();

        _staffList = new ObservableCollection<Staff>();
        initAsync();
    }


    //TODO: сделать чтобы подгрузка была незаметной
    private async void initAsync()
    {
        _staff = DependencyService.Get<Staff>();

        var staffParser = StaffParser.ParseInfo();

        await foreach (var staffInfo in staffParser)
        {
            _staffList.Add(staffInfo);
            StaffTable.ItemsSource = _staffList;
        }
        //StaffTable.ItemsSource = _staffList;
    }

    private List<Staff> GetSearchResults(string query)
    {
        //var ret = _staff.Where(x => x.Name.Contains(query)).ToList();
        //return ret;
        return null;
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;

        //Если поиск активен,то подгрузка заменит найденные элементы
        if (searchBar.Text == "")
        {
            //StaffTable.ItemsSource = _staff.staffList;
        }
        else
        {
            StaffTable.ItemsSource = GetSearchResults(searchBar.Text);
        }
    }

    private async void StaffTable_RemainingItemsThresholdReached(object sender, EventArgs e)
    {
        initAsync();
        //StaffTable.ItemsSource = _staff.staffList;
    }
}
