using myYSTU.Model;
using myYSTU.Parsers;
using System.Collections.ObjectModel;

namespace myYSTU.Views;

public partial class TimeTablePage : ContentPage
{
    private ObservableCollection<TimeTableSubject> subjectList = new ObservableCollection<TimeTableSubject>();
    private ObservableCollection<RadioButton> radioButtons = new ObservableCollection<RadioButton>();

    private DateTime currDay;
    private DateTime firstDayOfWeek;
    private int currWeekNumber;

    //TODO: Добавить кнопку для возврата в текущий день

    public TimeTablePage()
    {
        InitializeComponent();
        UpdateInfo();
    }

    private async void UpdateInfo()
    {
        try
        {
            await ParseAsync();
            internetError.IsVisible = false;
        }
        catch (HttpRequestException ex)
        {
            internetError.IsVisible = true;
            //Log.Error("", ex);
            return;
        }
        UpdateDaysList();
        activityIndicator.IsVisible = false;
        contentGrid.IsVisible = true;
    }

    private async Task ParseAsync(string date = "")
    {
        if (date == "")
        {
            currDay = DateTime.Today.Date; //Число в системе может быть отличным от настоящей текущей даты
            date = currDay.ToString("d");
        }
        else
        {
            currDay = DateTime.Parse(date, new System.Globalization.CultureInfo("ru-RU"));
        }

        subjectList.Clear();

        IAsyncEnumerable<TimeTableSubject> timeTableParser = null;
        try
        {
            timeTableParser = TimeTableParser.ParseInfoByDay(date);
            internetError.IsVisible = false;
            await foreach (var subjectInfo in timeTableParser)
            {
                subjectList.Add(subjectInfo);
            }
        }
        catch (HttpRequestException ex)
        {
            internetError.IsVisible = true;
            //Log.Error("", ex);
            return;
        }
        TimeTable.ItemsSource = subjectList;
    }

    private DateTime GetFirstDayOfWeek(DateTime input)
    {
        // Получаем понедельник для этой недели
        int delta = DayOfWeek.Monday - input.DayOfWeek;
        if (delta > 0)
            delta -= 7;
        return input.AddDays(delta);
    }

    //Выполняется при изменении выбранного дня (программно тоже считается)
    private async void timeTableUpdateHandler(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            var date = ((RadioButton)sender).ClassId;
            await ParseAsync(date);
        }
    }

    private void UpdateDaysList()
    {
        // Определяем разницу в днях между текущей датой и 1 сентября
        var today = DateTime.Now;
        DateTime firstDayOfSemester;

        //В Январе исп старое расписание
        if (today.Month == 1)
        {
            firstDayOfSemester = new DateTime(day: 1, month: 9, year: today.Year - 1);
        }
        else if (today.Month >= 2 && today.Month <= 8) //В Феврале уже новое
        {
            //TODO: переделать всё
            firstDayOfSemester = new DateTime(day: 1, month: 2, year: today.Year);
        }
        else
        {
            firstDayOfSemester = new DateTime(day: 1, month: 9, year: today.Year);
        }

        DateTime firstDay = GetFirstDayOfWeek(firstDayOfSemester);

        int daysDifference = (int)(currDay - firstDay).TotalDays + 1;

        // Определяем номер недели
        currWeekNumber = (int)Math.Ceiling(daysDifference / 7d);

        firstDayOfWeek = GetFirstDayOfWeek(currDay);

        //Обновляем расписание
        crday.Text = currDay.ToString("d");
        crweek.Text = currWeekNumber.ToString();

        radioButtons.Clear();
        for (int i = 0; i < 7; i++)
        {
            var d = firstDayOfWeek.AddDays(i);
            var rb = new RadioButton()
            {
                ClassId = d.ToString("d", new System.Globalization.CultureInfo("ru-RU")),
                Content = d.Day,
                IsChecked = d.Day == currDay.Day,
            };
            radioButtons.Add(rb);
        }
        DaysList.ItemsSource = radioButtons;
    }

    private void weekSwitchHandler(object sender, EventArgs e)
    {
        var classId = ((Button)sender).ClassId;

        int k = 1;
        if (classId == "goBackButton")
            k = -1;

        if (k == 1 && currWeekNumber >= 34)
            return;

        if (k == -1 && currWeekNumber <= 1)
            return;

        currDay = currDay.AddDays(7 * k);
        firstDayOfWeek = firstDayOfWeek.AddDays(7 * k);
        currWeekNumber += k;

        UpdateDaysList();
    }
}