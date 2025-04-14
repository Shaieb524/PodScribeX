using Microsoft.Extensions.Configuration;
using PodScribeX.Interfaces;
using PodScribeX.Services;
using PodScribeX.Services.Extractors;
using PodScribeX.Services.Recognition;

namespace PodScribeX;

/// <summary>
/// Main program entry point for PodScribeX
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Parse command line arguments
        string videoPath = args.Length > 0 ? args[0] : "the-lean-startup-review-eric-ries.mp4";
        string outputPath = args.Length > 1 ? args[1] : Path.ChangeExtension(videoPath, ".txt");

        // Create extractor with configuration
        var audioExtractor = new FFmpegAudioExtractor(configuration);

        // Create a speech recognition service (you need to implement or choose one)
        ISpeechRecognitionService speechService = new SubtitleExtractionService(); // Replace with appropriate implementation

        // Create the transcription service
        var transcriptionService = new VideoTranscriptionService(audioExtractor, speechService);

        Console.WriteLine($"Processing video: {videoPath}");

        try
        {
            // Transcribe the video directly (this handles extraction internally)
            string transcript = await transcriptionService.TranscribeVideoAsync(videoPath);

            // Save the transcript
            await transcriptionService.SaveTranscriptToFileAsync(transcript, outputPath);

            Console.WriteLine($"Transcription completed successfully. Output saved to {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during transcription: {ex.Message}");
        }
    }
}