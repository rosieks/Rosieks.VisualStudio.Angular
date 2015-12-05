namespace Rosieks.VisualStudio.Angular.Completion
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Services;

    [Export(typeof(ICompletionSourceProvider))] 
 	[Order(Before = "High")] 
 	[ContentType("JavaScript")]
    [ContentType("TypeScript")]
 	[Name("AngularViewCompletion")]
    internal class ViewCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        public INgHierarchyProvider HierarchyProvider { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new ViewCompletionSource(textBuffer, this.HierarchyProvider)) as ICompletionSource;
        }
    }
}
