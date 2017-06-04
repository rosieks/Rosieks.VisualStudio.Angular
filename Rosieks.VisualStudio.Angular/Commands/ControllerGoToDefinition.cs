namespace Rosieks.VisualStudio.Angular.Commands
{
    using EnvDTE;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Rosieks.VisualStudio.Angular.Extensions;
    using Rosieks.VisualStudio.Angular.Services;
    using System;
    using System.Linq;

    internal class ControllerGoToDefinition : CommandTargetBase<VSConstants.VSStd97CmdID>
    {
        private readonly DTE dte;
        private readonly IVsTextView adapter;
        private readonly IStandardClassificationService standardClassifications;
        private readonly INgHierarchyProvider ngHierarchyProvider;
        private readonly IClassifier classifier;

        public ControllerGoToDefinition(IVsTextView adapter, IWpfTextView textView, DTE dte, IStandardClassificationService standardClassifications, IClassifierAggregatorService classifierAggreagatorService, INgHierarchyProvider ngHierarchyProvider) : base(adapter, textView, VSConstants.VSStd97CmdID.GotoDefn)
        {
            this.adapter = adapter;
            this.dte = dte;
            this.standardClassifications = standardClassifications;
            this.classifier = classifierAggreagatorService.GetClassifier(textView.TextBuffer);
            this.ngHierarchyProvider = ngHierarchyProvider;
        }

        protected override bool IsEnabled()
        {
            return this.dte.ActiveDocument.Name.EndsWith(".js") || this.dte.ActiveDocument.Name.EndsWith(".ts");
        }

        protected override bool Execute(VSConstants.VSStd97CmdID commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            string controllerName = this.GetStringValue();
            if (!string.IsNullOrEmpty(controllerName))
            {
                var currentDocumentPath = ServiceProvider.GlobalProvider.GetCurrentDocumentPath();
                var ngHierarchy = this.ngHierarchyProvider.Get(currentDocumentPath);
                var controllerMetadata = ngHierarchy.Controllers.Value.FirstOrDefault(x => x.Name == controllerName);
                if (controllerMetadata != null)
                {
                    this.dte.OpenFileInPreviewTab(controllerMetadata.Path);

                    return true;
                }
                else
                {
                    this.dte.StatusBar.Text = $"Cannot find controller {controllerName}";

                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private string GetStringValue()
        {
            if (this.dte.ActiveDocument.Name.EndsWith(".js"))
            {
                return this.TextView.GetJavaScriptStringValue(this.standardClassifications);
            }
            else if (this.dte.ActiveDocument.Name.EndsWith(".ts"))
            {
                return this.TextView.GetTypeScriptStringValue(this.classifier, this.standardClassifications);
            }
            else
            {
                return null;
            }
        }
    }
}
