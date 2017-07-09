namespace Rosieks.VisualStudio.Angular.Completion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Extensions;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Services;

    internal class ControllerCompletionSource : ICompletionSource
    {
        private static readonly Regex parentTraversalRegex = new Regex(@"^(\.\./)+$");
        private static readonly ImageSource controllerIcon = BitmapFrame.Create(new Uri("pack://application:,,,/Rosieks.VisualStudio.Angular;component/Resources/GoToCodeCmd.png", UriKind.RelativeOrAbsolute));

        private ITextBuffer textBuffer;
        private readonly INgHierarchyProvider ngHierarchyProvider;

        public ControllerCompletionSource(ITextBuffer textBuffer, INgHierarchyProvider ngHierarchyProvider)
        {
            this.textBuffer = textBuffer;
            this.ngHierarchyProvider = ngHierarchyProvider;
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

            var info = AngularControllerCompletionUtils.FindCompletionInfo(line.GetText(), linePos);
            if (info == null)
            {
                return;
            }

            var callingFilename = this.textBuffer.GetFileName();
            var appHierarchy = this.ngHierarchyProvider.Get(callingFilename);

            IEnumerable<Completion> results = null;
            results = this.GetCompletions(appHierarchy);

            var trackingSpan = this.textBuffer.CurrentSnapshot.CreateTrackingSpan(info.Item2.Start + line.Start, info.Item2.Length, SpanTrackingMode.EdgeInclusive);
            completionSets.Add(new CompletionSet(
                "Angular views",
                "Angular views",
                trackingSpan,
                results,
                null
            ));
        }

        private IEnumerable<Completion> GetCompletions(NgHierarchy appHierarchy)
        {
            return appHierarchy
                .Controllers
                .Value
                .Select(p => new Completion(
                    p.Name,
                    p.Name,
                    "",
                    controllerIcon,
                    "Controller"));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    ///<summary>Contains host-agnostic methods used to provide Node.js module completions.</summary>
    ///<remarks>This is a separate class so that it can be unit-tested without running any
    ///field initializers that require the VS hosting environment.</remarks>
    public static class AngularControllerCompletionUtils
    {
        static readonly Regex regex = new Regex(@"(?<=\bcontroller\s*\:\s*(['""]))[a-z0-9_./+=-]*(?=\s*\1\s*\)?)?", RegexOptions.IgnoreCase);
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