namespace Rosieks.VisualStudio.Angular.Services.TypeScript
{
    using Rosieks.VisualStudio.Angular.Extensions;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    [Export(typeof(IHierarchyElementsProvider<NgController>))]
    class TypeScriptControllersProvider : IHierarchyElementsProvider<NgController>
    {
        public IReadOnlyList<NgController> GetElements(NgHierarchy hierarchy)
        {
            return FindControllers(hierarchy.RootPath);
        }

        private static IReadOnlyList<NgController> FindControllers(string rootPath)
        {
            return Directory
                .EnumerateFiles(rootPath, "*.ts", SearchOption.AllDirectories)
                .IsValidPath()
                .SelectMany(FindControllersInFile)
                .ToReadOnlyList();
        }

        private static IEnumerable<NgController> FindControllersInFile(string file)
        {
            var content = File.ReadAllText(file);
            var regex = new Regex(@"class\s+((\w[\w\d]+)(Controller|Ctrl))");
            return regex.Matches(content).Cast<Match>().Select(x => new NgController
            {
                Name = x.Groups[1].Value,
                Path = file
            });
        }
    }
}
