namespace Rosieks.VisualStudio.Angular.Completion
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Html.Editor.Completion;
    using Microsoft.Html.Editor.Completion.Def;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.Web.Core.ContentTypes;
    using Extensions;
    using Services;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    [HtmlCompletionProvider(CompletionTypes.Values, "a", "ui-sref")]
    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
    public class UISRefCompletion : IHtmlCompletionListProvider
    {
        private static readonly ImageSource LinkIcon = BitmapFrame.Create(new Uri("pack://application:,,,/Rosieks.VisualStudio.Angular;component/Resources/Link.png", UriKind.RelativeOrAbsolute));

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
                var ngHierarchy = NgHierarchyFactory.Find(fileName);
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
