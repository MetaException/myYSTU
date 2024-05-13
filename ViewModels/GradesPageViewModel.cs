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

        _ = UpdateInfo();
    }

    #region ObservableProperties

    [ObservableProperty]
    private bool _isInternetErrorVisible = false;

    [ObservableProperty]
    private bool _isActivityIndicatorVisible = true;

    [ObservableProperty]
    private bool _isContentGridVisible = false;

    [ObservableProperty]
    private ObservableCollection<Grades> _gradesCategories = new ObservableCollection<Grades>();

    [ObservableProperty]
    private ObservableCollection<Grades> _gradesList = new ObservableCollection<Grades>();
    #endregion

    private readonly Dictionary<int, ObservableCollection<Grades>> gradesDict = new Dictionary<int, ObservableCollection<Grades>>();

    private async Task UpdateInfo()
    {
        try
        {
            await ParseAsync();
            IsInternetErrorVisible = false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Grades parsing error");
            IsInternetErrorVisible = true;
            return;
        }
        IsActivityIndicatorVisible = false;
        IsContentGridVisible = true;
        UpdateGradesInfo();
    }

    private async Task ParseAsync()
    {
        var gradesParser = await ParserFactory.CreateParser<Grades>().ParseInfo();

        foreach (var gradeInfo in gradesParser)
        {
            //TODO: оптимизировать
            if (gradesDict.ContainsKey(gradeInfo.SemesterNumber))
            {
                gradesDict[gradeInfo.SemesterNumber].Add(gradeInfo);
            }
            else
            {
                gradesDict.Add(gradeInfo.SemesterNumber, new ObservableCollection<Grades> { gradeInfo });
                GradesCategories.Add(gradeInfo);
            }
        }
    }

    private void UpdateGradesInfo(int currSemester = 1)
    {
        GradesList = gradesDict[currSemester];
    }

    [RelayCommand]
    private void ButtonCategoryClicked(string sender)
    {
        UpdateGradesInfo(sender.Last() - '0');
    }
}
