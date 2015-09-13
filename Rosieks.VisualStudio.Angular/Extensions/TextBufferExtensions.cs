namespace Rosieks.VisualStudio.Angular.Extensions
{
    using System;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.TextManager.Interop;

    internal static class TextBufferExtensions
    {
        public static string GetFileName(this ITextBuffer buffer)
        {
            IVsTextBuffer bufferAdapter;

            if (!buffer.Properties.TryGetProperty(typeof(IVsTextBuffer), out bufferAdapter))
            {
                return null;
            }

            var persistFileFormat = bufferAdapter as IPersistFileFormat;
            string ppzsFilename = null;
            uint pnFormatIndex;
            int returnCode = -1;

            if (persistFileFormat != null)
            {
                try
                {
                    returnCode = persistFileFormat.GetCurFile(out ppzsFilename, out pnFormatIndex);
                }
                catch (NotImplementedException)
                {
                    return null;
                }
            }

            if (returnCode != VSConstants.S_OK)
            {
                return null;
            }

            return ppzsFilename;
        }

    }
}
