using myYSTU.Model;
using myYSTU.Parsers;
using System.Collections.ObjectModel;

namespace myYSTU.Views;

public partial class GradesPage : ContentPage
{
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
        catch (HttpRequestException ex)
        {
            //Log.Error("", ex);
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
        IAsyncEnumerable<Grades> gradesParser = new GradesParser().ParseInfo();

        await foreach (var gradeInfo in gradesParser)
        {
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