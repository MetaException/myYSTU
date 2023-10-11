namespace MauiApp1.Model
{
    public class Grades
    {
        public struct SubjectInfo 
        {
            public string Name{ get; set; }
            public string Type { get; set; }
            public string Grade { get; set; }
        }
        public Dictionary<int, List<SubjectInfo>> Subjects { get; set; }

        public Grades()
        {
            Subjects = new Dictionary<int, List<SubjectInfo>>();
        }
    }
}
