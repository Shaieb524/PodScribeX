using System.Diagnostics;
using PodScribeX.Interfaces;

namespace PodScribeX.Services.Recognition;

/// <summary>
/// Whisper.cpp local implementation for speech recognition
/// </summary>
public class WhisperSpeechRecognitionService : ISpeechRecognitionService
{
    private readonly string _modelPath;
    private readonly string _whisperExecutablePath;

    public WhisperSpeechRecognitionService(string whisperExecutablePath, string modelPath)
    {
        _whisperExecutablePath = whisperExecutablePath ?? throw new ArgumentNullException(nameof(whisperExecutablePath));
        _modelPath = modelPath ?? throw new ArgumentNullException(nameof(modelPath));
    }

    public string ServiceName => "OpenAI Whisper (Local)";

    public bool RequiresInternet => false;

    public async Task<string> TranscribeAudioAsync(string audioFilePath)
    {
        if (!File.Exists(audioFilePath))
        {
            throw new FileNotFoundException("Audio file not found", audioFilePath);
        }

        try
        {
            string outputPath = Path.Combine(Path.GetTempPath(), $"whisper_output_{Guid.NewGuid()}.txt");

            var startInfo = new ProcessStartInfo
            {
                FileName = _whisperExecutablePath,
                Arguments = $"-m \"{_modelPath}\" -f \"{audioFilePath}\" -o \"{outputPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            using var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Failed to start Whisper process");

            await process.WaitForExitAsync();

            if (File.Exists(outputPath))
            {
                string result = await File.ReadAllTextAsync(outputPath);
                File.Delete(outputPath);
                return result;
            }

            return "Failed to get output from Whisper";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during Whisper transcription: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }
}