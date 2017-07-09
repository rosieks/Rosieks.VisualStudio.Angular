namespace Rosieks.VisualStudio.Angular.Services.JavaScript
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Rosieks.VisualStudio.Angular.Extensions;

    [Export(typeof(IHierarchyElementsProvider<NgDirective>))]
    class JavaScriptDirectivesProvider : IHierarchyElementsProvider<NgDirective>
    {
        public IReadOnlyList<NgDirective> GetElements(NgHierarchy hierarchy)
        {
            return FindDirectives(hierarchy.RootPath);
        }

        private static IReadOnlyList<NgDirective> FindDirectives(string rootPath)
        {
            return Directory
                .EnumerateFiles(rootPath, "*.js", SearchOption.AllDirectories)
                .IsValidPath()
                .SelectMany(FindDirectiveInFile)
                .ToReadOnlyList();
        }

        private static IEnumerable<NgDirective> FindDirectiveInFile(string file)
        {
            var content = File.ReadAllText(file);
            var regex = new Regex(@"\.directive\(\s*(?>\'|\"")([^\'\""]+)");
            return regex.Matches(content).Cast<Match>().Select(x => new NgDirective
            {
                Name = x.Groups[1].Value,
                DashedName = CreateDashedName(x.Groups[1].Value),
                CodeFileName = file,
                ViewFileName = file.Replace(".js", ".html"),
                Restrict = NgDirectiveRestrict.All, // TODO: Add detection of restriction
                Attributes = ParseScope(content, x),
            });
        }

        private static NgDirectiveAttribute[] ParseScope(string content, Match directiveMatch)
        {
            var regex = new Regex(@"scope\s*\:\s*\{([^\}]*)\}");
            var postDirectiveCode = content.Substring(directiveMatch.Index + directiveMatch.Length);
            var match = regex.Match(postDirectiveCode);
            if (match != null && match.Groups.Count == 2)
            {
                return match.Groups[1].Value
                    .Split(',')
                    .Select(keyValuePair =>
                    {
                        var parts = keyValuePair.Split(':').Select(x => x.Trim()).ToArray();
                        if (parts.Length == 2)
                        {
                            var key = parts[0];
                            var value = parts[1].Length >= 3 ? parts[1].Substring(2, parts[1].Length - 3) : null;
                            if (value == null)
                            {
                                return null;
                            }
                            else if (value == string.Empty)
                            {
                                return new NgDirectiveAttribute { Name = key, DashedName = CreateDashedName(key) };
                            }
                            else
                            {
                                return new NgDirectiveAttribute { Name = value, DashedName = CreateDashedName(value) };
                            }
                        }
                        else
                        {
                            return null;
                        }
                    })
                    .Where(x => x != null)
                    .ToArray();
            }
            else
            {
                return null;
            }
        }

        private static string CreateDashedName(string value)
        {
            return Regex.Replace(value, @"(?<!-)([A-Z])", "-$1").ToLower();
        }
    }
}
