using MyDictionary.Core.Interfaces;
using System.Text;
using System.Text.Json;

namespace MyDictionary.Infrastructure.Services;

/// <summary>
/// Translation service using MyMemory Translation API (Free)
/// Alternative: Google Translate API, DeepL API
/// </summary>
public class TranslationService : ITranslationService
{
    private readonly HttpClient _httpClient;
    private const string API_URL = "https://api.mymemory.translated.net/get";

    public TranslationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> TranslateAsync(string text, string fromLanguage, string toLanguage)
    {
        // Try Google Translate (client5) first as it's often more reliable for simple words
        try
        {
            var googleUrl = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={Uri.EscapeDataString(text)}";
            var response = await _httpClient.GetAsync(googleUrl);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(content);
                var root = jsonDoc.RootElement;
                
                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var firstPart = root[0];
                    if (firstPart.ValueKind == JsonValueKind.Array && firstPart.GetArrayLength() > 0)
                    {
                        var translation = firstPart[0][0].GetString();
                        if (!string.IsNullOrEmpty(translation))
                        {
                            return translation;
                        }
                    }
                }
            }
        }
        catch
        {
            // Fallback to MyMemory
        }

        // Fallback: MyMemory API
        try
        {
            var url = $"{API_URL}?q={Uri.EscapeDataString(text)}&langpair={fromLanguage}|{toLanguage}";
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(content);
                
                if (jsonDoc.RootElement.TryGetProperty("responseData", out var responseData))
                {
                    if (responseData.TryGetProperty("translatedText", out var translatedText))
                    {
                        return translatedText.GetString() ?? text;
                    }
                }
            }
            
            return text; // Return original text if translation fails
        }
        catch
        {
            return text; // Return original text on error
        }
    }

    public async Task<bool> IsServiceAvailableAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{API_URL}?q=test&langpair=en|ar");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
