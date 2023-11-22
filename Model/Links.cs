namespace myYSTU.Model
{
    public static class Links
    {
        public static readonly string BaseUri = "https://www.ystu.ru";
        public static readonly string GradesLink = "/WPROG/lk/lkstud_oc.php";
        public static readonly string AccountInfoLink = "/WPROG/lk/lkstud.php";
        public static readonly string StaffLink = "/users/?PAGEN_1=";
        public static readonly string AuthorizeLink = "/WPROG/auth1.php";
        public static readonly string TimeTableLink = "/wprog/rasp/raspz1day.php";

        public static string TimeTableLinkParams { get; set; }
    }
}
