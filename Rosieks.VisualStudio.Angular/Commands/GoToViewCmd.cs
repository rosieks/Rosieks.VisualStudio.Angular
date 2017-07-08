namespace Rosieks.VisualStudio.Angular.Commands
{
    using System;
    using System.ComponentModel.Design;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Rosieks.VisualStudio.Angular.Extensions;

    internal sealed class GoToViewCmd
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("de807d66-a411-49e1-9493-bba01b7f46f3");

        private readonly Package package;
        private readonly DTE dte;

        private GoToViewCmd(Package package, DTE dte)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new OleMenuCommand(
                    this.MenuItemCallback,
                    null,
                    this.BeforeQueryStatus,
                    menuCommandID);
                commandService.AddCommand(menuItem);
                this.dte = dte;
            }
        }

        public static GoToViewCmd Instance
        {
            get;
            private set;
        }

        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        public static void Initialize(Package package, DTE dte)
        {
            Instance = new GoToViewCmd(package, dte);
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            IntPtr hierarchyPtr, selectionContainerPtr;
            uint projectItemId;
            IVsMultiItemSelect mis;
            IVsMonitorSelection monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));
            monitorSelection.GetCurrentSelection(out hierarchyPtr, out projectItemId, out mis, out selectionContainerPtr);

            IVsHierarchy hierarchy = Marshal.GetTypedObjectForIUnknown(hierarchyPtr, typeof(IVsHierarchy)) as IVsHierarchy;
            if (hierarchy != null)
            {
                object value;
                hierarchy.GetProperty(projectItemId, (int)__VSHPROPID.VSHPROPID_ExtSelectedItem, out value);
                string path;
                hierarchy.GetCanonicalName(projectItemId, out path);
                string viewPath = this.GetViewPath(path); path.Replace("controller.", "").Replace(".js", ".html").Replace(".ts", ".html");
                if (File.Exists(viewPath))
                {
                    this.dte.OpenFileInPreviewTab(viewPath);
                }
                else
                {
                    string fileName = Path.GetFileName(viewPath);
                    this.dte.StatusBar.Text = $"Cannot find file '{fileName}'";
                }
            }
        }

        private string GetViewPath(string path)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            fileName = fileName.Replace(".controller", "");
            string viewPath = Path.Combine(Path.GetDirectoryName(path), fileName);
            if (FileHelper.TryFind(viewPath, AngularPackage.ViewExtensions, out viewPath))
            {
                return viewPath;
            }
            else
            {
                return null;
            }
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            bool canGoToView = this.CanGoToView();
            menuCommand.Visible = canGoToView;
            menuCommand.Enabled = canGoToView;
        }

        private bool CanGoToView()
        {
            IntPtr hierarchyPtr, selectionContainerPtr;
            uint projectItemId;
            IVsMultiItemSelect mis;
            IVsMonitorSelection monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));
            monitorSelection.GetCurrentSelection(out hierarchyPtr, out projectItemId, out mis, out selectionContainerPtr);

            IVsHierarchy hierarchy = Marshal.GetTypedObjectForIUnknown(hierarchyPtr, typeof(IVsHierarchy)) as IVsHierarchy;
            if (hierarchy != null)
            {
                string path;
                hierarchy.GetCanonicalName(projectItemId, out path);
                return path != null && IsCode(path) && GetViewPath(path) != null;
            }
            else
            {
                return false;
            }
        }

        private bool IsCode(string path)
        {
            return AngularPackage.CodeExtensions.Any(codeExtension => path.EndsWith(codeExtension));
        }
    }
}
