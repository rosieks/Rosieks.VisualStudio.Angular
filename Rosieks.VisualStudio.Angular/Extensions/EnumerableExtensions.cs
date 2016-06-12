using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Rosieks.VisualStudio.Angular.Extensions
{
    internal static class EnumerableExtensions
    {
        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> items)
        {
            return new ReadOnlyCollection<T>(items.ToList());
        }

        public static IEnumerable<string> IsValidPath(this IEnumerable<string> paths)
        {
            return paths.Where(x => {
                if (x.Length >= 260)
                {
                    return false;
                }

                try
                {
                    Path.GetFullPath(x);
                    return true;
                }
                catch (PathTooLongException)
                {
                    return false;
                }
            });
        }
    }
}
