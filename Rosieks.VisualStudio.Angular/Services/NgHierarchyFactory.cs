namespace Rosieks.VisualStudio.Angular.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

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
                    Directives = new Lazy<IReadOnlyList<NgDirective>>(() => FindDirectives(rootPath)),
                    States = new Lazy<IReadOnlyList<NgState>>(() => FindStates(rootPath)),
                };
            }
            else
            {
                return NgHierarchy.Null;
            }
        }

        private static IReadOnlyList<NgState> FindStates(string rootPath)
        {
            return new ReadOnlyCollection<NgState>(
                Directory
                    .EnumerateFiles(rootPath, "*.js", SearchOption.AllDirectories)
                    .SelectMany(FindStatesInFile)
                    .ToArray());
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

        private static IReadOnlyList<NgDirective> FindDirectives(string rootPath)
        {
            return new ReadOnlyCollection<NgDirective>(
                Directory
                    .EnumerateFiles(rootPath, "*.js", SearchOption.AllDirectories)
                    .SelectMany(FindDirectiveInFile)
                    .ToArray());
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
