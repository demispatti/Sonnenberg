using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using log4net;
using SharpShell;
using Sonnenberg.Common;
using Sonnenberg.FileHelper;
using Sonnenberg.Language;
using Sonnenberg.ShellServer.MenuItems;
using Sonnenberg.ShellServer.Properties;

namespace Sonnenberg.ShellServer
{
    /// <summary>
    ///     The class responsible for creating the context menu
    ///     and all its dependencies.
    /// </summary>
    /// <seealso cref="ContextMenu" />
    /// <seealso cref="IDisposable" />
    /// <seealso cref="ContextMenuStrip" />
    /// <seealso cref="Logger" />
    internal class ContextMenu : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ContextMenu));
        
        private ContextMenuStrip _contextMenuStrip;
        
        private bool _disposedValue;
        
        private FileExtensions _fileExtensions;
        internal ContextMenu()
        {
            ConfigureLogger();
            SetFileExtensions();
            SetContextMenuStrip();
        }
        public void Dispose()
        {
	        Dispose(true);
	        GC.SuppressFinalize(this);
        }
        private void SetFileExtensions()
        {
            _fileExtensions = new FileExtensions();
        }
        private void SetContextMenuStrip()
        {
            _contextMenuStrip = new ContextMenuStrip();
        }
        private static void ConfigureLogger()
        {
            Logger.Configure();
        }

        /// <summary>
        /// Determines if the context menu strip should be generated and displayed.
        /// </summary>
        /// <param name="shellServer"></param>
        /// <returns>bool</returns>
        internal bool CanShowMenu(ShellServer shellServer)
        {
            // is Folder || is Dir
            var canShowMenu = shellServer.FolderPath != null || shellServer.SelectedItemPaths.Count() == 1;
            var menuType = GetClickedItemType(shellServer);

            if ("folder" == menuType && shellServer.SelectedItemPaths.Count() == 1 ||
                null != shellServer.FolderPath && "directory" == menuType && 0 != shellServer.FolderPath.Length) return canShowMenu;

            // Is Supported File
            var path = "file" == menuType ? shellServer.SelectedItemPaths.First() : shellServer.FolderPath;
            var ext = Path.GetExtension(path);
            if (! _fileExtensions.BlacklistedFileExtensions.Any(s => s.Equals(ext, StringComparison.OrdinalIgnoreCase) && "directory" != menuType)) return canShowMenu;
            // File type is not supported
            log.Info("Unsupported file type: \"" + ext + "\"");

            return false;
        }

        /// <summary>
        /// Invokes creating the <c>ContextMenuStrip</c> based on the type of item that was right-clicked by the user,
        /// and returns it.
        /// </summary>
        /// <param name="shellServer"></param>
        /// <returns>ContextMenuStrip</returns>
        internal ContextMenuStrip CreateMenu(ShellServer shellServer)
        {
            var menuType = GetClickedItemType(shellServer);
            var clickedItemPath = CopyPath.GetClickedItemPath(menuType, shellServer);
            var shellStartUpDirectory = OpenShell.GetShellStartUpDirectory(menuType, shellServer);

            switch (menuType)
            {
                case "directory":
	                return FolderMenu(menuType, clickedItemPath, clickedItemPath);

                case "folder":
	                return FolderMenu(menuType, clickedItemPath, shellStartUpDirectory);

                case "file":
	                return FileMenu(menuType, clickedItemPath, shellServer);

                default:
	                return new ContextMenuStrip();
            }
        }

        /// <summary>
        /// Creates and returns the context menu strip that makes up the context menu
        /// that gets served in case the user right-clicked the directory inside the Windows Explorer.
        /// </summary>
        /// <param name="menuType"></param>
        /// <param name="clickedItemPath"></param>
        /// <param name="shellStartUpDirectory"></param>
        /// <returns>ContextMenuStrip</returns>
        private ContextMenuStrip FolderMenu(string menuType, string clickedItemPath, string shellStartUpDirectory)
        {
            var toolStripMenuItem = new ToolStripMenuItem
            {
                Text = "folder" == menuType ? Strings.menuStripNameFolderMenu : Strings.menuStripNameDirectoryMenu,
                Image = Resources.imgSonnenberg
            };

            var openShell = new OpenShell();
            if (OpenShell.AppExists("powershell.exe"))
            {
                var openPowershellInsideMenuItem = openShell.CreateToolStripMenuItem(menuType, shellStartUpDirectory, "powershell.exe", false);
                toolStripMenuItem.DropDownItems.Add(openPowershellInsideMenuItem);

                var openPowershellInsideElevatedMenuItem = openShell.CreateToolStripMenuItem(menuType, shellStartUpDirectory, "powershell.exe", true);
                toolStripMenuItem.DropDownItems.Add(openPowershellInsideElevatedMenuItem);
            }

            if (OpenShell.AppExists("cmd.exe"))
            {
                var openCmdInsideMenuItem = openShell.CreateToolStripMenuItem(menuType, shellStartUpDirectory, "cmd.exe", false);
                toolStripMenuItem.DropDownItems.Add(openCmdInsideMenuItem);

                var openCmdInsideElevatedMenuItem = openShell.CreateToolStripMenuItem(menuType, shellStartUpDirectory, "cmd.exe", true);
                toolStripMenuItem.DropDownItems.Add(openCmdInsideElevatedMenuItem);
            }

            var copyPath = new CopyPath();
            var escapedFilePathMenuItem = copyPath.CreateToolStripMenuItem(menuType, clickedItemPath, true);
            toolStripMenuItem.DropDownItems.Add(escapedFilePathMenuItem);

            var copyPathMenuItem = copyPath.CreateToolStripMenuItem(menuType, clickedItemPath);
            toolStripMenuItem.DropDownItems.Add(copyPathMenuItem);

            //// Add our main menu.
            _contextMenuStrip.Items.Add(toolStripMenuItem);

            return _contextMenuStrip;
        }

        /// <summary>
        /// Creates and returns the context menu strip that makes up the context menu
        /// that gets served in case the user right-clicked an item inside a Windows Explorer directory.
        /// </summary>
        /// <param name="menuType"></param>
        /// <param name="clickedItemPath"></param>
        /// <param name="shellServer"></param>
        /// <returns>ContextMenuStrip</returns>
        private ContextMenuStrip FileMenu(string menuType, string clickedItemPath, ShellExtInitServer shellServer)
        {
            // Create our main menu.
            var toolStripMenuItem = new ToolStripMenuItem
            {
                Text = Strings.menuStripNameFilesMenu,
                Image = Resources.imgSonnenberg
            };

            // Add a "count lines" option for file types considered text files
            if ("text" == new FileTypes().GetFileType(Path.GetExtension(clickedItemPath)))
            {
                var countLines = new CountLines();
                var countLinesMenuItem = countLines.CreateToolStripMenuItem(shellServer.SelectedItemPaths);
                toolStripMenuItem.DropDownItems.Add(countLinesMenuItem);

                var countCleanLinesMenuItem = countLines.CreateToolStripMenuItem(shellServer.SelectedItemPaths, true);
                toolStripMenuItem.DropDownItems.Add(countCleanLinesMenuItem);
            }

            // Add items
            var copyPath = new CopyPath();
            var fwdSlashesFilePathMenuItem = copyPath.CreateToolStripMenuItem(menuType, clickedItemPath, true);
            toolStripMenuItem.DropDownItems.Add(fwdSlashesFilePathMenuItem);

            var filePathMenuItem = copyPath.CreateToolStripMenuItem(menuType, clickedItemPath);
            toolStripMenuItem.DropDownItems.Add(filePathMenuItem);

            _contextMenuStrip.Items.Add(toolStripMenuItem);

            return _contextMenuStrip;
        }

        /// <summary>
        /// Determines wether the clicked item is a file, a folder or a directory.
        /// </summary>
        /// <param name="shellServer"></param>
        /// <returns>string</returns>
        private static string GetClickedItemType(ShellExtInitServer shellServer)
        {
            var itemType = "unsupported";

            if (shellServer.FolderPath != null) return "directory";

            try
            {
                var path = shellServer.SelectedItemPaths.First();
                if (null != path)
                {
                    var length = Path.GetExtension(path).Length;
                    itemType = length == 0 ? "folder" : "file";
                }
            }
            catch (ArgumentNullException ex)
            {
                log.Error( $"{ex.Message} (getclickeditemtype)");

                throw;
            }

            return itemType;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing) _contextMenuStrip.Dispose();

            _disposedValue = true;
        }
    }
}