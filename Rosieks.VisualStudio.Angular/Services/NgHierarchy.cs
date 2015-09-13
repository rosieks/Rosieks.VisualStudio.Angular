namespace Rosieks.VisualStudio.Angular.Services
{
    using System;
    using System.Collections.Generic;

    internal class NgHierarchy
    {
        public NgHierarchy()
        {
        }

        public string RootPath { get; set; }

        public Lazy<IReadOnlyList<NgController>> Controllers { get; set; }

        public Lazy<IReadOnlyList<NgDirective>> Directives { get; set; }
    }
}
