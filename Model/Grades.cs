namespace myYSTU.Model
{
    public class Grades : IParsable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Grade { get; set; }
        public int SemesterNumber { get; set; }
    }
}
