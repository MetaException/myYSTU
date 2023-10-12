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


    private async void initAsync()
    {
        _staff = DependencyService.Get<Staff>();

        var staffParser = StaffParser.ParseInfo();

        await foreach (var staffInfo in staffParser)
        {
            _staffList.Add(staffInfo);
            StaffTable.ItemsSource = _staffList;
        }
    }

    private ObservableCollection<Staff> GetSearchResults(string query)
    {
        var ret = new ObservableCollection<Staff>();
        var to_ret = _staffList.Where(x => x.Name.Contains(query));
        foreach (var staff in to_ret)
        {
            ret.Add(staff);
        }
        return ret;
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;

        //Если поиск активен,то подгрузка заменит найденные элементы
        if (searchBar.Text == "")
        {
            StaffTable.ItemsSource = _staffList;
        }
        else
        {
            StaffTable.ItemsSource = GetSearchResults(searchBar.Text);
        }
    }
}
