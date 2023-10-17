using myYSTU.Model;
using myYSTU.Parsers;
using System.Collections.ObjectModel;

namespace myYSTU.Views;

public partial class TimeTablePage : ContentPage
{
    private readonly ObservableCollection<TimeTableSubject> subjectList;

    private DateTime currDay;
    private DateTime firstDayOfWeek;
    private int currWeekNumber;

    //Добавить кнопку для возврата в текущий день

    private string currWeek;

    public TimeTablePage()
    {
        InitializeComponent();

        subjectList = new ObservableCollection<TimeTableSubject>();
        currDay = DateTime.Now;

        ParseAsync();
    }

    private async void ParseAsync()
    {
        var weekList = await TimeTableParser.ParseWeekList();

        DateTime currentDay = DateTime.Now.Date;

        for (int i = 0; i < weekList.Length; i++)
        {
            if (weekList[i].Date > currentDay)
            {
                firstDayOfWeek = weekList[i - 1];
                currWeekNumber = i;
                break;
            }
        }

        currWeek = $"{currWeekNumber} - {firstDayOfWeek.Year}/{firstDayOfWeek.Month}/{firstDayOfWeek.Day}";
        currDay = currentDay;

        UpdateDaysList();
        UpdateTimeTable(currDay);
    }

    private void Rb_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            var date = new DateTime(currDay.Year, currDay.Month, int.Parse(((RadioButton)sender).ContentAsString()));
            currDay = date;
            //Изменять неделю тоже??
            UpdateTimeTable(date);
        }
    }

    private async void UpdateDaysList()
    {
        for (int i = 0; i < 7; i++)
        {
            var rb = new RadioButton() { Content = firstDayOfWeek.AddDays(i).Day, IsChecked = firstDayOfWeek.AddDays(i).Day == currDay.Day };
            rb.CheckedChanged += Rb_CheckedChanged;
            DateList.Add(rb);
        }
    }

    private async void UpdateTimeTable(DateTime date)
    {
        subjectList.Clear();
        TimeTable.ItemsSource = subjectList;
        crday.Text = currDay.ToString();
        crweek.Text = currWeekNumber.ToString();

        var timeTableParser = TimeTableParser.ParseInfoByDay(date.ToShortDateString());

        //var timeTableParser = TimeTableParser.ParseInfoByWeek(currWeek);

        await foreach (var subjectInfo in timeTableParser)
        {
            subjectList.Add(subjectInfo);
        }
        TimeTable.ItemsSource = subjectList;
    }

    private void GoBackWeek_Clicked(object sender, EventArgs e)
    {
        if (currWeekNumber <= 1)
            return;

        currDay = currDay.AddDays(-7);
        firstDayOfWeek = firstDayOfWeek.AddDays(-7);
        currWeekNumber--;

        DateList.Clear();

        UpdateDaysList();
        UpdateTimeTable(currDay);
    }

    private void GoNextWeek_Clicked(object sender, EventArgs e)
    {
        if (currWeekNumber >= 34)
            return;

        currDay = currDay.AddDays(7);
        firstDayOfWeek = firstDayOfWeek.AddDays(7);
        currWeekNumber++;

        DateList.Clear();

        UpdateDaysList();
        UpdateTimeTable(currDay);
    }
}