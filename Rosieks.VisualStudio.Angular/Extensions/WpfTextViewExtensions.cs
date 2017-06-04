namespace Rosieks.VisualStudio.Angular.Extensions
{
    using Microsoft.VisualStudio.JSLS;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using System;
    using System.Linq;

    internal static class WpfTextViewExtensions
    {
        private static readonly Type jsTaggerType = typeof(JavaScriptLanguageService).Assembly.GetType("Microsoft.VisualStudio.JSLS.Classification.Tagger");
        private static readonly Type tsTaggerType = Type.GetType("Microsoft.CodeAnalysis.Editor.TypeScript.Features.Classifier.LexicalClassificationTaggerProvider+Tagger, Microsoft.CodeAnalysis.TypeScript.EditorFeatures");

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

        public static string GetTypeScriptStringValue(this IWpfTextView textView, IClassifier classifier, IStandardClassificationService standardClassifications)
        {
            var buffers = textView.BufferGraph.GetTextBuffers(b => b.ContentType.IsOfType("TypeScript") && textView.GetSelection("TypeScript").HasValue && textView.GetSelection("TypeScript").Value.Snapshot.TextBuffer == b);

            if (!buffers.Any())
            {
                return null;
            }

            int position = textView.Caret.Position.BufferPosition;
            if (position == textView.TextBuffer.CurrentSnapshot.Length)
            {
                position = position - 1;
            }

            var span = new SnapshotSpan(textView.TextBuffer.CurrentSnapshot, position, 1);
            var cspans = classifier.GetClassificationSpans(span);
            var text = cspans
                .Where(s => s.ClassificationType == standardClassifications.StringLiteral)
                .Select(s => s.Span.GetText())
                .FirstOrDefault();

            return text != null ? text.Substring(1, text.Length - 2) : null;
        }
    }
}
