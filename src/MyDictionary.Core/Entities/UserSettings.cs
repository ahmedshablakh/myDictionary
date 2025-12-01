namespace MyDictionary.Core.Entities;

public class UserSettings
{
    public int Id { get; set; }
    public string DefaultLanguageFrom { get; set; } = "en";
    public string DefaultLanguageTo { get; set; } = "ar";
    public bool DarkMode { get; set; } = true;
    public int DailyGoal { get; set; } = 20;
    public bool EnableNotifications { get; set; } = true;
    public bool AutoPlayAudio { get; set; } = false;
    public string PreferredVoice { get; set; } = "default";
    public int CardsPerSession { get; set; } = 10;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
