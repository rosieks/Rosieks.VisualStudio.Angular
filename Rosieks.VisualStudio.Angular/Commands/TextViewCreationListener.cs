namespace Rosieks.VisualStudio.Angular.Commands
{
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Utilities;
    using System.ComponentModel.Composition;

    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public class TextViewCreationListener : IVsTextViewCreationListener
    {
        [Import]
        public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import]
        public IStandardClassificationService StandardClassificationService { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            var textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);

            textView.Properties.GetOrCreateSingletonProperty(() => new ControllerGoToDefinition(textViewAdapter, textView, AngularPackage.DTE, this.StandardClassificationService));
            textView.Properties.GetOrCreateSingletonProperty(() => new DirectiveGoToDefinition(textViewAdapter, textView, AngularPackage.DTE));
        }
    }
}
