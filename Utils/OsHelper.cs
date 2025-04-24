
namespace PodScribeX.Utils
{
    public static class OsHelper
    {
        public static string GetMediaAudioPath()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            return Path.Combine(projectRoot, "Media", "Audio");
        }

        public static string GetMediaScriptPath()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            return Path.Combine(projectRoot, "Media", "Script");
        }
    }
}
