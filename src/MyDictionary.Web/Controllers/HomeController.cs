using Microsoft.AspNetCore.Mvc;
using MyDictionary.Core.Interfaces;
using MyDictionary.Web.Models;

namespace MyDictionary.Web.Controllers;

public class HomeController : Controller
{
    private readonly IWordRepository _wordRepository;
    private readonly ITestResultRepository _testResultRepository;

    public HomeController(IWordRepository wordRepository, ITestResultRepository testResultRepository)
    {
        _wordRepository = wordRepository;
        _testResultRepository = testResultRepository;
    }

    public async Task<IActionResult> Index()
    {
        var dashboard = new DashboardViewModel
        {
            TotalWords = await _wordRepository.GetTotalCountAsync(),
            MasteredWords = await _wordRepository.GetMasteredCountAsync(),
            DueForReview = (await _wordRepository.GetDueForReviewAsync()).Count(),
            RecentTestResults = await _testResultRepository.GetRecentResultsAsync(5),
            WordsByDifficulty = await _wordRepository.GetCountByDifficultyAsync(),
            WordsByPartOfSpeech = await _wordRepository.GetCountByPartOfSpeechAsync()
        };

        return View(dashboard);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
