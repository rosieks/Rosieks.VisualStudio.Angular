namespace Rosieks.VisualStudio.Angular.Completion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using Microsoft.Html.Editor.Completion;
    using Microsoft.Html.Editor.Completion.Def;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.Web.Core.ContentTypes;
    using Microsoft.Web.Editor.EditorHelpers;
    using Microsoft.Web.Editor.Imaging;
    using Services;
    using System.ComponentModel.Composition;

    [HtmlCompletionProvider(CompletionTypes.Attributes, "*")]
    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
    internal class DirectiveAttributeCompletion : IHtmlCompletionListProvider
    {
        private static readonly ImageSource AttributeIcon = GlyphService.GetGlyph(StandardGlyphGroup.GlyphGroupVariable, StandardGlyphItem.GlyphItemPublic);

        [Import]
        public INgHierarchyProvider HierarchyProvider { get; set; }

        public string CompletionType
        {
            get
            {
                return CompletionTypes.Attributes;
            }
        }

        public IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
        {
            string fileName = context.Document.TextBuffer.GetFileName();
            var ngHierarchy = this.HierarchyProvider.Get(fileName);
            var completions = new List<HtmlCompletion>();

            AddDirectives(context, ngHierarchy, completions);
            AddDirectiveAttributes(context, ngHierarchy, completions);

            return completions;
        }

        private static void AddDirectiveAttributes(HtmlCompletionContext context, NgHierarchy ngHierarchy, List<HtmlCompletion> completions)
        {
            var directive = ngHierarchy.Directives.Value.FirstOrDefault(x => x.Restrict.HasFlag(NgDirectiveRestrict.Element) && x.DashedName == context.Element.Name);
            if (directive != null)
            {
                var attributes = directive.Attributes
                    .Select(
                        a => new HtmlCompletion(
                            a.DashedName,
                            a.DashedName,
                            a.Name,
                            AttributeIcon,
                            "DirectiveAttribute",
                            context.Session));

                completions.AddRange(attributes);
            }
        }

        private static void AddDirectives(HtmlCompletionContext context, NgHierarchy ngHierarchy, List<HtmlCompletion> completions)
        {
            var directives = ngHierarchy.Directives.Value
                .Where(d => d.Restrict.HasFlag(NgDirectiveRestrict.Attribute))
                .Select(
                    d => new HtmlCompletion(
                        d.DashedName,
                        d.DashedName,
                        d.Name,
                        AttributeIcon,
                        "Directive",
                        context.Session));

            completions.AddRange(directives);
        }
    }
}
