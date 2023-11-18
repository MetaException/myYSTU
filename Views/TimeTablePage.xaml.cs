using myYSTU.Model;
using myYSTU.Parsers;
using System.Collections.ObjectModel;

namespace myYSTU.Views;

public partial class TimeTablePage : ContentPage
{
    private ObservableCollection<TimeTableSubject> subjectList = new ObservableCollection<TimeTableSubject>();
    private ObservableCollection<RadioButtonTemplate> radioButtons = new ObservableCollection<RadioButtonTemplate>();

    private DateTime currDay;
    private DateTime firstDayOfWeek;
    private int currWeekNumber;

    //Добавить кнопку для возврата в текущий день

    public TimeTablePage()
    {
        Task.Run(async () => await ParseAsync()).Wait();
        InitializeComponent();
        UpdateDaysList();
    }

    private async Task ParseAsync()
    {
        var weekList = await TimeTableParser.ParseWeekList();

        currDay = DateTime.Today.Date; //TODO: Получать данные из интернета (например если по умолчанию дата стоит не другой год)

        for (int i = 0; i < weekList.Length; i++)
        {
            if (weekList[i] > currDay)
            {
                firstDayOfWeek = weekList[i - 1];
                currWeekNumber = i;
                break;
            }
        }
    }

    //Выполняется при изменении выбранного дня (программно тоже считается)
    private async void timeTableUpdateHandler(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            var date = ((RadioButton)sender).ClassId;
            currDay = DateTime.Parse(date, new System.Globalization.CultureInfo("ru-RU"));

            //Обновляем расписание
            crday.Text = date;
            crweek.Text = currWeekNumber.ToString();

            subjectList.Clear();

            var timeTableParser = TimeTableParser.ParseInfoByDay(date);
            await foreach (var subjectInfo in timeTableParser)
            {
                subjectList.Add(subjectInfo);
            }
            TimeTable.ItemsSource = subjectList;
        }
    }

    private void UpdateDaysList()
    {
        radioButtons.Clear();
        for (int i = 0; i < 7; i++)
        {
            var d = firstDayOfWeek.AddDays(i);
            var rb  = new RadioButtonTemplate()
            {
                Date = d.ToString("d", new System.Globalization.CultureInfo("ru-RU")),
                Day = d.Day,
                isChecked = d.Day == currDay.Day
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