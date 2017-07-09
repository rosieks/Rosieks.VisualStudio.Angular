namespace Rosieks.VisualStudio.Angular
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Rosieks.VisualStudio.Angular.Commands;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(AngularPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class AngularPackage : Package
    {
        public const string PackageGuidString = "e8c30913-75f4-4ddd-8c98-52534380bf71";

        private static DTE dte;

        public AngularPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        internal static DTE DTE
        {
            get
            {
                if (dte == null)
                {
                    dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE;
                }


                return dte;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            GoToViewCmd.Initialize(this, AngularPackage.DTE);
            GoToCodeCmd.Initialize(this, AngularPackage.DTE);
        }
    
        public static readonly string[] CodeExtensions = new[] { ".ts", ".js" };
        public static readonly string[] ViewExtensions = new[] { ".html", ".cshtml" };
    }
}
