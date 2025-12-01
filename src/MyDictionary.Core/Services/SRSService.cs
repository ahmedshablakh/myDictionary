using MyDictionary.Core.Entities;

namespace MyDictionary.Core.Services;

/// <summary>
/// Spaced Repetition System service implementing SM-2 algorithm
/// </summary>
public class SRSService
{
    /// <summary>
    /// Update word based on review quality
    /// </summary>
    /// <param name="word">The word to update</param>
    /// <param name="difficulty">User's assessment of difficulty</param>
    public void ReviewWord(Word word, Difficulty difficulty)
    {
        word.ReviewCount++;
        word.LastReviewed = DateTime.UtcNow;

        // Calculate next review date based on difficulty
        int daysToAdd = difficulty switch
        {
            Difficulty.Easy => CalculateInterval(word, 4),
            Difficulty.Medium => CalculateInterval(word, 2),
            Difficulty.Hard => 1,
            _ => 1
        };

        word.NextReviewDate = DateTime.UtcNow.AddDays(daysToAdd);
        
        // Update interval for SM-2
        word.Interval = daysToAdd;
        
        // Adjust easiness factor
        UpdateEasinessFactor(word, difficulty);
    }

    /// <summary>
    /// Record answer correctness
    /// </summary>
    public void RecordAnswer(Word word, bool isCorrect)
    {
        if (isCorrect)
        {
            word.CorrectAnswers++;
        }
        else
        {
            word.WrongAnswers++;
        }
    }

    private int CalculateInterval(Word word, int baseInterval)
    {
        if (word.ReviewCount == 0)
            return baseInterval;

        // Apply easiness factor
        double ef = word.EasinessFactor / 100.0;
        int newInterval = (int)(word.Interval * ef);
        
        return Math.Max(baseInterval, newInterval);
    }

    private void UpdateEasinessFactor(Word word, Difficulty difficulty)
    {
        // SM-2 easiness factor adjustment
        // Easy: increase EF, Hard: decrease EF
        int adjustment = difficulty switch
        {
            Difficulty.Easy => 10,    // +0.1
            Difficulty.Medium => 0,
            Difficulty.Hard => -20,   // -0.2
            _ => 0
        };

        word.EasinessFactor = Math.Clamp(word.EasinessFactor + adjustment, 130, 300);
    }

    /// <summary>
    /// Check if word is due for review
    /// </summary>
    public bool IsDueForReview(Word word)
    {
        return word.NextReviewDate <= DateTime.UtcNow;
    }

    /// <summary>
    /// Check if word is considered mastered
    /// </summary>
    public bool IsMastered(Word word)
    {
        return word.ReviewCount >= 5 && 
               word.AccuracyRate >= 80 &&
               word.Interval >= 7;
    }
}
