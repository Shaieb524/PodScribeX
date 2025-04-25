
namespace PodScribeX.Interfaces;

/// <summary>
/// Interface for speech recognition services
/// </summary>
public interface ISpeechRecognitionService
{
    /// <summary>
    /// Transcribes audio file to text
    /// </summary>
    /// <param name="audioFilePath">Path to the audio file</param>
    /// <returns>Transcribed text</returns>
    Task<string> TranscribeAudioAsync(string audioFilePath, string? outputFilePath);

    /// <summary>
    /// Gets the name of the recognition service
    /// </summary>
    string ServiceName { get; }

    /// <summary>
    /// Indicates whether this service requires internet connection
    /// </summary>
    bool RequiresInternet { get; }
}