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
    //TODO: добавить анимацию загрузки
    //TODO: переделать всё так что если возникла ошибка то дальше парсинг не идёт


    public TimeTablePage()
    {
        InitializeComponent();
        UpdateInfo();
    }
    private async Task UpdateInfo()
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
        activityIndicator.IsVisible = false;
        contentGrid.IsVisible = true;
        UpdateDaysList();
    }

    private async Task ParseAsync()
    {
        var weekList = await TimeTableParser.ParseWeekList();

        currDay = DateTime.Today.Date; //Число в системе может быть отличным от настоящей текущей даты
        currWeekNumber = TimeTableParser.GetCurrWeekNumber();
        firstDayOfWeek = weekList[currWeekNumber - 1];

        //Если дата системы не находится в текущей неделе, то устанавливаем первый день по умолчанию
        if (currDay > firstDayOfWeek.AddDays(7))
            currDay = firstDayOfWeek;

        //Обновляем расписание
        crday.Text = currDay.ToString();
        crweek.Text = currWeekNumber.ToString();
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
    }

    private void UpdateDaysList()
    {
        radioButtons.Clear();
        for (int i = 0; i < 7; i++)
        {
            var d = firstDayOfWeek.AddDays(i);
            var rb = new RadioButtonTemplate()
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