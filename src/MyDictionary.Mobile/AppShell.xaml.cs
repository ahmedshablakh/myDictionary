using MyDictionary.Mobile.Views;

namespace MyDictionary.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Register routes for navigation
		Routing.RegisterRoute("addword", typeof(AddWordPage));
	}
}
