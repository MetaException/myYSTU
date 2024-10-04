using myYSTU.Views;

namespace myYSTU
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("AuthPage", typeof(AuthPage));
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("MainPage/GradesPage", typeof(GradesPage));
            Routing.RegisterRoute("MainPage/StaffPage", typeof(StaffPage));
            Routing.RegisterRoute("MainPage/TimeTablePage", typeof(TimeTablePage));
        }
    }
}
