using MauiApp1.Model;
using MauiApp1.Parsers;
using System.Collections.ObjectModel;

namespace MauiApp1.Views;

public partial class StaffPage : ContentPage
{
    private Staff _staff;

    public ObservableCollection<Staff> Tasks { get; set; } = new ObservableCollection<Staff>();

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
            Tasks.Add(_staff);
        }
        Tasks.Add(_staff);
    }
}