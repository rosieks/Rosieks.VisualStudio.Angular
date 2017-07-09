namespace Rosieks.VisualStudio.Angular.Services
{
    using System.Collections.Generic;

    interface IHierarchyElementsProvider<TElement>
    {
        IReadOnlyList<TElement> GetElements(NgHierarchy hierarchy);
    }
}
