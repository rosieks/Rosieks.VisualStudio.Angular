using Microsoft.VisualStudio.JSLS;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosieks.VisualStudio.Angular.Extensions
{
    internal static class WpfTextViewExtensions
    {
        private static readonly Type jsTaggerType = typeof(JavaScriptLanguageService).Assembly.GetType("Microsoft.VisualStudio.JSLS.Classification.Tagger");

        public static string GetJavaScriptStringValue(this IWpfTextView textView, IStandardClassificationService standardClassifications)
        {
            var buffers = textView.BufferGraph.GetTextBuffers(b => b.ContentType.IsOfType("JavaScript") && textView.GetSelection("JavaScript").HasValue && textView.GetSelection("JavaScript").Value.Snapshot.TextBuffer == b);

            if (!buffers.Any())
            {
                return null;
            }

            var tagger = buffers.First().Properties.GetProperty<ITagger<ClassificationTag>>(jsTaggerType);

            var text = tagger.GetTags(new NormalizedSnapshotSpanCollection(new SnapshotSpan(textView.Caret.Position.BufferPosition, 0)))
                    .Where(s => s.Tag.ClassificationType == standardClassifications.StringLiteral)
                    .Select(s => s.Span.GetText())
                    .FirstOrDefault();

            return text != null ? text.Substring(1, text.Length - 2) : null;
        }
    }
}
