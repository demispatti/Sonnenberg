using System;
using System.Drawing;
using System.Windows.Forms;
using Sonnenberg.ContextMenu.Properties;
using Sonnenberg.Language;

namespace Sonnenberg.ContextMenu.SubMenuItems
{
    /// <summary>
    ///     The class responsible for copying the path of a given, clicked element.
    /// </summary>
    /// <remarks>
    ///     - Creates a ToolStripMenuItem
    ///     - Creates a click action
    ///     - Clears the clipboard and adds the path to the clipboard
    /// </remarks>
    /// <seealso cref="CopyPath" />
    internal class CopyPath : IDisposable
    {
        private bool _disposedValue;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal ToolStripMenuItem ItemDisplay(ToolStripMenuItem toolStripMenuItem, string clickedItemType, string clickedItemPath, bool isDarkTheme)
        {
            var copyPath = new CopyPath();
            var escapedFilePathMenuItem = copyPath.CreateItem(clickedItemType, clickedItemPath, true, isDarkTheme);
            toolStripMenuItem.DropDownItems.Add(escapedFilePathMenuItem);

            var copyPathMenuItem = copyPath.CreateItem(clickedItemType, clickedItemPath, false, isDarkTheme);
            toolStripMenuItem.DropDownItems.Add(copyPathMenuItem);
            copyPath.Dispose();

            return toolStripMenuItem;
        }

        private ToolStripMenuItem CreateItem(string clickedItemType, string clickedItemPath, bool forwardSlashes, bool isDarkTheme)
        {
            var icon = GetIcon(clickedItemType, isDarkTheme);

            //  Creates the item.
            var toolStripMenuItem = new ToolStripMenuItem
            {
                Text = GetText(clickedItemType, forwardSlashes),
                Image = icon.ToBitmap()
            };

            //  Adds click action.
            toolStripMenuItem.Click += (sender, args) => DoClickAction(clickedItemPath, forwardSlashes);
            icon.Dispose();

            return toolStripMenuItem;
        }

        private string GetText(string clickedItemType, bool forwardSlashes)
        {
            switch (clickedItemType)
            {
                case "Directory":
                    return forwardSlashes ? Strings.copyForwardslashedDirectoryPathText : Strings.copyDirectoryPathText;

                case "File":
                    return forwardSlashes ? Strings.copyForwardslashedFilePathText : Strings.copyFilePathText;

                default:
                    return forwardSlashes ? Strings.copyForwardslashedFolderPathText : Strings.copyFolderPathText;
            }
        }

        private Icon GetIcon(string clickedItemType, bool isDarkTheme)
        {
            Icon icon;

            switch (clickedItemType)
            {
                case "Directory":
                    icon = isDarkTheme ? Resources.Folderpath_dark_theme : Resources.Folderpath_light_theme;
                    break;

                case "Folder":
                    icon = isDarkTheme ? Resources.Folderpath_dark_theme : Resources.Folderpath_light_theme;
                    break;

                default:
                    icon = isDarkTheme ? Resources.Filepath_dark_theme : Resources.Filepath_light_theme;
                    break;
            }

            return new Icon(icon, 40, 40);
        }

        private void DoClickAction(string clickedItemPath, bool forwardslashes)
        {
            if (forwardslashes) clickedItemPath = clickedItemPath.Replace('\\', '/');

            Clipboard.Clear();
            Clipboard.SetText(clickedItemPath);
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
    }
}