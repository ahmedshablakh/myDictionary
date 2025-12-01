namespace MyDictionary.Core.Entities;

public class DailyChallenge
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string ChallengeName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TargetCount { get; set; }
    public int CurrentCount { get; set; }
    public bool IsCompleted { get; set; }
    public int XPReward { get; set; }
    public DateTime? CompletedAt { get; set; }
}
