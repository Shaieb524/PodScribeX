using System.Diagnostics;
using PodScribeX.Interfaces;

namespace PodScribeX.Services.Recognition;

/// <summary>
/// Implementation for extracting existing subtitles from videos
/// </summary>
public class SubtitleExtractionService : ISpeechRecognitionService
{
    private readonly string _ffmpegPath;

    public SubtitleExtractionService(string ffmpegPath = "ffmpeg")
    {
        _ffmpegPath = ffmpegPath;
    }

    public string ServiceName => "Subtitle Extraction";

    public bool RequiresInternet => false;

    public async Task<string> TranscribeAudioAsync(string audioFilePath)
    {
        // This method isn't actually used for subtitle extraction since we work with the video directly
        // However, we need to implement it as part of the interface
        return "This method should not be called for subtitle extraction";
    }
    public Task<string> TranscribeAudioAsync(string audioFilePath, string? outputFilePath)
    {
        throw new NotImplementedException();
    }

    public async Task<string> ExtractSubtitlesFromVideoAsync(string videoFilePath)
    {
        if (!File.Exists(videoFilePath))
        {
            throw new FileNotFoundException("Video file not found", videoFilePath);
        }

        string srtPath = Path.Combine(Path.GetTempPath(), $"subtitles_{Guid.NewGuid()}.srt");

        try
        {
            // Extract subtitles if they exist
            var startInfo = new ProcessStartInfo
            {
                FileName = _ffmpegPath,
                Arguments = $"-i \"{videoFilePath}\" -map 0:s:0 \"{srtPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            };

            using var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Failed to start FFmpeg process");

            await process.WaitForExitAsync();

            if (File.Exists(srtPath))
            {
                // Process SRT file to extract just the text without timestamps
                string subtitleText = await File.ReadAllTextAsync(srtPath);
                string cleanedText = CleanSubtitleText(subtitleText);
                return cleanedText;
            }
            else
            {
                return "No subtitles found in the video";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting subtitles: {ex.Message}");
            return $"Error: {ex.Message}";
        }
        finally
        {
            if (File.Exists(srtPath))
            {
                File.Delete(srtPath);
            }
        }
    }

    private static string CleanSubtitleText(string srtContent)
    {
        // Very basic SRT cleaning - remove numbers and timestamps
        var lines = srtContent.Split('\n');
        var textLines = new List<string>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            // Skip empty lines, numbers, and timestamp lines
            if (string.IsNullOrWhiteSpace(line) ||
                int.TryParse(line, out _) ||
                line.Contains("-->"))
            {
                continue;
            }

            textLines.Add(line);
        }

        return string.Join("\n", textLines);
    }


}