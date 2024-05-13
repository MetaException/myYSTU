using CommunityToolkit.Mvvm.ComponentModel;

namespace myYSTU.Models;

public partial class TimeTableDayModel : ObservableObject
{
    [ObservableProperty]
    private DateTime _date;
}
