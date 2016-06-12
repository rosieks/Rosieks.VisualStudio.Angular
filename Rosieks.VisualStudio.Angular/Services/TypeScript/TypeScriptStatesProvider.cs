using Rosieks.VisualStudio.Angular.Extensions;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rosieks.VisualStudio.Angular.Services.TypeScript
{
    [Export(typeof(IHierarchyElementsProvider<NgState>))]
    class TypeScriptStatesProvider : IHierarchyElementsProvider<NgState>
    {
        public IReadOnlyList<NgState> GetElements(NgHierarchy hierarchy)
        {
            return FindStates(hierarchy.RootPath);
        }

        private static IReadOnlyList<NgState> FindStates(string rootPath)
        {
            return Directory
                .EnumerateFiles(rootPath, "*.ts", SearchOption.AllDirectories)
                .IsValidPath()
                .SelectMany(FindStatesInFile)
                .ToReadOnlyList();
        }

        private static IEnumerable<NgState> FindStatesInFile(string file)
        {
            var content = File.ReadAllText(file);
            var regex = new Regex(@"\.state\(\s*(?>\'|\"")([^\'\""]+)");
            return regex.Matches(content).Cast<Match>().Select(x => new NgState
            {
                Name = x.Groups[1].Value,
            });
        }
    }
}
