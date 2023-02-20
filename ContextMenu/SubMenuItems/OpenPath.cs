using log4net;
using Sonnenberg.Common;
using Sonnenberg.ContextMenu.Properties;
using Sonnenberg.Language;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Sonnenberg.ContextMenu.SubMenuItems
{
    /// <summary>
    /// The class responsible for creating a <c>ToolStripMenuItem</c> and its click action.
    /// It starts a CMD-, Git-CMD- or Git-Bash-Process and cds into the respective directory.
    /// The processes can be started with limited or elevated privileges.
    /// </summary>
    /// <remarks>
    /// - Creates a ToolStripMenuItem
    /// - Creates a click action
    /// - Checks that the requested executable is installed
    /// - Determines the directory where the clicked item resides
    /// - Determines the directory the CMD process will cd into
    /// - Assembles the required <c>StartInfo</c> parameters
    /// - Starts a CMD-, Git-CMD- or Git-Bash-Process and cds into the respective directory
    /// </remarks>
    /// <seealso cref="ContextMenu" />
    /// <seealso cref="Logger" />
    internal class OpenPath : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(OpenPath));

        private bool _disposedValue;

        internal OpenPath()
        {
            ConfigureLogger();
        }

        private static void ConfigureLogger()
        {
            Logger.Configure();
        }

        internal ToolStripMenuItem ItemDisplay(ToolStripMenuItem toolStripMenuItem, string clickedItemType,
            string targetDirectory, bool isDarkTheme)
        {
            var openPath = new OpenPath();
            var openPathMenuItem = openPath.CreateItem(clickedItemType, targetDirectory, isDarkTheme);
            toolStripMenuItem.DropDownItems.Add(openPathMenuItem);
            openPath.Dispose();

            return toolStripMenuItem;
        }

        private ToolStripMenuItem CreateItem(string clickedItemType, string shortcutTargetFolder,
            bool isDarkTheme)
        {
            var icon = GetIcon(isDarkTheme);

            var toolStripMenuItem = new ToolStripMenuItem
            {
                Text = GetText(clickedItemType),
                Image = icon.ToBitmap()
            };

            //  Add click action.
            toolStripMenuItem.Click += (sender, args) => DoClickAction(shortcutTargetFolder);
            icon.Dispose();

            return toolStripMenuItem;
        }

        private string GetText(string clickedItemType)
        {
            return "FileShortcut" == clickedItemType
                ? Strings.openFileShortcutTargetFolderText
                : Strings.openFolderShortcutTargetFolderText;
        }

        private Icon GetIcon(bool isDarkTheme)
        {
            var icon = isDarkTheme ? Resources.Folderpath_dark_theme : Resources.Folderpath_light_theme;

            return new Icon(icon, 40, 40);
        }

        private static void DoClickAction(string shortcutTargetFolder)
        {
            StartProcess(shortcutTargetFolder);
        }

        private static void StartProcess(string shortcutTargetFolder)
        {
            Process.Start(new ProcessStartInfo()
            {
                WorkingDirectory = @"C:\Windows\System32",
                FileName = shortcutTargetFolder,
                Verb = "open",
                UseShellExecute = true
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                _disposedValue = true;
                Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}