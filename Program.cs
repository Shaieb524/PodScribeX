using Microsoft.Extensions.Configuration;
using PodScribeX.Interfaces;
using PodScribeX.Services;
using PodScribeX.Services.Extractors;
using PodScribeX.Services.Recognition;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

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

        Console.WriteLine("Welcome to PodScribeX!");
        Console.WriteLine("====================");
        Console.WriteLine("Please select an option:");
        Console.WriteLine("1. Audio to Script - Convert audio file to text");
        Console.WriteLine("2. Video to Script - Convert video file to text");
        Console.WriteLine("3. Video to Audio - Extract audio from video");
        Console.WriteLine("4. Video to Audio to Script - Convert video to audio and text");
        Console.Write("Enter your choice (1-4): ");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await ProcessAudioToScript(configuration);
                break;
            case "2":
                await ProcessVideoToScript(configuration);
                break;
            case "3":
                await ProcessVideoToAudio(configuration);
                break;
            case "4":
                await ProcessVideoToAudioToScript(configuration);
                break;
            default:
                Console.WriteLine("Invalid choice. Exiting...");
                break;
        }
    }

    private static async Task ProcessAudioToScript(IConfiguration configuration)
    {
        Console.WriteLine("\nAudio to Script Conversion");
        Console.WriteLine("------------------------");
        Console.WriteLine("Note: Audio files should be in the Media/Audio directory");
        Console.Write("Enter audio filename: ");
        
        string audioFile = Console.ReadLine();
        string audioPath = Path.Combine("Media", "Audio", audioFile);
        string outputPath = Path.Combine("Media", "Script", Path.ChangeExtension(audioFile, ".txt"));
        
        Console.WriteLine($"Processing audio file: {audioPath}");
        Console.WriteLine($"Output will be saved to: {outputPath}");
        Console.WriteLine("Not implemented yet. Coming soon!");
    }

    private static async Task ProcessVideoToScript(IConfiguration configuration)
    {
        Console.WriteLine("\nVideo to Script Conversion");
        Console.WriteLine("------------------------");
        Console.WriteLine("Note: Video files should be in the Media/Video directory");
        Console.Write("Enter video filename: ");
        
        string videoFile = Console.ReadLine();
        string videoPath = Path.Combine("Media", "Video", videoFile);
        string outputPath = Path.Combine("Media", "Script", Path.ChangeExtension(videoFile, ".txt"));
        
        Console.WriteLine($"Processing video file: {videoPath}");
        Console.WriteLine($"Output will be saved to: {outputPath}");
        Console.WriteLine("Not implemented yet. Coming soon!");
    }

    private static async Task ProcessVideoToAudio(IConfiguration configuration)
    {
        Console.WriteLine("\nVideo to Audio Conversion");
        Console.WriteLine("------------------------");
        Console.WriteLine("Note: Video files should be in the Media/Video directory");
        Console.Write("Enter video filename: ");
        
        string videoFile = Console.ReadLine();
        string videoPath = Path.Combine("Media", "Video", videoFile);
        string audioPath = Path.Combine("Media", "Audio", Path.ChangeExtension(videoFile, ".wav"));
        
        Console.WriteLine($"Processing video file: {videoPath}");
        Console.WriteLine($"Audio will be extracted to: {audioPath}");
        
        try
        {
            var audioExtractor = new FFmpegAudioExtractor(configuration);
            bool result = await audioExtractor.ExtractAudioAsync(videoFile);
            
            if (result)
            {
                Console.WriteLine($"Audio extraction completed successfully! Audio saved to: {audioPath}");
            }
            else
            {
                Console.WriteLine("Audio extraction failed. Please check the logs for details.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static async Task ProcessVideoToAudioToScript(IConfiguration configuration)
    {
        Console.WriteLine("\nVideo to Audio to Script Conversion");
        Console.WriteLine("----------------------------------");
        Console.WriteLine("Note: Video files should be in the Media/Video directory");
        Console.Write("Enter video filename: ");

        string videoFile = Console.ReadLine();
        string videoPath = Path.Combine("Media", "Video", videoFile);
        string audioPath = Path.Combine("Media", "Audio", Path.ChangeExtension(videoFile, ".wav"));
        string outputPath = Path.Combine("Media", "Script", Path.ChangeExtension(videoFile, ".txt"));

        Console.WriteLine($"Processing video file: {videoPath}");
        Console.WriteLine($"Audio will be extracted to: {audioPath}");
        Console.WriteLine($"Script will be saved to: {outputPath}");
        Console.WriteLine("Not implemented yet. Coming soon!");
    }


}