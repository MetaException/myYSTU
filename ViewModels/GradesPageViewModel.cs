using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using myYSTU.Models;
using myYSTU.Parsers;
using NLog;
using System.Collections.ObjectModel;

namespace myYSTU.ViewModels;

public partial class GradesPageViewModel : ObservableObject
{
    private readonly ILogger _logger;

    public GradesPageViewModel(ILogger logger)
    {
        _logger = logger;
    }

    private Dictionary<int, ObservableCollection<Grades>> gradesDict = new Dictionary<int, ObservableCollection<Grades>>(); 

    #region ObservableProperties

    [ObservableProperty]
    private int _selectedSemester = 1;

    [ObservableProperty]
    private bool _isInternetErrorVisible = false;

    [ObservableProperty]
    private bool _isDataLoaded = false;

    [ObservableProperty]
    private IEnumerable<int> _gradesCategories;

    [ObservableProperty]
    private IEnumerable<Grades> _gradesList;
    #endregion

    #region RelayCommands

    [RelayCommand]
    private async Task OnAppearing()
    {
        try
        {
            await ParseAsync();
            IsDataLoaded = true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Grades parsing error");
            IsInternetErrorVisible = true;
            return;
        }
        UpdateGradesInfo();
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
                gradesDict.Add(gradeInfo.SemesterNumber, new ObservableCollection<Grades> { gradeInfo });
            }
        }

        GradesCategories = gradesDict.Keys;
    }
}
