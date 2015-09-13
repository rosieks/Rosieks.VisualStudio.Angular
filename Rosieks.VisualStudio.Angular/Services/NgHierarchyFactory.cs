using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rosieks.VisualStudio.Angular.Services
{
    internal class NgHierarchyFactory
    {
        public static NgHierarchy Find(string path)
        {
            int index = path.LastIndexOf("app");
            if (index > -1)
            {
                var rootPath = path.Substring(0, index);

                return new NgHierarchy()
                {
                    RootPath = rootPath,
                    Controllers = new Lazy<IReadOnlyList<NgController>>(() => FindControllers(rootPath)),
                };
            }
            else
            {
                return null;
            }
        }

        private static IReadOnlyList<NgController> FindControllers(string rootPath)
        {
            return new ReadOnlyCollection<NgController>(
                Directory
                    .EnumerateFiles(rootPath, "*.js", SearchOption.AllDirectories)
                    .SelectMany(FindControllersInFile)
                    .ToArray());
        }

        private static IEnumerable<NgController> FindControllersInFile(string file)
        {
            var content = File.ReadAllText(file);
            var regex = new Regex(@"\.controller\(\s*(?>\'|\"")([^\'\""]+)");
            return regex.Matches(content).Cast<Match>().Select(x => new NgController
            {
                Name = x.Groups[1].Value,
                Path = file
            });
        }
    }
}
