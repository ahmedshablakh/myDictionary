using MyDictionary.Mobile.ViewModels;

namespace MyDictionary.Mobile.Views;

public partial class FlashcardsPage : ContentPage
{
	public FlashcardsPage(FlashcardsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		
		if (BindingContext is FlashcardsViewModel vm)
		{
			await vm.LoadFlashcardsCommand.ExecuteAsync(null);
		}
	}
}
