using The49.Maui.BottomSheet;

namespace myYSTU.Views;

public partial class InfoBottomSheet : BottomSheet
{
	public InfoBottomSheet()
	{
		InitializeComponent();
    }
	public void setInfo(Model.Person person)
	{
		BindableLayout.SetItemsSource(PersonData, new List<Model.Person>(1) {person});
    }
}