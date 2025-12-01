using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyDictionary.Core.Entities;
using MyDictionary.Core.Interfaces;
using MyDictionary.Core.Services;

namespace MyDictionary.Mobile.ViewModels;

public partial class FlashcardsViewModel : BaseViewModel
{
    private readonly IWordRepository _wordRepository;
    private readonly SRSService _srsService;
    private readonly ITextToSpeechService _ttsService;

    [ObservableProperty]
    private ObservableCollection<Word> flashcards = new();

    [ObservableProperty]
    private Word? currentCard;

    [ObservableProperty]
    private int currentIndex;

    [ObservableProperty]
    private bool isFlipped;

    [ObservableProperty]
    private double progress;

    public bool HasCards => Flashcards.Count > 0;
    
    public string ProgressText => $"Card {CurrentIndex} of {Flashcards.Count}";

    public FlashcardsViewModel(IWordRepository wordRepository, SRSService srsService, ITextToSpeechService ttsService)
    {
        _wordRepository = wordRepository;
        _srsService = srsService;
        _ttsService = ttsService;
        Title = "Flashcards";
    }

    [RelayCommand]
    async Task LoadFlashcardsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            Flashcards.Clear();

            var dueWords = await _wordRepository.GetDueForReviewAsync();
            foreach (var word in dueWords)
            {
                Flashcards.Add(word);
            }

            if (Flashcards.Count > 0)
            {
                CurrentCard = Flashcards[0];
                CurrentIndex = 1;
                Progress = 1.0 / Flashcards.Count;
            }
            
            OnPropertyChanged(nameof(HasCards));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load flashcards: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    void FlipCard()
    {
        IsFlipped = !IsFlipped;
    }

    [RelayCommand]
    async Task SubmitDifficultyAsync(string difficultyStr)
    {
        if (CurrentCard == null) return;

        if (!Enum.TryParse<Difficulty>(difficultyStr, out var difficulty))
            return;

        try
        {
            _srsService.ReviewWord(CurrentCard, difficulty);
            _srsService.RecordAnswer(CurrentCard, difficulty != Difficulty.Hard);
            CurrentCard.Difficulty = difficulty;
            
            await _wordRepository.UpdateAsync(CurrentCard);

            NextCard();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to submit review: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    async Task NavigateToDashboardAsync()
    {
        await Shell.Current.GoToAsync("//dashboard");
    }

    async void NextCard()
    {
        IsFlipped = false;

        if (CurrentIndex < Flashcards.Count)
        {
            CurrentCard = Flashcards[CurrentIndex];
            CurrentIndex++;
            Progress = (double)CurrentIndex / Flashcards.Count;
            OnPropertyChanged(nameof(ProgressText));
        }
        else
        {
            await Shell.Current.DisplayAlert("All Done!", "You've completed all your reviews for today.", "OK");
            await Shell.Current.GoToAsync("//dashboard");
        }
        
        OnPropertyChanged(nameof(HasCards));
    }

    [RelayCommand]
    async Task SpeakWordAsync()
    {
        if (CurrentCard == null) return;
        await _ttsService.SpeakAsync(CurrentCard.WordText, CurrentCard.SourceLanguage);
    }

    [RelayCommand]
    async Task SpeakTranslationAsync()
    {
        if (CurrentCard == null) return;
        await _ttsService.SpeakAsync(CurrentCard.Translation, CurrentCard.TargetLanguage);
    }

    [RelayCommand]
    async Task SpeakExampleAsync()
    {
        if (CurrentCard == null || string.IsNullOrEmpty(CurrentCard.ExampleSentence)) return;
        await _ttsService.SpeakAsync(CurrentCard.ExampleSentence, CurrentCard.SourceLanguage);
    }
}
