using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PodScribeX.Interfaces;

namespace PodScribeX.Services.Extractors
{
    public class FFmpegAudioExtractor : IAudioExtractor
    {
        private readonly string _ffmpegPath;
        private readonly string _inputFolder;
        private readonly string _outputFolder;
        private readonly string _audioCodec;
        private readonly int _sampleRate;
        private readonly int _channels;

        public event EventHandler<string> ProgressUpdated;

        public FFmpegAudioExtractor(IConfiguration configuration)
        {
            // TODO -> refac : Convert to options class 
            _ffmpegPath = configuration["FFmpegSettings:ExePath"];
            _inputFolder = configuration["FFmpegSettings:InputFolder"];
            _outputFolder = configuration["FFmpegSettings:OutputFolder"];
            _audioCodec = configuration["FFmpegSettings:AudioSettings:Codec"];
            _sampleRate = int.Parse(configuration["FFmpegSettings:AudioSettings:SampleRate"]);
            _channels = int.Parse(configuration["FFmpegSettings:AudioSettings:Channels"]);

            Directory.CreateDirectory(_outputFolder); // Ensure output directory exists
        }

        public async Task<bool> ExtractAudioAsync(string videoFileName, string outputAudioFileName = null)
        {
            // Notify progress
            OnProgressUpdated("Starting audio extraction process...");
            
            string videoFilePath = Path.Combine(_inputFolder, videoFileName);

            if (!File.Exists(videoFilePath))
            {
                OnProgressUpdated($"Error: Video file not found at {videoFilePath}");
                throw new FileNotFoundException("Video file not found", videoFilePath);
            }

            // If no output filename provided, use the same name but with .wav extension
            if (string.IsNullOrEmpty(outputAudioFileName))
            {
                outputAudioFileName = Path.GetFileNameWithoutExtension(videoFileName) + ".wav";
            }

            string outputAudioPath = Path.Combine(_outputFolder, outputAudioFileName);
            OnProgressUpdated($"Output audio will be saved to: {outputAudioPath}");

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = $"-i \"{videoFilePath}\" -vn -acodec {_audioCodec} -ar {_sampleRate} -ac {_channels} \"{outputAudioPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                OnProgressUpdated("Starting FFmpeg process...");
                
                using var process = Process.Start(startInfo)
                    ?? throw new InvalidOperationException("Failed to start FFmpeg process");
                
                // Capture and report FFmpeg output
                process.ErrorDataReceived += (sender, args) => 
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        OnProgressUpdated($"FFmpeg: {args.Data}");
                    }
                };
                
                process.BeginErrorReadLine();
                
                // Wait for process completion
                await process.WaitForExitAsync();
                
                bool success = process.ExitCode == 0 && File.Exists(outputAudioPath);
                
                if (success)
                {
                    OnProgressUpdated("Audio extraction completed successfully!");
                }
                else
                {
                    OnProgressUpdated($"Audio extraction failed with exit code: {process.ExitCode}");
                }
                
                return success;
            }
            catch (Exception ex)
            {
                OnProgressUpdated($"Error extracting audio: {ex.Message}");
                return false;
            }
        }
        
        // Raise the progress event (publish event)
        protected virtual void OnProgressUpdated(string message)
        {
            ProgressUpdated?.Invoke(this, message);
            Console.WriteLine(message); 
        }
    }
}