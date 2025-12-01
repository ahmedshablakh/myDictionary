using MyDictionary.Mobile.ViewModels;

namespace MyDictionary.Mobile.Views;

public partial class TestsPage : ContentPage
{
	public TestsPage(TestsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
