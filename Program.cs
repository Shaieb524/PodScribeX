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
        Console.WriteLine("PodScribeX - Convert Videos to Text");
        Console.WriteLine("=====================================");

        if (args.Length < 1)
        {
            Console.WriteLine("Usage: PodScribeX.exe <videoPath> [outputPath]");
            return;
        }

        string videoPath = args[0];
        string outputPath = args.Length > 1 ? args[1] : Path.ChangeExtension(videoPath, ".wav");

        // Create audio extractor
        var audioExtractor = new FFmpegAudioExtractor("C:\\Users\\robin\\OneDrive\\Desktop\\dev\\Personal\\PodScribeX\\PodScribeX\\ThirdParties\\ffmpeg.exe");

        await audioExtractor.ExtractAudioAsync(videoPath, outputPath);

        Console.WriteLine("Doneeee");

    }
}