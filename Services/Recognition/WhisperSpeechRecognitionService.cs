using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using PodScribeX.Interfaces;

namespace PodScribeX.Services.Recognition;

/// <summary>
/// Whisper.cpp local implementation for speech recognition
/// </summary>
public class WhisperSpeechRecognitionService : ISpeechRecognitionService
{
    private readonly string _whisperExecutablePath;
    private readonly string _inputFolder;
    private readonly string _outputFolder;
    private readonly string _modelType;

    public WhisperSpeechRecognitionService(IConfiguration configuration)
    {
        // TODO -> refac : Convert to options class
        _whisperExecutablePath = configuration["WhisperSettings:ExePath"];
        _inputFolder = configuration["WhisperSettings:InputFolder"];
        _outputFolder = configuration["WhisperSettings:OutputFolder"];
        _modelType = configuration["WhisperSettings:Model"];
    }

    public string ServiceName => "OpenAI Whisper (Local)";

    public bool RequiresInternet => false;

    public async Task<string> TranscribeAudioAsync(string audioFileName)
    {
        if (!File.Exists(audioFileName))
        {
            throw new FileNotFoundException("Audio file not found", audioFileName);
        }

        try
        {
            string outputPath = Path.Combine(Path.GetTempPath(), $"whisper_output_{Guid.NewGuid()}.txt");

            var startInfo = new ProcessStartInfo
            {
                FileName = _whisperExecutablePath,
                Arguments = $"--model \"{_modelType}\" -f \"{_inputFolder}\\{audioFileName}\" -o \"{outputPath}\"",
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