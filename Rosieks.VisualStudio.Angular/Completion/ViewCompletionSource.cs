namespace Rosieks.VisualStudio.Angular.Completion
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Extensions;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Services;
    using Microsoft.Web.Editor.Imaging;

    internal class ViewCompletionSource : ICompletionSource
    {
        private static readonly Regex parentTraversalRegex = new Regex(@"^(\.\./)+$");
        private static readonly ImageSource folderIcon = GlyphService.GetGlyph(StandardGlyphGroup.GlyphOpenFolder, StandardGlyphItem.GlyphItemPublic);
        private static readonly ImageSource viewIcon = BitmapFrame.Create(new Uri("pack://application:,,,/Rosieks.VisualStudio.Angular;component/Resources/GoToViewCmd.png", UriKind.RelativeOrAbsolute));
        private static readonly Completion parentFolder = new Completion("../", "../", "Prefix to access files in the parent directory", folderIcon, "Folder");

        private ITextBuffer textBuffer;

        public ViewCompletionSource(ITextBuffer textBuffer)
        {
            this.textBuffer = textBuffer;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            var position = session.GetTriggerPoint(this.textBuffer).GetPoint(this.textBuffer.CurrentSnapshot);
            var line = position.GetContainingLine();

            if (line == null)
            {
                return;
            }

            int linePos = position - line.Start.Position;

            var info = AngularViewCompletionUtils.FindCompletionInfo(line.GetText(), linePos);
            if (info == null)
            {
                return;
            }

            var callingFilename = this.textBuffer.GetFileName();
            var ngHierarchy = NgHierarchyFactory.Find(callingFilename);
            if (ngHierarchy != NgHierarchy.Null)
            {
                IEnumerable<Completion> results = null;
                if (String.IsNullOrWhiteSpace(info.Item1))
                {
                    results = GetRootCompletions(ngHierarchy);
                }
                else
                {
                    results = GetRelativeCompletions(ngHierarchy, info.Item1);
                }

                var trackingSpan = this.textBuffer.CurrentSnapshot.CreateTrackingSpan(info.Item2.Start + line.Start, info.Item2.Length, SpanTrackingMode.EdgeInclusive);
                completionSets.Add(new CompletionSet(
                    "Angular views",
                    "Angular views",
                    trackingSpan,
                    results,
                    null
                ));
            }
        }

        private IEnumerable<Completion> GetRelativeCompletions(NgHierarchy appHierarchy, string item1)
        {
            var views = Directory
                .EnumerateFiles(appHierarchy.RootPath + item1, "*.html", SearchOption.TopDirectoryOnly)
                .Select(p => new Completion(
                    p.Substring(appHierarchy.RootPath.Length + item1.Length),
                    p.Substring(appHierarchy.RootPath.Length + item1.Length),
                    "",
                    viewIcon,
                    "View"));

            var directories = Directory
                .EnumerateDirectories(appHierarchy.RootPath + item1)
                .Select(p => new Completion(
                    p.Substring(appHierarchy.RootPath.Length + item1.Length),
                    p.Substring(appHierarchy.RootPath.Length + item1.Length) + "/",
                    "",
                    folderIcon,
                    "Folder"));

            return directories.Concat(views);
        }

        private IEnumerable<Completion> GetRootCompletions(NgHierarchy appHierarchy)
        {
            var views = Directory
                .EnumerateFiles(appHierarchy.RootPath, "*.html", SearchOption.TopDirectoryOnly)
                .Select(p => new Completion(
                    p.Substring(appHierarchy.RootPath.Length),
                    p.Substring(appHierarchy.RootPath.Length),
                    "",
                    viewIcon,
                    "View"));

            var directories = Directory
                .EnumerateDirectories(appHierarchy.RootPath)
                .Select(p => new Completion(
                    p.Substring(appHierarchy.RootPath.Length),
                    p.Substring(appHierarchy.RootPath.Length) + "/",
                    "",
                    folderIcon,
                    "Folder"));

            return directories.Concat(views);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    ///<summary>Contains host-agnostic methods used to provide Node.js module completions.</summary>
    ///<remarks>This is a separate class so that it can be unit-tested without running any
    ///field initializers that require the VS hosting environment.</remarks>
    public static class AngularViewCompletionUtils
    {
        static readonly Regex regex = new Regex(@"(?<=\btemplateUrl\s*\:\s*(['""]))[a-z0-9_./+=-]*(?=\s*\1\s*\)?)?", RegexOptions.IgnoreCase);
        public static Tuple<string, Span> FindCompletionInfo(string line, int cursorPosition)
        {
            var match = regex.Matches(line)
                             .Cast<Match>()
                             .FirstOrDefault(m => m.Index <= cursorPosition && cursorPosition <= m.Index + m.Length);
            if (match == null)
                return null;

            string prefix = null;

            int precedingSlash;
            if (cursorPosition == match.Index + match.Length)
                precedingSlash = match.Value.LastIndexOf('/');
            else if (cursorPosition == match.Index)
                precedingSlash = -1;
            else
                precedingSlash = match.Value.LastIndexOf('/', cursorPosition - match.Index - 1);

            if (precedingSlash >= 0)
            {
                precedingSlash++;       // Skip the slash character
                prefix = match.Value.Substring(0, precedingSlash);  // Remove(precedingSlash) fails if the / is at the end
            }
            else
                precedingSlash = 0;


            var followingSlash = match.Value.IndexOf('/', cursorPosition - match.Index);
            if (followingSlash < 0)
                followingSlash = match.Length;

            return Tuple.Create(
                prefix,
                Span.FromBounds(precedingSlash + match.Index, followingSlash + match.Index)
            );
        }
    }


}