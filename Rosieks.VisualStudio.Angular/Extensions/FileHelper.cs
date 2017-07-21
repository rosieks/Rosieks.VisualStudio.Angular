namespace Rosieks.VisualStudio.Angular.Extensions
{
    using System.IO;
    internal class FileHelper
    {
        internal static bool TryFind(string[] paths, string[] extensions, out string fileName)
        {
            foreach (var path in paths)
            {
                if (TryFind(path, extensions, out fileName))
                {
                    return true;
                };
            }

            fileName = null;
            return false;
        }

        internal static bool TryFind(string path, string[] extensions, out string fileName)
        {
            foreach (var extension in extensions)
            {
                fileName = path + extension;
                if (File.Exists(fileName))
                {
                    return true;
                }
            }

            fileName = null;
            return false;
        }
    }
}
