namespace Rosieks.VisualStudio.Angular.Extensions
{
    using System;
    using EnvDTE;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    internal static class DTEExtensions
    {
        public static void OpenFileInPreviewTab(this DTE dte, string file)
        {
            IVsNewDocumentStateContext newDocumentStateContext = null;

            try
            {
                var openDoc3 = Package.GetGlobalService(typeof(SVsUIShellOpenDocument)) as IVsUIShellOpenDocument3;

                Guid reason = VSConstants.NewDocumentStateReason.Navigation;
                newDocumentStateContext = openDoc3.SetNewDocumentState((uint)__VSNEWDOCUMENTSTATE.NDS_Provisional, ref reason);

                dte.ItemOperations.OpenFile(file);
            }
            finally
            {
                if (newDocumentStateContext != null)
                {
                    newDocumentStateContext.Restore();
                }
            }
        }
    }
}
