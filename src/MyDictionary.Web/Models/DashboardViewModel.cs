using MyDictionary.Core.Entities;

namespace MyDictionary.Web.Models;

public class DashboardViewModel
{
    public int TotalWords { get; set; }
    public int MasteredWords { get; set; }
    public int DueForReview { get; set; }
    public IEnumerable<TestResult> RecentTestResults { get; set; } = new List<TestResult>();
    public Dictionary<Difficulty, int> WordsByDifficulty { get; set; } = new();
    public Dictionary<PartOfSpeech, int> WordsByPartOfSpeech { get; set; } = new();
}
