using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyDictionary.Core.Entities;
using MyDictionary.Core.Interfaces;

namespace MyDictionary.Mobile.ViewModels;

public partial class TestsViewModel : BaseViewModel
{
    private readonly IWordRepository _wordRepository;
    private readonly ITextToSpeechService _ttsService;
    
    [ObservableProperty]
    private bool isTesting;

    [ObservableProperty]
    private bool isMenuVisible = true;

    [ObservableProperty]
    private string questionText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> options = new();

    [ObservableProperty]
    private int score;

    [ObservableProperty]
    private int currentQuestionIndex;

    [ObservableProperty]
    private int totalQuestions = 5;

    private List<Word> _testWords = new();
    private Word? _currentWord;

    [ObservableProperty]
    private bool isMultipleChoice;

    [ObservableProperty]
    private bool isTypingTest;

    [ObservableProperty]
    private string typedAnswer;

    public TestsViewModel(IWordRepository wordRepository, ITextToSpeechService ttsService)
    {
        _wordRepository = wordRepository;
        _ttsService = ttsService;
        Title = "Tests";
        TypedAnswer = string.Empty;
    }

    [RelayCommand]
    async Task StartMultipleChoiceAsync()
    {
        await StartTestAsync(TestType.MultipleChoice);
    }

    [RelayCommand]
    async Task StartTypingTestAsync()
    {
        await StartTestAsync(TestType.Typing);
    }

    private async Task StartTestAsync(TestType type)
    {
        try
        {
            IsBusy = true;
            
            var allWords = await _wordRepository.GetAllAsync();
            if (allWords.Count() < 4)
            {
                await Shell.Current.DisplayAlert("Not Enough Words", "You need at least 4 words to start a test.", "OK");
                return;
            }

            _testWords = allWords.OrderBy(x => Guid.NewGuid()).Take(TotalQuestions).ToList();
            
            Score = 0;
            CurrentQuestionIndex = 0;
            IsMenuVisible = false;
            IsTesting = true;
            
            IsMultipleChoice = type == TestType.MultipleChoice;
            IsTypingTest = type == TestType.Typing;

            LoadNextQuestion();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to start test: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void LoadNextQuestion()
    {
        if (CurrentQuestionIndex >= _testWords.Count)
        {
            EndTest();
            return;
        }

        _currentWord = _testWords[CurrentQuestionIndex];
        QuestionText = $"What is the translation of '{_currentWord.WordText}'?";
        TypedAnswer = string.Empty;

        if (IsMultipleChoice)
        {
            // Generate options
            var wrongOptions = _testWords
                .Where(w => w.Id != _currentWord.Id)
                .OrderBy(x => Guid.NewGuid())
                .Take(3)
                .Select(w => w.Translation)
                .ToList();

            var allOptions = new List<string> { _currentWord.Translation };
            allOptions.AddRange(wrongOptions);
            Options = new ObservableCollection<string>(allOptions.OrderBy(x => Guid.NewGuid()));
        }
        
        CurrentQuestionIndex++;
    }

    [RelayCommand]
    async Task SubmitAnswerAsync(string answer)
    {
        if (_currentWord == null) return;

        bool isCorrect = false;

        if (IsMultipleChoice)
        {
            isCorrect = answer == _currentWord.Translation;
        }
        else if (IsTypingTest)
        {
            isCorrect = string.Equals(TypedAnswer?.Trim(), _currentWord.Translation, StringComparison.OrdinalIgnoreCase);
        }

        if (isCorrect)
        {
            Score++;
            await Shell.Current.DisplayAlert("Correct!", "Good job!", "Next");
        }
        else
        {
            await Shell.Current.DisplayAlert("Wrong", $"The correct answer was: {_currentWord.Translation}", "Next");
        }

        LoadNextQuestion();
    }

    [RelayCommand]
    void EndTest()
    {
        IsTesting = false;
        IsMenuVisible = true;
        Shell.Current.DisplayAlert("Test Completed", $"You scored {Score} out of {_testWords.Count}", "OK");
    }

    [RelayCommand]
    async Task CancelTestAsync()
    {
        var result = await Shell.Current.DisplayAlert("Quit Test", "Are you sure you want to quit?", "Yes", "No");
        if (result)
        {
            IsTesting = false;
            IsMenuVisible = true;
        }
    }

    private enum TestType
    {
        MultipleChoice,
        Typing
    }

    [RelayCommand]
    async Task StartListeningTestAsync() => await Shell.Current.DisplayAlert("Coming Soon", "Listening test will be available soon!", "OK");

    [RelayCommand]
    async Task StartMatchingTestAsync() => await Shell.Current.DisplayAlert("Coming Soon", "Matching test will be available soon!", "OK");

    [RelayCommand]
    async Task SpeakQuestionAsync()
    {
        if (_currentWord == null) return;
        await _ttsService.SpeakAsync(_currentWord.WordText, _currentWord.SourceLanguage);
    }

    [RelayCommand]
    async Task SpeakAnswerAsync()
    {
        if (_currentWord == null) return;
        await _ttsService.SpeakAsync(_currentWord.Translation, _currentWord.TargetLanguage);
    }
}
