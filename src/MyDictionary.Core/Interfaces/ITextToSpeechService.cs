namespace MyDictionary.Core.Interfaces;

public interface ITextToSpeechService
{
    Task SpeakAsync(string text, string? languageCode = null);
    Task StopAsync();
    bool IsSpeaking { get; }
    
    // For web application
    Task<byte[]> GenerateSpeechAsync(string text, string language);
    string GetAudioUrl(string text, string language);
}
