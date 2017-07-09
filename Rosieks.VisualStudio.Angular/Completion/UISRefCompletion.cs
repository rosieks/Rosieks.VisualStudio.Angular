namespace Rosieks.VisualStudio.Angular.Completion
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Extensions;
    using Microsoft.Html.Editor.Completion;
    using Microsoft.Html.Editor.Completion.Def;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.Web.Core.ContentTypes;
    using Services;

    [HtmlCompletionProvider(CompletionTypes.Values, "a", "ui-sref")]
    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
    internal class UISRefCompletion : IHtmlCompletionListProvider
    {
        private static readonly ImageSource LinkIcon = BitmapFrame.Create(new Uri("pack://application:,,,/Rosieks.VisualStudio.Angular;component/Resources/Link.png", UriKind.RelativeOrAbsolute));

        [Import]
        public INgHierarchyProvider HierarchyProvider { get; set; }

        public string CompletionType
        {
            get
            {
                return CompletionTypes.Values;
            }
        }

        public IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
        {
            var attr = context.Element.GetAttribute("ui-sref");
            if (attr == null)
            {
                return new HtmlCompletion[0];
            }
            else
            {
                string fileName = context.Document.TextBuffer.GetFileName();
                var ngHierarchy = this.HierarchyProvider.Get(fileName);
                return ngHierarchy.States
                    .Value
                    .Select(x => new HtmlCompletion(
                        x.Name,
                        x.Name,
                        x.Name,
                        LinkIcon,
                        "State",
                        context.Session))
                    .ToArray();
            }
        }
    }
}
