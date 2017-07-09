namespace Rosieks.VisualStudio.Angular.Services
{
    using System;

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

        public NgDirectiveRestrict Restrict { get; set; }

        public NgDirectiveAttribute[] Attributes { get; set; }
    }

    internal class NgDirectiveAttribute
    {
        public string DashedName { get; set; }
        public string Name { get; set; }
    }
}