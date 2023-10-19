namespace myYSTU.Model
{
    public class TimeTableSubject
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Audithory { get; set; }
        public string Lecturer { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    public class RadioButtonTemplate
    {
        public int Day { get; set; }
        public bool isChecked { get; set; }
    }
}
