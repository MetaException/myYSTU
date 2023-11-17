using myYSTU.Model;
using myYSTU.Parsers;
using System.Collections.ObjectModel;

namespace myYSTU.Views;

public partial class GradesPage : ContentPage
{
    private readonly Dictionary<int, ObservableCollection<Grades>> gradesDict;

    int currSemester = 1;

    public GradesPage()
    {
        InitializeComponent();
        //CollectionView Размещение внутри VerticalStackLayout может остановить прокрутку CollectionView и может ограничить количество отображаемых элементов. В этой ситуации замените VerticalStackLayout элементом Grid.
        gradesDict = new Dictionary<int, ObservableCollection<Grades>>();
        ParseAsync();
    }

    private async void ParseAsync()
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

                var buttonCategory = new Button() { Text = $"Семестр {gradeInfo.SemesterNumber}", ClassId = gradeInfo.SemesterNumber.ToString() };
                buttonCategory.Clicked += ButtonCategory_Clicked;
                GradesCategories.Add(buttonCategory);
            }

            UpdateGradesInfo();
        }
    }

    private void UpdateGradesInfo()
    {
        GradesTable.ItemsSource = gradesDict[currSemester];
    }

    private void ButtonCategory_Clicked(object sender, EventArgs e)
    {
        currSemester = ((Button)sender).ClassId[0] - '0';
        UpdateGradesInfo();
    }
}