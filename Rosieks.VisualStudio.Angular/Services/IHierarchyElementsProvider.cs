using System.Collections.Generic;

namespace Rosieks.VisualStudio.Angular.Services
{
    interface IHierarchyElementsProvider<TElement>
    {
        IReadOnlyList<TElement> GetElements(NgHierarchy hierarchy);
    }
}
