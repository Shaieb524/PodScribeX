
namespace VideoScribeX.Interfaces
{
    public interface IAudioExtractor
    {
        Task<bool> ExtractAudioAsync(string videoFilePath, string outputAudioPath);
    }
}
