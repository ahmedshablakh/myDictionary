using Microsoft.AspNetCore.Mvc;
using MyDictionary.Core.Entities;
using MyDictionary.Core.Interfaces;
using MyDictionary.Core.Services;

namespace MyDictionary.Web.Controllers;

public class FlashcardsController : Controller
{
    private readonly IWordRepository _wordRepository;
    private readonly SRSService _srsService;
    private readonly IUserSettingsRepository _userSettingsRepository;
    private readonly ITextToSpeechService _ttsService;

    public FlashcardsController(
        IWordRepository wordRepository,
        SRSService srsService,
        IUserSettingsRepository userSettingsRepository,
        ITextToSpeechService ttsService)
    {
        _wordRepository = wordRepository;
        _srsService = srsService;
        _userSettingsRepository = userSettingsRepository;
        _ttsService = ttsService;
    }

    public async Task<IActionResult> Index(bool practiceMode = false)
    {
        IEnumerable<Word> words;

        if (practiceMode)
        {
            // Get 20 random words for practice
            words = await _wordRepository.GetRandomWordsAsync(20);
            ViewBag.IsPracticeMode = true;
            ViewBag.Message = "Practice Mode: Reviewing random words.";
        }
        else
        {
            // Get words due for review
            words = await _wordRepository.GetDueForReviewAsync();
            ViewBag.IsPracticeMode = false;
            
            if (!words.Any())
            {
                ViewBag.Message = "Great job! You're all caught up for today.";
            }
        }

        return View(words);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitReview(int id, Difficulty difficulty)
    {
        var word = await _wordRepository.GetByIdAsync(id);
        if (word == null) return NotFound();

        // Update word's difficulty level based on user selection
        word.Difficulty = difficulty;

        // Update word stats using SRS service
        _srsService.ReviewWord(word, difficulty);
        
        // Record correctness (assuming Easy/Medium is correct, Hard might be struggle)
        // This is a simplification, usually we'd ask "Did you know it?"
        _srsService.RecordAnswer(word, difficulty != Difficulty.Hard);

        await _wordRepository.UpdateAsync(word);

        return Json(new { success = true, nextReview = word.NextReviewDate });
    }
    
    [HttpGet]
    public IActionResult GetAudioUrl(string text, string lang)
    {
        var url = _ttsService.GetAudioUrl(text, lang);
        return Json(new { url });
    }
}
