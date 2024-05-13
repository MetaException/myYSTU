using The49.Maui.BottomSheet;

namespace myYSTU.Views;

public partial class InfoBottomSheet : BottomSheet
{
	public InfoBottomSheet()
	{
		InitializeComponent();
    }
	public void setInfo(Models.Person person)
	{
		BindableLayout.SetItemsSource(PersonData, new List<Models.Person>(1) {person});
    }
}