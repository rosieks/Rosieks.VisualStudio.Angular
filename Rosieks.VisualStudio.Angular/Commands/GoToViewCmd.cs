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

                var htmlFileExtensions = new List<string>() { ".html", ".cshtml" };
                var pathsToSearch = new List<string>();

                foreach (var fileExtension in htmlFileExtensions) {
                    pathsToSearch.Add(path.Replace("controller.", "").Replace(".js", fileExtension).Replace(".ts", fileExtension));
                }

                //string viewPath = path.Replace("controller.", "").Replace(".js", ".html").Replace(".ts", ".html");

                var viewFound = false;

                foreach (var searchPath in pathsToSearch) {
                    if (File.Exists(searchPath)) {
                        viewFound = true;
                        this.dte.OpenFileInPreviewTab(searchPath);
                        break;
                    }
                }

                //if (File.Exists(viewPath))
                //{
                //    this.dte.OpenFileInPreviewTab(viewPath);
                //}
                //else
                if (viewFound == false) {
                    string fileName = Path.GetFileName(pathsToSearch.First());
                    this.dte.StatusBar.Text = $"Cannot find file '{fileName}'";
                }
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
                object value;
                hierarchy.GetProperty(projectItemId, (int)__VSHPROPID.VSHPROPID_Name, out value);
                return value != null && (value.ToString().EndsWith(".js") || value.ToString().EndsWith(".ts"));
            }
            else
            {
                return false;
            }
        }
    }
}
