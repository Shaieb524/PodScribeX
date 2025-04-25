using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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

    public event EventHandler<string> ProgressUpdated;

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

    public async Task<string> TranscribeAudioAsync(string audioFileName, string outputAudioFileName = null)
    {
        // Notify progress
        OnProgressUpdated("Starting audio transcription process...");

        string audioFilePath = Path.Combine(_inputFolder, audioFileName);

        if (!File.Exists(audioFilePath))
        {
            OnProgressUpdated($"Error: Audio file not found at {audioFilePath}");
            throw new FileNotFoundException("Audio file not found", audioFileName);
        }

        try
        {
            if (string.IsNullOrEmpty(outputAudioFileName))
            {
                outputAudioFileName = Path.GetFileNameWithoutExtension(audioFileName) + ".txt";
            }

            string outputFilePath = Path.Combine(_outputFolder, outputAudioFileName);
            OnProgressUpdated($"Output transcript will be saved to: {outputFilePath}");

            // For some stupid readon, the output folder must not have a trailing backslash coz it is adding quotation to the cmd 
            string clnOutputFolder = _outputFolder.TrimEnd('\\');

            var startInfo = new ProcessStartInfo
            {
                FileName = _whisperExecutablePath,
                Arguments = $"\"{audioFilePath}\" --model \"{_modelType}\" -o \"{clnOutputFolder}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            OnProgressUpdated("Starting Whisper transcription process...");

            using var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Failed to start Whisper process");

            // Capture and report Whisper output
            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    OnProgressUpdated($"Whisper: {args.Data}");
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    OnProgressUpdated($"Whisper Error: {args.Data}");
                }
            };

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Wait for process completion
            await process.WaitForExitAsync();

            bool success = process.ExitCode == 0 && File.Exists(outputFilePath);

            if (success)
            {
                OnProgressUpdated("Transcription completed successfully!");
                string result = await File.ReadAllTextAsync(outputFilePath);
                //File.Delete(outputFilePath);
                return result;
            }
            else
            {
                OnProgressUpdated($"Transcription failed with exit code: {process.ExitCode}");
                return "Failed to get output from Whisper";
            }
        }
        catch (Exception ex)
        {
            OnProgressUpdated($"Error during Whisper transcription: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }

    // Raise the progress event (publish event)
    protected virtual void OnProgressUpdated(string message)
    {
        ProgressUpdated?.Invoke(this, message);
        Console.WriteLine(message);
    }
}