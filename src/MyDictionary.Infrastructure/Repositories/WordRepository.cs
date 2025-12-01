using Microsoft.EntityFrameworkCore;
using MyDictionary.Core.Entities;
using MyDictionary.Core.Interfaces;
using MyDictionary.Infrastructure.Data;

namespace MyDictionary.Infrastructure.Repositories;

public class WordRepository : IWordRepository
{
    private readonly ApplicationDbContext _context;

    public WordRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Word>> GetAllAsync()
    {
        return await _context.Words
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<Word?> GetByIdAsync(int id)
    {
        return await _context.Words.FindAsync(id);
    }

    public async Task<IEnumerable<Word>> GetByFilterAsync(
        Difficulty? difficulty = null,
        PartOfSpeech? partOfSpeech = null,
        string? languageFrom = null,
        string? languageTo = null,
        string? category = null,
        bool? isFavorite = null)
    {
        var query = _context.Words.AsQueryable();

        if (difficulty.HasValue)
            query = query.Where(w => w.Difficulty == difficulty.Value);

        if (partOfSpeech.HasValue)
            query = query.Where(w => w.PartOfSpeech == partOfSpeech.Value);

        if (!string.IsNullOrEmpty(languageFrom))
            query = query.Where(w => w.LanguageFrom == languageFrom);

        if (!string.IsNullOrEmpty(languageTo))
            query = query.Where(w => w.LanguageTo == languageTo);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(w => w.Category == category);

        if (isFavorite.HasValue)
            query = query.Where(w => w.IsFavorite == isFavorite.Value);

        return await query.OrderByDescending(w => w.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<Word>> GetDueForReviewAsync()
    {
        return await _context.Words
            .Where(w => w.NextReviewDate <= DateTime.UtcNow)
            .OrderBy(w => w.NextReviewDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Word>> GetRandomWordsAsync(int count)
    {
        // Get all IDs first (lightweight)
        var allIds = await _context.Words.Select(w => w.Id).ToListAsync();
        
        if (!allIds.Any()) return new List<Word>();

        // Shuffle IDs in memory
        var random = new Random();
        var randomIds = allIds.OrderBy(x => random.Next()).Take(count).ToList();

        // Fetch the actual words
        return await _context.Words
            .Where(w => randomIds.Contains(w.Id))
            .ToListAsync();
    }

    public async Task<Word> AddAsync(Word word)
    {
        _context.Words.Add(word);
        await _context.SaveChangesAsync();
        return word;
    }

    public async Task UpdateAsync(Word word)
    {
        _context.Entry(word).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var word = await _context.Words.FindAsync(id);
        if (word != null)
        {
            _context.Words.Remove(word);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Words.CountAsync();
    }

    public async Task<int> GetMasteredCountAsync()
    {
        return await _context.Words
            .CountAsync(w => w.ReviewCount >= 5 && 
                           (w.CorrectAnswers + w.WrongAnswers) > 0 &&
                           ((double)w.CorrectAnswers / (w.CorrectAnswers + w.WrongAnswers)) >= 0.8);
    }

    public async Task<Dictionary<Difficulty, int>> GetCountByDifficultyAsync()
    {
        return await _context.Words
            .GroupBy(w => w.Difficulty)
            .Select(g => new { Difficulty = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Difficulty, x => x.Count);
    }

    public async Task<Dictionary<PartOfSpeech, int>> GetCountByPartOfSpeechAsync()
    {
        return await _context.Words
            .GroupBy(w => w.PartOfSpeech)
            .Select(g => new { PartOfSpeech = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PartOfSpeech, x => x.Count);
    }

    public async Task<IEnumerable<Word>> SearchAsync(string searchTerm)
    {
        return await _context.Words
            .Where(w => w.WordText.Contains(searchTerm) || 
                       w.Translation.Contains(searchTerm) ||
                       (w.Notes != null && w.Notes.Contains(searchTerm)))
            .ToListAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Word> words)
    {
        _context.Words.AddRange(words);
        await _context.SaveChangesAsync();
    }
}
