namespace MyDictionary.Core.Interfaces;

public interface IAIService
{
    Task<string> GenerateExampleSentenceAsync(string word, string translation, string partOfSpeech, string languageFrom = "en", string languageTo = "ar");
}
