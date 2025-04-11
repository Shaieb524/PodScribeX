
namespace PodScribeX.Interfaces;

/// <summary>
/// Interface for audio extraction from video
/// </summary>
public interface IAudioExtractor
{
    /// <summary>
    /// Extracts audio from a video file
    /// </summary>
    /// <param name="videoFilePath">Path to the video file</param>
    /// <param name="outputAudioPath">Path where audio will be saved</param>
    /// <returns>True if extraction was successful</returns>
    Task<bool> ExtractAudioAsync(string videoFilePath, string outputAudioPath);
}