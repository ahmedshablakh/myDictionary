using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyDictionary.Core.Entities;
using MyDictionary.Core.Interfaces;

namespace MyDictionary.Mobile.ViewModels;

[QueryProperty(nameof(WordId), "id")]
public partial class AddWordViewModel : BaseViewModel
{
    private readonly IWordRepository _wordRepository;
    private readonly ITranslationService _translationService;
    private readonly IAIService _aiService;

    [ObservableProperty]
    private int wordId;

    [ObservableProperty]
    private string wordText = string.Empty;

    [ObservableProperty]
    private string translation = string.Empty;

    [ObservableProperty]
    private string exampleSentence = string.Empty;

    [ObservableProperty]
    private string exampleTranslation = string.Empty;

    [ObservableProperty]
    private string notes = string.Empty;

    [ObservableProperty]
    private PartOfSpeech selectedPartOfSpeech = PartOfSpeech.Noun;

    [ObservableProperty]
    private Difficulty selectedDifficulty = Difficulty.Medium;

    public int SelectedPartOfSpeechIndex
    {
        get => (int)SelectedPartOfSpeech;
        set
        {
            if (Enum.IsDefined(typeof(PartOfSpeech), value))
                SelectedPartOfSpeech = (PartOfSpeech)value;
        }
    }

    public int SelectedDifficultyIndex
    {
        get => (int)SelectedDifficulty - 1; // Easy=1, Medium=2, Hard=3
        set
        {
            var difficultyValue = value + 1;
            if (Enum.IsDefined(typeof(Difficulty), difficultyValue))
                SelectedDifficulty = (Difficulty)difficultyValue;
        }
    }

    public List<string> PartOfSpeechList { get; } = Enum.GetNames(typeof(PartOfSpeech)).ToList();
    public List<string> DifficultyList { get; } = Enum.GetNames(typeof(Difficulty)).ToList();

    [ObservableProperty]
    private string languageFrom = "en";

    [ObservableProperty]
    private string languageTo = "ar";

    public AddWordViewModel(
        IWordRepository wordRepository,
        ITranslationService translationService,
        IAIService aiService)
    {
        _wordRepository = wordRepository;
        _translationService = translationService;
        _aiService = aiService;
        Title = "Add Word";
    }

    async partial void OnWordIdChanged(int value)
    {
        if (value > 0)
        {
            await LoadWordAsync(value);
        }
    }

    async Task LoadWordAsync(int id)
    {
        try
        {
            var word = await _wordRepository.GetByIdAsync(id);
            if (word != null)
            {
                Title = "Edit Word";
                WordText = word.WordText;
                Translation = word.Translation;
                ExampleSentence = word.ExampleSentence ?? string.Empty;
                ExampleTranslation = word.ExampleTranslation ?? string.Empty;
                Notes = word.Notes ?? string.Empty;
                SelectedPartOfSpeech = word.PartOfSpeech;
                SelectedDifficulty = word.Difficulty;
                LanguageFrom = word.LanguageFrom;
                LanguageTo = word.LanguageTo;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load word: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    async Task AutoTranslateAsync()
    {
        if (string.IsNullOrWhiteSpace(WordText)) return;

        try
        {
            var translatedText = await _translationService.TranslateAsync(WordText, LanguageFrom, LanguageTo);
            Translation = translatedText;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to translate: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    async Task GenerateExampleAsync()
    {
        if (string.IsNullOrWhiteSpace(WordText)) return;

        try
        {
            IsBusy = true;
            var example = await _aiService.GenerateExampleSentenceAsync(
               WordText, Translation, SelectedPartOfSpeech.ToString(), LanguageFrom, LanguageTo);
            ExampleSentence = example;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to generate example: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task TranslateExampleAsync()
    {
        if (string.IsNullOrWhiteSpace(ExampleSentence)) return;

        try
        {
            IsBusy = true;
            var translatedExample = await _translationService.TranslateAsync(ExampleSentence, LanguageFrom, LanguageTo);
            ExampleTranslation = translatedExample;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to translate example: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task SaveWordAsync()
    {
        if (string.IsNullOrWhiteSpace(WordText) || string.IsNullOrWhiteSpace(Translation))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Word and Translation are required", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            if (WordId > 0)
            {
                // Update existing word
                var word = await _wordRepository.GetByIdAsync(WordId);
                if (word != null)
                {
                    word.WordText = WordText;
                    word.Translation = Translation;
                    word.ExampleSentence = ExampleSentence;
                    word.ExampleTranslation = ExampleTranslation;
                    word.Notes = Notes;
                    word.PartOfSpeech = SelectedPartOfSpeech;
                    word.Difficulty = SelectedDifficulty;
                    word.LanguageFrom = LanguageFrom;
                    word.LanguageTo = LanguageTo;

                    await _wordRepository.UpdateAsync(word);
                }
            }
            else
            {
                // Add new word
                var word = new Word
                {
                    WordText = WordText,
                    Translation = Translation,
                    ExampleSentence = ExampleSentence,
                    ExampleTranslation = ExampleTranslation,
                    Notes = Notes,
                    PartOfSpeech = SelectedPartOfSpeech,
                    Difficulty = SelectedDifficulty,
                    LanguageFrom = LanguageFrom,
                    LanguageTo = LanguageTo,
                    NextReviewDate = DateTime.UtcNow // Make it due immediately
                };

                await _wordRepository.AddAsync(word);
            }

            await Shell.Current.DisplayAlert("Success", "Word saved successfully", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to save word: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
