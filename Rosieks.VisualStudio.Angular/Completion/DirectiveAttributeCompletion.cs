namespace Rosieks.VisualStudio.Angular.Completion
{
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

    [HtmlCompletionProvider(CompletionTypes.Attributes, "*")]
    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
    public class DirectiveAttributeCompletion : IHtmlCompletionListProvider
    {
        private static readonly ImageSource AttributeIcon = GlyphService.GetGlyph(StandardGlyphGroup.GlyphGroupVariable, StandardGlyphItem.GlyphItemPublic);

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
            var ngHierarchy = NgHierarchyFactory.Find(fileName);

            return ngHierarchy.Directives.Value
                .Where(d => d.Restrict.HasFlag(NgDirectiveRestrict.Attribute))
                .Select(
                    d => new HtmlCompletion(
                        d.DashedName,
                        d.DashedName,
                        d.Name,
                        AttributeIcon,
                        "Directive",
                        context.Session))
                .ToList();
        }
    }
}
