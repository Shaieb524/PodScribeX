
namespace VideoScribeX.Interfaces
{
    public interface ISpeechRecognitionService
    {
        Task<string> TranscribeAudioAsync(string audioFilePath);

        string ServiceName { get; }

        bool RequiresInternet { get; }
    }
}
