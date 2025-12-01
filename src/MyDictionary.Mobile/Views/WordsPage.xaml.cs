using MyDictionary.Mobile.ViewModels;

namespace MyDictionary.Mobile.Views;

public partial class WordsPage : ContentPage
{
	public WordsPage(WordsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		
		if (BindingContext is WordsViewModel vm)
		{
			await vm.LoadWordsCommand.ExecuteAsync(null);
		}
	}
}
