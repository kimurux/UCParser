using System.IO;
using System.Threading.Tasks;

namespace UCParser.Utils
{
    public static class FileSystemHelper
    {
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string GetSafeFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }

            return fileName
                .Replace('<', '_')
                .Replace('>', '_')
                .Replace(':', '_')
                .Replace(',', '_')
                .Replace('`', '_');
        }

        public static async Task WriteAllTextAsync(string path, string contents)
        {
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                await writer.WriteAsync(contents);
            }
        }
    }
}
