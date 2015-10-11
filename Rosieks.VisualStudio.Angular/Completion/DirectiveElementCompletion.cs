namespace Rosieks.VisualStudio.Angular.Completion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Html.Editor.Completion;
    using Microsoft.Html.Editor.Completion.Def;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.Web.Core.ContentTypes;
    using Rosieks.VisualStudio.Angular.Extensions;
    using Rosieks.VisualStudio.Angular.Services;
    using System.ComponentModel.Composition;

    [HtmlCompletionProvider(CompletionTypes.Children, "*")]
    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
    public class DirectiveElementCompletion : IHtmlCompletionListProvider
    {
        private static readonly ImageSource ElementIcon = BitmapFrame.Create(new Uri("pack://application:,,,/Rosieks.VisualStudio.Angular;component/Resources/HtmlElement.png", UriKind.RelativeOrAbsolute));

        [Import]
        internal INgHierarchyProvider HierarchyProvider { get; set; }

        public string CompletionType
        {
            get
            {
                return CompletionTypes.Children;
            }
        }

        public IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
        {
            string fileName = context.Document.TextBuffer.GetFileName();
            var ngHierarchy = this.HierarchyProvider.Get(fileName);

            return ngHierarchy.Directives.Value
                .Where(d => d.Restrict.HasFlag(NgDirectiveRestrict.Element))
                .Select(
                    d => new HtmlCompletion(
                        d.DashedName,
                        d.DashedName,
                        d.Name,
                        ElementIcon,
                        "Directive",
                        context.Session))
                .ToList();
        }
    }
}
