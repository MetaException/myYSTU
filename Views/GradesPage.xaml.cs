using myYSTU.Model;
using myYSTU.Parsers;
using System.Collections.ObjectModel;

namespace myYSTU.Views;

public partial class GradesPage : ContentPage
{
    private readonly Dictionary<int, ObservableCollection<Grades>> gradesDict = new Dictionary<int, ObservableCollection<Grades>>();
    private readonly List<Grades> gradesCategories = new List<Grades>();

    private readonly ParseManager parseManager = DependencyService.Get<ParseManager>();

    public GradesPage()
    {
        InitializeComponent();
        Task.Run(async () => await ParseAsync()).Wait();

        //TODO: Костыль
        if (!parseManager.isError)
        {
            SetGradesCategories();
            UpdateGradesInfo();
        }
    }

    private async Task ParseAsync()
    {
        var gradesParser = parseManager.ParseInfo(new GradesParser(), Links.GradesLink);

        if (parseManager.isError)
        {
            internetError.IsVisible = true;
            return;
        }    

        await foreach (Grades gradeInfo in gradesParser)
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