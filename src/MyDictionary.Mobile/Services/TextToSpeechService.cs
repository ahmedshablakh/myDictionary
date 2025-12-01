using System.Globalization;
using MyDictionary.Core.Interfaces;

namespace MyDictionary.Mobile.Services;

public class TextToSpeechService : ITextToSpeechService
{
    private bool _isSpeaking;

    public bool IsSpeaking => _isSpeaking;

    public async Task SpeakAsync(string text, string? languageCode = null)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        try
        {
            _isSpeaking = true;

            var locales = await TextToSpeech.Default.GetLocalesAsync();
            
            // Try to find the locale based on language code
            Locale? selectedLocale = null;
            
            if (!string.IsNullOrEmpty(languageCode))
            {
                // Map common language names to locale codes
                var localeCode = MapLanguageToLocale(languageCode);
                selectedLocale = locales.FirstOrDefault(l => 
                    l.Language.StartsWith(localeCode, StringComparison.OrdinalIgnoreCase));
            }

            var options = new SpeechOptions
            {
                Pitch = 1.0f,
                Volume = 1.0f,
                Locale = selectedLocale
            };

            await TextToSpeech.Default.SpeakAsync(text, options);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"TTS Error: {ex.Message}");
        }
        finally
        {
            _isSpeaking = false;
        }
    }

    public async Task StopAsync()
    {
        try
        {
            await TextToSpeech.Default.SpeakAsync(string.Empty);
            _isSpeaking = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"TTS Stop Error: {ex.Message}");
        }
    }

    private string MapLanguageToLocale(string language)
    {
        return language.ToLower() switch
        {
            "english" => "en",
            "arabic" => "ar",
            "spanish" => "es",
            "french" => "fr",
            "german" => "de",
            "italian" => "it",
            "portuguese" => "pt",
            "russian" => "ru",
            "chinese" => "zh",
            "japanese" => "ja",
            "korean" => "ko",
            _ => "en"
        };
    }

    // Web application methods (not implemented for mobile)
    public Task<byte[]> GenerateSpeechAsync(string text, string language)
    {
        throw new NotImplementedException("Use SpeakAsync for mobile applications");
    }

    public string GetAudioUrl(string text, string language)
    {
        throw new NotImplementedException("Use SpeakAsync for mobile applications");
    }
}
