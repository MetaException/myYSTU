using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using myYSTU.Models;
using myYSTU.Parsers;
using Serilog;
using System.Collections.ObjectModel;

namespace myYSTU.ViewModels;

public partial class TimeTablePageViewModel : ObservableObject
{
    //TODO: Добавить кнопку для возврата в текущий день

    #region ObservableProperties

    [ObservableProperty]
    private bool _isInternetErrorVisible = false;

    [ObservableProperty]
    private int _currentWeek;

    [ObservableProperty]
    private TimeTableDayModel _selectedDay;

    [ObservableProperty]
    private IEnumerable<TimeTableSubject> _subjectList;

    [ObservableProperty]
    private ObservableCollection<TimeTableDayModel> _daysList;

    [ObservableProperty]
    private string _internetErrorText;

    #endregion

    #region RelayCommands

    [RelayCommand]
    private async Task OnAppearing()
    {
        Log.Debug("[TimeTablePageViewModel] [OnAppearing] TimeTable Page is loading...");
        UpdateDaysList();
        Log.Debug("[TimeTablePageViewModel] [OnAppearing] TimeTable Page is loaded successfully...");
    }

    //Выполняется при изменении выбранного дня (программно тоже считается)
    [RelayCommand]
    private async Task TimeTableUpdate()
    {
        if (SelectedDay != null)
        {
            await ParseAsync();
        }
    }

    [RelayCommand]
    private async Task WeekSwitch(string direction)
    {
        int k = 1;
        if (direction == "backward")
            k = -1;

        if (k == 1 && CurrentWeek >= 34 || k == -1 && CurrentWeek <= 1)
            return;

        foreach (var day in DaysList)
        {
            day.Date = day.Date.AddDays(7 * k);
        }

        //Обновляем расписание
        CurrentWeek += k;

        await ParseAsync();
    }
    #endregion

    private async Task ParseAsync()
    {
        if (SelectedDay != null)
        {
            var temp = SelectedDay; // SelectedDay почему-то становиться null после следующей строки

            try
            {
                SubjectList = await ParserFactory.CreateParser<TimeTableSubject>().ParseInfo(temp.Date.ToString("d"));
            }
            catch (ArgumentNullException ex)
            {
                Log.Error(ex, "[TimeTablePageViewModel] [OnAppearing] TimeTable parsing error");
                IsInternetErrorVisible = true;
                InternetErrorText = "Ошибка получения данных с сервера";
            }
            catch (HttpRequestException ex)
            {
                Log.Error(ex, "[TimeTablePageViewModel] [OnAppearing] TimeTable parsing error");
                IsInternetErrorVisible = true;
                InternetErrorText = "Ошибка подключения к серверу";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[TimeTablePageViewModel] [OnAppearing] TimeTable parsing error");
                IsInternetErrorVisible = true;
                InternetErrorText = "Неизвестная ошибка";
            }

            SelectedDay = temp;
        }
    }

    private static DateTime GetFirstDayOfWeek(DateTime input)
    {
        // Получаем понедельник для этой недели
        int delta = DayOfWeek.Monday - input.DayOfWeek;
        if (delta > 0)
            delta -= 7;
        return input.AddDays(delta);
    }

    private void UpdateDaysList()
    {
        // Определяем разницу в днях между текущей датой и 1 сентября. В Январе исп старое расписание
        var today = DateTime.Today.Date;

        DateTime firstDayOfSemester;
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

        DateTime firstDay = GetFirstDayOfWeek(firstDayOfSemester); // Первый день недели первой недели семестра

        int daysDifference = (int)(today - firstDay).TotalDays + 1;

        // Определяем номер недели
        CurrentWeek = (int)Math.Ceiling(daysDifference / 7d);

        DaysList = new ObservableCollection<TimeTableDayModel>();

        // Первый день этой недели
        var firstDayOfWeek = GetFirstDayOfWeek(today);

        for (int i = 0; i < 7; i++)
        {
            var newDay = new TimeTableDayModel { Date = firstDayOfWeek };
            DaysList.Add(newDay);

            if (newDay.Date.Day == today.Day)
            {
                SelectedDay = newDay;
            }

            firstDayOfWeek = firstDayOfWeek.AddDays(1);
        }
    }
}
