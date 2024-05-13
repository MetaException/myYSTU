namespace myYSTU.Models
{
    public class Grades : IModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Grade { get; set; }
        public int SemesterNumber { get; set; }
    }
}
