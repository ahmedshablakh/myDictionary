using Microsoft.AspNetCore.Mvc;
using MyDictionary.Core.Entities;
using MyDictionary.Core.Interfaces;
using MyDictionary.Infrastructure.Services;

namespace MyDictionary.Web.Controllers;

public class WordsController : Controller
{
    private readonly IWordRepository _wordRepository;
    private readonly ITranslationService _translationService;
    private readonly IAIService _aiService;
    private readonly PdfGeneratorService _pdfGeneratorService;
    private readonly IUserSettingsRepository _userSettingsRepository;

    public WordsController(
        IWordRepository wordRepository,
        ITranslationService translationService,
        IAIService aiService,
        PdfGeneratorService pdfGeneratorService,
        IUserSettingsRepository userSettingsRepository)
    {
        _wordRepository = wordRepository;
        _translationService = translationService;
        _aiService = aiService;
        _pdfGeneratorService = pdfGeneratorService;
        _userSettingsRepository = userSettingsRepository;
    }

    public async Task<IActionResult> Index(string search, Difficulty? difficulty, PartOfSpeech? type)
    {
        IEnumerable<Word> words;

        if (!string.IsNullOrEmpty(search))
        {
            words = await _wordRepository.SearchAsync(search);
        }
        else
        {
            words = await _wordRepository.GetByFilterAsync(difficulty, type);
        }

        ViewBag.Search = search;
        ViewBag.Difficulty = difficulty;
        ViewBag.Type = type;

        return View(words);
    }

    public async Task<IActionResult> Create()
    {
        var settings = await _userSettingsRepository.GetSettingsAsync();
        return View(new Word 
        { 
            LanguageFrom = settings.DefaultLanguageFrom,
            LanguageTo = settings.DefaultLanguageTo
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Word word)
    {
        if (ModelState.IsValid)
        {
            await _wordRepository.AddAsync(word);
            return RedirectToAction(nameof(Index));
        }
        return View(word);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var word = await _wordRepository.GetByIdAsync(id);
        if (word == null) return NotFound();
        return View(word);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Word word)
    {
        if (id != word.Id) return NotFound();

        if (ModelState.IsValid)
        {
            await _wordRepository.UpdateAsync(word);
            return RedirectToAction(nameof(Index));
        }
        return View(word);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _wordRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetTranslation(string text, string from, string to)
    {
        if (string.IsNullOrWhiteSpace(text)) return BadRequest();
        
        var translation = await _translationService.TranslateAsync(text, from, to);
        return Json(new { translation });
    }

    [HttpGet]
    public async Task<IActionResult> GenerateExample(string word, string translation, string partOfSpeech, string languageFrom = "en", string languageTo = "ar")
    {
        if (string.IsNullOrWhiteSpace(word)) return BadRequest();
        
        var example = await _aiService.GenerateExampleSentenceAsync(word, translation, partOfSpeech, languageFrom, languageTo);
        return Json(new { example });
    }

    [HttpPost]
    public async Task<IActionResult> GenerateMissingExamples()
    {
        var words = await _wordRepository.GetAllAsync();
        var wordsToUpdate = words.Where(w => string.IsNullOrWhiteSpace(w.ExampleSentence)).ToList();
        
        int count = 0;
        foreach (var word in wordsToUpdate)
        {
            try 
            {
                var example = await _aiService.GenerateExampleSentenceAsync(
                    word.WordText, 
                    word.Translation, 
                    word.PartOfSpeech.ToString(), 
                    word.LanguageFrom, 
                    word.LanguageTo
                );
                
                word.ExampleSentence = example;
                await _wordRepository.UpdateAsync(word);
                count++;
            }
            catch
            {
                // Continue if one fails
                continue;
            }
        }

        TempData["Success"] = $"Successfully generated examples for {count} words.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ExportPdf()
    {
        var words = await _wordRepository.GetAllAsync();
        var pdfBytes = _pdfGeneratorService.GenerateWordListPdf(words, "All Vocabulary");
        
        return File(pdfBytes, "application/pdf", $"MyDictionary_Words_{DateTime.Now:yyyyMMdd}.pdf");
    }

    public IActionResult ImportCsv()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a valid CSV file.";
            return View();
        }

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Only CSV files are supported.";
            return View();
        }

        try
        {
            var words = new List<Word>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                // Skip header line
                await reader.ReadLineAsync();
                
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var values = line.Split(',');
                    if (values.Length < 2) continue; // Need at least word and translation

                    var word = new Word
                    {
                        WordText = values[0].Trim(),
                        Translation = values[1].Trim(),
                        PartOfSpeech = values.Length > 2 && Enum.TryParse<PartOfSpeech>(values[2].Trim(), out var pos) 
                            ? pos 
                            : PartOfSpeech.Other,
                        Difficulty = values.Length > 3 && Enum.TryParse<Difficulty>(values[3].Trim(), out var diff) 
                            ? diff 
                            : Difficulty.Medium,
                        LanguageFrom = values.Length > 4 ? values[4].Trim() : "en",
                        LanguageTo = values.Length > 5 ? values[5].Trim() : "ar",
                        NextReviewDate = DateTime.UtcNow.AddDays(1)
                    };

                    words.Add(word);
                }
            }

            // Add all words to database
            foreach (var word in words)
            {
                await _wordRepository.AddAsync(word);
            }

            TempData["Success"] = $"Successfully imported {words.Count} words!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error importing file: {ex.Message}";
            return View();
        }
    }
}
