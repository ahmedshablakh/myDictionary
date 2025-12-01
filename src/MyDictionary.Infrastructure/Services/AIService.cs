using MyDictionary.Core.Interfaces;

namespace MyDictionary.Infrastructure.Services;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient;

    public AIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GenerateExampleSentenceAsync(
        string word, 
        string translation, 
        string partOfSpeech, 
        string languageFrom = "en", 
        string languageTo = "ar")
    {
        // For now, use simple templates
        // TODO: Integrate with OpenAI/Gemini API for better results
        
        var templates = partOfSpeech.ToLower() switch
        {
            "noun" => new[]
            {
                $"The {word} is very important.",
                $"I saw a beautiful {word} yesterday.",
                $"This {word} belongs to me."
            },
            "verb" => new[]
            {
                $"I {word} every day.",
                $"She will {word} tomorrow.",
                $"They {word} together."
            },
            "adjective" => new[]
            {
                $"The weather is {word} today.",
                $"She looks very {word}.",
                $"This is a {word} example."
            },
            "adverb" => new[]
            {
                $"He speaks {word}.",
                $"She works {word}.",
                $"They arrived {word}."
            },
            _ => new[]
            {
                $"This is an example with {word}.",
                $"I use {word} often.",
                $"The {word} is here."
            }
        };

        var random = new Random();
        var sentence = templates[random.Next(templates.Length)];
        
        // Simulate async operation
        await Task.Delay(100);
        
        return sentence;
    }
}
