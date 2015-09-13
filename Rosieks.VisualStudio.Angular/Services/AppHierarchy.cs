namespace Rosieks.VisualStudio.Angular.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class AppHierarchy
    {
        public AppHierarchy(string rootPath)
        {
            this.RootPath = rootPath;
        }

        public string RootPath { get; private set; }
    }
}
