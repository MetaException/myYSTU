namespace MauiApp1.Model
{
    public class Staff
    {
        public struct StaffInfo 
        {
            public string Name{ get; set; }
            public string Post { get; set; }
            public ImageSource Avatar { get; set; }
        }
        public List<StaffInfo> staffList { get; set; }

        public Staff()
        {
            staffList = new List<StaffInfo>();
        }
    }
}
