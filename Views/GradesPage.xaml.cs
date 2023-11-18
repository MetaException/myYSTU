using myYSTU.Model;
using myYSTU.Parsers;
using System.Collections.ObjectModel;

namespace myYSTU.Views;

public partial class GradesPage : ContentPage
{
    private readonly Dictionary<int, ObservableCollection<Grades>> gradesDict = new Dictionary<int, ObservableCollection<Grades>>();
    private readonly List<Grades> gradesCategories = new List<Grades>();

    int currSemester = 1;

    public GradesPage()
    {
        Task.Run(async () => await ParseAsync()).Wait();
        InitializeComponent();
        SetGradesCategories();
        UpdateGradesInfo();
    }

    private async Task ParseAsync()
    {
        var gradesParser = GradesParser.ParseInfo();

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

    private void UpdateGradesInfo()
    {
        GradesTable.ItemsSource = gradesDict[currSemester];
    }

    private void ButtonCategory_Clicked(object sender, EventArgs e)
    {
        currSemester = ((Button)sender).Text.Last() - '0';
        UpdateGradesInfo();
    }
}