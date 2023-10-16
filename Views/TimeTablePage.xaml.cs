using myYSTU.Model;
using myYSTU.Parsers;
using System.Collections.ObjectModel;

namespace myYSTU.Views;

public partial class TimeTablePage : ContentPage
{
    private readonly ObservableCollection<TimeTableSubject> subjectList;

    private string currWeek;

    public TimeTablePage()
    {
        InitializeComponent();

        subjectList = new ObservableCollection<TimeTableSubject>();
        currentDayLabel.Text = DateTime.Now.ToShortDateString();

        ParseAsync();
    }

    private async Task ParseCurrentWeekAsync()
    {
        var weekList = await TimeTableParser.ParseWeekList();

        DateTime cw = new DateTime();
        int weekNumber = 0;

        for (int i = 0; i < weekList.Length; i++)
        {
            if (weekList[i].Date > DateTime.Now.Date)
            {
                cw = weekList[i - 1];
                weekNumber = i;
                break;
            }
        }

        currWeek = $"{weekNumber} - {cw.Year}/{cw.Month}/{cw.Day}";
    }

    private async void ParseAsync()
    {
        await ParseCurrentWeekAsync();

        var timeTableParser = TimeTableParser.ParseInfoByDay(currentDayLabel.Text);
        //var timeTableParser = TimeTableParser.ParseInfoByWeek(currWeek);

        await foreach (var subjectInfo in timeTableParser)
        {
            subjectList.Add(subjectInfo);
        }
        TimeTable.ItemsSource = subjectList;
    }
}