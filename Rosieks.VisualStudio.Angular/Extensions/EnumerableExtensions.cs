using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rosieks.VisualStudio.Angular.Extensions
{
    internal static class EnumerableExtensions
    {
        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> items)
        {
            return new ReadOnlyCollection<T>(items.ToList());
        }
    }
}
