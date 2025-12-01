namespace MyDictionary.Core.Entities;

public class Word
{
    public int Id { get; set; }
    public string WordText { get; set; } = string.Empty;
    public string Translation { get; set; } = string.Empty;
    public PartOfSpeech PartOfSpeech { get; set; }
    public Difficulty Difficulty { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int ReviewCount { get; set; }
    public DateTime? LastReviewed { get; set; }
    public DateTime NextReviewDate { get; set; }
    public string LanguageFrom { get; set; } = "en";
    public string LanguageTo { get; set; } = "ar";
    
    // Convenience properties for TTS
    public string SourceLanguage => LanguageFrom;
    public string TargetLanguage => LanguageTo;
    
    public bool IsFavorite { get; set; }
    public string? Category { get; set; }
    public string? ExampleSentence { get; set; }
    public string? ExampleTranslation { get; set; }
    public string? Notes { get; set; }
    
    // SRS Properties
    public int EasinessFactor { get; set; } = 250; // 2.5 * 100
    public int Interval { get; set; } = 0;
    
    // Statistics
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    
    public double AccuracyRate => 
        (CorrectAnswers + WrongAnswers) == 0 ? 0 : 
        (double)CorrectAnswers / (CorrectAnswers + WrongAnswers) * 100;
}

public enum PartOfSpeech
{
    Noun,
    Verb,
    Adjective,
    Adverb,
    Pronoun,
    Preposition,
    Conjunction,
    Interjection,
    Other
}

public enum Difficulty
{
    Easy = 1,
    Medium = 2,
    Hard = 3
}
