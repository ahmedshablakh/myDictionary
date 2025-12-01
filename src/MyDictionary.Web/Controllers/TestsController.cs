using Microsoft.AspNetCore.Mvc;
using MyDictionary.Core.Entities;
using MyDictionary.Core.Interfaces;

namespace MyDictionary.Web.Controllers;

public class TestsController : Controller
{
    private readonly IWordRepository _wordRepository;
    private readonly ITestResultRepository _testResultRepository;

    public TestsController(IWordRepository wordRepository, ITestResultRepository testResultRepository)
    {
        _wordRepository = wordRepository;
        _testResultRepository = testResultRepository;
    }

    public async Task<IActionResult> Index()
    {
        var recentResults = await _testResultRepository.GetRecentResultsAsync(10);
        return View(recentResults);
    }

    public async Task<IActionResult> Start(TestType type, int count = 10)
    {
        var words = await _wordRepository.GetRandomWordsAsync(count);
        if (!words.Any())
        {
            return RedirectToAction(nameof(Index)); // Show error or message
        }

        var model = new TestViewModel
        {
            Type = type,
            Questions = words.Select(w => new TestQuestion
            {
                WordId = w.Id,
                WordText = w.WordText,
                CorrectAnswer = w.Translation,
                // For multiple choice, we'd generate options here
                Options = GenerateOptions(w, words)
            }).ToList()
        };

        if (type == TestType.Matching)
        {
            return View("RunMatching", model);
        }

        return View("Run", model);
    }

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] TestSubmissionViewModel submission)
    {
        if (submission == null)
        {
            return BadRequest(new { success = false, message = "Invalid submission data" });
        }

        try
        {
            var result = new TestResult
            {
                TestDate = DateTime.UtcNow,
                TestType = submission.Type,
                TotalQuestions = submission.TotalQuestions,
                CorrectAnswers = submission.CorrectAnswers,
                WrongAnswers = submission.WrongAnswers,
                Duration = TimeSpan.FromSeconds(submission.DurationSeconds)
            };

            await _testResultRepository.AddAsync(result);

            var redirectUrl = Url.Action("Result", "Tests", new { id = result.Id }) ?? $"/Tests/Result/{result.Id}";
            return Json(new { success = true, redirectUrl });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    public async Task<IActionResult> Result(int id)
    {
        var result = await _testResultRepository.GetByIdAsync(id);
        if (result == null) return NotFound();
        return View(result);
    }

    private List<string> GenerateOptions(Word correctWord, IEnumerable<Word> allWords)
    {
        var options = allWords
            .Where(w => w.Id != correctWord.Id)
            .OrderBy(x => Guid.NewGuid())
            .Take(3)
            .Select(w => w.Translation)
            .ToList();
        
        options.Add(correctWord.Translation);
        return options.OrderBy(x => Guid.NewGuid()).ToList();
    }
}

public class TestViewModel
{
    public TestType Type { get; set; }
    public List<TestQuestion> Questions { get; set; } = new();
}

public class TestQuestion
{
    public int WordId { get; set; }
    public string WordText { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
}

public class TestSubmissionViewModel
{
    public TestType Type { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public int DurationSeconds { get; set; }
}
