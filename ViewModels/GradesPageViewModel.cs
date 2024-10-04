using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using myYSTU.Models;
using myYSTU.Parsers;
using Serilog;

namespace myYSTU.ViewModels;

public partial class GradesPageViewModel : ObservableObject
{
    private Dictionary<int, List<Grades>> gradesDict = new Dictionary<int, List<Grades>>(); 

    #region ObservableProperties

    [ObservableProperty]
    private int _selectedSemester = 1;

    [ObservableProperty]
    private bool _isInternetErrorVisible = false;

    [ObservableProperty]
    private IEnumerable<int> _gradesCategories;

    [ObservableProperty]
    private IEnumerable<Grades> _gradesList;

    [ObservableProperty]
    private string _internetErrorText;
    #endregion

    #region RelayCommands

    [RelayCommand]
    private async Task OnAppearing()
    {
        Log.Debug("[GradesPageViewModel] [OnAppearing] Grades Page is loading...");
        try
        {
            await ParseAsync();
        }
        catch (ArgumentNullException ex)
        {
            Log.Error(ex, "[GradesPageViewModel] [OnAppearing] Grades parsing error");
            IsInternetErrorVisible = true;
            InternetErrorText = "Ошибка получения данных с сервера";
            return;
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "[GradesPageViewModel] [OnAppearing] Grades parsing error");
            IsInternetErrorVisible = true;
            InternetErrorText = "Ошибка подключения к серверу";
            return;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[GradesPageViewModel] [OnAppearing] Grades parsing error");
            IsInternetErrorVisible = true;
            InternetErrorText = "Неизвестная ошибка";
            return;
        }
        UpdateGradesInfo();
        Log.Debug("[GradesPageViewModel] [OnAppearing] Grades Page is loaded successfully");
    }
    
    [RelayCommand]
    private void UpdateGradesInfo()
    {
        GradesList = gradesDict[SelectedSemester];
    }

    #endregion

    private async Task ParseAsync()
    {
        var gradesParser = await ParserFactory.CreateParser<Grades>().ParseInfo();

        foreach (var gradeInfo in gradesParser)
        {
            if (gradesDict.ContainsKey(gradeInfo.SemesterNumber))
            {
                gradesDict[gradeInfo.SemesterNumber].Add(gradeInfo);
            }
            else
            {
                gradesDict.Add(gradeInfo.SemesterNumber, new List<Grades> { gradeInfo });
            }
        }

        GradesCategories = gradesDict.Keys;
    }
}
