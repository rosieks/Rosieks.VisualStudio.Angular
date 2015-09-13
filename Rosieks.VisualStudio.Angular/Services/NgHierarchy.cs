namespace Rosieks.VisualStudio.Angular.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class NgHierarchy
    {
        public NgHierarchy()
        {
        }

        public string RootPath { get; set; }

        public Lazy<IReadOnlyList<NgController>> Controllers { get; set; }
    }
}
