namespace MyDictionary.Core.Entities;

public class TestResult
{
    public int Id { get; set; }
    public DateTime TestDate { get; set; } = DateTime.UtcNow;
    public TestType TestType { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public TimeSpan Duration { get; set; }
    public double Score => TotalQuestions == 0 ? 0 : (double)CorrectAnswers / TotalQuestions * 100;
    public string? Notes { get; set; }
}

public enum TestType
{
    MultipleChoice,
    Typing,
    Listening,
    Matching,
    Mixed
}
