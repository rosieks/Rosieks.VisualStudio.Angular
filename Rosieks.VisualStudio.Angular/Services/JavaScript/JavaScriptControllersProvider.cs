using Rosieks.VisualStudio.Angular.Extensions;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rosieks.VisualStudio.Angular.Services.JavaScript
{
    [Export(typeof(IHierarchyElementsProvider<NgController>))]
    class JavaScriptControllersProvider : IHierarchyElementsProvider<NgController>
    {
        public IReadOnlyList<NgController> GetElements(NgHierarchy hierarchy)
        {
            return FindControllers(hierarchy.RootPath);
        }

        private static IReadOnlyList<NgController> FindControllers(string rootPath)
        {
            return Directory
                .EnumerateFiles(rootPath, "*.js", SearchOption.AllDirectories)
                .SelectMany(FindControllersInFile)
                .ToReadOnlyList();
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
