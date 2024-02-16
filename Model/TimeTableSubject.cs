namespace myYSTU.Model
{
    public class TimeTableSubject : IModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Audithory { get; set; }
        public string Lecturer { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
