using System;

namespace Rosieks.VisualStudio.Angular.Services
{
    [Flags]
    internal enum NgDirectiveRestrict
    {
        Element = 1,
        Attribute = 2,
        Class = 4,
        All = 7,
    }

    internal class NgDirective
    {
        public string Name { get; set; }

        public string DashedName { get; set; }

        public string CodeFileName { get; set; }

        public string ViewFileName { get; set; }

        public NgDirectiveRestrict Restrict { get; internal set; }
    }
}