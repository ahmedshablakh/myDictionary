using MyDictionary.Core.Entities;

namespace MyDictionary.Core.Interfaces;

public interface IWordRepository
{
    Task<IEnumerable<Word>> GetAllAsync();
    Task<Word?> GetByIdAsync(int id);
    Task<IEnumerable<Word>> GetByFilterAsync(
        Difficulty? difficulty = null, 
        PartOfSpeech? partOfSpeech = null,
        string? languageFrom = null,
        string? languageTo = null,
        string? category = null,
        bool? isFavorite = null);
    Task<IEnumerable<Word>> GetDueForReviewAsync();
    Task<IEnumerable<Word>> GetRandomWordsAsync(int count);
    Task<Word> AddAsync(Word word);
    Task UpdateAsync(Word word);
    Task DeleteAsync(int id);
    Task<int> GetTotalCountAsync();
    Task<int> GetMasteredCountAsync();
    Task<Dictionary<Difficulty, int>> GetCountByDifficultyAsync();
    Task<Dictionary<PartOfSpeech, int>> GetCountByPartOfSpeechAsync();
    Task<IEnumerable<Word>> SearchAsync(string searchTerm);
    Task AddRangeAsync(IEnumerable<Word> words);
}
