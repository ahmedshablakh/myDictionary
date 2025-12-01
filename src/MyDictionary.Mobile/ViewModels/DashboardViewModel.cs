using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyDictionary.Core.Interfaces;

namespace MyDictionary.Mobile.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly IWordRepository _wordRepository;
    private readonly ITestResultRepository _testResultRepository;

    [ObservableProperty]
    private int totalWords;

    [ObservableProperty]
    private int wordsDueToday;

    [ObservableProperty]
    private int masteredWords;

    [ObservableProperty]
    private double learningStreak;

    [ObservableProperty]
    private double masteryPercentage;

    [ObservableProperty]
    private double masteryProgress;

    [ObservableProperty]
    private int reviewsCompletedToday;

    [ObservableProperty]
    private double reviewProgress;

    [ObservableProperty]
    private int wordsThisWeek;

    [ObservableProperty]
    private double weeklySuccessRate;

    [ObservableProperty]
    private int studyDaysThisWeek;

    [ObservableProperty]
    private string motivationalMessage = "Keep learning!";

    public DashboardViewModel(IWordRepository wordRepository, ITestResultRepository testResultRepository)
    {
        _wordRepository = wordRepository;
        _testResultRepository = testResultRepository;
        Title = "Dashboard";
    }

    [RelayCommand]
    async Task LoadDashboardAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            TotalWords = await _wordRepository.GetTotalCountAsync();
            MasteredWords = await _wordRepository.GetMasteredCountAsync();
            
            var dueWords = await _wordRepository.GetDueForReviewAsync();
            WordsDueToday = dueWords.Count();
            
            // Calculate mastery percentage
            if (TotalWords > 0)
            {
                MasteryPercentage = (double)MasteredWords / TotalWords * 100;
                MasteryProgress = (double)MasteredWords / TotalWords;
            }
            else
            {
                MasteryPercentage = 0;
                MasteryProgress = 0;
            }

            // Calculate words added this week
            var allWords = await _wordRepository.GetAllAsync();
            var weekAgo = DateTime.Now.AddDays(-7);
            WordsThisWeek = allWords.Count(w => w.CreatedAt >= weekAgo);

            // Calculate study days this week (simplified)
            StudyDaysThisWeek = Math.Min((int)DateTime.Now.DayOfWeek + 1, 7);

            // Calculate weekly success rate (simplified)
            var recentTests = await _testResultRepository.GetRecentResultsAsync(7);
            if (recentTests.Any())
            {
                WeeklySuccessRate = recentTests.Average(t => t.Score) * 100;
            }
            else
            {
                WeeklySuccessRate = 0;
            }

            // Calculate reviews completed today (simplified)
            ReviewsCompletedToday = 0; // TODO: Implement actual tracking
            ReviewProgress = WordsDueToday > 0 ? (double)ReviewsCompletedToday / WordsDueToday : 0;
            
            // Calculate streak based on consecutive study days
            LearningStreak = CalculateStreak();

            // Set motivational message
            MotivationalMessage = GetMotivationalMessage();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load dashboard: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private int CalculateStreak()
    {
        // Simplified streak calculation
        // In a real app, you'd track actual study days
        if (WordsDueToday > 0 || ReviewsCompletedToday > 0)
            return (int)LearningStreak + 1;
        return Math.Max(1, (int)LearningStreak);
    }

    private string GetMotivationalMessage()
    {
        if (MasteryPercentage >= 80)
            return "🌟 Excellent progress! You're a vocabulary master!";
        else if (MasteryPercentage >= 60)
            return "💪 Great work! Keep up the momentum!";
        else if (MasteryPercentage >= 40)
            return "📚 You're making good progress!";
        else if (MasteryPercentage >= 20)
            return "🚀 Keep going! Every word counts!";
        else if (TotalWords > 0)
            return "🌱 Great start! Keep adding words!";
        else
            return "👋 Welcome! Add your first word to get started!";
    }

    [RelayCommand]
    async Task NavigateToFlashcardsAsync()
    {
        await Shell.Current.GoToAsync("//flashcards");
    }

    [RelayCommand]
    async Task NavigateToWordsAsync()
    {
        await Shell.Current.GoToAsync("//words");
    }

    [RelayCommand]
    async Task NavigateToTestsAsync()
    {
        await Shell.Current.GoToAsync("//tests");
    }
}
