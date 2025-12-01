namespace MyDictionary.Core.Interfaces;

public interface ITranslationService
{
    Task<string> TranslateAsync(string text, string fromLanguage, string toLanguage);
    Task<bool> IsServiceAvailableAsync();
}
