using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rosieks.VisualStudio.Angular.Services
{
    internal class AppHierarchyFactory
    {
        public static AppHierarchy Find(string path)
        {
            int index = path.LastIndexOf("app");
            if (index > -1)
            {
                var rootPath = path.Substring(0, index);

                return new AppHierarchy()
                {
                    RootPath = rootPath,
                    Controllers = new Lazy<IReadOnlyList<string>>(() => FindControllers(rootPath)),
                };
            }
            else
            {
                return null;
            }
        }

        private static IReadOnlyList<string> FindControllers(string rootPath)
        {
            return new ReadOnlyCollection<string>(
                Directory
                    .EnumerateFiles(rootPath, "*.js", SearchOption.AllDirectories)
                    .SelectMany(FindControllersInFile)
                    .ToArray());
        }

        private static IEnumerable<string> FindControllersInFile(string file)
        {
            var content = File.ReadAllText(file);
            var regex = new Regex(@"\.controller\(\s*(?>\'|\"")([^\'\""]+)");
            return regex.Matches(content).Cast<Match>().Select(x => x.Groups[1].Value);
        }
    }
}
