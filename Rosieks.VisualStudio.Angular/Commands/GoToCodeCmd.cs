﻿namespace Rosieks.VisualStudio.Angular.Commands
{
    using System;
    using System.ComponentModel.Design;
    using System.IO;
    using System.Runtime.InteropServices;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Rosieks.VisualStudio.Angular.Extensions;

    internal sealed class GoToCodeCmd
    {
        public const int CommandId = 256;

        public static readonly Guid CommandSet = new Guid("b4e66242-e9d0-4e4a-9112-6e5bd72b91b1");

        private readonly Package package;
        private readonly DTE dte;

        private GoToCodeCmd(Package package, DTE dte)
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

        public static GoToCodeCmd Instance
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
            Instance = new GoToCodeCmd(package, dte);
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
                string codePath = this.GetCodePath(path);
                if (File.Exists(codePath))
                {
                    this.dte.OpenFileInPreviewTab(codePath);
                }
                else
                {
                    string fileName = Path.GetFileName(codePath);
                    this.dte.StatusBar.Text = $"Cannot find file '{fileName}'";
                }
            }
        }

        private string GetCodePath(string path)
        {
            // TODO - GET THE FILE EXTENSIONS FROM THE PATH AND USE THAT FOR THE REPLACE INSTEAD OF HARD CODING EXTENSIOSN
            var htmlFileExtensions = new List<string>() { ".html", ".cshtml" };
            var pathsToSearch = new List<string>();

            foreach (var fileExtension in htmlFileExtensions) {
                pathsToSearch.Add(path.Contains(".directive.") ? path.Replace(fileExtension, ".ts") : path.Replace(fileExtension, ".controller.ts"));
                pathsToSearch.Add(path.Contains(".directive.") ? path.Replace(fileExtension, ".js") : path.Replace(fileExtension, ".controller.js")); // ALSO CHECK FOR NON TYPESCRIPT FILES
            }
            //var codePath = path.Contains(".directive.") ? path.Replace(".html", ".ts") : path.Replace(".html", ".controller.ts");
            foreach (var searchPath in pathsToSearch) {
                if (File.Exists(searchPath)) {
                    return searchPath;
                }
            }

            //if (File.Exists(codePath))
            //{
            //    return codePath;
            //}
            //else
            //{
            return pathsToSearch.First().Replace(".ts", ".js");
            //}
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = (OleMenuCommand)sender;
            bool canGoCode = this.CanGoToCode();
            menuCommand.Visible = canGoCode;
            menuCommand.Enabled = canGoCode;
        }

        private bool CanGoToCode()
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
                hierarchy.GetProperty(projectItemId, (int)__VSHPROPID.VSHPROPID_Name, out value);
                return value != null && (value.ToString().EndsWith(".html") || value.ToString().EndsWith(".cshtml"));
            }
            else
            {
                return false;
            }
        }
    }
}
