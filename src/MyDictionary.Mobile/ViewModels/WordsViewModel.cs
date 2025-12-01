using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyDictionary.Core.Entities;
using MyDictionary.Core.Interfaces;

namespace MyDictionary.Mobile.ViewModels;

public partial class WordsViewModel : BaseViewModel
{
    private readonly IWordRepository _wordRepository;

    [ObservableProperty]
    private ObservableCollection<Word> words = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private Difficulty? selectedDifficulty;

    [ObservableProperty]
    private PartOfSpeech? selectedPartOfSpeech;

    public WordsViewModel(IWordRepository wordRepository)
    {
        _wordRepository = wordRepository;
        Title = "My Words";
        
        // Initialize filters
        DifficultyFilters = new List<string> { "All" }.Concat(Enum.GetNames(typeof(Difficulty))).ToList();
        PartOfSpeechFilters = new List<string> { "All" }.Concat(Enum.GetNames(typeof(PartOfSpeech))).ToList();
    }

    public List<string> DifficultyFilters { get; }
    public List<string> PartOfSpeechFilters { get; }

    [RelayCommand]
    async Task LoadWordsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            Words.Clear();

            IEnumerable<Word> wordsList;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                wordsList = await _wordRepository.SearchAsync(SearchText);
            }
            else
            {
                wordsList = await _wordRepository.GetByFilterAsync(
                    SelectedDifficulty, 
                    SelectedPartOfSpeech);
            }

            foreach (var word in wordsList)
            {
                Words.Add(word);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load words: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task AddWordAsync()
    {
        await Shell.Current.GoToAsync("addword");
    }

    [RelayCommand]
    async Task EditWordAsync(Word word)
    {
        if (word == null) return;
        
        await Shell.Current.GoToAsync($"addword?id={word.Id}");
    }

    [RelayCommand]
    async Task DeleteWordAsync(Word word)
    {
        if (word == null) return;

        var result = await Shell.Current.DisplayAlert(
            "Delete Word", 
            $"Are you sure you want to delete '{word.WordText}'?", 
            "Delete", 
            "Cancel");

        if (result)
        {
            try
            {
                await _wordRepository.DeleteAsync(word.Id);
                Words.Remove(word);
                await Shell.Current.DisplayAlert("Success", "Word deleted successfully", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to delete word: {ex.Message}", "OK");
            }
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        Task.Run(async () => await LoadWordsAsync());
    }
}
