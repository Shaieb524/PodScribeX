using PodScribeX.Interfaces;
using PodScribeX.Utils;

namespace PodScribeX.Services;

/// <summary>
/// Main transcription service that coordinates the process
/// </summary>
public class VideoTranscriptionService
{
    private readonly IAudioExtractor _audioExtractor;
    private readonly ISpeechRecognitionService _speechRecognitionService;

    public VideoTranscriptionService(IAudioExtractor audioExtractor, ISpeechRecognitionService speechRecognitionService)
    {
        _audioExtractor = audioExtractor ?? throw new ArgumentNullException(nameof(audioExtractor));
        _speechRecognitionService = speechRecognitionService ?? throw new ArgumentNullException(nameof(speechRecognitionService));
    }

    public async Task<string> TranscribeVideoAsync(string videoFilePath, string? tempAudioPath = null)
    {
        // Create temporary audio file path if not provided
        tempAudioPath ??= Path.Combine(OsHelper.GetMediaAudioPath(), $"audio_{Guid.NewGuid()}.wav");

        try
        {
            Console.WriteLine($"Extracting audio from video using {_audioExtractor.GetType().Name}...");

            // Extract audio from the video
            bool extractionSuccess = await _audioExtractor.ExtractAudioAsync(videoFilePath, tempAudioPath);
            if (!extractionSuccess)
            {
                throw new Exception("Failed to extract audio from video");
            }

            Console.WriteLine($"Transcribing audio using {_speechRecognitionService.ServiceName}...");

            // Transcribe the audio
            string transcript = await _speechRecognitionService.TranscribeAudioAsync(tempAudioPath, null);

            return transcript;
        }
        finally
        {
            // Clean up temporary audio file
            if (File.Exists(tempAudioPath))
            {
                File.Delete(tempAudioPath);
                Console.WriteLine("Temporary audio file deleted");
            }
        }
    }

    public async Task SaveTranscriptToFileAsync(string transcript, string outputFilePath)
    {
        await File.WriteAllTextAsync(outputFilePath, transcript);
        Console.WriteLine($"Transcript saved to {outputFilePath}");
    }

}