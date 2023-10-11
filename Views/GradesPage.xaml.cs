using MauiApp1.Model;
using MauiApp1.Parsers;

namespace MauiApp1.Views;

public partial class GradesPage : ContentPage
{
    private Grades _grades;

    int currSemester = 1;

    public GradesPage()
    {
        InitializeComponent();
        initAsync();
    }

    private async void initAsync()
    {
        await GradesParser.ParseInfo();
        _grades = DependencyService.Get<Grades>();

        foreach (var gradesPerSemester in _grades.Subjects.Keys)
        {
            var buttonCategory = new Button() { Text = $"Семестр {gradesPerSemester}", ClassId = gradesPerSemester.ToString() };
            buttonCategory.Clicked += ButtonCategory_Clicked;
            GradesCategories.Add(buttonCategory);
        }

        UpdateGradesInfo();
    }

    private void ButtonCategory_Clicked(object sender, EventArgs e)
    {
        currSemester = ((Button)sender).ClassId[0] - '0';
        UpdateGradesInfo();
    }

    private void UpdateGradesInfo()
    {
        GradeTable.ItemsSource = _grades.Subjects[currSemester];
    }
}