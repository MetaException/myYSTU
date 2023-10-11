using MauiApp1.Model;
using MauiApp1.Parsers;

namespace MauiApp1.Views;

public partial class StaffPage : ContentPage
{
    private Staff _staff;

    public StaffPage()
    {
        InitializeComponent();
        initAsync();
    }


    //TODO: сделать чтобы подгрузка была незаметной
    private async void initAsync()
    {
        _staff = DependencyService.Get<Staff>();

        var staffParser = StaffParser.ParseInfo();

        await foreach (var staffInfo in staffParser)
        {
            _staff.staffList.Add(staffInfo);
            if (_staff.staffList.Count % 20 == 0)
            {
                StaffTable.ItemsSource = null;
                StaffTable.ItemsSource = _staff.staffList;
            }
        }
        StaffTable.ItemsSource = null;
        StaffTable.ItemsSource = _staff.staffList;
    }
}
