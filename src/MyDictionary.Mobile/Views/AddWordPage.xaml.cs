using MyDictionary.Mobile.ViewModels;

namespace MyDictionary.Mobile.Views;

public partial class AddWordPage : ContentPage
{
	public AddWordPage(AddWordViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
