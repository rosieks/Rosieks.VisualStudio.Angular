namespace Rosieks.VisualStudio.Angular.Services
{
    using Extensions;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    [Export(typeof(INgHierarchyProvider))]
    internal class NgHierarchyProvider : INgHierarchyProvider
    {
        [ImportMany]
        public IHierarchyElementsProvider<NgDirective>[] DirectiveProviders { get; set; }

        [ImportMany]
        public IHierarchyElementsProvider<NgController>[] ControllerProviders { get; set; }

        [ImportMany]
        public IHierarchyElementsProvider<NgState>[] StateProviders { get; set; }

        public NgHierarchy Get(string path)
        {
            int index = path.LastIndexOf("app");
            if (index > -1)
            {
                var rootPath = path.Substring(0, index);

                var hierarchy = new NgHierarchy()
                {
                    RootPath = rootPath,
                };
                hierarchy.Directives = new Lazy<IReadOnlyList<NgDirective>>(() => GetDirectives(hierarchy));
                hierarchy.Controllers = new Lazy<IReadOnlyList<NgController>>(() => GetControllers(hierarchy));
                hierarchy.States = new Lazy<IReadOnlyList<NgState>>(() => GetStates(hierarchy));
                return hierarchy;
            }
            else
            {
                return NgHierarchy.Null;
            }
        }

        private IReadOnlyList<NgState> GetStates(NgHierarchy hierarchy)
        {
            return this.StateProviders.SelectMany(p => p.GetElements(hierarchy)).ToReadOnlyList();
        }

        private IReadOnlyList<NgController> GetControllers(NgHierarchy hierarchy)
        {
            return this.ControllerProviders.SelectMany(p => p.GetElements(hierarchy)).ToReadOnlyList();
        }

        private IReadOnlyList<NgDirective> GetDirectives(NgHierarchy hierarchy)
        {
            return this.DirectiveProviders.SelectMany(p => p.GetElements(hierarchy)).ToReadOnlyList();
        }
    }
}
