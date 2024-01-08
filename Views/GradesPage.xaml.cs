using myYSTU.Model;
using myYSTU.Parsers;
using NLog;
using System.Collections.ObjectModel;

namespace myYSTU.Views;

public partial class GradesPage : ContentPage
{
    private readonly ILogger _logger = DependencyService.Get<Logger>();

    private readonly Dictionary<int, ObservableCollection<Grades>> gradesDict = new Dictionary<int, ObservableCollection<Grades>>();
    private readonly List<Grades> gradesCategories = new List<Grades>();

    public GradesPage()
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
        catch (Exception ex)
        {
            _logger.Error(ex, "Grades parsing error");
            internetError.IsVisible = true;
            return;
        }
        activityIndicator.IsVisible = false;
        contentGrid.IsVisible = true;
        SetGradesCategories();
        UpdateGradesInfo();
    }

    private async Task ParseAsync()
    {
        var gradesParser = new GradesParser().ParseInfo();

        await foreach (var gradeInfo in gradesParser)
        {
            //TODO: оптимизировать
            if (gradesDict.ContainsKey(gradeInfo.SemesterNumber))
            {
                gradesDict[gradeInfo.SemesterNumber].Add(gradeInfo);
            }
            else
            {
                gradesDict.Add(gradeInfo.SemesterNumber, new ObservableCollection<Grades> { gradeInfo });
                gradesCategories.Add(gradeInfo);
            }
        }
    }

    private void SetGradesCategories()
    {
        GradesCategories.ItemsSource = gradesCategories;
    }

    private void UpdateGradesInfo(int currSemester = 1)
    {
        GradesTable.ItemsSource = gradesDict[currSemester];
    }

    private void ButtonCategory_Clicked(object sender, EventArgs e)
    {
        UpdateGradesInfo(((Button)sender).Text.Last() - '0');
    }
}