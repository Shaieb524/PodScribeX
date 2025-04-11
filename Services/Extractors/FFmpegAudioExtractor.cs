using System.Diagnostics;
using PodScribeX.Interfaces;

namespace PodScribeX.Services.Extractors;

/// <summary>
/// Implements audio extraction using FFmpeg
/// </summary>
public class FFmpegAudioExtractor : IAudioExtractor
{
    private readonly string _ffmpegPath;

    public FFmpegAudioExtractor(string ffmpegPath = "ffmpeg")
    {
        _ffmpegPath = ffmpegPath;
    }

    public async Task<bool> ExtractAudioAsync(string videoFilePath, string outputAudioPath)
    {
        if (!File.Exists(videoFilePath))
        {
            throw new FileNotFoundException("Video file not found", videoFilePath);
        }

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _ffmpegPath,
                Arguments = $"-i \"{videoFilePath}\" -vn -acodec pcm_s16le -ar 44100 -ac 2 \"{outputAudioPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            };

            using var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Failed to start FFmpeg process");

            await process.WaitForExitAsync();

            return process.ExitCode == 0 && File.Exists(outputAudioPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting audio: {ex.Message}");
            return false;
        }
    }
}